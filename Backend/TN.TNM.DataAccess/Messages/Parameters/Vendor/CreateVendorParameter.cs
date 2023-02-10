using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class CreateVendorParameter : BaseParameter
    {
        public VendorEntityModel Vendor { get; set; }
        public ContactEntityModel Contact { get; set; }
        public List<ContactEntityModel> VendorContactList { get; set; }
    }
}
