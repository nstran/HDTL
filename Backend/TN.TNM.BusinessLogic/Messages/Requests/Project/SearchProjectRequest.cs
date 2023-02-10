using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;


namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class SearchProjectRequest : BaseRequest<SearchProjectParameter>
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
        public override SearchProjectParameter ToParameter()
        {
            return new SearchProjectParameter()
            {
                UserId = UserId,               
                ProjectName = ProjectName,
                ProjectCode = ProjectCode,
                ProjectStartS = ProjectStartS,
                ProjectStartE = ProjectStartE,
                ProjectEndS = ProjectEndS,
                ProjectEndE = ProjectEndE,
                ActualStartS = ActualStartS,
                ActualStartE = ActualStartE,
                ActualEndS = ActualEndS,
                ActualEndE = ActualEndE,
                ListStatusProject = ListStatusProject, 
                ListEmployee = ListEmployee,
                ListProjectType = ListProjectType,
                EstimateCompleteTimeS = EstimateCompleteTimeS,
                EstimateCompleteTimeE = EstimateCompleteTimeE,
                CompleteRateS = CompleteRateS,
                CompleteRateE = CompleteRateE
            };
        }
    }
}
