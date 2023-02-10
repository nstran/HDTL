using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.CustomerCare
{
    public class CustomerCareEntityModel:BaseModel<Databases.Entities.CustomerCare>
    {
        public Guid CustomerCareId { get; set; }
        public string CustomerCareCode { get; set; }
        public Guid? EmployeeCharge { get; set; }
        public DateTime? EffecttiveFromDate { get; set; }
        public DateTime? EffecttiveToDate { get; set; }
        public string CustomerCareTitle { get; set; }
        public string CustomerCareContent { get; set; }
        public Guid? CustomerCareContactType { get; set; }
        public string CustomerCareContentSms { get; set; }
        public bool? IsSendNow { get; set; }
        public bool? IsEvent { get; set; }
        public DateTime? SendDate { get; set; }
        public TimeSpan? SendHour { get; set; }
        public Guid? CustomerCareEvent { get; set; }
        public TimeSpan? CustomerCareEventHour { get; set; }
        public string CustomerCareContentEmail { get; set; }
        public bool? IsSendEmailNow { get; set; }
        public DateTime? SendEmailDate { get; set; }
        public TimeSpan? SendEmailHour { get; set; }
        public string CustomerCareVoucher { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? PercentDiscountAmount { get; set; }
        public int? GiftCustomerType1 { get; set; }
        public Guid? GiftTypeId1 { get; set; }
        public double? GiftTotal1 { get; set; }
        public int? GiftCustomerType2 { get; set; }
        public Guid? GiftTypeId2 { get; set; }
        public double? GiftTotal2 { get; set; }
        public int? CustomerCareType { get; set; }
        public int? ProgramType { get; set; }
        public Guid? StatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
        public decimal? ExpectedAmount { get; set; }

        public string CustomerCareContactTypeName { get; set; }
        public string StatusName { get; set; }
        public string StatusCode { get; set; }
        public string EmployeeChargeName { get; set; }
        // public bool? IsSentMail { get; set; }
        public bool? IsFilterSendMailCondition { get; set; }
        public int? TinhTrangEmail { get; set; }

        public int? NumberCode { get; set; }
        public int? YearCode { get; set; }
        public int? MonthCode { get; set; }
        public int? DateCode { get; set; }
        public int TypeCustomer { get; set; }
        public DateTime? ActiveDate { get; set; }

        public CustomerCareEntityModel()
        {

        }

        public CustomerCareEntityModel(Databases.Entities.CustomerCare customerCare)
        {
            Mapper(customerCare, this);
        }

        public override Databases.Entities.CustomerCare ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new Databases.Entities.CustomerCare();
            Mapper(this, entity);
            return entity;
        }
    }
}
