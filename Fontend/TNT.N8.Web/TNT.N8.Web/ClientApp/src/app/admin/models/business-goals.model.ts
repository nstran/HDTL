export class BusinessGoals {
    businessGoalsId: string;
    organizationId: string;
    year: string;
    active: boolean;
    createdDate: Date;
    createdById: string;
    updatedDate: Date;
    updatedById: string;

    constructor() {
        this.businessGoalsId = '00000000-0000-0000-0000-000000000000';
        this.organizationId = '00000000-0000-0000-0000-000000000000';
    }
}

export class BusinessGoalsDetail {
    businessGoalsDetailId: string;
    businessGoalsId: string;
    productCategoryId: string;
    businessGoalsType: string;

    january: number;
    february: number;
    march: number;
    april: number;
    may: number;
    june: number;
    july: number;
    august: number;
    september: number;
    october: number;
    november: number;
    december: number;

    productCategoryCode: string;
    productCategoryName: string;
    organizationName: string;
    organizationId: string;

    active: boolean;

    constructor() {
        this.businessGoalsDetailId = '00000000-0000-0000-0000-000000000000';
        this.businessGoalsId = '00000000-0000-0000-0000-000000000000';
        this.productCategoryId = '00000000-0000-0000-0000-000000000000';
        this.january = 0;
        this.february = 0;
        this.march = 0;
        this.april = 0;
        this.may = 0;
        this.june = 0;
        this.july = 0;
        this.august = 0;
        this.september = 0;
        this.october = 0;
        this.november = 0;
        this.december = 0;
        this.active = true;
    }
}