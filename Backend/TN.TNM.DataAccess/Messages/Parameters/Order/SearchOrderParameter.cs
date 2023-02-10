using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class SearchOrderParameter : BaseParameter
    {
        public List<Guid> ListPacketIdSelected { get; set; }
        public List<Guid> ListCreatorIdSelected { get; set; }
        public List<Guid> ListCusId { get; set; }
        public List<int> ListStatusSelected { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool IsOrderAction { get; set; }
        public bool IsOrderProcess { get; set; }
        public List<Guid> ListCusSelected { get; set; }
        public List<Guid> ListProductCateSelected { get; set; }
    }
}
