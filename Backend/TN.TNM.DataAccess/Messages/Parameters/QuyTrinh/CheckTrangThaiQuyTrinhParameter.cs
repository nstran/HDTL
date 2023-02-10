using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.QuyTrinh
{
    public class CheckTrangThaiQuyTrinhParameter : BaseParameter
    {
        public int DoiTuongApDung { get; set; }
        public Guid? Id { get; set; }
    }
}
