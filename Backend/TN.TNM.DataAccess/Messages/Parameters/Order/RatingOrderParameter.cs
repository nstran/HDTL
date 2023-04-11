using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class RatingOrderParameter: BaseParameter
    {
        public Guid OrderId { get; set; }
        public int RateStar { get; set; }
        public string RateContent { get; set; }
        public List<EmployeeRatingStar> ListEmployeeRatingStar { get; set; }
    }

    public class EmployeeRatingStar
    {
        public Guid? EmployeeId { get; set; }
        public Guid? OrderProcessId { get; set; }
        public string RateContent { get; set; }
    }
}
