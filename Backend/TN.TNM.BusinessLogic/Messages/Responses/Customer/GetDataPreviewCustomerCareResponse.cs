using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetDataPreviewCustomerCareResponse : BaseResponse
    {
        public DateTime EffecttiveFromDate { get; set; }
        
        public DateTime EffecttiveToDate { get; set; }
        
        public DateTime SendDate { get; set; }
        
        public string StatusName { get; set; }
        
        public string PreviewEmailName { get; set; }
        
        public string PreviewEmailTitle { get; set; }
        
        public string PreviewEmailContent { get; set; }
        
        public string PreviewEmailTo { get; set; }
        
        public string PreviewEmailCC { get; set; }
        
        public string PreviewEmailBcc { get; set; }
        
        public string PreviewSmsPhone { get; set; }
        
        public string PreviewSmsContent { get; set; }

        public List<FileInFolderEntityModel> ListPreviewFile { get; set; }
    }
}
