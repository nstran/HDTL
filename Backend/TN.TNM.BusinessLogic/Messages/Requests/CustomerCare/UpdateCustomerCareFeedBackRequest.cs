using TN.TNM.BusinessLogic.Models.CustomerCare;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;
using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class UpdateCustomerCareFeedBackRequest : BaseRequest<UpdateCustomerCareFeedBackParameter>
    {
        public CustomerCareFeedBackEntityModel CustomerCareFeedBack { get; set; }

        public override UpdateCustomerCareFeedBackParameter ToParameter()
        {
            return new UpdateCustomerCareFeedBackParameter
            {
                CustomerCareFeedBack = this.CustomerCareFeedBack,
                UserId = this.UserId
            };
        }
    }
}
