using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Salary;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class ImportChamCongParameter : BaseParameter
    {
        public List<DataImportChamCongModel> ListData { get; set; }
    }
}
