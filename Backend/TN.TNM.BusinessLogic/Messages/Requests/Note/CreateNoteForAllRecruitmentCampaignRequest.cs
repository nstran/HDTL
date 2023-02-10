using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Folder;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.DataAccess.Messages.Parameters.Note;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Requests.Note
{
    public class CreateNoteForAllRecruitmentCampaignRequest : BaseRequest<CreateNoteForAllRecruitmentCampaignParameter>
    {
        public DataAccess.Models.Note.NoteModel Note { get; set; }

        public string ObjType { get; set; }
        public List<FileUploadModel> ListNoteDocument { get; set; }

        public override CreateNoteForAllRecruitmentCampaignParameter ToParameter()
        {
            var parameter = new CreateNoteForAllRecruitmentCampaignParameter()
            {
                UserId = UserId,
                Note = Note.ToEntity(),
                ObjType = ObjType,
                ListNoteDocument = new List<FileUploadEntityModel>()
            };

            if (ListNoteDocument?.Count > 0)
            {
                ListNoteDocument.ForEach(item =>
                {
                    var file = new FileUploadEntityModel();
                    file.FileInFolder = item.FileInFolder.ToEntityModel();
                    file.FileSave = item.FileSave;
                    parameter.ListNoteDocument.Add(file);
                });
            }

            return parameter;
        }
    }
}
