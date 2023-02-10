using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class DeXuatTangLuongDetailParameter:BaseParameter
    {
        public int DeXuatTLId { get; set; }
        public decimal QuyLuongConLai { get; set; }
    }
}
