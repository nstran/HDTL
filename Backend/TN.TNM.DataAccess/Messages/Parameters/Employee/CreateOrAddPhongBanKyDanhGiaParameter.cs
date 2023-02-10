using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CreateOrAddPhongBanKyDanhGiaParameter: BaseParameter
    {
        public Guid? OrganizationId { get; set; }
        public Decimal? QuyLuong { get; set; }
        public int KyDanhGiaId { get; set; }

    }
}
