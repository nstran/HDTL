using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Databases.Entities
    ;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterDataKeHoachOtDetailResult : BaseResult
    {        
        public KeHoachOt KeHoachOt { get; set; }
        public List<KeHoachOtPhongBan> ListKeHoachOtPhongBan { get; set; }
        public List<KeHoachOtThanhVien> ListKeHoachOtThanhVien { get; set; }
        public List<CauHinhOt> ListLoaiOt { get; set; }
        public List<OrganizationEntityModel> ListOrganization { get; set; }
    }
}
