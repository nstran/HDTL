using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.CauHinhOtMođel;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterSearchKeHoachOtResult : BaseResult
    {
        public List<KeHoachOtEntityModel> listKeHoachOt { get; set; }
        public CompanyConfigEntityModel CompanyConfig { get; set; }
        public List<TrangThaiGeneral> listTrangThaiKeHoach { get; set; }
    }

}
