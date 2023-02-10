using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.QuyTrinh
{
    public class SearchQuyTrinhParameter : BaseParameter
    {
        public List<Guid> ListEmployeeId { get; set; }
        public string TenQuyTrinh { get; set; }
        public string MaQuyTrinh { get; set; }
        public DateTime? CreatedDateFrom { get; set; }
        public DateTime? CreatedDateTo { get; set; }
        public List<bool> ListTrangThai { get;set; }
    }
}
