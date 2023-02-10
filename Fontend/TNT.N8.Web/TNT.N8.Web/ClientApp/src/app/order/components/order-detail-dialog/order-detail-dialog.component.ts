import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService, TreeNode } from 'primeng/api';

import { CustomerOrderService } from '../../services/customer-order.service';

import { ProductCategoryService } from '../../../admin/components/product-category/services/product-category.service';
import { ServicePacket } from '../../../../../src/app/product/models/productPacket.model';
import { AttributeConfigurationEntityModel, CustomerOrderExtension, ServicePacketMappingOptionsEntityModel, TrangThaiGeneral } from '../../../../../src/app/product/models/product.model';
import { compensateScroll } from '@fullcalendar/core/util/misc';

class ResultDialog {
  status: boolean;
  //Lưu lại tất cả các thuộc tính của từng option đã điền
  listAtrrOption: CustomerOrderExtension[];
  //Thuộc tính gói dịch vụ
  listAttrPacket: Array<CustomerOrderExtension>;
  //Infor gói dịch vụ
  packetService: ServicePacket;
  //Các tùy chọn dịch vụ đã chọn ở level cuối cùng
  listOptionSave: any;
  //tree
  listOptionTreeNode: NewTreeNode[];
}


class rowDataAddedOption {
  serviceName: string;
  optionName: string;
  optionId: string;
  number: number;
  cost: number;
  //Lưu lại tất cả các thuộc tính của từng option đã điền
  listAtrrOption: CustomerOrderExtension[];
  //Thuộc tính gói dịch vụ
  listAttrPacket: Array<CustomerOrderExtension>;
  //Infor gói dịch vụ
  packetService: ServicePacket;
  //Các tùy chọn dịch vụ đã chọn ở level cuối cùng
  listOptionSave: any;
}



export interface NewTreeNode extends TreeNode {
  listAttr?: any;
  path: string;
  number: string;
  margin: string;
}


@Component({
  selector: 'app-order-detail-dialog',
  templateUrl: './order-detail-dialog.component.html',
  styleUrls: ['./order-detail-dialog.component.css'],
  providers: [DialogService]
})


export class OrderDetailDialogComponent implements OnInit {
  @ViewChild('priceInitial') priceInitialElement: ElementRef;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;
  toDay: Date = new Date();
  currentYear: number = this.toDay.getFullYear();
  cols: any[];

  /*Các biến điều kiện*/
  isCreate: boolean = true; //true: Tạo mới sản phẩm dịch vụ(hoặc chi phí phát sinh), false: Sửa sản phẩm dịch vụ(hoặc chi phí phát sinh)
  selectedOrderDetailType: number = 0;  //0: Sản phẩm dịch vụ, 1: Chi phí phát sinh
  isShowRadioProduct: boolean = true;
  isShowRadioOC: boolean = true;

  /*End*/

  //list gói dv
  listServicePacket: Array<ServicePacket> = [];
  //List kiểu dữ liệu
  listDataType: Array<TrangThaiGeneral> = [];
  //List tùy chọn dịch vụ
  listOption: Array<ServicePacketMappingOptionsEntityModel> = [];
  //List thuộc tính của tùy chọn dv
  listOptionAttr: Array<CustomerOrderExtension> = [];
  //List thuộc tính của gói
  listAttrPacket: Array<CustomerOrderExtension> = [];

  listOptionTreeNode: NewTreeNode[] = [];

  selectedOption = [];
  selectedOptionRoot = [];


  packetService: ServicePacket = new ServicePacket();

  dataRow: rowDataAddedOption;

  statusOrder: number = 1;

  listIdSelectOption: Array<string> = [];

