export class ForgotPassModel{
    UserId: string;
    UserName: string;
    FullName: string;
    EmailAddress: string;
    Password: string;
    Re_password: string;
}

export class ChangePasswordModel {
    code : string;
    userName: string;
    newPassword: string;
    confirmPassword: string
}

export class ChangePasswordResult {
    statusCode: number;
    messageCode: string;
    message : string;
}