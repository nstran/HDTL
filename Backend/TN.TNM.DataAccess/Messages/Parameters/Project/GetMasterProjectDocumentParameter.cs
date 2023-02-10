using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class GetMasterProjectDocumentParameter : BaseParameter
    {
        public Guid ProjectId { get; set; }
    }
}
