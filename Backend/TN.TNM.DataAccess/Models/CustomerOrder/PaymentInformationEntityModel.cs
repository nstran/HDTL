using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.CustomerOrder
{
    public class PaymentInformationEntityModel
    {
        public Guid ObjectId { get; set; }

        public string ObjectTpe { get; set; }

        public string ObjectCode { get; set; }

        public DateTime CreatedDate { get; set; }

        public decimal AmountCollected { get; set; }

        public Guid CreatedById { get; set; }

        public string CreatedByName { get; set; }
        
        public string CreatedByCode { get; set; }
        
        public string CreatedByCodeName { get; set; }

        public string PaymentInfoCode { get; set; }
    }
}
