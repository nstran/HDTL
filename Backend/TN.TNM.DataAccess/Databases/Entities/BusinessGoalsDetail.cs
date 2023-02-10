using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class BusinessGoalsDetail
    {
        public Guid BusinessGoalsDetailId { get; set; }
        public Guid BusinessGoalsId { get; set; }
        public Guid ProductCategoryId { get; set; }
        public string BusinessGoalsType { get; set; }
        public decimal? January { get; set; }
        public decimal? February { get; set; }
        public decimal? March { get; set; }
        public decimal? April { get; set; }
        public decimal? May { get; set; }
        public decimal? June { get; set; }
        public decimal? July { get; set; }
        public decimal? August { get; set; }
        public decimal? September { get; set; }
        public decimal? October { get; set; }
        public decimal? November { get; set; }
        public decimal? December { get; set; }
        public bool Active { get; set; }
        public Guid? TenantId { get; set; }
    }
}
