using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class SendEmailInterviewParameter : BaseParameter
    {
        public EmailInterviewEntityModel EmailInterview { get; set; }
        public List<IFormFile> ListFormFile { get; set; }
        public string FolderType { get; set; }
    }
}
