using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class AddTaiSanToDotKiemKeParameter: BaseParameter
    {
        public int DotKiemKeId { get; set; }
        public int TaiSanId { get; set; }
    }
}
