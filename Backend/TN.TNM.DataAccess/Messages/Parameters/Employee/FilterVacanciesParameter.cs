using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class FilterVacanciesParameter : BaseParameter
    {
        public List<Guid> SelectedViTriId { get; set; }
        public List<Guid> SelectedChienDichId { get; set; }
        public List<int> SelectedMucDoUTId { get; set; }
        public List<Guid> SelectedKinhNghiemId { get; set; }
        public List<Guid> SelectedLoaiCVId { get; set; }
        public List<Guid> SelectedNguoiPTId { get; set; }
        public decimal? StartMoney { get; set; }
        public decimal? EndMoney { get; set; }
    }
}
