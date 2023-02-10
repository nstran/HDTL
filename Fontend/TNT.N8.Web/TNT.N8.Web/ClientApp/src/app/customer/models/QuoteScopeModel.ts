export class QuoteScopeModel {
  scopeId: string;
  tt: string;
  category: string;
  description: string;
  quoteId: string;
  level: Number;
  parentId: string;
  scopeChildList: Array<QuoteScopeModel>;
  children: Array<any>;

  constructor() { }

  //constructor() {
  //  this.scopeId = '00000000-0000-0000-0000-000000000000',
  //    this.tt = '0',
  //    this.quoteId = '00000000-0000-0000-0000-000000000000',
  //    this.category = '',
  //    this.description = ''
  //}
}
