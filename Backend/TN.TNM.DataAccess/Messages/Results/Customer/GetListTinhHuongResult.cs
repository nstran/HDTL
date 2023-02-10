using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.TinhHuong;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetListTinhHuongResult : BaseResult
    {
        public List<KichHoatTinhHuongModel> ListData { get; set; }
    }
}
