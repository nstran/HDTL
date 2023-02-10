using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.BusinessGoals;
using TN.TNM.DataAccess.Messages.Results.Admin.BusinessGoals;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IBusinessGoalsDataAccess
    {
        GetMasterDataBusinessGoalsResult GetMasterDataBusinessGoals(GetMasterDataBusinessGoalsParameter parameter);
        CreateOrUpdateBusinessGoalsResult CreateOrUpdateBusinessGoals(CreateOrUpdateBusinessGoalsParameter parameter);
    }
}
