using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.QuyTrinh
{
    public class DuLieuQuyTrinhModel
    {
        public string NodeName { get; set; }
        public List<DuLieuPhongBanPheDuyetModel> ListDonVi { get; set; }
        public bool IsActive { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsComplete { get; set; }
        public string Tooltip { get; set; }

        public DuLieuQuyTrinhModel()
        {
            ListDonVi = new List<DuLieuPhongBanPheDuyetModel>();
            IsActive = false;
            IsCurrent = false;
            IsComplete = false;
        }
    }
}
