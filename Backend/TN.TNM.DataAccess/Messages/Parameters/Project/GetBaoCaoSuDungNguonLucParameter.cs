using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class GetBaoCaoSuDungNguonLucParameter : BaseParameter
    {
        public decimal? Thang { get; set; }
        public decimal? Nam { get; set; }
        public List<Guid> ListProjectId { get; set; }
    }
}
