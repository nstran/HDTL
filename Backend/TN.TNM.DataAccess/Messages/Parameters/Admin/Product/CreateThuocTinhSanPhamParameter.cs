using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.ProductAttributeCategory;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Product
{
    public class CreateThuocTinhSanPhamParameter : BaseParameter
    {
        public Guid ProductId { get; set; }
        public ProductAttributeCategoryEntityModel ThuocTinh { get; set; }
    }
}
