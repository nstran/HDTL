using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.TinhHuong
{
    public class ResponseKichHoatModel
    {
        public decimal error { get; set; }
        public MessageKichHoatModel message { get; set; }
    }

    public class MessageKichHoatModel
    {
        public string session { get; set; }
    }
}
