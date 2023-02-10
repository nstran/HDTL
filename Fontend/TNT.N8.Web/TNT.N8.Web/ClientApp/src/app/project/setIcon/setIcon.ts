import { Injectable } from "@angular/core";
@Injectable()
export class SetIconTaskType {
    setIconTaskType(listTaskType: any): Array<any> {
        listTaskType.forEach(item => {
            switch (item.categoryCode) {
                case "TTYC":
                    item.icon = "far fa-lightbulb"
                    break;
                case "TD":
                    item.icon = "far fa-calendar-check"
                    break;
                case "Loi":
                    item.icon = "fas fa-bug";
                    break;
                case "KT":
                    item.icon = "fas fa-tasks";
                    break;
                case "BGNT":
                    item.icon = "far fa-share-square";
                    break;
                case "BT":
                    item.icon = "fas fa-toolbox";
                    break;
                case "NC":
                    item.icon = "fas fa-expand-arrows-alt";
                    break;
                case "XD":
                    item.icon = "far fa-chart-bar";
                    break;
                case "BH":
                    item.icon = "fas fa-shield-alt";
                    break;
                case "DT":
                    item.icon = "fas fa-book-open";
                    break;
                case "TKK":
                    item.icon = "fas fa-users-cog";
                    break;
                case "TK":
                    item.icon = "fas fa-swatchbook";
                    break;
                default:
                    item.icon = "fab fa-deviantart";
                    break;
            }
        });

        return listTaskType;
    }

    setIconTaskTypeInListTask(listTask: any): Array<any> {
        listTask.forEach(item => {
            if (item.taskTypeCode) {
                switch (item.taskTypeCode) {
                    case "TTYC":
                        item.icon = "far fa-lightbulb"
                        break;
                    case "TD":
                        item.icon = "far fa-calendar-check"
                        break;
                    case "Loi":
                        item.icon = "fas fa-bug";
                        break;
                    case "KT":
                        item.icon = "fas fa-tasks";
                        break;
                    case "BGNT":
                        item.icon = "far fa-share-square";
                        break;
                    case "BT":
                        item.icon = "fas fa-toolbox";
                        break;
                    case "NC":
                        item.icon = "fas fa-expand-arrows-alt";
                        break;
                    case "XD":
                        item.icon = "far fa-chart-bar";
                        break;
                    case "BH":
                        item.icon = "fas fa-shield-alt";
                        break;
                    case "DT":
                        item.icon = "fas fa-book-open";
                        break;
                    case "TKK":
                        item.icon = "fas fa-users-cog";
                        break;
                    case "TK":
                        item.icon = "fas fa-swatchbook";
                        break;
                    default:
                        item.icon = "fab fa-deviantart";
                        break;
                }
            }
        });

        return listTask;
    }
}
