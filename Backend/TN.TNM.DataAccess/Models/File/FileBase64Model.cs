using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.File
{
    public class FileBase64Model
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string Base64 { get; set; }
    }
}
