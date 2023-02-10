using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.BusinessGoals
{
    public class BusinessGoalsDetailEntityModel
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

        public string ProductCategoryCode { get; set; }
        public string ProductCategoryName { get; set; }
        public Guid? OrganizationId { get; set; }
        public string OrganizationName { get; set; }
    }
}
