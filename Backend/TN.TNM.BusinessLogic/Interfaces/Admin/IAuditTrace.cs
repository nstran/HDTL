using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Messages.Requests.Admin;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.AuditTrace;
using TN.TNM.BusinessLogic.Messages.Responses.Admin;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.AuditTrace;

namespace TN.TNM.BusinessLogic.Interfaces.Admin
{
    public interface IAuditTrace
    {
        /// <summary>
        /// Get list nhat ky dang nhap he thong va nhat ky thay doi cac doi tuong trong he thong
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        GetMasterDataTraceResponses GetMasterDataTrace(GetMasterDataTraceRequests requests);

        /// <summary>
        /// Search nhật ký theo tiêu chí
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SearchTraceResponses SearchTrace(SearchTraceRequest request);
    }
}
