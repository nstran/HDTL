using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.DeXuatXinNghiModel
{
    public class DeXuatXinNghiModel : BaseModel<DeXuatXinNghi>
    {
        public int? DeXuatXinNghiId { get; set; }
        public string Code { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? PositionId { get; set; }
        public int LoaiDeXuatId { get; set; }
        public string LyDo { get; set; }
        public int? TrangThaiId { get; set; }
        public string LyDoTuChoi { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }

        /* Virtual Field */
        public List<DateTime> ListDate { get; set; }
        public string ListDateText { get; set; }
        public int? TuCa { get; set; }
        public int? DenCa { get; set; }
        public int? Ca { get; set; }
        public string EmployeeCodeName { get; set; }
        public string OrganizationName { get; set; }
        public string PositionName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string StatusName { get; set; }
        public string NguoiPheDuyet { get; set; }
        public string TenLoaiDeXuat { get; set; }
        public string BackgroupStatusColor { get; set; }
        public decimal? TongNgayNghi { get; set; }
        public decimal? SoNgayPhepConLai { get; set; }

        public DeXuatXinNghiModel()
        {

        }

        //Map từ Entity => Model
        public DeXuatXinNghiModel(DeXuatXinNghi entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override DeXuatXinNghi ToEntity()
        {
            var entity = new DeXuatXinNghi();
            Mapper(this, entity);
            return entity;
        }
    }
}
