using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetListCauHinhChecklistResult : BaseResult
    {
        public List<CauHinhChecklistEntityModel> ListCauHinhChecklist { get; set; }
        public bool IsShowButton { get; set; }
    }
}
