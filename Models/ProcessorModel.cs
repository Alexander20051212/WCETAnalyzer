
namespace WCETAnalyzer.Models
{
    public class ProcessorModel
    {
        public int BaseOperationCost { get; set; }

        public int CacheMissPenalty { get; set; }

        public int BranchMispredictionPenalty { get; set; }

        public int DeadlineCycles { get; set; }
    }
}
