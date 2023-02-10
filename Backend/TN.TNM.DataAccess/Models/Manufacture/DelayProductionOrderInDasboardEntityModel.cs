using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Manufacture
{
    public class DelayProductionOrderInDasboardEntityModel
    {
        public Guid? ProductionOrderId { get; set; }
        public string ProductionOrderCode { get; set; }
        public string CustomerName { get; set; }
        public DateTime? EndDate { get; set; }
        public string StatusCode { get; set; }
        public double Area { get; set; } //Số m2 phải làm
        public double AreaRemain { get; set; }  //Số m2 còn lại

        //Đối với trường hợp phân quyền dữ liệu Nhân viên:
        //true: có item đi qua tổ hiện tại, false: không có item đi qua tổ hiện tại 
        public bool IsShow { get; set; } 
    }
}
