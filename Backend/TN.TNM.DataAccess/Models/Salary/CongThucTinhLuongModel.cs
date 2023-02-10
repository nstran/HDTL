using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class CongThucTinhLuongModel : BaseModel<CongThucTinhLuong>
    {
        public int? CongThucTinhLuongId { get; set; }
        public string CongThuc { get; set; }

        /* Virtual Field */
        public string CongThucText { get; set; }

        public CongThucTinhLuongModel()
        {

        }

        //Map từ Entity => Model
        public CongThucTinhLuongModel(CongThucTinhLuong entity)
        {
            CongThucText = setCongThucText(entity.CongThuc);

            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override CongThucTinhLuong ToEntity()
        {
            var entity = new CongThucTinhLuong();
            Mapper(this, entity);
            return entity;
        }

        private string setCongThucText(string congThuc)
        {
            string congThucText = congThuc;

            var listTokenTinhLuong = GeneralList.GetTrangThais("TokenTinhLuong");

            listTokenTinhLuong.ForEach(item =>
            {
                if (congThucText.Contains(item.ValueText))
                {
                    congThucText = congThucText.Replace(item.ValueText, item.Name);
                }
            });

            return congThucText;
        }
    }
}
