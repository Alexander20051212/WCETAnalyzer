using System.Collections.Generic;

namespace WCETAnalyzer.src.Models
{
    public class InputDataModel
    {
        public ProcessorModel Processor { get; set; } = new();

        public List<OperationModel> Operations { get; set; } = new();
    }
}
