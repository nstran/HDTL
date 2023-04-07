using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.BankAccount;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Lead;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetCustomerByIdResult : BaseResult
    {
        public List<ProvinceEntityModel> ListProvince { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
        public List<CustomerAdditionalInformationEntityModel> ListCustomerAdditionalInformation { get; set; }
        public List<CustomerOtherContactModel> ListCusContact { get; set; }
        public List<string> CustomerCode { get; set; }
        public CustomerEntityModel Customer { get; set; }
        public ContactEntityModel Contact { get; set; }
        public List<EmployeeEntityModel> ListEmployeeEntityModel { get; set; }
        public List<CategoryEntityModel> ListCategoryByCustomerGroup { get; set; }

    }
}
