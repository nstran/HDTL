export class ListCustomerSearch {
  public noPic: boolean; //Chưa có người phụ trách
  public isBusinessCus: boolean; //Là KH doanh nghiệp
  public isPersonalCus: boolean;  //Là KH cá nhân
  public isAgentCus: boolean;  //Là KH đại lý
  public statusCareId: string; //Trạng thái chăm sóc
  public customerGroupIdList: Array<string>; //Nhóm KH
  public personInChargeId: string; //Người phụ trách
  public nhanVienChamSocId: string; //Nhân viên chăm sóc
  public areaId: string; //Khu vực
  public fromDate: string; //Thời gian tạo từ
  public toDate: string;  //Thời gian tạo đến
  public firstName: string; //Họ tên
  public phone: string;
  public email: string;
  public adddress: string; //Địa chỉ

  constructor() {
    this.noPic = false;
    this.isBusinessCus = false;
    this.isPersonalCus = false;
    this.isAgentCus = false;
    this.statusCareId = null;
    this.customerGroupIdList = [];
    this.personInChargeId = null;
    this.nhanVienChamSocId = null;
    this.areaId = null;
    this.fromDate = null;
    this.toDate = null;
    this.firstName = null;
    this.phone = null;
    this.email = null;
    this.adddress = null;
  }
}