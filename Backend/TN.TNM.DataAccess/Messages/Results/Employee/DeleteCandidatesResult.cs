using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class DeleteCandidatesResult : BaseResult
    {
        public List<NoteEntityModel> ListNote { get; set; }
        public List<CandidateEntityModel> ListCandidate { get; set; }
    }
}
