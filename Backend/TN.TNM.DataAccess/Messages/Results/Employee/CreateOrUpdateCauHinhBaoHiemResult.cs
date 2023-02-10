using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class CreateOrUpdateCauHinhBaoHiemResult : BaseResult
    {
        public int CauHinhBaoHiemId { get; set; }
    }
}
