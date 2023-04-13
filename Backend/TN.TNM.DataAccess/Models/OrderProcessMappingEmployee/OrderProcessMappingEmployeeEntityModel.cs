using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.OrderProcessMappingEmployee
{
    public class OrderProcessMappingEmployeeEntityModel
    {
        public Guid? EmployeeId { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Guid? OrderProcessId { get; set; }
        public string RateContent { get; set; }
        public DateTime CreatedDate { get; set; }
        public string OrderCode { get; set; }
    }
}
