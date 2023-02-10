import { ApisauceInstance, ApiResponse } from "apisauce";
import { Storage, StorageKey } from "../storage/index";

export class UserApi {
  private _apisauce: ApisauceInstance;
  private _storage = new Storage();
  constructor(apisauce: ApisauceInstance) {
    this._apisauce = apisauce;
  }

  async login(payload: any): Promise<any> {
    try {
      const response = await this._apisauce.post(`/api/auth`, payload);
      return response.data;
    } catch (error) {
      console.log(error);
    }
  }
  async signUp(payload: any): Promise<any> {
    try {
      const response = await this._apisauce.post(`/api/auth/register`, payload);
      return response.data;
    } catch (error) {
      console.log(error);
    }
  }

  async getListProvince(): Promise<any> {
    try {
      const response = await this._apisauce.post(`/api/auth/takeListProvince`, {});
      return response.data;
    } catch (error) {
      console.log(error);
    }
  }

  async editUserProfile(payload: any): Promise<any> {
    const accessToken = await this._storage.getItem(StorageKey.TOKEN);
    const response = await this._apisauce.post(
      `/api/auth/editUserProfile`,
      payload,
      { headers: { Authorization: `Bearer ${accessToken}` } }
    );
    return response.data;
  }

  async changePassword(payload: any): Promise<any> {
    const accessToken = await this._storage.getItem(StorageKey.TOKEN);
    const response = await this._apisauce.post(
      `/api/auth/changePassword`,
      payload,
      { headers: { Authorization: `Bearer ${accessToken}` } }
    );
    return response.data;
  }

  async getListServicePacket(payload: any): Promise<any> {
    const accessToken = await this._storage.getItem(StorageKey.TOKEN);
    const response = await this._apisauce.post(
      `/api/Product/getListServicePacket`,
      payload,
      { headers: { Authorization: `Bearer ${accessToken}` } }
    );
    return response.data;
  }

  async searchOptionOfPacketService(payload: any): Promise<any> {
    const accessToken = await this._storage.getItem(StorageKey.TOKEN);
    const response = await this._apisauce.post(
      `/api/order/searchOptionOfPacketService`,
      payload,
      { headers: { Authorization: `Bearer ${accessToken}` } }
    );
    return response.data;
  }

  async TakeMobileAppConfigurationIntro(payload: any): Promise<any> {
    const accessToken = await this._storage.getItem(StorageKey.TOKEN);
    const response = await this._apisauce.post(
      `/api/MobileAppConfiguration/TakeMobileAppConfigurationIntro`,
      payload,
      { headers: { Authorization: `Bearer ${accessToken}` } }
    );
    return response.data;
  }
  async TakeMobileAppConfigurationLoginAndRegister(payload: any): Promise<any> {
    const accessToken = await this._storage.getItem(StorageKey.TOKEN);
    const response = await this._apisauce.post(
      `/api/MobileAppConfiguration/TakeMobileAppConfigurationLoginAndRegister`,
      payload,
      { headers: { Authorization: `Bearer ${accessToken}` } }
    );
    return response.data;
  }

  async TakeMobileAppConfigurationPayment(payload: any): Promise<any> {
    const accessToken = await this._storage.getItem(StorageKey.TOKEN);
    const response = await this._apisauce.post(
      `/api/MobileAppConfiguration/TakeMobileAppConfigurationPayment`,
      payload,
      { headers: { Authorization: `Bearer ${accessToken}` } }
    );
    return response.data;
  }

  async createCustomerOrder(payload: any): Promise<any> {
    const accessToken = await this._storage.getItem(StorageKey.TOKEN);
    const response = await this._apisauce.post(
      `/api/order/createCustomerOrder`,
      payload,
      { headers: { Authorization: `Bearer ${accessToken}` } }
    );
    return response.data;
  }

