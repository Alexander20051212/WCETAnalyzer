using System;
using System.IO;
using System.Text.Json;
using WCETAnalyzer.Services;
using WCETAnalyzer.src.Models;

namespace WCETAnalyzer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string inputPath = Path.Combine(
                AppContext.BaseDirectory,
                "data",
                "input.json");

            try
            {
                if (!File.Exists(inputPath))
                {
                    Console.WriteLine("Ошибка: файл данных не найден.");
                    Console.WriteLine($"Ожидаемый путь: {inputPath}");
                    return;
                }

                string json = File.ReadAllText(inputPath);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                InputDataModel? input =
                    JsonSerializer.Deserialize<InputDataModel>(
                        json, options);

                if (input == null || input.Processor == null
                    || input.Operations == null
                    || input.Operations.Count == 0)
                {
                    Console.WriteLine(
                        "Ошибка: входные данные пусты или некорректны.");
                    return;
                }

                ProcessorModel processor = input.Processor;

                if (processor.BaseOperationCost <= 0
                    || processor.CacheMissPenalty < 0
                    || processor.BranchMispredictionPenalty < 0
                    || processor.DeadlineCycles <= 0)
                {
                    Console.WriteLine(
                        "Ошибка: проверьте параметры процессора и дедлайн.");
                    return;
                }

                foreach (OperationModel op in input.Operations)
                {
                    if (string.IsNullOrWhiteSpace(op.Name)
                        || op.MemoryAccesses < 0
                        || op.Branches < 0)
                    {
                        Console.WriteLine(
                            "Ошибка: обнаружена некорректная операция.");
                        return;
                    }
                }

                Console.WriteLine("ОЦЕНКА ВРЕМЕНИ ВЫПОЛНЕНИЯ (WCET)");
                Console.WriteLine(new string('-', 76));

                Console.WriteLine(
                    $"{"Операция",-22} {"Тип",-12} " +
                    $"{"Обращения",-12} {"Ветвления",-11} " +
                    $"{"Лучшее",-9} {"Худшее",-9}");

                Console.WriteLine(new string('-', 76));

                foreach (OperationModel op in input.Operations)
                {
                    int best = WcetCalculator.BestCase(op, processor);
                    int worst = WcetCalculator.WorstCase(op, processor);

                    Console.WriteLine(
                        $"{op.Name,-22} {op.Type,-12} " +
                        $"{op.MemoryAccesses,-12} {op.Branches,-11} " +
                        $"{best,-9} {worst,-9}");
                }

                int bcet = WcetCalculator.Bcet(
                    input.Operations, processor);

                int wcet = WcetCalculator.Wcet(
                    input.Operations, processor);

                double ratio = WcetCalculator.NondeterminismRatio(
                    input.Operations, processor);

                SourceBreakdownResult breakdown =
                    WcetCalculator.SourceBreakdown(
                        input.Operations, processor);

                Console.WriteLine(new string('-', 76));
                Console.WriteLine($"BCET: {bcet} тактов");
                Console.WriteLine($"WCET: {wcet} тактов");
                Console.WriteLine(
                    $"Коэффициент недетерминизма: {ratio:F2}");

                Console.WriteLine(
                    $"Вклад памяти: {breakdown.MemoryPenalty} тактов");

                Console.WriteLine(
                    $"Вклад ветвлений: {breakdown.BranchPenalty} тактов");

                Console.WriteLine(
                    $"Общий штраф: {breakdown.TotalPenalty} тактов");

                Console.WriteLine(
                    $"Дедлайн: {processor.DeadlineCycles} тактов");

                Console.WriteLine(
                    wcet <= processor.DeadlineCycles
                        ? "Результат: фрагмент укладывается в дедлайн."
                        : "Результат: фрагмент НЕ укладывается в дедлайн.");
            }
            catch (JsonException ex)
            {
                Console.WriteLine(
                    $"Ошибка чтения JSON: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine(
                    $"Ошибка доступа к файлу: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Непредвиденная ошибка: {ex.Message}");
            }
        }
    }
}
