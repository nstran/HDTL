using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.DynamicColumnTable;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetBaoCaoBaoCaoTuyenDungResult : BaseResult
    {
        public List<List<DataRowModel>> ListData { get; set; }
        public List<DataHeaderModel> ListHeaderRow1 { get; set; }
        public List<DataHeaderModel> ListHeaderRow2 { get; set; }
    }
}
