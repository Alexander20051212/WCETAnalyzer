using System;
using System.Collections.Generic;
using System.Linq;
using WCETAnalyzer.Models;

namespace WCETAnalyzer.Services
{
    public class SourceBreakdownResult
    {
        public int MemoryPenalty { get; set; }

        public int BranchPenalty { get; set; }

        public int TotalPenalty => MemoryPenalty + BranchPenalty;
    }

    public static class WcetCalculator
    {
        // Лучшее время одной операции:
        // базовая стоимость, без штрафов за память и ветвления.
        public static int BestCase(
            OperationModel op,
            ProcessorModel model)
        {
            return model.BaseOperationCost;
        }

        // Худшее время одной операции:
        // базовая стоимость + штрафы за все промахи
        // кэша и ошибки предсказания переходов.
        public static int WorstCase(
            OperationModel op,
            ProcessorModel model)
        {
            return model.BaseOperationCost
                + op.MemoryAccesses * model.CacheMissPenalty
                + op.Branches * model.BranchMispredictionPenalty;
        }

        // Суммарное лучшее время фрагмента.
        public static int Bcet(
            List<OperationModel> fragment,
            ProcessorModel model)
        {
            return fragment.Sum(op => BestCase(op, model));
        }

        // Суммарное худшее время фрагмента.
        public static int Wcet(
            List<OperationModel> fragment,
            ProcessorModel model)
        {
            return fragment.Sum(op => WorstCase(op, model));
        }

        // Отношение WCET к BCET.
        public static double NondeterminismRatio(
            List<OperationModel> fragment,
            ProcessorModel model)
        {
            int best = Bcet(fragment, model);
            int worst = Wcet(fragment, model);

            if (best <= 0)
            {
                throw new InvalidOperationException(
                    "BCET должен быть больше нуля.");
            }

            return (double)worst / best;
        }

        // Раздельный вклад памяти и ветвлений.
        public static SourceBreakdownResult SourceBreakdown(
            List<OperationModel> fragment,
            ProcessorModel model)
        {
            return new SourceBreakdownResult
            {
                MemoryPenalty = fragment.Sum(
                    op => op.MemoryAccesses * model.CacheMissPenalty),

                BranchPenalty = fragment.Sum(
                    op => op.Branches * model.BranchMispredictionPenalty)
            };
        }
    }
}
