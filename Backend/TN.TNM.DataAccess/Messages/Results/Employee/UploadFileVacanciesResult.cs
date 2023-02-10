using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class UploadFileVacanciesResult : BaseResult
    {
        public List<FileInFolderEntityModel> ListFileInFolder { get; set; }
    }
}
