using System.Collections.Generic;

namespace WCETAnalyzer.Models
{
    public class InputDataModel
    {
        public ProcessorModel Processor { get; set; } = new();

        public List<OperationModel> Operations { get; set; } = new();
    }
}
