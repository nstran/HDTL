export class QuotePlanModel {
  planId: string;
  tt: number;
  finished: string;
  execTime: string;
  sumExecTime: string;
  quoteId: string;

  constructor() {
      this.planId = '00000000-0000-0000-0000-000000000000',
      this.tt = 0,
      this.quoteId = '00000000-0000-0000-0000-000000000000',
      this.finished = '',
      this.execTime = '',
      this.sumExecTime = ''
  }
}
