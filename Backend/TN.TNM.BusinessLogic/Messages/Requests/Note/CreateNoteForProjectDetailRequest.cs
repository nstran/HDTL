using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Folder;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.DataAccess.Messages.Parameters.Note;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;
using NoteModel = TN.TNM.BusinessLogic.Models.Note.NoteModel;

namespace TN.TNM.BusinessLogic.Messages.Requests.Note
{
    public class CreateNoteForProjectDetailRequest : BaseRequest<CreateNoteForProjectDetailParameter>
    {
        public NoteModel Note { get; set; }

        public string FolderType { get; set; }

        public List<FileUploadModel> ListNoteDocument { get; set; }

        public override CreateNoteForProjectDetailParameter ToParameter()
        {
            var parameter = new CreateNoteForProjectDetailParameter()
            {
                UserId = UserId,
                Note = Note.ToEntity(),
                FolderType = FolderType,
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
