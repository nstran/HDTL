﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class SearchRevenueProductResult: BaseResult
    {
        public List<DataAccess.Models.Order.ProductRevenueEntityModel> ListProductRevenue { get; set; }
    }
}
