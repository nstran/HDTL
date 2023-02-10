
using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class BaoCaoPhanBoParameter : BaseParameter
    {
        public List<Guid?> ListEmployeeId { get; set; }
        public List<Guid?> ListOrganizationId { get; set; }
        public List<Guid?> ListPhanLoaiTaiSanId { get; set; }
        public List<int?> ListHienTrangTaiSan { get; set; }

    }
}
