using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.QuyTrinh
{
    public class DuLieuPhongBanPheDuyetModel
    {
        public Guid OrganizationId { get; set; }
        public string TenDonVi { get; set; }
        public int TrangThai { get; set; }
    }
}
