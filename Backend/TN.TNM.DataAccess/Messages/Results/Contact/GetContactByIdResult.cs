using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Contact
{
    public class GetContactByIdResult : BaseResult
    {
        public ContactEntityModel Contact { get; set; }
        public string FullAddress { get; set; }
    }
}
