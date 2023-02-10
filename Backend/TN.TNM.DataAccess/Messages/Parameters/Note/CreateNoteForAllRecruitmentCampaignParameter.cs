using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Note
{
    public class CreateNoteForAllRecruitmentCampaignParameter : BaseParameter
    {
        public Databases.Entities.Note Note { get; set; }

        public string ObjType { get; set; }

        public List<FileUploadEntityModel> ListNoteDocument { get; set; }
    }
}
