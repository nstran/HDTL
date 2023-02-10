using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class SearchCustomerRequest : BaseRequest<SearchCustomerParameter>
    {
        public bool NoPic { get; set; }

        public bool IsBusinessCus { get; set; }

        public bool IsPersonalCus { get; set; }

        public Guid? StatusCareId { get; set; }

        public List<Guid?> CustomerGroupIdList { get; set; }

        public List<Guid?> PersonInChargeIdList { get; set; }

        public Guid? NhanVienChamSocId { get; set; }

        public Guid? AreaId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public bool KhachDuAn { get; set; }

        public bool KhachBanLe { get; set; }

        public override SearchCustomerParameter ToParameter()
        {
            return new SearchCustomerParameter() {
                UserId = UserId,
                Address = Address,
                Email = Email,
                FirstName = FirstName,
                FromDate = FromDate,
                      LastName = LastName,
                Phone = Phone,
                ToDate = ToDate,
            };
        }
    }
}
