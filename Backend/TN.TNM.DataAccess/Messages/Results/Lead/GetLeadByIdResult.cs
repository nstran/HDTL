using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Company;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class GetLeadByIdResult : BaseResult
    {
        public LeadEntityModel Lead { get; set; }
        public List<CompanyEntityModel> ListCompany { get; set; }
        public CompanyEntityModel Company { get; set; }
        public string InterestedGroupName { get; set; }
        public string Status_Code { get; set; }
        public string PotentialName { get; set; }
        public string Status_Name { get; set; }
        public string PIC_Name { get; set; }
        public string PositionName { get; set; }
        public string PaymentMethodName { get; set; }
        public string InterestedName { get; set; }
        public Guid? ResponsibleName { get; set; }
        public ContactEntityModel Contact { get; set; }
        public List<EmployeeEntityModel> Employee{ get; set; }
        public List<CategoryEntityModel> StatusCategory{ get; set; }
        public List<CategoryEntityModel> Potential { get; set; }
        public List<CategoryEntityModel> InterestedList { get; set; }
        public List<CategoryEntityModel> PaymentMethod { get; set; }
        public List<CategoryEntityModel> Genders { get; set; }
        public string FullAddress { get; set; }
        public int CountLead { get; set; }
        public int StatusSaleBiddingAndQuote { get; set; }
    }
}
