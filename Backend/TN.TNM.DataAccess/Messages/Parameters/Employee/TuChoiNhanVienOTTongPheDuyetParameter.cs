using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.OT;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class TuChoiNhanVienOTTongPheDuyetParameter: BaseParameter
    {
        public List<Guid> ListIdTuChoi { get; set; }
        public List<KeHoachOtThanhVienEntityModel> ListNv { get; set; }

        public int KeHoachOtId { get; set; }
    }
}
