using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Options;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Product
{
    public class EditServicePacketMappingOptionParameter : BaseParameter
    {
        public Guid Id { get; set; }
        public int SortOrder { get; set; }
        public string Name { get; set; }
    }
}
