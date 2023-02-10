using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class GetBaoCaoTongHopCacDuAnResult : BaseResult
    {
        public List<ProjectPipelineModel> ListProjectPipeline { get; set; }
    }
}
