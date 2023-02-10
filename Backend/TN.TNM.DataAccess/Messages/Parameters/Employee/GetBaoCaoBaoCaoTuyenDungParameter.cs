using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetBaoCaoBaoCaoTuyenDungParameter : BaseParameter
    {
        public decimal? Thang { get; set; }
        public decimal? Nam { get; set; }
        public List<Guid?> ListRecruitId { get; set; }
        public List<Guid?> ListEmployeeId { get; set; }
        public List<Guid?> ListVacanciesId { get; set; }
    }
}
