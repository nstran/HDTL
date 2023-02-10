export class QuotePaymentTerm {
    paymentTermId: string;
    quoteId: string;
    orderNumber: number;
    milestone: string;
    paymentPercentage: string;
    createdDate: Date;
    CreatedById: string;

    constructor() {
        this.paymentTermId = '00000000-0000-0000-0000-000000000000',
        this.quoteId = '00000000-0000-0000-0000-000000000000',
        this.orderNumber = 0,
        this.milestone = '',
        this.paymentPercentage = '',
        this.createdDate = null,
        this.CreatedById = '00000000-0000-0000-0000-000000000000'
    }
}
