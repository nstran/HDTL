﻿using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class MobileAppConfiguration
    {
        public Guid Id { get; set; }
        public string IntroduceColor { get; set; }
        public string IntroduceImageOrVideo { get; set; }
        public string IntroduceSologan { get; set; }
        public string LoginAndRegisterScreenImage { get; set; }
        public string LoginScreenColor { get; set; }
        public string LoginScreenIcon { get; set; }
        public string PaymentScreenIconVnpay { get; set; }
        public bool? IsPaymentScreenIconVnpay { get; set; }
        public string PaymentScreenContentVnpay { get; set; }
        public string OrderNotificationImage { get; set; }
        public Guid CreatedById { get; set; }
        public Guid UpdatedById { get; set; }
        public Guid TenantId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string PaymentScreenIconTransfer { get; set; }
        public bool? IsPaymentScreenIconTransfer { get; set; }
        public string PaymentScreenContentTransfer { get; set; }
    }
}
