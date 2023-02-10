using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Salary;

namespace TN.TNM.DataAccess.Messages.Results.Salary
{
    public class ImportChamCongResult : BaseResult
    {
        public List<DataImportChamCongModel> ListDataError { get; set; }
    }
}
