using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.CustomerCare
{
    public class SendQuickEmailParameter : BaseParameter
    {
        public Queue Queue { get; set; }
        public List<FileUploadEntityModel> ListFile { get; set; }

        public string FolderType { get; set; }

    }
}
