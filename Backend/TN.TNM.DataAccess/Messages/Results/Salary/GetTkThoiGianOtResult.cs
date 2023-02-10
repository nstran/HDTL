using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.DynamicColumnTable;

namespace TN.TNM.DataAccess.Messages.Results.Salary
{
    public class GetTkThoiGianOtResult : BaseResult
    {
        public List<List<DataRowModel>> ListData { get; set; }
        public List<List<DataHeaderModel>> ListDataHeader { get; set; }
    }
}
