using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class DataImportChamCongModel
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string No { get; set; }
        public string ThoiGianChamCong { get; set; }
        public bool? IsError { get; set; }
        public string NoteError { get; set; }
    }
}
