using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetMasterDataBaoCaoTuyenDungParameter : BaseParameter
    {
        public decimal? Thang { get; set; }
        public decimal? Nam { get; set; }
        public List<Guid> ListRecruitmentCampaignId { get; set; }
        public List<Guid> ListEmployeeId { get; set; }
        public List<Guid> ListVacanciesId { get; set; }

    }
}
