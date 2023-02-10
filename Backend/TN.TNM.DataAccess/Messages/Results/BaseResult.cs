using System;
using System.Collections.Generic;
using System.Net;
using TN.TNM.Common.CommonObject;

namespace TN.TNM.DataAccess.Messages.Results
{

    public class BaseResult
    {
        public bool Status { get; set; }

        public string Message { get; set; }

        public List<NoteObject> Notes { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string MessageCode { get; set; }
    }
}