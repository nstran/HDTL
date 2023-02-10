using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Xml;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Dashboard;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;
using TN.TNM.DataAccess.Messages.Results.Admin.Dashboard;
using TN.TNM.DataAccess.Messages.Results.Admin.Product;

namespace TN.TNM.Api.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardDataAccess iDashboardDataAccess;

        public DashboardController(
            IDashboardDataAccess _iDashboardDataAccess
        ) 
        {
            iDashboardDataAccess = _iDashboardDataAccess;
        }

        [HttpPost]
        [Route("api/Dashboard/takeRevenueStatisticDashboard")]
        [Authorize(Policy = "Member")]
        public TakeRevenueStatisticDashboardResult TakeRevenueStatisticDashboard(TakeRevenueStatisticDashboardParameter request)
        {
            return this.iDashboardDataAccess.TakeRevenueStatisticDashboard(request);
        }

        [HttpPost]
        [Route("api/Dashboard/takeRevenueStatisticWaitPaymentDashboard")]
        [Authorize(Policy = "Member")]
        public TakeRevenueStatisticWaitPaymentDashboardResult TakeRevenueStatisticWaitPaymentDashboard(TakeRevenueStatisticDashboardParameter request)
        {
            return this.iDashboardDataAccess.TakeRevenueStatisticWaitPaymentDashboard(request);
        }

        [HttpPost]
        [Route("api/Dashboard/takeStatisticServiceTicketDashboard")]
        [Authorize(Policy = "Member")]
        public TakeStatisticServiceTicketDashboardResult TakeStatisticServiceTicketDashboard(TakeRevenueStatisticDashboardParameter request)
        {
            return this.iDashboardDataAccess.TakeStatisticServiceTicketDashboard(request);
        }

        [HttpPost]
        [Route("api/Dashboard/takeRevenueStatisticEmployeeDashboard")]
        [Authorize(Policy = "Member")]
        public TakeRevenueStatisticEmployeeDashboardResult TakeRevenueStatisticEmployeeDashboard([FromBody] TakeRevenueStatisticDashboardByFilterParameter request)
        {
            return this.iDashboardDataAccess.TakeRevenueStatisticEmployeeDashboard(request);
        }

        [HttpPost]
        [Route("api/Dashboard/takeRevenueStatisticServicePacketDashboard")]
        [Authorize(Policy = "Member")]
        public TakeRevenueStatisticServicePacketDashboardResult TakeRevenueStatisticServicePacketDashboard([FromBody]TakeRevenueStatisticDashboardByFilterParameter request)
        {
            return this.iDashboardDataAccess.TakeRevenueStatisticServicePacketDashboard(request);
        }

        [HttpPost]
        [Route("api/Dashboard/takeRatingStatisticDashboard")]
        [Authorize(Policy = "Member")]
        public TakeRatingStatistictDashboardResult TakeRatingStatisticDashboard([FromBody] TakeRevenueStatisticDashboardByFilterParameter request)
        {
            return this.iDashboardDataAccess.TakeRatingStatisticDashboard(request);
        }
    }
}
