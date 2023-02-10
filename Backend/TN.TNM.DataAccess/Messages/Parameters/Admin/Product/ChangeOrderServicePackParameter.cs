using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Product
{
    public class ChangeOrderServicePackParameter: BaseParameter
    {
        public Guid Id { get; set; }
        public int Stt { get; set; }
    }
}
