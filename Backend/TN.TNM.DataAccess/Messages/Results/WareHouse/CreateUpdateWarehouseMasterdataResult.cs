using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class CreateUpdateWarehouseMasterdataResult: BaseResult
    {
        public WareHouseEntityModel WarehouseEntityModel { get; set; }
        public List<DataAccess.Models.Employee.EmployeeEntityModel> ListEmployeeEntityModel { get; set; }
        public List<string> ListWarehouseCode { get; set; }
    }
}
