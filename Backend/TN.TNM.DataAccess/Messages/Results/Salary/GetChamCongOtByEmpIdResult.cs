using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.DynamicColumnTable;

namespace TN.TNM.DataAccess.Messages.Results.Salary
{
    public class GetChamCongOtByEmpIdResult : BaseResult
    {
        public List<List<DataRowModel>> ListData { get; set; }
        public List<List<DataHeaderModel>> ListDataHeader { get; set; }
        public List<CategoryEntityModel> ListLoaiOt { get; set; }
    }
}