  packId: string = this.emptyGuid;


  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    private customerOrderService: CustomerOrderService,
    public dialogService: DialogService,
    private productCategoryService: ProductCategoryService,
  ) {
    this.isCreate = this.config.data.isCreate;
    this.packId = this.config.data.packId;
    if (!this.isCreate) {
      //Nếu là sửa
      this.dataRow = this.config.data.dataRow;
      this.statusOrder = this.config.data.statusOrder;
    }
  }

  ngOnInit() {
    this.setForm();
    this.setTable();
    this.getMasterData();
  }

  async getMasterData() {
    this.loading = true;
    let customerOrderResult: any = await this.customerOrderService.getMasterDataOrderDetailDialogAsync();
    this.loading = false;
    if (customerOrderResult.statusCode == 200) {
      this.listServicePacket = customerOrderResult.listPacketService;

      if (this.packId != null) {
        this.packetService = this.listServicePacket.find(x => x.id == this.packId);
        this.changePacketService(this.packetService);
      }
    }
    else if (customerOrderResult.statusCode !== 200) {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: customerOrderResult.messageCode };
      this.showMessage(msg);
    }
  }

  setForm() {

  }

  setTable() {
    this.cols = [{ field: 'nameCustom', header: 'Tên tùy chọn dịch vụ' }];
  }

  /*Event set giá trị mặc định cho các control*/
  setDefaultValueForm() {
    //set gói
    this.listAttrPacket.forEach(item => {
      let attr = this.dataRow.listAttrPacket.find(x => x.attributeConfigurationId == item.id);
      if (item.dataType == 3) {
        item.value = new Date(attr.value); //Datetime
      } else {
        item.value = attr.value;
      }
    });


    //set lại select của option
    this.checkNode(this.listOptionTreeNode, this.listIdSelectOption)
  }
  /*End*/

  /*Event khi thay đổi sản phẩm dịch vụ*/
  async changePacketService(packet: ServicePacket) {
    this.loading = true;
    let result: any = await this.customerOrderService.searchOptionOfPacketService(packet.id);
    this.loading = false;
    if (result.statusCode == 200) {
      this.listDataType = result.listDataType;
      this.listOption = result.listOption;
      this.listOptionAttr = result.listOptionAttr;
      this.listAttrPacket = result.listAttrPacket;

      this.listOptionTreeNode = [];
      let listDataRoot = this.listOption.filter((x) => x.parentId == null);
      let level = 0;
      let listSelecttedOption = [];
      if (!this.isCreate) {
        listSelecttedOption = this.dataRow.listOptionSave.map(x => x.data.id);
        this.listIdSelectOption = this.dataRow.listOptionSave.map(x => x.data.id);
      }

      this.selectedOption = [];
      await listDataRoot.forEach((item) => {
        let isSelected = false;
        if (!this.isCreate && listSelecttedOption.indexOf(item.id) != -1) isSelected = true;
        //Lấy các thuộc tính và số lượng của tùy chọn dịch vụ
        let number = "1";
        let listAttr = this.listOptionAttr.filter(x => x.objectType == 1 && x.objectId == item.optionId);
        if (!this.isCreate) {
          number = this.dataRow.listOptionSave.find(x => x.data.id == item.id) ? this.dataRow.listOptionSave.find(x => x.data.id == item.id).number : "";
          if (listAttr.length > 0) {
            listAttr.forEach((attr) => {
              let answerInfor = this.dataRow.listAtrrOption.find(x => x.objectId == attr.objectId && x.attributeConfigurationId == attr.id && x.servicePacketMappingOptionsId == item.id);
              if (answerInfor) {
                attr.value = answerInfor.dataType == 3 ? new Date(answerInfor.value) : answerInfor.value;
              }
            });
          }
        }
        let listChild = this.mapDataTreeNode(item, level, listSelecttedOption, item.name);
        let _listAttr = listAttr.map(x => Object.assign({}, x));
        _listAttr.forEach(item => {
          item.servicePacketMappingOptionsId = item.id
        });
        let nodeRoot: NewTreeNode = {
          data: item,
          path: item.name,
          key: item.id,
          number: number,
          leaf: listChild.length == 0,
          children: listChild,
          listAttr: _listAttr, //Lấy các thuộc tính của tùy chọn dịch vụ
          margin: (40 + 16 * level).toString() + "px"
        };
        nodeRoot.expanded = true;
        this.listOptionTreeNode.push(nodeRoot);
      });
      this.listOptionTreeNode = [...this.listOptionTreeNode];
      if (!this.isCreate) this.setDefaultValueForm();
    }
    else if (result.statusCode !== 200) {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
  }
  /*End*/


  mapDataTreeNode(option: ServicePacketMappingOptionsEntityModel, level: number, listSelecttedOption: Array<string>, path: string): Array<NewTreeNode> {
    level++;
    let result: Array<NewTreeNode> = [];
    let listChildren = this.listOption.filter((x) => x.parentId == option.id);
    listChildren.forEach((item) => {
      let isSelected = false;
      if (!this.isCreate && listSelecttedOption.indexOf(item.id) != -1) isSelected = true;
      let newPath = path + " ---> " + item.name;
      let dataRoot = item;
      let listChildrenNode: Array<NewTreeNode> = [];
      if (this.listOption.find(x => x.parentId == item.id)) {
        listChildrenNode = this.mapDataTreeNode(item, level, listSelecttedOption, newPath);
      }
      //Lấy các thuộc tính và số lượng của tùy chọn dịch vụ
      let number = "1";
      let listAttr = this.listOptionAttr.filter(x => x.objectType == 1 && x.objectId == item.optionId);
      if (!this.isCreate) {
        number = this.dataRow.listOptionSave.find(x => x.data.id == item.id) ? this.dataRow.listOptionSave.find(x => x.data.id == item.id).number : "";
        if (listAttr.length > 0) {
          listAttr.forEach((attr) => {
            let answerInfor = this.dataRow.listAtrrOption.find(x => x.objectId == attr.objectId && x.attributeConfigurationId == attr.id && x.servicePacketMappingOptionsId == item.id);
            if (answerInfor) {
              attr.value = answerInfor.dataType == 3 ? new Date(answerInfor.value) : answerInfor.value;
            }
          });
        }
      }
      let _listAttr = listAttr.map(x => Object.assign({}, x));
      _listAttr.forEach(attr => {
        attr.servicePacketMappingOptionsId = item.id
      });
   
      let node: NewTreeNode = {
        data: dataRoot,
        key: item.id,
        number: number,
        path: newPath,
        leaf: listChildrenNode.length == 0,
        children: listChildrenNode,
        listAttr: _listAttr, //Lấy các thuộc tính của tùy chọn dịch vụ
        margin: (40 + 16 * level).toString() + "px"
      };
      node.expanded = true;
      result.push(node);
    });
    return result;
  }



  checkNode(nodes: Array<any>, str: string[]) {
    for (let i = 0; i < nodes.length; i++) {
      //Nếu tại node đó có con và con của nó
      if (!nodes[i].leaf) {
        nodes[i].children.forEach(child => {
          if (child.leaf) {
            for (let j = 0; j < nodes[i].children.length; j++) {
              if (str.includes(nodes[i].children[j].key)) {
                if (!this.selectedOption.includes(nodes[i].children[j])) {
                  this.selectedOption.push(nodes[i].children[j]);
                }
              }
            }
          }
        });
      }else{
        if(str.includes(nodes[i].key) && nodes[i].margin == "40px"){
          this.selectedOption.push(nodes[i]);
        } 
      }

      if (nodes[i].leaf) {
        return;
      }

      this.checkNode(nodes[i].children, str);
      let count = nodes[i].children.length;
      let c = 0;
      for (let j = 0; j < nodes[i].children.length; j++) {
        if (this.selectedOption.includes(nodes[i].children[j])) {
          c++;
        }
        if (nodes[i].children[j].partialSelected) nodes[i].partialSelected = true;
      }
      if (c == 0) { }
      else if (c == count) {
        nodes[i].partialSelected = false;
        if (!this.selectedOption.includes(nodes[i])) {
          this.selectedOption.push(nodes[i]);
        }
      }
      else {
        nodes[i].partialSelected = true;
      }
    }
  }

  /*Event Hủy dialog*/
  cancel() {
    let result = new ResultDialog();

    result.status = false;

    //Lưu lại tất cả các thuộc tính của từng option đã điền
    result.listAtrrOption = [];

    //Thuộc tính gói dịch vụ
    result.listAttrPacket = [];

    //Infor gói dịch vụ
    result.packetService = null;

    this.ref.close(result);
  }
  /*End*/

  /*Event Lưu dialog*/
  save() {
    //Lưu lại tất cả các thuộc tính của từng option đã điền (level cuối cùng).
    //Nếu người dùng chọn level ở giữa => báo chọn level cuối cùng
    let listAtrrOption: CustomerOrderExtension[] = [];
    //Lay cac tùy chọn dich vu o level cuoi cung đã chọn
    let listOptionSaveTemp = this.selectedOption.filter(x => x.children.length == 0);

    let listKeyMapping = [];

    let listOptionSave = [];


    listOptionSaveTemp.forEach(x => {
      if(listKeyMapping.indexOf(x.key) == -1){
        listOptionSave.push(x);
        listKeyMapping.push(x.key);
      }
    });

    let checkQuantityOption = false;
    listOptionSave.forEach(item => {
      item.optionId = item.data.id;
      if (!item.number) checkQuantityOption = true;
      //nếu có thông tin thuộc tính
      if (item.listAttr.length > 0) {
        item.listAttr.forEach(item => {
          item.attributeConfigurationId = item.id;
        });
        listAtrrOption = listAtrrOption.concat(item.listAttr);
      }
    });

    if (checkQuantityOption) {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: "Vui lòng nhập đầy đủ thông tin số lượng của dịch vụ!" };
      this.showMessage(msg);
      return;
    }

    let result = new ResultDialog();
    result.status = true;

    //Lưu lại tất cả các thuộc tính của từng option đã điền
    result.listAtrrOption = listAtrrOption;
    let checkAttrOption = false;
    result.listAtrrOption.forEach(item => {
      if (!item.value) checkAttrOption = true;
    });

    //Thuộc tính gói dịch vụ
    result.listAttrPacket = this.listAttrPacket;
    let checkAttrPack = false;
    result.listAttrPacket.forEach(item => {
      item.attributeConfigurationId = item.id;
      if (!item.value) checkAttrPack = true;
    });

    if (checkAttrOption) {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: "Vui lòng nhập đầy đủ thông tin dịch vụ!" };
      this.showMessage(msg);
      return;
    }

    if (checkAttrPack) {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: "Vui lòng nhập đầy đủ thông tin gói dịch vụ!" };
      this.showMessage(msg);
      return;
    }

    //Infor gói dịch vụ
    result.packetService = this.packetService;
    //Tùy chọn đã lưu
    result.listOptionSave = listOptionSave;
    this.ref.close(result);
  }


  showMessage(msg: any) {
    this.messageService.add(msg);
  }

}

//So sánh giá trị nhập vào với một giá trị xác định
function compareNumberValidator(number: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } | null => {
    if (control.value !== undefined && (parseFloat(control.value.replace(/,/g, '')) > number)) {
      return { 'numberInvalid': true };
    }
    return null;
  };
}

//Không được nhập chỉ có khoảng trắng
function forbiddenSpaceText(control: FormControl) {
  let text = control.value;
  if (text && text.trim() == "") {
    return {
      forbiddenSpaceText: {
        parsedDomain: text
      }
    }
  }
  return null;
}