using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class GetListCustomerRequestApprovalParameter : BaseParameter
    {
        public bool NoPic { get; set; }

        public bool IsBusinessCus { get; set; }

        public bool IsPersonalCus { get; set; }

        public bool KhachDuAn { get; set; }

        public bool khachBanLe { get; set; }

        public Guid StatusCareId { get; set; }

        public List<Guid?> CustomerGroupIdList { get; set; }

        public List<Guid?> PersonInChargeIdList { get; set; }

        public List<Guid?> PersonInCareIdList { get; set; }

        public Guid AreaId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public List<Guid?> CustomerServiceLevelIdList { get; set; }

        public bool IsHKDCus { get; set; }

        public string CustomerCode { get; set; }

        public string TaxCode { get; set; }

        public bool IsIdentificationCus { get; set; }

        public bool IsFreeCus { get; set; }
    }
}
