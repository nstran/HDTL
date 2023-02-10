using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Note
{
    public class XoaFileGhiChuParameter : BaseParameter
    {
        public Guid FileInFolderId { get; set; }
    }
}
