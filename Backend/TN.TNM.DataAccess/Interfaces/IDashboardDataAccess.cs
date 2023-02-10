using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Dashboard;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;
using TN.TNM.DataAccess.Messages.Results.Admin.Dashboard;
using TN.TNM.DataAccess.Messages.Results.Admin.Product;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IDashboardDataAccess
    {
        TakeRevenueStatisticDashboardResult TakeRevenueStatisticDashboard(TakeRevenueStatisticDashboardParameter parameter);

        TakeRevenueStatisticWaitPaymentDashboardResult TakeRevenueStatisticWaitPaymentDashboard(TakeRevenueStatisticDashboardParameter parameter);

        TakeStatisticServiceTicketDashboardResult TakeStatisticServiceTicketDashboard(TakeRevenueStatisticDashboardParameter parameter);

        TakeRevenueStatisticEmployeeDashboardResult TakeRevenueStatisticEmployeeDashboard(TakeRevenueStatisticDashboardByFilterParameter parameter);

        TakeRevenueStatisticServicePacketDashboardResult TakeRevenueStatisticServicePacketDashboard(TakeRevenueStatisticDashboardByFilterParameter parameter);

        TakeRatingStatistictDashboardResult TakeRatingStatisticDashboard(TakeRevenueStatisticDashboardByFilterParameter parameter);
    }
}
