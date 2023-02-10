using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.DataAccess.Messages.Parameters.CustomerCare
{
    public class UpdateCustomerCareFeedBackParameter:BaseParameter
    {
        public CustomerCareFeedBackEntityModel CustomerCareFeedBack { get; set; }

    }
}
