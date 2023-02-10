using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class SearchProjectParameter : BaseParameter
    {      
        public string ProjectName { get; set; }
        public string ProjectCode { get; set; }
        public DateTime? ProjectStartS { get; set; }
        public DateTime? ProjectStartE { get; set; }
        public DateTime? ProjectEndS { get; set; }
        public DateTime? ProjectEndE { get; set; }
        public DateTime? ActualStartS { get; set; }
        public DateTime? ActualStartE { get; set; }
        public DateTime? ActualEndS { get; set; }
        public DateTime? ActualEndE { get; set; }       
        public List<Guid?> ListStatusProject { get; set; }
        public List<Guid?> ListProjectType { get; set; }
        public List<Guid?> ListEmployee { get; set; }

        public decimal? EstimateCompleteTimeS { get; set; }
        
        public decimal? EstimateCompleteTimeE { get; set; }
        public decimal? CompleteRateS { get; set; }
        public decimal? CompleteRateE { get; set; }
    }
}
