export class ListTaskSearch {
    public listTaskTypeId: Array<string>;
    public listStatusId: Array<string>;
    public listPersonInChargedId: Array<string>;
    public listCreatedId: Array<string>;
    public listPriority: Array<number>;
    public fromDate: string;
    public toDate: string;
    public selectedOption: string;
    public selectedOptionStatus: string;

    constructor() {
        this.listTaskTypeId = [];
        this.listStatusId = [];
        this.listPersonInChargedId = [];
        this.listCreatedId = [];
        this.listPriority = [];
        this.fromDate = null;
        this.toDate = null;
        this.selectedOption = "all";
        this.selectedOptionStatus = "TEMP";
    }
}