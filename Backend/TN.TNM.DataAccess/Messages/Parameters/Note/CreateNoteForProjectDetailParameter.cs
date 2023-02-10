using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Parameters.Note
{
    public class CreateNoteForProjectDetailParameter : BaseParameter
    {
        public Databases.Entities.Note Note { get; set; }

        public string FolderType { get; set; }

        public List<FileUploadEntityModel> ListNoteDocument { get; set; }
    }
}
