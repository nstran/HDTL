using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.File;
using TN.TNM.DataAccess.Models.User;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CreateEmployeeParameter : BaseParameter
    {
        public EmployeeEntityModel Employee { get; set; }
        public ContactEntityModel Contact { get; set; }
        public UserEntityModel User { get; set; }
        public bool IsAccessable { get; set; }
        public List<Guid> ListPhongBanId { get; set; }
        public bool IsAuto { get; set; }
        public Guid? CandidateId { get; set; }
        public FileBase64Model FileBase64 { get; set; }
    }
}
