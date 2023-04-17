import React, {useEffect, useRef, useState} from 'react';
import {observer} from 'mobx-react-lite';
import {Animated, Dimensions, FlatList, StyleSheet, Text, View, ViewStyle,Image, TextInput, Modal, TouchableOpacity, Platform, ScrollView, KeyboardAvoidingView, Alert} from 'react-native';
import {Header, Screen} from '../../components';
import {useIsFocused, useNavigation, useRoute} from "@react-navigation/native"
// import { useStores } from "../../models"
import {color} from '../../theme';
import CenterSpinner from '../../components/center-spinner/center-spinner';
import Ionicons from 'react-native-vector-icons/Ionicons';
import { images } from '../../images';
import DatePicker from 'react-native-date-picker'
import { formatDate, formatNumber, numberFormat, showToast, StatusBarHeight_2 } from '../../services';
import { UnitOfWorkService } from '../../services/api/unitOfWork-service';
import { useStores } from '../../models';
import {modelOrder, CustomerDetai_Modal, OrderDetailExten_Modal, AttrPackAndOption_Modal } from "./type"
import { Toast } from 'react-native-toast-message/lib/src/Toast';
import IframeRenderer, { iframeModel } from '@native-html/iframe-plugin';
import RenderHtml from "react-native-render-html";

const _unitOfWork = new UnitOfWorkService()

const layout = Dimensions.get('window');

const ROOT: ViewStyle = {
    backgroundColor: color.primary,
    flex: 1,
};

const renderers = {
    iframe: IframeRenderer
  };

const customHTMLElementModels = {
    iframe: iframeModel
};

