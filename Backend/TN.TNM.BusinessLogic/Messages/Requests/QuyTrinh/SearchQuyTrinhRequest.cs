using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.QuyTrinh;

namespace TN.TNM.BusinessLogic.Messages.Requests.QuyTrinh
{
    public class SearchQuyTrinhRequest : BaseRequest<SearchQuyTrinhParameter>
    {
        public List<Guid> ListEmployeeId { get; set; }
        public string TenQuyTrinh { get; set; }
        public string MaQuyTrinh { get; set; }
        public DateTime? CreatedDateFrom { get; set; }
        public DateTime? CreatedDateTo { get; set; }
        public List<bool> ListTrangThai { get; set; }
        public override SearchQuyTrinhParameter ToParameter()
        {
            return new SearchQuyTrinhParameter()
            {
                UserId = UserId,
                ListEmployeeId = ListEmployeeId,
                TenQuyTrinh = TenQuyTrinh,
                MaQuyTrinh = MaQuyTrinh,
                CreatedDateFrom = CreatedDateFrom,
                CreatedDateTo = CreatedDateTo,
                ListTrangThai = ListTrangThai
            };
        }
    }
}
