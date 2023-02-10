using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Document;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class EditLeadByIdResponse : BaseResponse
    {
        public bool IsChangePic { get; set; }
        public bool IsChangePotential { get; set; }
        public bool IsChangeStatus { get; set; }
        public string PicName { get; set; }
        public string Potential { get; set; }
        public string StatusName { get; set; }
        public DataAccess.Models.Email.SendEmailEntityModel SendEmailEntityModel { get; set; }

        public List<FileInFolderEntityModel> ListFile { get; set; }
        public List<LinkOfDocumentEntityModel> ListLinkOfDocument { get; set; }
    }
}
