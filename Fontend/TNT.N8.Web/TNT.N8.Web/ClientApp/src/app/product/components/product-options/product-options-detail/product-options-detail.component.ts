import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MenuItem, MessageService } from 'primeng/api';
import { CreateOrUpdateOptionsModel } from '../model/options';
import { InfoComponent } from './info/info.component';
import { ListMissionComponent } from './list-mission/list-mission.component';
import { PropertiesComponent } from './properties/properties.component';

@Component({
  selector: 'app-product-options-detail',
  templateUrl: './product-options-detail.component.html',
  styleUrls: ['./product-options-detail.component.css']
})
export class ProductOptionsDetailComponent implements OnInit {
  loading = false;
  items: MenuItem[];
  @ViewChild(ListMissionComponent) createMilestoneOutput;
  @ViewChild(InfoComponent) infoComponentOutput;
  @ViewChild(PropertiesComponent) propertiesComponentOutput;
  createOrUpdateOptionsModel: CreateOrUpdateOptionsModel = new CreateOrUpdateOptionsModel();
  activeItem: MenuItem;
  id: string = null;

  constructor(
    private messageService: MessageService,
    private _activatedRoute: ActivatedRoute,
  ) { }

  ngOnInit(): void {
    this._activatedRoute.params.subscribe(params => {
      this.id = params['optionId'];
    });

    this.items = [
      { label: 'Thông tin chung', },
      { label: 'Thuộc tính' },
      { label: 'Nhà cung cấp' },
    ];
    this.activeItem = this.items[0];
  }
  
  showToast(severity: string, summary: string, detail: string): void {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

}
