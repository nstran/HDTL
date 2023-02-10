using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.DynamicColumnTable;
using TN.TNM.DataAccess.Models.Salary;

namespace TN.TNM.DataAccess.Messages.Results.Salary
{
    public class GetTkDiMuonVeSomResult : BaseResult
    {
        public List<List<DataRowModel>> ListData { get; set; }
        public List<List<DataHeaderModel>> ListDataHeader { get; set; }
        public List<ColorChamCongModel> ListColorChamCong { get; set; }
    }
}
