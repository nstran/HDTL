using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models.QuyTrinh;

namespace TN.TNM.DataAccess.Messages.Results.QuyTrinh
{
    public class GetDetailQuyTrinhResult : BaseResult
    {
        public QuyTrinhModel QuyTrinh { get; set; }
        public List<CauHinhQuyTrinhModel> ListCauHinhQuyTrinh { get; set; }
        public List<TrangThaiGeneral> ListDoiTuongApDung { get; set; }
    }
}
