
namespace WCETAnalyzer.Models
{
    public class OperationModel
    {
        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public int MemoryAccesses { get; set; }

        public int Branches { get; set; }
    }
}
