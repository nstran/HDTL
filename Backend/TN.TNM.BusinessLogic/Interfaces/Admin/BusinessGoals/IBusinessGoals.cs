using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.BusinessGoals;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.BusinessGoals;

namespace TN.TNM.BusinessLogic.Interfaces.Admin.BusinessGoals
{
    public interface IBusinessGoals
    {
        GetMasterDataBusinessGoalsResponse GetMasterDataBusinessGoals(GetMasterDataBusinessGoalsRequest request);
        CreateOrUpdateBusinessGoalsResponse CreateOrUpdateBusinessGoals(CreateOrUpdateBusinessGoalsRequest request);
    }
}
