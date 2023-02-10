using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Databases.Entities
    ;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models.CauHinhOtMođel;
using TN.TNM.DataAccess.Models.ChamCong;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterCreateKeHoachOtResult : BaseResult
    {        
        public List<CategoryEntityModel> ListLoaiOt { get; set; }
        public List<OrganizationEntityModel> ListOrganization { get; set; }
        public List<EmployeeEntityModel> CurrentEmp { get; set; }
        public List<TrangThaiGeneral> ListLoaiCaOt { get; set; }
        public CauHinhOtCaNgayModel CauHinhOtCaNgay { get; set; }
    }
}
