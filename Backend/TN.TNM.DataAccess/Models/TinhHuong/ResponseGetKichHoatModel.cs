using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.TinhHuong
{
    public class ResponseGetKichHoatModel
    {
        public decimal error { get; set; }
        public string message { get; set; }
        public string total { get; set; }
        public string next_offset { get; set; }
        public List<ResponseSuccessListGetKichHoatModel> data { get; set; }
    }

    public class ResponseSuccessListGetKichHoatModel
    {
        public string id { get; set; }
        public string fullname { get; set; }
        public string phone_number { get; set; }
        public string status { get; set; }
        public string agent { get; set; }
        public string digit { get; set; }
        public string duration { get; set; }
        public string billsec { get; set; }
        public string retry { get; set; }
        //public string session { get; set; }
    }
}
