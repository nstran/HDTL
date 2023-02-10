export class QuoteDocument {
  quoteDocumentId: string;
  quoteId: string;
  documentName: string;
  documentSize: string;
  documentUrl: string;
  active: boolean;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  uploadByName: string;

  constructor() {
    this.quoteDocumentId = '00000000-0000-0000-0000-000000000000';
    this.quoteId = '00000000-0000-0000-0000-000000000000';
    this.documentName = '';
    this.documentSize = '';
    this.documentUrl = '';
    this.active = true;
    this.createdById = JSON.parse(localStorage.getItem('auth')).UserId;
    this.createdDate = new Date();
    this.updatedById = JSON.parse(localStorage.getItem('auth')).UserId;
    this.updatedDate = new Date();
    this.uploadByName = '';
  }
}
