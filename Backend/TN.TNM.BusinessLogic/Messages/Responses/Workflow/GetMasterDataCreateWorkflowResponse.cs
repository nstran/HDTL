using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Workflow;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using PositionModel = TN.TNM.BusinessLogic.Models.Employee.PositionModel;

namespace TN.TNM.BusinessLogic.Messages.Responses.Workflow
{
    public class GetMasterDataCreateWorkflowResponse : BaseResponse
    {
        public List<PositionModel> ListPosition { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<SystemFeatureModel> ListSystemFeature { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
    }
}
