﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.BillSale
{
    public class AddOrEditBillSaleResponse:BaseResponse
    {
        public Guid? BillSaleId { get; set; }
    }
}
