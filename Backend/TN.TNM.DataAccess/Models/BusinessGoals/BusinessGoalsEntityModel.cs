using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.BusinessGoals
{
    public class BusinessGoalsEntityModel
    {
        public Guid BusinessGoalsId { get; set; }
        public Guid OrganizationId { get; set; }
        public string Year { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }


        public string OrganizationName { get; set; }
        public List<BusinessGoalsDetailEntityModel> ListBusinessGoalsDetail { get; set; }
    }
}
