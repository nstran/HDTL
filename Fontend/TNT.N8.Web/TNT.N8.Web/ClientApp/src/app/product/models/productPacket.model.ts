

export class ServicePacket{
  id: string;
  name: string;
  attributeName: string;
  message: string;
  locationId: string;
  status: string;
  productCategoryId: string;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  tenantId: string;
  constructor() {
    this.name = "";
    this.id = "00000000-0000-0000-0000-000000000000";
    this.createdDate = new Date();
  }
}
