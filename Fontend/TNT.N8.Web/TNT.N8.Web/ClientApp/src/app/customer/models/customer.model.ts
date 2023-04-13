export class CustomerModel {
  CustomerId: string;
  CustomerCode: string;
  CustomerGroupId: string | null;
  CustomerName: string;
  LeadId: string;
  StatusId: string;
  CustomerServiceLevelId: string;
  CustomerServiceLevelName: string;
  PersonInChargeId: string;
  CustomerCareStaff: string;
  CustomerType: number;
  PaymentId: string;
  FieldId: string;
  ScaleId: string;
  TotalSaleValue: number;
  TotalReceivable: number;
  NearestDateTransaction: Date;
  MaximumDebtValue: number;
  MaximumDebtDays: number;
  MainBusinessSector: string;
  BusinessRegistrationDate: Date;
  EnterpriseType: string;
  BusinessType: string;
  IsGraduated: boolean;
  TotalEmployeeParticipateSocialInsurance: number;
  BusinessScale: string;
  TotalCapital: number;
  TotalRevenueLastYear: number;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: string;
  Active: boolean;
  IsApproval: boolean;
  ApprovalStep: number;
  InvestmentFundId: string;
  StatusCareId: string;
  EmployeeTakeCareId: string;
  ContactDate: Date;
  SalesUpdate: string;
  EvaluateCompany: string;
  SalesUpdateAfterMeeting: string;
  AreaId: string;
  KhachDuAn: boolean;
  SubjectsApplication : boolean;
  StaffChargeIds : string[];

  /**
   *
   */
  constructor() {
    this.EmployeeTakeCareId = '00000000-0000-0000-0000-000000000000',
    this.ContactDate = null,
    this.SalesUpdate = '',
    this.EvaluateCompany = '',
    this.SalesUpdateAfterMeeting = ''
  }
}

export class TakeListEvaluateResult {
  listOrderProcessMappingEmployee : OrderProcessMappingEmployeeEntityModel[];
  statusCode: number;
  message : string;
}
export class OrderProcessMappingEmployeeEntityModel {
  employeeId: string | null;
  customerId: string | null;
  customerName: string;
  orderProcessId: string | null;
  rateContent: string;
  createdDate: string;
  orderCode: string;
}