using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.MilestoneConfiguration;
using TN.TNM.DataAccess.Messages.Results.MilestoneConfiguration;
using TN.TNM.DataAccess.Messages.Results.Options;
using Task = System.Threading.Tasks.Task;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IMilestoneConfigurationDataAccess 
    {
        Task<GetListMilestoneConfigurationResult> GetListMilestoneConfigurationResult();
        Task<CreateOrUpdateMilestoneConfigurationResult> CreateOrUpdateMilestoneConfiguration(CreateOrUpdateMilestoneConfigurationParameter parameter);
        Task<MilestoneConfigurationResult> DeleteMilestoneConfiguration(DeleteMilestoneConfigurationParameter request);
        Task<GetListMilestoneConfigurationResult> GetListMilestoneConfigurationByOptionIdResult(GetListMilestoneConfigurationByOptionIdParameter parameter);
        Task<UpdateListMilestoneConfigurationResult> UpdateListMilestoneConfigure(UpdateListMilestoneConfigurationParameter parameter);
    }
}
