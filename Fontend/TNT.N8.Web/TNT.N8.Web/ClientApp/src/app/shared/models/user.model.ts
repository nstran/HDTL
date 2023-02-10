import { Pipe } from '@angular/core';
@Pipe({ name: 'UserModel' })
export class UserModel {
  UserId: string;
  UserName: string;
  EmployeeCode: string;
  Password: string;
  EmployeeId: string;
  Disabled: Boolean;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  Active: Boolean;
}
