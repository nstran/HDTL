export class ListOrderSearch {
  orderCode: string;
  customerName: string;
  listStatusId: Array<string>;
  phone: string;
  fromDate: string;
  toDate: string;
  vat: number;
  productId: string;
  quoteId: string;
  contractId: string;

  constructor() {
    this.orderCode = null;
    this.customerName = null;
    this.listStatusId = [];
    this.phone = null;
    this.fromDate = null;
    this.toDate = null;
    this.vat = 1;
    this.productId = null;
    this.quoteId = null;
    this.contractId = null;
  }
}