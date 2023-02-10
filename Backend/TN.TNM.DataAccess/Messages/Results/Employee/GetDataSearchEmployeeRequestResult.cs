using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetDataSearchEmployeeRequestResult : BaseResult
    {
        public bool IsShowOrganization { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public List<TrangThaiGeneral> ListStatus { get; set; }
        public List<TrangThaiGeneral> ListKyHieuChamCong { get; set; }
    }
}
