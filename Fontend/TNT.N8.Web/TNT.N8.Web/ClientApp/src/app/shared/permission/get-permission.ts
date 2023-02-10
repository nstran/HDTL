import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class GetPermission {

    constructor(private httpClient: HttpClient) { }

    getUserPermission() {
        const url = localStorage.getItem('ApiEndPoint') + '/api/auth/getUserPermission';
        return new Promise((resolve, reject) => {
            return this.httpClient.post(url, {}).toPromise()
                .then((response: Response) => {
                    resolve(response);
                });
        });
    }

    async getPermission(resource: string) {
        let listCurrentActionResource: Array<string> = [];

        let result: any = await this.getUserPermission();

        if (result.statusCode != 200) {
            return { status: false, listCurrentActionResource: [] }
        }

        let listPermissionResource = result.listPermissionResource.toString();

        let listAllPermission: Array<string> = listPermissionResource.split(",");
        listAllPermission.forEach(item => {
            if (item.trim().toLowerCase().indexOf(resource.toLowerCase()) != -1) {
                let action = item.toLowerCase().slice(item.toLowerCase().lastIndexOf("/") + 1);
                listCurrentActionResource.push(action);
            }
        });

        //Nếu User không có quyền gì thì chuyển về trang home
        if (listCurrentActionResource.length <= 0) {
            return { status: false, listCurrentActionResource: [] }
        }

        //Nếu User không có quyền view thì chuyển về trang home
        if (listCurrentActionResource.indexOf("view") == -1) {
            return { status: false, listCurrentActionResource: [] }
        }

        return { status: true, listCurrentActionResource: listCurrentActionResource }
    }

    async getPermissionLocal(listPermissionResource: string, resource: string) {
        let listCurrentActionResource: Array<string> = [];
        let listAllPermission: Array<string> = listPermissionResource.split(",");
        listAllPermission.forEach(item => {
            if (item.toLowerCase().indexOf(resource.toLowerCase()) != -1) {
                let action = item.toLowerCase().slice(item.toLowerCase().lastIndexOf("/") + 1);
                listCurrentActionResource.push(action);
            }
        });

        //Nếu User không có quyền gì thì chuyển về trang home
        if (listCurrentActionResource.length <= 0) {
            return { status: false, listCurrentActionResource: [] }
        }

        //Nếu User không có quyền view thì chuyển về trang home
        if (listCurrentActionResource.indexOf("view") == -1) {
            return { status: false, listCurrentActionResource: [] }
        }

        return { status: true, listCurrentActionResource: listCurrentActionResource }
    }
}
