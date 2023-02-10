export class TakeRevenueStatisticDashboardResult {
    revenueStatisticModel : RevenueStatisticModel;
}

export class RevenueStatisticModel {
    revenueOfDay: number | null;
    revenueOfWeek: number | null;
    revenueOfMonth: number | null;
    revenueOfQuarter: number | null;
    revenueOfYear: number | null;
    revenueOfYesterday: number | null;
    revenueOfLastWeek: number | null;
    revenueOfLastMonth: number | null;
    revenueOfLastQuarter: number | null;
    revenueOfLastYear: number | null;
    revenueWaitPayment: number | null;
}

export class TakeRevenueStatisticWaitPaymentDashboardResult {
    listRevenueStatisticWaitPaymentModel : RevenueStatisticWaitPaymentModel[];
}

export class RevenueStatisticWaitPaymentModel {
    productCategoryName: string;
    amount: number | null;
    percent: number | null;
    percentOfServicePacket: number | null;
    servicePacketName: string;
    listRevenueStatisticWaitPaymentModel: RevenueStatisticWaitPaymentModel[];
}

export class DataHighChart {
    name: string | null;
    y: number | null;
}

export class TakeStatisticServiceTicketDashboardResult {
    listNewStatus : NewStatus[];
    listProgressStatus : ProgressStatus[];
    listDoneStatus : DoneStatus[];
    listCancelStatus : CancelStatus[];
}

export class NewStatus {
    productCategoryName: string;
    count: number | null;
    servicePacketName: string;
    listStatisticServicePacketNewStatus : NewStatus[];
}

export class ProgressStatus {
    productCategoryName: string;
    count: number | null;
    servicePacketName: string;
    listStatisticServicePacketProgressStatus : ProgressStatus[];
}

export class DoneStatus {
    productCategoryName: string;
    count: number | null;
    servicePacketName: string;
    listStatisticServicePacketDoneStatus : DoneStatus[];
}

export class CancelStatus {
    productCategoryName: string;
    count: number | null;
    servicePacketName: string;
    listStatisticServicePacketCancelStatus : CancelStatus[];
}

export class TakeRevenueStatisticEmployeeDashboardResult {
    listRevenueStatisticEmployeeModel : RevenueStatisticEmployeeModel[];
}

export class RevenueStatisticEmployeeModel {
    employeeName: string;
    amount: number | null;
}

export class TakeRevenueStatisticServicePacketDashboardResult {
    listRevenueStatisticServicePacketModel : RevenueStatisticServicePacketModel[];
    listRevenueStatisticServicePacketModelByServicePacket : RevenueStatisticServicePacketModel[];

}

export class RevenueStatisticServicePacketModel {
    productCategoryName: string;
    amount : number;
    listAmount: (number | null)[];
    servicePacketName : string | null;
    percentOfServicePacket : number | null;
}

export class TakeRatingStatistictDashboardResult {
    listRatingStatisticServicePacketModel : RatingStatisticServicePacketModel[];
    listRatingStatisticServicePacketModelByServicePacket : RatingStatisticServicePacketModel[];
    listRatingStatisticStarServicePacketModel : RatingStatisticStarServicePacketModel[];
}

export class RatingStatisticServicePacketModel {
    productCategoryName: string;
    servicePacketName: string;
    listRate: number[];
    rate : number;
}

export class RatingStatisticStarServicePacketModel {
    servicePacketName: string;
    count: number;
    rateStar : number;
}