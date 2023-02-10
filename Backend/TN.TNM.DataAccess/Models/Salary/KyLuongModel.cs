using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class KyLuongModel : BaseModel<KyLuong>
    {
        public int? KyLuongId { get; set; }
        public string TenKyLuong { get; set; }
        public decimal SoNgayLamViec { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public int? TrangThai { get; set; }
        public string LyDoTuChoi { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }

        /* Virtual Field */
        public string TenTrangThai { get; set; }
        public string BackgroupStatusColor { get; set; }
        public string CreatedName { get; set; }

        public KyLuongModel()
        {

        }

        //Map từ Entity => Model
        public KyLuongModel(KyLuong entity)
        {
            var listStatus = GeneralList.GetTrangThais("TrangThaiKyLuong");
            TenTrangThai = listStatus.FirstOrDefault(x => x.Value == entity.TrangThai)?.Name;
            BackgroupStatusColor = GetBackgroundStatusColor(entity.TrangThai);

            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override KyLuong ToEntity()
        {
            var entity = new KyLuong();
            Mapper(this, entity);
            return entity;
        }

        private string GetBackgroundStatusColor(int trangThai)
        {
            string color = "";

            //Mới
            if (trangThai == 1)
            {
                color = "#AEA4A0";
            }
            //Chờ phê duyệt
            else if (trangThai == 2)
            {
                color = "#FFCC00";
            }
            //Đã phê duyệt
            else if (trangThai == 3)
            {
                color = "#007AFF";
            }
            //Từ chối
            else if (trangThai == 4)
            {
                color = "#CC3C00";
            }
            //Hoàn thành
            else if (trangThai == 5)
            {
                color = "#4BCA81";
            }

            return color;
        }
    }
}
