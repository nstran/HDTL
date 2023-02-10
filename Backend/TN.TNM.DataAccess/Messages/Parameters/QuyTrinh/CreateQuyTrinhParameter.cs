using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.QuyTrinh;

namespace TN.TNM.DataAccess.Messages.Parameters.QuyTrinh
{
    public class CreateQuyTrinhParameter : BaseParameter
    {
        public QuyTrinhModel QuyTrinh { get; set; }
        public List<CauHinhQuyTrinhModel> ListCauHinhQuyTrinh { get; set; }
    }
}
