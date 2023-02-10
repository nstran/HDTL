using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.TinhHuong
{
    public class KichHoatRequestModel
    {
        public int campaign_id { get; set; }
        public string fullname { get; set; } = "";
        public string phone_number { get; set; }
        public string content { get; set; }
        public string tts_voice { get; set; }
    }
}
