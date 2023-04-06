using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Consts.Product
{
    public class ProductConsts
    {
        //CategoryType
        public const string CategoryTypeCodeService = "SERVICE";
        public const string CategoryTypeCodeOptionType = "OPTIONTYPE";
        public const string CategoryTypeCodeATR = "ATTRIBUTE";
        public const string CategoryTypeCodeActionStep = "ACTIONSTEP";
        public const string CategoryTypeCodeNotificationConfig = "NOTIFICATIONCONFIG";
        public const string CategoryTypeCodeUnit = "CategoryUnit";
        public const string CategoryTypeCodeSubjectsApplication = "SubjectsApplication";

        //CategoryType
        public const string CategoryCodeUserReview = "UserReview";
        public const string CategoryCodeUpMobile = "UpMobile";

        //Danh mục nhiệm vụ
        public const string CategoryTypeCodeEmpMission = "EmpMission";


        //Bước hỗ trợ dịch vụ ( nhân viên cập nhật các điểm báo cáo) CategoryCode
        public const string CategoryCodeCreateStep = "CREATE";
        public const string CategoryCodeConfirmStep = "CONFIRM";
        public const string CategoryCodeCforderStep = "CFORDER";
        public const string CategoryCodeSupportStep = "SUPPORT";
        public const string CategoryCodeDoneStep = "DONE";
    }
    public class OptionConsts
    {
        public const string OptionServiceName = "OPTIONSNAME";
    }
}
