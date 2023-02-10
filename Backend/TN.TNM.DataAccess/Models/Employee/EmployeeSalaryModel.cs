using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class EmployeeSalaryModel
    {
        public Guid EmployeeSalaryId { get; set; }
        public decimal? EmployeeSalaryBase { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