export const ChooseServiceScreen1 = observer(function ChooseServiceScreen1() {
    const navigation = useNavigation();
    const {HDLTModel} = useStores();
    const [isLoading, setLoading] = useState(false);
    const isFocus = useIsFocused()
    const [isRefresh, setRefresh] = useState(false)
    const [showModal, setShowModal] = useState({
        modal_choose_dv: false,
        modal_thanh_toan: false,
        modal_chon_thanh_toan: false,
        show_detail_option: false,
        modal_success: false,
        open_keyboard: false
    })
    const [data_Parent, setData_Parent] = useState()
    const [listData, setListData] = useState([])
    const [listAttrPacket, setListAttrPacket] = useState([]) // các thuộc tính chung của gói
    const [listOptionAttr, setListOptionAttr] = useState([])
    const [listData_Fist, setListData_First] = useState([])
    const { params }: any = useRoute();
    const [data_select, setData_Select] = useState({}) // gói được chọn khi mở modal điền thông tin thuộc tính tùy chọn
    const [properties_option, setProperties_option] = useState([])
    const [formData,setFormData] = useState({})
    const [listDataClass, setListDataClass] = useState([])  // các lớp lồng nhau
    const [ListCustomerDetail, setListCustomerDetail] = useState([]) // list các dịch vụ
    const [ListAttrPackAndOption, setListAttrPackAndOption] = useState([])  // list các thuộc tính dịch vụ
    const [data_yc_bosung, setDataYcBoSung] = useState([])   // list danh sách yêu cầu bổ sung
    const [listDataOption_properties, setListDataOption_properties] = useState({})   // Lưu thông tin của các tùy chọn theo gói để hiển thị, id_class_0 => id tùy chọn => data các thuộc tính
    const [ID_class_first, setID_class_first] = useState('')  // Id của lớp đầu tiên
    const [dataDate,setDataDate] = useState({})
    const [dataConfigPayment,setDataConfigPayment] = useState([])
    const [TypePayment,setTypePayment] = useState({})
    const [countYc, setCountYc] = useState('1') // số lượng của phiêu dịch vụ con
    const [image_payment, setImage_payment] = useState('')
    const [image_success, setImage_Sucess] = useState('')


    const scrollViewRef = useRef(null);


    useEffect(() => {
      fetchData();
    }, [isFocus,isRefresh]);
    const fetchData = async () => {
        setLoading(true)
        setListDataClass([])
        console.log("params: ", params);
        
        if(params) setData_Parent(params?.data)

        if(isFocus){
            let user_id = await HDLTModel.getUserInfoByKey('userId')
            let response = await _unitOfWork.user.searchOptionOfPacketService(
                {   PacketServiceId: params?.data?.id,
                    "UserId": user_id
                }
            )

            console.log("response detail: ", response);
            
            
            let response_payment = await _unitOfWork.user.takeMobileAppConfiguration({})      
            if(response_payment?.statusCode == 200) {
                setDataConfigPayment(response_payment?.listPayMent)
                setImage_payment(response_payment?.mobileAppConfigurationEntityModel?.paymentScreenIconTransfer)
                setImage_Sucess(response_payment?.mobileAppConfigurationEntityModel?.orderNotificationImage)
            }

            
            if(response?.statusCode == 200){
                let _listData = [...response?.listOption]
                setListData(_listData)
                setListAttrPacket(response?.listAttrPacket)
                setListOptionAttr(response?.listOptionAttr)
                
                let new_arr1 = _listData.filter(item => item?.parentId == null)
                new_arr1.sort((a,b) => {
                    return a?.sortOrder - b?.sortOrder
                })
                setListData_First(new_arr1)

            }
        }

        setLoading(false)
    };

    // 
    const handleChooseOption = async (item, id_parent) => {
        if(id_parent) setID_class_first(id_parent)
        else  setID_class_first(item?.id)
       
        setData_Select(item)

        let _listOptionAttr = [...listOptionAttr]
        let new_arr = _listOptionAttr.filter(item_2 => item_2?.objectId == item?.optionId || item_2.objectId == params?.data?.id) // lấy các thuộc tính theo gói
        let all_thuoc_tinh_tuy_chon = [...new_arr]
            
        setProperties_option(all_thuoc_tinh_tuy_chon);
    
        if(listDataOption_properties[id_parent]){
            let data = listDataOption_properties[id_parent][item?.id]
            setFormData(data)
        }

        // lấy count yc dịch vụ con
        let _ListCustomerDetail = ListCustomerDetail.filter(item2 => item2?.optionId == item?.id)
        
        
        if(_ListCustomerDetail?.length > 0){
            setCountYc(_ListCustomerDetail[0]?.quantity.toString())
        } 

        handleSetShowModal("modal_choose_dv", true)
    }

    const handleChooseClassOption = (item) => {
        let data = [...listDataClass]
        data.push(item)
        setListDataClass(data)
        
        let new_arr1 = listData.filter(item => item?.parentId == data[data?.length - 1]?.id)
        new_arr1.sort((a,b) => {
            return a?.sortOrder - b?.sortOrder
        })
        
        setListData_First(new_arr1)
    }

    const handleDeletePhieuYc = async (id, id_parent) => {
        // Xoá trong ListCustomerDetail
        let _ListCustomerDetail = [...ListCustomerDetail]
        let new_arr = _ListCustomerDetail.filter(item => item?.optionId != id)
        setListCustomerDetail(new_arr)

        // Xoá trong ListAttrPackAndOption
        let _ListAttrPackAndOption = [...ListAttrPackAndOption]
        let new_arr2 = _ListAttrPackAndOption.filter(item => item?.objectId != id)
        setListAttrPackAndOption(new_arr2)

        // Xoá trong listDataOption_properties
        let _listDataOption_properties = {...listDataOption_properties}
        let data_parent_child = {..._listDataOption_properties[id_parent]}
        delete data_parent_child[id]
        _listDataOption_properties[id_parent] = {...data_parent_child}

        setListDataOption_properties(_listDataOption_properties)
    }

    const Check_tao_yeu_cau = () => {
        let check = true;
        for(let i = 0; i < properties_option?.length; i++){
            if(!formData[properties_option[i]?.id]?.value){
                showToast("error", properties_option[i]?.categoryName + ' không được để trống')
                check = false
                break
            }
        }
        return check
    }

    const handle_dat_phieu_yc = async () => {

        if(Check_tao_yeu_cau()){
        // lấy mảng list customer detail
            let _ListCustomerDetail = ListCustomerDetail.filter(item => item?.optionId != data_select?.id)
            // oibj_customerDetail = modal CustomerDetai_Modal
            let obj_customerDetail = {
                orderDetailId: "00000000-0000-0000-0000-000000000000",
                vendorId: "",
                orderId: "00000000-0000-0000-0000-000000000000",
                productId: "",
                ProductCategoryId: "00000000-0000-0000-0000-000000000000",
                quantity: 1,
                unitPrice: 0,
                currencyUnit: "00000000-0000-0000-0000-000000000000",
                exchangeRate: 1,
                vat: 0,
                discountType: true,
                discountValue: 0,
                description: "",
                orderDetailType: 0,
                unitId: "",
                incurredUnit: "",
                active: true,
                createdById: "00000000-0000-0000-0000-000000000000",
                createdDate: "2022-12-22T09:22:26.260Z",
                updatedById: null,
                UpdatedDate: null,
                OrderProductDetailProductAttributeValue: [],
                ExplainStr: "",
                NameVendor: "",
                ProductNameUnit: "",
                NameMoneyUnit: "",
                SumAmount: 0,
                GuaranteeTime: null,
                GuaranteeDatetime: null,
                AmountDiscount: 0,
                ExpirationDate: null,
                WarehouseId: null,
                priceInitial: null,
                IsPriceInitial: false,
                WarrantyPeriod: null,
                ActualInventory: null,
                BusinessInventory: null,
                OrderNumber: 0,
                UnitLaborPrice: 0,
                UnitLaborNumber: 0,
                SumAmountLabor: 0,
                FolowInventory: false,
                servicePacketId: null,
                optionId: null
            }

            obj_customerDetail.priceInitial = data_select?.price
            obj_customerDetail.optionId = data_select?.id 
            obj_customerDetail.servicePacketId = data_Parent?.id
            obj_customerDetail.vat = data_select?.vat
            obj_customerDetail.quantity = parseFloat(countYc) 
            
            _ListCustomerDetail.push(obj_customerDetail)
            setListCustomerDetail(_ListCustomerDetail)

            // tạo list AttrPackAndOption_Modal
            let _ListAttrPackAndOption = ListAttrPackAndOption.filter(item => item?.objectId != data_select?.optionId)
            
            if(formData){ 
                let keys = Object.keys(formData)
                for(let i = 0; i < keys?.length; i++){
                    let obj = {
                        attributeConfigurationId: null,
                        objectId: null,
                        objectType: null,
                        value: null,
                        dataType: null
                    }
                    obj.attributeConfigurationId = keys[i]
                    obj.objectId = data_select?.optionId
                    obj.objectType = 1
                    obj.dataType = formData[keys[i]]?.dataType
                    obj.value = formData[keys[i]]?.value
                    obj.servicePacketMappingOptionsId = data_select?.id
                    _ListAttrPackAndOption.push(obj)
                }
            }

            console.log("_ListAttrPackAndOption: ", _ListAttrPackAndOption);
            
            
            setListAttrPackAndOption(_ListAttrPackAndOption)

            // lưu data option và thuoc tinh

            let _listDataOption_properties = listDataOption_properties
            // _listDataOption_properties[data_select?.id] = {...formData}
            // setListDataOption_properties(_listDataOption_properties)
    
            if(listDataClass?.length == 0){           
                if(ID_class_first == data_select?.id){
                    
                    let obj = {..._listDataOption_properties[data_select?.id]}
                    obj[data_select?.id] = {...formData}
                    _listDataOption_properties[data_select?.id] = obj
                }else{
                    let obj = {..._listDataOption_properties[ID_class_first]}
                    obj[data_select?.id] = {...formData}
                    _listDataOption_properties[ID_class_first] = obj
                }
                // let obj = {..._listDataOption_properties[data_select?.id]}
                // obj[data_select?.id] = {...formData}
                // _listDataOption_properties[data_select?.id] = obj
            }
            else{
                let obj = {..._listDataOption_properties[listDataClass[0]?.id]}
                obj[data_select?.id] = {...formData}
                _listDataOption_properties[listDataClass[0]?.id] = obj
            }
            
            setListDataOption_properties(_listDataOption_properties)

            console.log("_listDataOption_properties: ", _listDataOption_properties);
            

            // tạo phiếu yêu cầu
            resetData()
            let new_arr1 = listData.filter(item => item?.parentId == null)
                new_arr1.sort((a,b) => {
                    return a?.sortOrder - b?.sortOrder
            })
            setListDataClass([])
            setListData_First(new_arr1)
            setCountYc('1')
            handleSetShowModal("modal_choose_dv", false)
        }
    }

    const submitDatDv = async () => {
        setLoading(true)
        let customerID = await HDLTModel.getUserInfoByKey('customerId')
        let userId = await HDLTModel.getUserInfoByKey('userId')

        let payload = modelOrder
        payload.CusOrder.CustomerId = customerID
        payload.UserId = userId
        payload.ServicePacketId = data_Parent?.id
        payload.ListCustomerDetail = [...ListCustomerDetail]

        // lấy dữ liệu AttrPackAndOption của thông tin chung
        let _ListAttrPackAndOption_chung = []
        listAttrPacket.map((i) => {
            let obj = {
                attributeConfigurationId: null,
                objectId: null,
                objectType: null,
                value: null,
                dataType: null
            }

            obj.attributeConfigurationId = i.id
            obj.objectId = i.objectId
            obj.objectType = 2
            obj.dataType = i.dataType
            obj.value = i.value
            obj.servicePacketMappingOptionsId = null

            _ListAttrPackAndOption_chung.push(obj)
        })
        // ListAttrPackAndOption của cả chung và riêng từng gói tùy chọn con
        payload.ListAttrPackAndOption = [...ListAttrPackAndOption, ..._ListAttrPackAndOption_chung]

        let _ListOrderDetailExten = []
        payload.ListOrderDetailExten = []
        if(data_yc_bosung?.length > 0 && data_yc_bosung[0]?.name != ''){
            payload.CusOrder.PaymentMethodOrder = 1
            payload.CusOrder.PaymentContent = 'Chuyển khoản'
            data_yc_bosung.map((item) => {
                if(item?.name) {
                    _ListOrderDetailExten.push({
                        id: "00000000-0000-0000-0000-000000000000",
                        orderId: "00000000-0000-0000-0000-000000000000",
                        name: item?.name,
                        quantity: 1,
                        price: null,
                        status: 2,
                        statusName: null,
                        statusObject: null,
                        createdById: "00000000-0000-0000-0000-000000000000",
                        createdDate: "2022-12-22T09:21:57.849Z",
                        updatedById: null,
                        updatedDate: null,
                        edit: false
                    })
                }
            })
            payload.ListOrderDetailExten = [..._ListOrderDetailExten]
            let response = await _unitOfWork.user.createCustomerOrder(payload)
            if(response?.statusCode == 200){
                let res_change_status = await _unitOfWork.user.changeStatusCustomerOrder({
                    OrderId: response?.customerOrderID,
                    StatusOrder:1,
                    UserId: userId
                })
                showToast('success', "Đơn của bạn đang được phê duyệt!")
                navigation.navigate('MainScreen', {screen: 'YeuCauScreen'})
            }
            else showToast('error', response?.messageCode)
        }else{
            if(TypePayment?.id) {
                payload.CusOrder.PaymentMethod = TypePayment?.id
                payload.CusOrder.PaymentContent = TypePayment?.content
        
                let response = await _unitOfWork.user.createCustomerOrder(payload)
                if(response?.statusCode == 200){
          
                    let res_change_status = await _unitOfWork.user.changeStatusCustomerOrder({
                            OrderId: response?.customerOrderID,
                            StatusOrder:1,
                            UserId: userId
                    })
                    // showToast('success', response?.messageCode)
                    handleSetShowModal("modal_success", true)       
                }
                else showToast('error', response?.messageCode)
            }else showToast('error', "Bạn chưa chọn hình thức thanh toán!")
        } 
        setLoading(false)     
    }

    // check điền thông tin chung đầy đủ chưa
    const check_dien_thong_tin_chung =  () => {
        let check  = true
        if(listAttrPacket?.length > 0){
            for(let i = 0; i < listAttrPacket?.length; i++){
                if(!listAttrPacket[i].value){
                    showToast('error', `Thông tin chung ${listAttrPacket[i].categoryName} không được để trống`)
                    check = false
                    break;
                }
            }
           
        }
        return check
    }

    const submitDatDv_Sau = async () => {
        let customerID = await HDLTModel.getUserInfoByKey('customerId')
        let userId = await HDLTModel.getUserInfoByKey('userId')

        let payload = modelOrder
        payload.CusOrder.CustomerId = customerID
        payload.UserId = userId
        payload.ServicePacketId = data_Parent?.id
        payload.ListCustomerDetail = [...ListCustomerDetail]

        let _ListAttrPackAndOption_chung = []
        listAttrPacket.map((i) => {
            let obj = {
                attributeConfigurationId: null,
                objectId: null,
                objectType: null,
                value: null,
                dataType: null
            }

            obj.attributeConfigurationId = i.id
            obj.objectId = i.objectId
            obj.objectType = 2
            obj.dataType = i.dataType
            obj.value = i.value
            obj.servicePacketMappingOptionsId = null

            _ListAttrPackAndOption_chung.push(obj)
        })
        // ListAttrPackAndOption của cả chung và riêng từng gói tùy chọn con
        payload.ListAttrPackAndOption = [...ListAttrPackAndOption, ..._ListAttrPackAndOption_chung]
    
        let _ListOrderDetailExten = []
        payload.ListOrderDetailExten = []
        if(data_yc_bosung?.length > 0 && data_yc_bosung[0]?.name != ''){
            // payload.CusOrder.PaymentMethodOrder = 1
            // payload.CusOrder.PaymentContent = 'Chuyển khoản'
            data_yc_bosung.map((item) => {
                if(item?.name) {
                    _ListOrderDetailExten.push({
                        id: "00000000-0000-0000-0000-000000000000",
                        orderId: "00000000-0000-0000-0000-000000000000",
                        name: item?.name,
                        quantity: 1,
                        price: null,
                        status: 2,
                        statusName: null,
                        statusObject: null,
                        createdById: "00000000-0000-0000-0000-000000000000",
                        createdDate: "2022-12-22T09:21:57.849Z",
                        updatedById: null,
                        updatedDate: null,
                        edit: false
                    })
                }
            })
            payload.ListOrderDetailExten = [..._ListOrderDetailExten]
        }
        setLoading(true)
        let response = await _unitOfWork.user.createCustomerOrder(payload)
        setLoading(false)
        if(response?.statusCode == 200){
            showToast('success', "Đơn của bạn đã được đặt sau!")
            resetData_all()
        }
        else showToast('error', response?.messageCode)
     
              
    }

    const hanleCreateYcBoSung = () => {
        let data = [...data_yc_bosung]
        if(data[data?.length - 1]?.name == ''){
            showToast('error', 'Bạn chưa nhập yêu cầu bổ sung trước đó!')
            return
        }
        data.push({name : ''})
        setDataYcBoSung(data)
    }

    const Calculate_Count_Service_Select = () => {
        let _listDataOption_properties = listDataOption_properties
        let keys = Object.keys(_listDataOption_properties)
        let count = 0;
        keys.map((item) => {
            let keys_2 = Object.keys(_listDataOption_properties?.[item])
            count = count + keys_2?.length
        })
        return count    
    }

    const Calculate_Total_Price = () => {
        let _listDataOption_properties = listDataOption_properties
        let keys = Object.keys(_listDataOption_properties)
        let price = 0;
        keys.map((item) => {
            let keys_2 = Object.keys(_listDataOption_properties?.[item])
            keys_2.map((item_2) => {
                let data = listData.filter(item => item?.id == item_2)
                price += Calculate_Price_Select(data[0])
            })     
        })
        return price  
    }

    const Calculate_Price_Select = (item) => {
        let price = item?.price
        if(item?.vat) price = price + price * item?.vat / 100

        let _ListCustomerDetail = ListCustomerDetail.filter(item2 => item2?.optionId == item?.id)
        
        price = price * _ListCustomerDetail[0]?.quantity

        return price
    }

    const showVatOption = (item) => {
        if(item?.price){
            return (
                <Text style={{color: color.orange}}>(VAT: {item?.vat ? item.vat : 0} %)</Text>
            )
        }
    }

    const handleSetShowModal = (name, value) => {
        let _formData = {...showModal}
        _formData[name] = value
        setShowModal(_formData)
    }

    const goToPage = (page) => {
        navigation.navigate(page);
    };
    const onRefresh = () => {
        setRefresh(true)
    };

    const resetData = () => {
        setDataDate({})
        setData_Select({})
        setProperties_option([])
        setFormData({})
        setCountYc('1')
    }

    const resetData_all = () => {
        setListDataClass([])
        setListAttrPackAndOption([])
        setDataYcBoSung([])
        setListDataOption_properties({})
        setListCustomerDetail([])
        setID_class_first("")
        setDataDate({})
        setTypePayment({})
        resetData()
        setRefresh(true)
    }

    const changeTextInputFormData = (item, value) => {
        let _formData = {...formData}
        _formData[item?.id] = {
            categoryName: item?.categoryName,
            dataType: item?.dataType,
            value: value
        }
        setFormData(_formData)
    }

    const ShowDateFormData = (item, value) => {
        let _dataDate = {...dataDate}
        
        _dataDate[item?.id] = value
        setDataDate(_dataDate)
    }

    const showCountYcDichVuChild = (id) => {
        let _ListCustomerDetail = ListCustomerDetail.filter(item2 => item2?.optionId == id)
        
        return _ListCustomerDetail[0]?.quantity
    }

    // hiển thi chi tiết các thuộc tính và tuỳ chọn vừa trong trong lớp đầu tiên
    const showDataOptionSelect = (id_child, id_parent) => {
        // let showData = []   
        if(!id_parent){
            let data = listDataOption_properties[id_child]
            if(data){
                let keys = Object.keys(data)
                let listData_child = []
                keys.map((item) => {
                    let obj_child = listData.find(_item => _item?.id == item)
                    let showData = []
                    let data_2 = data[item]
                    let keys_2 = Object.keys(data_2)
                    keys_2.map((item_2) => {
                        let obj = data_2[item_2]
                        showData.push(obj)
                    })
                    obj_child.showData = [...showData]
                    listData_child.push(obj_child)
                })
                return (
                    <View>
                        {listData_child && listData_child.map((item_3) => {
                            return (
                                <View>
                                    <View style={{width: layout.width, backgroundColor: color.blue_nhat,  paddingVertical: 12}}>
                                        <View style={{borderBottomWidth: 1, borderBottomColor: color.lighterGrey, paddingHorizontal: 16, flexDirection: 'row', justifyContent: 'space-between'}}>
                                            <Text style={[styles.text_header_2,{fontSize: 15, paddingBottom: 10}]}>{item_3?.name}</Text>
                                            {showVatOption(item_3)}
                                        </View>
                                        <TouchableOpacity 
                                            style={{flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', paddingRight: 16}}
                                            onPress={() => handleChooseOption(item_3, id_child)}
                                        >
                                            <View style={{width: '90%', marginTop: 12, paddingLeft: 16}}>
                                                {item_3?.showData && item_3?.showData.map((item_4) => {
                                                    return (
                                                        <View>
                                                            <Text style={{marginBottom: 5}}>{item_4?.categoryName} : {item_4?.dataType != 3 ? item_4?.value : formatDate(item_4?.value)}</Text>
                                                        </View>
                                                    )
                                                })}
                                                <View>
                                                    <Text style={{marginBottom: 5}}>Số lượng: {showCountYcDichVuChild(item_3?.id)}</Text>
                                                </View>
                                            </View>
                                            <Ionicons name='caret-forward-outline' size={20} />
                                        </TouchableOpacity>
                                        <Text ellipsizeMode="clip" numberOfLines={1} style={{fontSize: 17, paddingHorizontal: 16, color: color.lighterGrey}}>- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -</Text>
                                        <View style={{flexDirection: 'row', justifyContent: 'space-between', paddingHorizontal: 16}}>
                                            <TouchableOpacity style={{width: '10%'}} onPress={() => handleDeletePhieuYc(item_3?.id, id_child)}>
                                                <Text style={{color: color.error}}>Huỷ</Text>
                                            </TouchableOpacity>
                                            <Text>{formatNumber(Calculate_Price_Select(item_3))} đ</Text>
                                        </View>
                                    </View>
                                </View>
                            )
                        })}
                    </View> 
                )          
            }
        }  
    }

    const showDataOptionSelect_thanh_toan = (id_child, id_parent) => {
        if(!id_parent){
            let data = listDataOption_properties[id_child]
            if(data){
                let keys = Object.keys(data)
                let listData_child = []
                keys.map((item) => {
                    let obj_child = listData.find(_item => _item?.id == item)
                    let showData = []
                    let data_2 = data[item]
                    let keys_2 = Object.keys(data_2)
                    keys_2.map((item_2) => {
                        let obj = data_2[item_2]
                        showData.push(obj)
                    })
                    obj_child.showData = [...showData]
                    listData_child.push(obj_child)
                })
                return (
                    <View>
                        {listData_child && listData_child.map((item_3) => {
                            return (
                                <View>
                                    <View style={{paddingVertical: 12}}>
                                        <View style={{flexDirection: 'row', justifyContent: 'space-between'}}>
                                            <Text style={[styles.text_black,{fontSize: 13}]}>{item_3?.name}</Text>
                                            <Text style={[styles.text_black,{fontSize: 13}]}>{formatNumber(Calculate_Price_Select(item_3))} vnd</Text>
                                        </View>
                                        {showModal?.show_detail_option ?
                                        <View style={{width: '90%', marginTop: 12}}>
                                            {item_3?.showData && item_3?.showData.map((item_4) => {
                                                return (
                                                    <View>
                                                        <Text style={[styles.text_black,{marginBottom: 5, fontSize: 13, fontWeight: '300'}]}>{item_4?.categoryName} : {item_4?.dataType != 3 ? item_4?.value : formatDate(item_4?.value)}</Text>
                                                    </View>
                                                )
                                            })}
                                            <View>
                                                <Text style={{marginBottom: 5}}>Số lượng: {showCountYcDichVuChild(item_3?.id)}</Text>
                                            </View>
                                        </View>
                                        : null }                
                                    </View>
                                </View>
                            )
                        })}
                    </View> 
                )          
            }
        }  
    }

    const topComponent = () => {
        return (
             <View style={{flex: 1}}>
                <View style={{ alignItems: 'center', marginTop: 16}}>
                    <Text style={styles.text_header}>{listDataClass?.length > 0 ? listDataClass[listDataClass?.length - 1].name : data_Parent?.name}</Text>
                    <Text style={[styles.text, {marginTop: 12}]}>{listData_Fist?.length} dịch vụ - {data_Parent?.provinceName}</Text>
                </View>
                <View style={[styles.box,{marginTop: 16}]}>
                    <Image source={images.icon_chon_dv} style={{width: 22, height: 22}} />
                    <Text style={[styles.text_header_2, {marginLeft: 16}]}>Chọn dịch vụ</Text>
                </View>
                {listData_Fist && listData_Fist.map((item, index) => {
                    return (
                        <View>
                                <TouchableOpacity 
                                    style={[styles.box2]} 
                                    onPress={() => {
                                        if(item?.price) {
                                            // nếu có giá thì mở modal điền thông tin dịch vụ
                                            handleChooseOption(item , listDataClass[0]?.id)
                                        } 
                                        else {
                                            // Thay đổi class đầu tiên hiển thị thành item
                                            handleChooseClassOption(item)
                                        }}
                                    }
                                >
                                    <View style={{flexDirection: 'row', justifyContent: 'space-between', paddingHorizontal: 16, paddingVertical: 12}}>
                                        <Text numberOfLines={2} style={[styles.text,{color: color.black, fontWeight: '500'}]}>{item?.name} {item?.price ?  '- Đơn giá: ' + formatNumber(item?.price) + '/' + item?.categoryUnitName : ''} {showVatOption(item)}</Text>
                                        {!item?.price ?
                                            <Ionicons name='caret-forward-outline' size={20} />
                                        : null}
                                    </View>
                                    
                                    {/* {listDataOption_properties[item?.id] ? 
                                        showDataOptionSelect(item?.id) 
                                    : null} */}
                                </TouchableOpacity>
                                {showDataOptionSelect(item?.id, listDataClass[0]?.id)}   
                        </View>
                        
                    )
                })}

                {listDataClass?.length == 0 ? 
                <View style={{marginBottom: 50}}>
                    <View style={[styles.box,{justifyContent: 'space-between', alignItems: 'center', paddingVertical: 16}]}>
                        <View style={[{flexDirection: 'row'}]}>
                            <Image source={images.icon_dv_bosung} style={{width: 22, height: 22}} />
                            <Text style={[styles.text_header_2, {marginLeft: 16}]}>Yêu cầu bổ sung</Text>
                        </View>
                        <TouchableOpacity onPress={hanleCreateYcBoSung}>
                            <Ionicons name='add-outline' size={30} />
                        </TouchableOpacity>
                    </View>
                      {data_yc_bosung && data_yc_bosung.map((item,index) => {
                        return (
                            <View style={[styles.box4,{}]}>
                                <TextInput 
                                    style={{width: '100%', paddingHorizontal: 12, fontSize: 16, paddingVertical: 10}}
                                    placeholder='Nhập yêu cầu bổ sung'
                                    multiline
                                    value={data_yc_bosung[index]?.name}
                                    onChangeText={(value) => {
                                        let data = [...data_yc_bosung]
                                        data[index].name = value
                                        setDataYcBoSung(data)
                                    }}
                                    onFocus={() => {
                                        handleSetShowModal('open_keyboard', true)
                                        scrollViewRef.current.scrollToEnd({animated: true})
                                        }
                                    }
                                    onEndEditing={() => handleSetShowModal('open_keyboard', false)}
                                />
                            </View>
                        )
                    })}
       
                </View>
                : null }

                {listDataClass?.length == 0 && listAttrPacket?.length > 0 && 
                    <View style={{marginBottom: 50}}>
                        <View style={[styles.box,{justifyContent: 'space-between', alignItems: 'center', paddingVertical: 16}]}>
                            <View style={[{flexDirection: 'row'}]}>
                                <Image source={images.icon_dv_bosung} style={{width: 22, height: 22}} />
                                <Text style={[styles.text_header_2, {marginLeft: 16}]}>Thông tin chung</Text>
                            </View>
                        </View>
                        {listAttrPacket?.map((item, index) => {
                            return(
                                <View style={{paddingHorizontal: 16}}>
                                    <Text style={[styles.text_black,{marginVertical: 16}]}>{item?.categoryName} <Text style={{color: color.error}}>*</Text></Text>
                                    {item?.dataType != 3 ?
                                        <TextInput
                                            style={[styles.input]}
                                            value={item.value ? item.value : ''}
                                            onChangeText={(value) => {
                                                let _listAttrPacket = [...listAttrPacket]
                                                _listAttrPacket[index].value = value
                                                setListAttrPacket(_listAttrPacket)
                                            }}
                                            onFocus={() => {
                                                handleSetShowModal('open_keyboard', true)
                                                // scrollViewRef.current.scrollToEnd({animated: true})
                                                }
                                            }
                                            onEndEditing={() => handleSetShowModal('open_keyboard', false)}
                                        />
                                    :
                                        <View style={{width: '100%'}}>
                                            <TouchableOpacity style={styles.inputDate} onPress={() => {ShowDateFormData(item, true)}}>
                                                <Text>{formatDate(item?.value)}</Text>
                                                <Ionicons name={'calendar-outline'} color="black" size={24}/>
                                            </TouchableOpacity>
                                            {dataDate?.[item?.id] ? 
                                                <DatePicker
                                                    // minimumDate={lastUpload}
                                                    // maximumDate={new Date()}
                                                    textColor={Platform?.OS == 'ios' ? color.black : color.black}
                                                    mode="date"
                                                    modal
                                                    open={dataDate?.[item?.id]}
                                                    date={item.value ? item.value : new Date()}
                                                    onConfirm={(date) => {
                                                        // lưu ngày
                                                        let _listAttrPacket = [...listAttrPacket]
                                                        _listAttrPacket[index].value = date
                                                        setListAttrPacket(_listAttrPacket)
                                                        // đóng ngày
                                                        ShowDateFormData(item, false)      
                                                    }}
                                                    onCancel={() => {
                                                        ShowDateFormData(item, false)
                                                    }}
                                                    locale='vi'
                                                    confirmText='Xác nhận'
                                                    cancelText='Hủy'
                                                    title={null}

                                                />
                                            :  null }
                                        </View>
                                    }
                                </View>
                            )
                        })}
                    </View>
                }


                <View style={{height: showModal?.open_keyboard ? 0 : 150}}></View>
            </View>
        );
    };

    return (
        <>
            {isLoading && <CenterSpinner/>}
            <Screen style={ROOT} preset="fixed">
                <Header headerText='Đặt dịch vụ' onLeftPress={() => {
                    if(listDataClass?.length > 0){
                        let data = [...listDataClass]
                        data.pop()
                        setListDataClass(data)
                        if(listDataClass?.length == 1){
                            let new_arr1 = listData.filter(item => item?.parentId == null)
                            new_arr1.sort((a,b) => {
                                return a?.sortOrder - b?.sortOrder
                            })
                            setListData_First(new_arr1)
                        }else{
                            let new_arr1 = listData.filter(item => item?.parentId == data[data?.length - 1]?.id)
                            new_arr1.sort((a,b) => {
                                return a?.sortOrder - b?.sortOrder
                            })                         
                            setListData_First(new_arr1)
                        }
                        setListDataClass(data)
                    }else navigation.goBack()
                    
                }}/>
                <View style={{flex: 1, backgroundColor: color.white}}>
                    <ScrollView 
                        style={{height: listDataClass?.length == 0 ? layout.height - 230 : layout.height - 120}}
                        ref={scrollViewRef}
                    >
                        {topComponent()}
                    </ScrollView>
                    {listDataClass?.length == 0 && !showModal?.open_keyboard  ?
                        <View style={{paddingHorizontal: 15,paddingVertical: 12,width: layout.width, position: 'absolute', bottom: 0, height: 120, backgroundColor: color.lighterGrey}}>
                            <View style={{flexDirection: 'row', justifyContent: 'space-between'}}>
                                <Text style={[styles.text_header_2]}>Lựa chọn - <Text style={{fontWeight: '400'}}>{Calculate_Count_Service_Select()} dịch vụ</Text></Text>
                                <Text style={[styles.text_header_2]}>{formatNumber(Calculate_Total_Price())} đ</Text>
                            </View>
                            <View style={{flexDirection: 'row', justifyContent: 'space-between', marginTop: 16}} >
                                <TouchableOpacity 
                                    style={[styles.btn_blue, {width: '30%'}]}
                                    onPress={async () => {
                                        let user_id = await HDLTModel.getUserInfoByKey('userId')
                                        if(user_id){
                                            let check = check_dien_thong_tin_chung()
                                            
                                            if(!check) return
                                            if(ListCustomerDetail?.length > 0){
                                                submitDatDv_Sau()
                                            }else{
                                                showToast("error", 'Bạn chưa có đơn hàng!')
                                            }
                                        }else{
                                            Alert.alert(
                                                "Bạn có muốn đăng nhập vào hệ thống!",
                                                "",
                                                [
                                                  {text: 'Hủy', onPress: () => console.log('Later button clicked')},
                                                  {
                                                    text: "Có",
                                                    onPress: () => {
                                                        navigation.navigate('LoginScreen')
                                                    },
                                                  },
                                                ]
                                              );
                                        }                    

                                    }}
                                >
                                    <Text style={[styles.text_header_2,{color: color.white}]} >Đặt Sau</Text>
                                </TouchableOpacity>
                                <TouchableOpacity style={[styles.btn_blue,{width: '65%'}]} 
                                    onPress={async () => {
                                        let user_id = await HDLTModel.getUserInfoByKey('userId')
                                        console.log("userID: ", user_id);
                                        
                                        if(user_id){
                                            let check = check_dien_thong_tin_chung()
                                            
                                            if(!check) return
                                            if(ListCustomerDetail?.length > 0){
                                                if(data_yc_bosung?.length > 0 && data_yc_bosung[0]?.name){
                                                    submitDatDv()
                                                }else  handleSetShowModal("modal_thanh_toan", true)
                                            }else{
                                                showToast("error", 'Bạn chưa có đơn hàng!')
                                            } 
                                        }else{
                                            Alert.alert(
                                                "Bạn có muốn đăng nhập vào hệ thống!",
                                                "",
                                                [
                                                  {text: 'Hủy', onPress: () => console.log('Later button clicked')},
                                                  {
                                                    text: "Có",
                                                    onPress: () => {
                                                        navigation.navigate('LoginScreen',{
                                                            type : 'ChooseServiceScreen1',
                                                            data: params?.data
                                                        })
                                                    },
                                                  },
                                                ]
                                              );
                                        }
                                }} >
                                    <Text style={[styles.text_header_2, {color: color.white}]}>Đặt Dịch Vụ</Text>
                                </TouchableOpacity>
                            </View>
                        </View>
                    : null }
                </View>
                
            
                <Modal
                    animationType={"slide"}
                    transparent={true}
                    visible={showModal?.modal_choose_dv}
                    onRequestClose={() => { }}
                >
                    <View style={styles.modal_container}>
                        <View style={{}}>
                            <View style={styles.header_apply}>
                                <Text style={{ marginLeft: 17, fontSize: 18, color: color.white, fontWeight: '800' }}>Chi tiết dịch vụ</Text>
                                <TouchableOpacity style={{marginRight: 10, marginTop: 10}} onPress={() => {
                                    resetData()
                                    handleSetShowModal("modal_choose_dv", false)
                                }}>
                                    <Ionicons name={'close-outline'} color='black' size={30} />
                                </TouchableOpacity>
                            </View>
                            <View style={{ paddingHorizontal: 20, height: layout.height*9/10 - 170}}>
                                <KeyboardAvoidingView
                                    behavior={Platform.OS === "ios" ? "padding" : "height"}
                                    style={{flex: 1}}
                                    keyboardVerticalOffset={150}
                                >
                                    <ScrollView
                                        showsVerticalScrollIndicator={false}
                                    >
                                        <View style={{paddingVertical: 16}}>
                                            <Text style={[styles.text_header_2, {marginBottom: 12}]}>{data_select?.name}</Text>
                                            <View>
                                                <RenderHtml
                                                    renderers={renderers}
                                                    source={{
                                                        html: `<div style="color: #1d1d1d ">${data_select?.description}</div>`,
                                                    }}
                                                    customHTMLElementModels={customHTMLElementModels}
                                                    tagsStyles={{
                                                        p: {
                                                            color: color.black
                                                        }
                                                    }}
                                                    renderersProps={{
                                                    }}
                                                />
                                            </View>
                                            {/* <Text style={{fontWeight: '400', color: color.black, fontSize: 15}}>Đưa đón đến bệnh viện bao gồm các bệnh viện Hồng Ngọc, Bạch Mai, Việt Đức</Text> */}
                                            <View style={[styles.box3,{marginTop: 0, justifyContent: 'flex-start', paddingHorizontal: 0}]}>
                                                <Image source={images.icon_book} style={{width: 22, height: 22}} />
                                                <Text style={[styles.text_header_2, {marginLeft: 16}]}>Thông tin đặt dịch vụ</Text>
                                            </View>

                                            {properties_option && properties_option.map((item,index) => {
                                                return (
                                                    <View>
                                                        <Text style={[styles.text_black,{marginVertical: 16}]}>{item?.categoryName} <Text style={{color: color.error}}>*</Text></Text>
                                                        {item?.dataType != 3 ?
                                                            <TextInput
                                                                style={[styles.input]}
                                                                value={formData?.[item?.id]?.value}
                                                                onChangeText={(value) => {
                                                                    changeTextInputFormData(item, value)
                                                                }}
                                                            />
                                                        : 
                                                            <View style={{width: '100%'}}>
                                                                    <TouchableOpacity style={styles.inputDate} onPress={() => {ShowDateFormData(item, true)}}>
                                                                        <Text>{formatDate(formData?.[item?.id]?.value)}</Text>
                                                                        <Ionicons name={'calendar-outline'} color="black" size={24}/>
                                                                    </TouchableOpacity>
                                                                    {dataDate?.[item?.id] ? 
                                                                        <DatePicker
                                                                            // minimumDate={lastUpload}
                                                                            // maximumDate={new Date()}
                                                                            textColor={Platform?.OS == 'ios' ? color.black : color.black}
                                                                            mode="date"
                                                                            modal
                                                                            open={dataDate?.[item?.id]}
                                                                            date={formData?.[item?.id] ? formData?.[item?.id]?.value : new Date()}
                                                                            onConfirm={(date) => {
                                                                                changeTextInputFormData(item, date)
                                                                                ShowDateFormData(item, false)      
                                                                            }}
                                                                            onCancel={() => {
                                                                                ShowDateFormData(item, false)
                                                                            }}
                                                                            locale='vi'
                                                                            confirmText='Xác nhận'
                                                                            cancelText='Hủy'
                                                                            title={null}

                                                                        />
                                                                    :  null }
                                                            </View>
                                                        }
                                                    </View>
                                                )
                                            })}
                                            <View style={{marginBottom: 16}}>
                                                <Text style={[styles.text_black,{marginVertical: 16}]}>Số lượng</Text>
                                                <TextInput
                                                    style={[styles.input]}
                                                    value={countYc}
                                                    onChangeText={(value) => {
                                                        setCountYc(value)
                                                    }}
                                                    keyboardType='numeric'
                                                />
                                            </View>
                                        </View>
                                    </ScrollView>
                                </KeyboardAvoidingView>
                            </View>
                        </View>
                        <View style={{position: 'absolute', width: layout.width*8/10, bottom: 0, marginBottom: 50, marginLeft: layout.width/10 }}>
                            <TouchableOpacity style={[{ width: '100%', borderRadius: 10, paddingVertical: 14, backgroundColor: color.blue, alignItems: 'center' }]} onPress={() => {
                                handle_dat_phieu_yc()
                            }}>
                                <Text style={[{fontSize: 18, fontWeight: '700', color: color.white}]}>OK</Text>
                            </TouchableOpacity>
                        </View>
                    </View>
                    <Toast />
                </Modal>

                <Modal
                    animationType={"slide"}
                    transparent={true}
                    visible={showModal?.modal_thanh_toan}
                    onRequestClose={() => { }}
                >
                    <View style={[styles.modal_container_thanh_toan]}>
                        <View style={styles.header_thanh_toan}>
                            <TouchableOpacity 
                                style={{width: '10%', justifyContent: 'center', alignItems: 'center'}}
                                onPress={() => handleSetShowModal("modal_thanh_toan", false)}>
                                <Ionicons name='chevron-back-outline' size={30} color={color.black}/>
                            </TouchableOpacity>
                            <Text style={styles.text_header} >Thanh toán dịch vụ</Text>
                            <View style={{width: '5%'}}></View>
                        </View>
                        <ScrollView style={{flex: 1}}>
                            <View style={[styles.body_thanh_toan]}>
                                <Text style={[styles.text_header_2,{fontSize: 16}]}>Dịch vụ</Text>
                                <Text style={[styles.text_header,{fontSize: 16, marginTop: 5}]}>{data_Parent?.name}</Text>

                                <Text style={[styles.text_header_2,{fontSize: 16, marginTop: 5}]}>Thông tin chung</Text>
                                {listAttrPacket?.length > 0 && listAttrPacket?.map((AttrPacke_chung) => {
                                    return(
                                        <View style={[{flexDirection: 'row'}]}>
                                            <Text style={[styles.text_black,{fontSize: 14, marginTop: 5}]}>- {AttrPacke_chung.categoryName}: {AttrPacke_chung.dataType == 3 ? formatDate(AttrPacke_chung.value) :  AttrPacke_chung.value}</Text>
                                        </View>
                                    )
                                })} 
                                <View style={{flexDirection: 'row', justifyContent: 'space-between'}}>
                                    <Text style={[styles.text_header_2,{fontSize: 16, marginTop: 16}]}>Chi tiết dịch vụ:</Text>
                                    {showModal?.show_detail_option ? 
                                        <TouchableOpacity onPress={() => handleSetShowModal("show_detail_option", false)}>
                                            <Text style={[styles.text_header_2,{fontSize: 16, marginTop: 16}]}>Thu gọn</Text>
                                        </TouchableOpacity>
                                    :
                                        <TouchableOpacity onPress={() => handleSetShowModal("show_detail_option", true)}>
                                            <Text style={[styles.text_header_2,{fontSize: 16, marginTop: 16}]}>Xem</Text>
                                        </TouchableOpacity>
                                    }
                                </View>
                                
                                
                                {listData_Fist && listData_Fist.map((item, index) => {
                                    return (
                                        <View>
                                            {showDataOptionSelect_thanh_toan(item?.id, listDataClass[0]?.id)}   
                                        </View>
                                        
                                    )
                                })}
                 
                                <View style={[styles.box_flex,{marginTop: 16}]}>
                                    <Text style={[styles.text_header_2,{fontSize: 16}]}>Số tiền</Text>
                                    <Text>{formatNumber(Calculate_Total_Price())} vnd</Text>
                                </View>
                                <View style={[styles.box_flex]}>
                                    <Text style={[styles.text_header_2,{fontSize: 16, color: color.orange}]}>Giảm giá</Text>
                                    <Text style={{color: color.orange}}>0 vnd</Text>
                                </View>
                                <View style={[styles.box_flex]}>
                                    <Text style={[styles.text_header_2,{fontSize: 16}]}>Thanh toán</Text>
                                    <Text>{formatNumber(Calculate_Total_Price())} vnd</Text>
                                </View>
                            </View>
                            <View style={{backgroundColor: color.trang_nhat_2, paddingHorizontal: 16, paddingBottom: 16,paddingTop: 24, flexDirection: 'row'}}>
                                {image_payment ? 
                                <Image source={{uri: image_payment}} style={{width: 22, height: 22}} />
                                : <Image source={images.iconPayment} style={{width: 22, height: 22}} /> }
                                <Text style={[styles.text_header_2, {marginLeft: 12}]}>Chọn hình thức thanh toán</Text>
                            </View>
                            <TouchableOpacity 
                                style={{backgroundColor: color.white, paddingHorizontal: 16, paddingVertical: 18, flexDirection: 'row', justifyContent: 'space-between'}}
                                onPress={() => handleSetShowModal('modal_chon_thanh_toan', true)}
                            >
                                <Text style={[styles.text_header_2, {color: color.black}]}>{TypePayment?.categoryName ? TypePayment?.categoryName : 'Lựa chọn hình thức thanh toán'}</Text>
                                <Ionicons name='caret-forward-outline' size={20} />
                            </TouchableOpacity>
                            <View style={{flex: 1, backgroundColor: color.trang_nhat_2, alignItems: 'center', marginBottom: 16}}>
                                <TouchableOpacity 
                                    style={{backgroundColor: color.blue, width: layout.width - 32, paddingVertical: 15, borderRadius: 6, alignItems: 'center', marginVertical: 33}}
                                    onPress={() => {
                                        submitDatDv()
                                    }
                                    }
                                >
                                    <Text style={[styles.text_header_2, {color: color.white}]}>Xác Nhận</Text>
                                </TouchableOpacity>
                            </View>
                        </ScrollView>
                        
                    </View>
                    <Toast /> 
                    <Modal
                         animationType={"slide"}
                         transparent={true}
                         visible={showModal?.modal_chon_thanh_toan}
                         onRequestClose={() => { }}
                    >
                        <View style={{height: layout.height - 70, marginTop: 70, backgroundColor: color.white, borderRadius: 10}}>
                            <View style={{padding: 20, backgroundColor: color.blue, borderTopStartRadius: 10,borderTopEndRadius: 10, flexDirection: 'row', justifyContent: 'space-between'}}>
                                <Text style={[styles.text_header_2, {color: color.white}]}>Hình thức thanh toán</Text>
                                <TouchableOpacity onPress={() => handleSetShowModal('modal_chon_thanh_toan', false)}>
                                    <Ionicons name={'close-outline'} color={color.white} size={30} />
                                </TouchableOpacity>
                            </View>
                            <View style={{padding: 16, flexDirection: 'row'}}>
                                {image_payment ? 
                                <Image source={{uri: image_payment}} style={{width: 22, height: 22}} />
                                : <Image source={images.iconPayment} style={{width: 22, height: 22}} /> }
                                <Text style={[styles.text_header_2, {marginLeft: 12}]}>Chọn hình thức thanh toán</Text>
                            </View>
                            {dataConfigPayment.map((item) => {
                                return (
                                    <TouchableOpacity 
                                        style={{marginHorizontal: 16, borderWidth: 1, borderColor: color.lighterGrey, paddingHorizontal: 16, paddingVertical: 20, borderRadius: 10, marginTop: 8}}
                                        onPress={() => {
                                            let obj = {
                                                id: item?.id,
                                                categoryName: item?.categoryName,
                                                content: item?.content
                                            }
                                            
                                            setTypePayment({...obj})
                                            handleSetShowModal('modal_chon_thanh_toan', false)
                                        }}
                                    >
                                        <View style={{flexDirection: 'row'}}>
                                            {/* <Image source={{uri: dataConfigPayment?.paymentScreenIconTransfer}} style={{width: 22, height: 15}}/> */}
                                            <Text style={{fontWeight: '700', fontSize: 15, color: color.black, marginLeft: 10}}>{item?.categoryName}</Text>
                                        </View>
                                        <RenderHtml
                                            // contentWidth={layout.width/10*8}
                                            // tagsStyles={tagsStyles}
                                            renderers={renderers}
                                            // WebView={WebView}
                                            source={{
                                                html: `<div>${item?.content}</div>`,
                                            }}
                                            customHTMLElementModels={customHTMLElementModels}
                                            // defaultWebViewProps={webViewProps}
                                            tagsStyles={{
                                            // p: {
                                            //     color: "#797979"
                                            // }
                                            }}
                                            renderersProps={{
                                                iframe: {
                                                scalesPageToFit: true,
                                                webViewProps: {
                                                    /* Any prop you want to pass to iframe WebViews */
                                                }
                                                }
                                            }}
                                        /> 
                                        {/* <Text>{item?.content}</Text> */}
                                    </TouchableOpacity>
                                )
                            })}
                            {/* {dataConfigPayment?.isPaymentScreenIconTransfer ?
                                <TouchableOpacity 
                                    style={{marginHorizontal: 16, borderWidth: 1, borderColor: color.lighterGrey, paddingHorizontal: 16, paddingVertical: 20, borderRadius: 10, marginTop: 8}}
                                    onPress={() => {
                                        setTypePayment(1)
                                        handleSetShowModal('modal_chon_thanh_toan', false)
                                    }}
                                >
                                    <View style={{flexDirection: 'row'}}>
                                        <Image source={{uri: dataConfigPayment?.paymentScreenIconTransfer}} style={{width: 22, height: 15}}/>
                                        <Text style={{fontWeight: '700', fontSize: 15, color: color.black, marginLeft: 10}}>Chuyển khoản</Text>
                                    </View>
                                    <Text>{dataConfigPayment?.paymentScreenContentTransfer}</Text>
                                </TouchableOpacity>
                            : null }
                            {dataConfigPayment?.isPaymentScreenIconVnpay ? 
                                <TouchableOpacity 
                                    style={{marginHorizontal: 16, borderWidth: 1, borderColor: color.lighterGrey, paddingHorizontal: 16, paddingVertical: 20, borderRadius: 10, marginTop: 14}}
                                    onPress={() => {
                                        setTypePayment(2)
                                        handleSetShowModal('modal_chon_thanh_toan', false)
                                    }}
                                >
                                    <View style={{flexDirection: 'row'}}>
                                        <Image source={{uri: dataConfigPayment?.paymentScreenIconVnpay}} style={{width: 22, height: 15}}/>
                                        <Text style={{fontWeight: '700', fontSize: 15, color: color.black, marginLeft: 10}}>Thanh toán qua VNPAY</Text>
                                    </View>
                                    <Text>{dataConfigPayment?.paymentScreenContentVnpay}</Text>
                                </TouchableOpacity>
                            : null } */}
                        </View>
                    </Modal>

                    <Modal
                         animationType={"slide"}
                         transparent={true}
                         visible={showModal?.modal_success}
                         onRequestClose={() => { }}
                    >
                        <View style={{height: layout.height/10*8, marginTop: layout.height/10, backgroundColor: color.lighterGrey, borderRadius: 20, width: layout.width - 60, marginLeft: 30, alignItems: 'center', padding: 16}}>
                            <Image source={{uri: image_success}} style={{height: layout.width - 100, width: layout.width - 100}} />
                            <Text style={[styles.text_header,{marginVertical: 20}]}>Thành công!</Text>
                            <Text style={[styles.text_black,{fontWeight: '400', textAlign: 'center'}]} >Dịch vụ của bạn đã được đặt thành công.Chúng tôi sẽ liên hệ bạn trong thời gian sớm nhất.</Text>
                            <TouchableOpacity 
                                style={[styles.btn_blue,{width: '100%', marginTop: 30}]}
                                onPress={() => navigation.navigate('MainScreen', {screen: 'YeuCauScreen'}) }
                            >
                                <Text style={[styles.text_header_2,{color: color.white}]}>OK</Text>
                            </TouchableOpacity>
                        </View>
                    </Modal>
                </Modal>
            </Screen>
        </>
    );
});

const styles = StyleSheet.create({
    text_header: {
        fontSize: 20,
        fontWeight: '700',
        color: color.black
    },
    text_header_2: {
        fontSize: 18,
        fontWeight: '700',
        color: color.blue
    },
    text: {
        color: color.xam,
        fontSize: 15,
        fontWeight: '400',
        width: '80%'
    },
    text_black: {
        color: color.black,
        fontSize: 15,
        fontWeight: '500',
        width: '80%'
    },
    box: {
        flexDirection: 'row',
        paddingHorizontal: 16,
        paddingVertical: 24,
        backgroundColor: color.nau_nhat,
        // borderBottomWidth: 0.5,
        // borderTopWidth: 1
    },
    box2: {
        // paddingHorizontal: 16,
        // paddingVertical: 18,
        backgroundColor: color.white,
        borderBottomWidth: 1,
        borderBottomColor: color.nau_nhat,
        width: layout.width
    },
    box3: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        paddingHorizontal: 16,
        paddingVertical: 18,
        backgroundColor: color.white,
        borderBottomWidth: 1,
        borderBottomColor: color.nau_nhat
    },
    modal_container: {
        backgroundColor: color.white, 
        height: layout.height/10*9,
        minHeight: 650,
        marginTop:  layout.height/10,
        borderRadius: 20
    },
    modal_container_thanh_toan: {
        backgroundColor: color.white, 
        height: layout.height,
        marginTop: StatusBarHeight_2
    },
    header_apply: {
        flexDirection: 'row',
        borderTopRightRadius: 20,
        borderTopLeftRadius: 20,
        height: 50,
        justifyContent: 'space-between',
        alignItems: 'center',
        backgroundColor: color.blue
    },
    box4: {
        flexDirection: 'row',
        borderWidth: 1,
        borderColor: color.black,
        marginHorizontal: 16,
        borderRadius: 6,
        marginTop: 10
    },
    box_item: {
        padding: 10,
        borderWidth: 1,
        borderColor: color.lighterGrey,
        flexDirection: 'row',
        borderRadius: 7,
        alignItems: 'center',
        marginTop: 16
    },
    box_select: {
        width: 25,
        height: 25,
        borderWidth: 1,
        marginRight: 16
    },
    input: {
        width: '100%',
        borderWidth: 1,
        borderRadius: 7,
        paddingLeft: 10,
        paddingVertical: 7
    },
    inputDate: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
        marginRight: 8,
        borderWidth: 1,
        borderColor: color.black,
        // paddingHorizontal: 12,
        backgroundColor: color.white,
        paddingLeft: 16,
        paddingRight: 8,
        paddingVertical: 8,
        borderRadius: 4,
        width: '100%'
    },
    inputTime: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
        marginLeft: 8,
        // borderWidth: 1,
        // borderColor: color.white,
        // paddingHorizontal: 12,
        backgroundColor: color.white,
        paddingLeft: 16,
        paddingRight: 8,
        paddingVertical: 8,
        borderRadius: 4
    },
    btn_blue: {
        backgroundColor: color.blue,
        borderRadius: 6,
        paddingVertical: 15,
        justifyContent: 'center',
        alignItems: 'center'
    },
    text_input : {
        borderWidth: 1,
        borderColor: color.black,
        marginLeft: 10,
        paddingLeft: 10,
        fontSize: 16,
        width: '20%',
        borderRadius: 7,
        paddingVertical: 7
    },
    header_thanh_toan: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        paddingVertical: 15,
        paddingLeft: 5,
        borderBottomColor: color.lighterGrey,
        borderBottomWidth: 1
    },
    body_thanh_toan: {
        padding: 16,
        marginTop: 6,
        borderTopColor: color.lighterGrey,
        borderTopWidth: 1
    },
    box_flex: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        marginBottom: 10
    }
});
