using System;
using TN.TNM.DataAccess.Messages.Parameters.Asset;
using TN.TNM.DataAccess.Messages.Parameters.Contract;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Parameters.FireBase;
using TN.TNM.DataAccess.Messages.Results.Asset;
using TN.TNM.DataAccess.Messages.Results.Employee;
using TN.TNM.DataAccess.Messages.Results.FireBase;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IFireBaseDataAccess
    {
        TakeDataFireBaseByUserIdResult TakeDataFireBaseByUserId(TakeDataFireBaseByUserIdParameter parameter);

        CreateDataFireBaseResult CreateDataFireBase(CreateDataFireBaseParameter parameter);
    }
}
