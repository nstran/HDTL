import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { map } from 'rxjs/operators';
import { CreateOrUpdateMilestoneConfigurationResult, ListMilestoneConfigurationModel, MilestoneConfigurationEntityModel } from '../model/list-milestone';

@Injectable({
  providedIn: 'root'
})
export class ListMilestoneService {

  constructor(private httpClient: HttpClient) { }
  getLlistMilestoneService() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/milestone/getListMilestoneConfiguration';
    return this.httpClient.get(url, {}).pipe(
      map((response: ListMilestoneConfigurationModel) => {
        return <ListMilestoneConfigurationModel>response;
      }));
  }
  deleteMilestoneConfigure(id: string): Observable<CreateOrUpdateMilestoneConfigurationResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/milestone/deleteMilestoneConfiguration';
    return this.httpClient.post(url, {
      Id: id
    }).pipe(
      map((response: CreateOrUpdateMilestoneConfigurationResult) => {
        return <CreateOrUpdateMilestoneConfigurationResult>response;
      }));
  }
  createMilestoneConfigure(milestoneConfigurationEntityModel: MilestoneConfigurationEntityModel): Observable<CreateOrUpdateMilestoneConfigurationResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/milestone/createMilestoneConfiguration';
    return this.httpClient.post(url, {
      MilestoneConfigurationEntityModel: milestoneConfigurationEntityModel
    }).pipe(
      map((response: CreateOrUpdateMilestoneConfigurationResult) => {
        return <CreateOrUpdateMilestoneConfigurationResult>response;
      }));
  }
  getlistMilestoneServiceByOptionId(id: string): Observable<ListMilestoneConfigurationModel> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/milestone/getListMilestoneConfigurationByOptionId';
    return this.httpClient.post(url, {
      Id: id
    }).pipe(
      map((response: ListMilestoneConfigurationModel) => {
        return <ListMilestoneConfigurationModel>response;
      }));
  }
  UpdateListMilestoneConfigure(milestoneConfigurationEntityModel: MilestoneConfigurationEntityModel[]): Observable<CreateOrUpdateMilestoneConfigurationResult> {
    const url = localStorage.getItem('ApiEndPoint') + '/api/milestone/updateListMilestoneConfigure';
    return this.httpClient.post(url, {
      ListMilestoneConfigurationEntityModel: milestoneConfigurationEntityModel
    }).pipe(
      map((response: CreateOrUpdateMilestoneConfigurationResult) => {
        return <CreateOrUpdateMilestoneConfigurationResult>response;
      }));
  }
}
