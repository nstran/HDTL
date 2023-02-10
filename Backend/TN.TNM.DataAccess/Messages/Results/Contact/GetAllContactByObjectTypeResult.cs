using System.Collections.Generic;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Contact
{
    public class GetAllContactByObjectTypeResult : BaseResult
    {
        public List<ContactEntityModel> ContactList { get; set; }
    }
}
