using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;

namespace TN.TNM.DataAccess.Messages.Results.QuyTrinh
{
    public class GetMasterDataCreateQuyTrinhResult : BaseResult
    {
        public List<TrangThaiGeneral> ListDoiTuongApDung { get; set; }
    }
}
