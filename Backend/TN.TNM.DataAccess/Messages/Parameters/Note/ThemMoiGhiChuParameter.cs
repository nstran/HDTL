using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Parameters.Note
{
    public class ThemMoiGhiChuParameter : BaseParameter
    {
        public Databases.Entities.Note Note { get; set; }
        public string ObjectType { get; set; }
        public List<FileUploadEntityModel> ListFile { get; set; }
    }

    public class ThemMoiGhiChuParameter1 : BaseParameter
    {
        public string ObjectType { get; set; }
    }

}
