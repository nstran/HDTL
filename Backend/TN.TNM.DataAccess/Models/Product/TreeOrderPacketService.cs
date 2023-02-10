using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Product
{
    public class TreeOrderPacketService
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public Guid ListProduct { get; set; }
        public Guid  Type { get; set; }
        // 1: Dich vu, 2: Tuy chon dich vu, 3: Thuoc tinh tuy chon dich vu
        public Guid ListOption { get; set; }
        public Guid ListOptionAttr { get; set; }
    }
}