  async changeStatusCustomerOrder(payload: any): Promise<any> {
    const accessToken = await this._storage.getItem(StorageKey.TOKEN);
    const response = await this._apisauce.post(
      `/api/order/changeStatusCustomerOrder`,
      payload,
      { headers: { Authorization: `Bearer ${accessToken}` } }
    );
    return response.data;
  }

  async getListOrderOfCus(payload: any): Promise<any> {
    const accessToken = await this._storage.getItem(StorageKey.TOKEN);
    const response = await this._apisauce.post(
      `/api/order/getListOrderOfCus`,
      payload,
      { headers: { Authorization: `Bearer ${accessToken}` } }
    );
    return response.data;
  }

  async getMasterDataOrderDetail(payload: any): Promise<any> {
    const accessToken = await this._storage.getItem(StorageKey.TOKEN);
    const response = await this._apisauce.post(
      `/api/order/getMasterDataOrderDetail`,
      payload,
      { headers: { Authorization: `Bearer ${accessToken}` } }
    );
    return response.data;
  }

  async deleteOrderOptionByCus(payload: any): Promise<any> {
    const accessToken = await this._storage.getItem(StorageKey.TOKEN);
    const response = await this._apisauce.post(
      `api/order/deleteOrderOptionByCus`,
      payload,
      { headers: { Authorization: `Bearer ${accessToken}` } }
    );
    return response.data;
  }

  async getDashboardDoanhthu(payload: any): Promise<any> {
    const accessToken = await this._storage.getItem(StorageKey.TOKEN);
    const response = await this._apisauce.post(
      `/api/Dashboard/takeRevenueStatisticServicePacketDashboard`,
      payload,
      { headers: { Authorization: `Bearer ${accessToken}` } }
    );
    return response.data;
  }

  async getDashboardChoThanhToan(payload: any): Promise<any> {
    const accessToken = await this._storage.getItem(StorageKey.TOKEN);
    const response = await this._apisauce.post(
      `/api/Dashboard/takeRevenueStatisticWaitPaymentDashboard`,
      payload,
      { headers: { Authorization: `Bearer ${accessToken}` } }
    );
    return response.data;
  }

  async takeMobileAppConfiguration(payload: any): Promise<any> {
    const accessToken = await this._storage.getItem(StorageKey.TOKEN);
    const response = await this._apisauce.post(
      `/api/MobileAppConfiguration/takeMobileAppConfiguration`,
      payload,
      { headers: { Authorization: `Bearer ${accessToken}` } }
    );
    return response.data;
  }

  async removeDeviceId(payload: any): Promise<any> {
    const accessToken = await this._storage.getItem(StorageKey.TOKEN);
    const response = await this._apisauce.post(
      `api/auth/removeDeviceId`,
      payload,
      { headers: { Authorization: `Bearer ${accessToken}` } }
    );
    return response.data;
  }

  async createDataFireBase(payload: any): Promise<any> {
    const accessToken = await this._storage.getItem(StorageKey.TOKEN);
    const response = await this._apisauce.post(
      `/api/FireBase/createDataFireBase`,
      payload,
      { headers: { Authorization: `Bearer ${accessToken}` } }
    );
    return response.data;
  }

  // api lấy thông tin admin đánh giá
  async takeRatingStatisticDashboard(payload: any): Promise<any> {
    const accessToken = await this._storage.getItem(StorageKey.TOKEN);
    const response = await this._apisauce.post(
      `/api/Dashboard/takeRatingStatisticDashboard`,
      payload,
      { headers: { Authorization: `Bearer ${accessToken}` } }
    );
    return response.data;
  }

  // api lấy thông tin dashboard admin phiếu yêu cầu
  async takeStatisticServiceTicketDashboard(payload: any): Promise<any> {
    const accessToken = await this._storage.getItem(StorageKey.TOKEN);
    const response = await this._apisauce.post(
      `/api/Dashboard/takeStatisticServiceTicketDashboard`,
      payload,
      { headers: { Authorization: `Bearer ${accessToken}` } }
    );
    return response.data;
  }
}
