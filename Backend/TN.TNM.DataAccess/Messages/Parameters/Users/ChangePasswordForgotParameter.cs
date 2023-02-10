namespace TN.TNM.DataAccess.Messages.Parameters.Users
{
    public class ChangePasswordForgotParameter : BaseParameter
    {
        public string Code { get; set; }

        public string UserName { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
