using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class UpdateVendorByIdParameter : BaseParameter
    {
        public VendorEntityModel Vendor { get; set; }
        public ContactEntityModel Contact { get; set; }
    }
}
