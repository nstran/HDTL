using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.MobileAppConfiguration;
using TN.TNM.DataAccess.Messages.Results.Admin;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IMobileAppConfigurationDataAccess
    {
        CreateOrEditMobileAppConfigurationResult CreateOrEditMobileAppConfiguration(CreateOrEditMobileAppConfigurationParameter parameter);
        TakeMobileAppConfigurationResult TakeMobileAppConfiguration(TakeMobileAppConfigurationParameter parameter);
        TakeMobileAppConfigurationResult TakeMobileAppConfigurationIntro(TakeMobileAppConfigurationParameter parameter);
        TakeMobileAppConfigurationResult TakeMobileAppConfigurationLoginScreen(TakeMobileAppConfigurationParameter parameter);
        TakeMobileAppConfigurationResult TakeMobileAppConfigurationLoginAndRegister(TakeMobileAppConfigurationParameter parameter);
        TakeMobileAppConfigurationResult TakeMobileAppConfigurationPayment(TakeMobileAppConfigurationParameter parameter);
        TakeMobileAppConfigurationResult TakeMobileAppConfigurationNotificationImage(TakeMobileAppConfigurationParameter parameter);
        CreateOrUpdatePaymentMethodResult CreateOrUpdatePaymentMethod(CreateOrUpdatePaymentMethodParameter parameter);
        CreateOrUpdatePaymentMethodResult DeletePaymentMethod(CreateOrUpdatePaymentMethodParameter parameter);
        CreateOrEditMobileAppConfigurationResult CreateOrEditAdvertisementConfiguration(CreateOrEditAdvertisementConfigurationParameter parameter);
        CreateOrEditMobileAppConfigurationResult DeleteAdvertisementConfiguration(DeleteAdvertisementConfigurationParameter parameter);
    }
}
