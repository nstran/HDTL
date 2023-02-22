import React, {useEffect, useState} from 'react';
import {observer} from 'mobx-react-lite';
import {Animated, Dimensions, FlatList, StyleSheet, View, ViewStyle, Image, ScrollView, TouchableOpacity, Text, Modal, Alert} from 'react-native';
import {Header, Screen} from '../../components';
import {useIsFocused, useNavigation, useRoute} from "@react-navigation/native"
// import { useStores } from "../../models"
import {color} from '../../theme';
import CenterSpinner from '../../components/center-spinner/center-spinner';
import Ionicons from 'react-native-vector-icons/Ionicons';
import { images } from '../../images';
import { useStores } from '../../models';
import { calculateDate, formatDate, formatNumber, showToast, StatusBarHeight_2 } from '../../services';

import { UnitOfWorkService } from '../../services/api/unitOfWork-service';
import Toast from 'react-native-toast-message';

import IframeRenderer, { iframeModel } from '@native-html/iframe-plugin';
import RenderHtml from "react-native-render-html";
import { equalTo, firebaseDatabase, firebaseDatabaseRef, firebaseSet, get, orderByChild, push, query } from '../../config/firebase';

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


export const DetailYeuCauScreen = observer(function DetailYeuCauScreen() {
    const navigation = useNavigation();
    const {HDLTModel} = useStores();
    const [isLoading, setLoading] = useState(false);
    const isFocus = useIsFocused()
    const [isRefresh, setRefresh] = useState(false)
    const [userID, setUserID] = useState()
    const [tapIndex_Detail, setTapIndexDetail] = useState(true)
    const { params }: any = useRoute();
    const [masterData, setMasterData] = useState()
    const [masterDataDetail, setMasterDataDetail] = useState()
    const [masterData_Phieu_hotro, setMasterData_Phieu_hotro] = useState()
    const [showModal, setShowModal] = useState({
        modal_choose_dv: false,
        modal_thanh_toan: false,
        modal_chon_thanh_toan: false,
        show_detail_option: false
    })
    const [dataConfigPayment,setDataConfigPayment] = useState([])
    const [TypePayment,setTypePayment] = useState({})
    const [image_payment, setImage_payment] = useState('') 


    useEffect(() => {
      fetchData();
    }, [isFocus,isRefresh]);
    const fetchData = async () => {
        setLoading(true)
        setRefresh(false)

        let response_payment = await _unitOfWork.user.takeMobileAppConfiguration({})
        if(response_payment?.statusCode == 200) {
            setDataConfigPayment(response_payment?.listPayMent)
            setImage_payment(response_payment?.mobileAppConfigurationEntityModel?.paymentScreenIconTransfer)
        }

        let user_id = await HDLTModel.getUserInfoByKey('userId')
        setUserID(user_id)
        let response = await _unitOfWork.user.getMasterDataOrderDetail(
            {OrderId: params?.item?.orderId,
            UserId: user_id
        })
        
        if(response?.statusCode == 200) {
            console.log("Detail: ", response);
            
            let response_master = await _unitOfWork.user.searchOptionOfPacketService(
                {   PacketServiceId: response?.customerOrder?.servicePacketId,
                    UserId: user_id
                }
            )
            if(response_master?.statusCode == 200) setMasterData(response_master)
            setMasterDataDetail(response)
        }

        setLoading(false)
        
    };

    const handleHuyPhieuYc = async () => {
        Alert.alert(
            "Bạn muốn huỷ phiếu yêu cầu",
            "",
            [
              {text: 'Cancel', onPress: () => console.log('Later button clicked')},
              {
                text: "OK",
                onPress: async () => {
                    let res_change_status = await _unitOfWork.user.changeStatusCustomerOrder({
                        OrderId: masterDataDetail?.customerOrder?.orderId,
                        StatusOrder: 6 ,
                        UserId: userID
                    })
                    if(res_change_status?.statusCode == 200){
                        showToast('success', 'Bạn đã huỷ phiếu yêu cầu thành công!')
                        navigation.navigate('MainScreen', {screen: 'YeuCauScreen'}) 
                    }
                },
              },
            ]
          );
    }

    const showDetailProperty = (data) => {   
        let listAtrrOption = [...masterDataDetail?.listAtrrOption]
        masterDataDetail?.listAtrrOption.map((item,index) => {
            
            let data = masterData?.listOptionAttr.filter(item_2 => item_2?.id == item?.attributeConfigurationId)
            
            listAtrrOption[index].categoryName = data[0]?.categoryName
        })
     
        
        listAtrrOption = listAtrrOption.filter(item => item?.servicePacketMappingOptionsId == data?.optionId)

        return (
            <View style={{width: '95%', marginLeft: 15, marginTop: 10}}>
                {listAtrrOption.map((item) => {
                    return (
                        <Text style={styles.text}>{item?.categoryName}: {item?.dataType == 3 ? formatDate(item?.value) : item?.value}</Text>
                    )
                })}
                <Text style={styles.text}>Số lượng: {data?.quantity} </Text>
            </View>
        )
    }

    const handleSetShowModal = (name, value) => {
        let _formData = {...showModal}
        _formData[name] = value
        setShowModal(_formData)
    }

    const calculatePriceVat = (item) => {
        let price = item?.priceInitial*item?.quantity
        if(item?.vat) price += item?.priceInitial*item?.quantity*item?.vat / 100
        return price
    }
    const calculateTotalPrice = () => {
        let price = 0
        masterDataDetail?.listDetail.map((item) => {
            price += calculatePriceVat(item)
        })
        masterDataDetail?.listOptionExten.map((item) => {
            price += item?.price
        })
        return price
    }

    const calculateGiamGia = () => {
        let price = calculateTotalPrice()

        if(masterDataDetail?.customerOrder?.discountType){
            price = masterDataDetail?.customerOrder?.discountValue
        }else{
            price = price * masterDataDetail?.customerOrder?.discountValue / 100
        }
        return price
    }

    const handleXacNhanYc = async () => {
        if(TypePayment?.id) {

                let res_change_status = await _unitOfWork.user.changeStatusCustomerOrder({
                        OrderId: masterDataDetail?.customerOrder?.orderId,
                        StatusOrder: 3,
                        UserId: userID,
                        PaymentMethod: TypePayment?.id
                })
                
                if(res_change_status?.statusCode == 200) {
                    showToast('success', res_change_status?.messageCode)             
                    navigation.navigate('MainScreen', {screen: 'YeuCauScreen'}) 
                }
                
    
        }else showToast('error', "Bạn chưa chọn hình thức thanh toán!")
    }

    const handleDeleteOption = async ( id , isExten, index) => {
        
        Alert.alert(
            "Bạn muốn xoá dịch vụ này",
            "",
            [
              {text: 'Cancel', onPress: () => console.log('Later button clicked')},
              {
                text: "OK",
                onPress: async () => {
                    let payload = {
                        Id: id,
                        IsExtend : isExten
                    }
                    if(index > 1 && !isExten){
                        let response = await _unitOfWork.user.deleteOrderOptionByCus(payload)
                        if(response?.statusCode == 200){
                            showToast("success", 'Xoá thành công!')
                            setRefresh(true)
                        }
                    }else showToast("error", 'Không thể xoá!')
                    
                    if(isExten){
                        let response = await _unitOfWork.user.deleteOrderOptionByCus(payload)
                        if(response?.statusCode == 200){
                            showToast("success", 'Xoá thành công!')
                            setRefresh(true)
                        }
                    }
                    
                },
              },
            ]
          );
    }

    const getData_Phieu_hotro_dichvu = async () => {
        console.log(userID);
        let orderId_yeucau = masterDataDetail?.listDetail[0]?.orderId
        let orderId_phieuhotro = ''
        
        masterDataDetail?.listCustomerOrderAction.map((item) => {
            if(item?.objectId == orderId_yeucau) orderId_phieuhotro = item?.orderId
        })


        if(orderId_phieuhotro){
            
            let res = await _unitOfWork.user.getMasterDataOrderActionDetail({
                UserId: userID, 
                OrderActionId: orderId_phieuhotro,
            })
            setMasterData_Phieu_hotro(res)
            console.log("Res ho tro: ", res);
            
        }

    }

    const chatWith_quanly = async () => {
        setLoading(true)
        let _userId = await HDLTModel.getUserInfoByKey('userId')
        let fullname = await HDLTModel.getUserInfoByKey('userFullName')
        let response = await _unitOfWork.user.createDataFireBase({
            RoomName:`${_userId}_${masterDataDetail?.quanLyGoi_UserId}`,
            OtherId: masterDataDetail?.quanLyGoi_UserId,
            UserId: _userId
        })
        
        if(response?.statusCode == 200){
            await get(query(firebaseDatabaseRef(firebaseDatabase, "rooms"), orderByChild('roomname'), equalTo(response?.roomname)))
            .then((snapshot) => { 
                if(!snapshot.exists()){
                    
                    // firebaseSet(firebaseDatabaseRef(firebaseDatabase, `rooms/${response?.roomname}`), {
                    //     receiver: masterDataDetail?.quanLyGoi_UserId,
                    //     receiverName: masterDataDetail?.quanLyGoi_Name,
                    //     roomname: response?.roomname,
                    //     userCreate: _userId,
                    //     userCreateName: fullname
                    // });
                    push(firebaseDatabaseRef(firebaseDatabase, `rooms`), {
                        receiver: masterDataDetail?.quanLyGoi_UserId,
                        receiverName: masterDataDetail?.quanLyGoi_Name,
                        roomname: response?.roomname,
                        userCreate: _userId,
                        userCreateName: fullname
                      })
                }
            }) 
        }
        let arr_name = masterDataDetail?.quanLyGoi_Name.split(" ")      
        let text_name = arr_name[0]?.charAt(0) + arr_name[arr_name?.length - 1]?.charAt(0)
        navigation.navigate("ChatScreen", {
            // data: {...item, url_avatar, text_name},
            data: {
                roomname: response?.roomname,
                receiver: masterDataDetail?.quanLyGoi_UserId,
                nickname_reciver: masterDataDetail?.quanLyGoi_Name,
                url_avatar : '', 
                text_name : text_name
            },
        })
        setLoading(false)
    }

    const converName_Tuychon = (str) => {
        let arr = str.split('--->')
        return arr[arr?.length - 1]
    }
    const converName_nhanvien = (str) => {
        let arr = str.split(', ')
        return arr[arr?.length - 1]
    }


    const goToPage = (page) => {
        navigation.navigate(page);
    };
    const onRefresh = () => {
        setRefresh(true)
    };

    const topComponent = () => {
        return (
            <View>
                <View style={styles.modal_detail_container}>
                            <View style={{flex: 1, paddingBottom: 50}}>
                                {/* <ScrollView style={{marginBottom: 20}}> */}
                                    {/* Chuyen tapindex */}
                                    <View style={{flexDirection: 'row', justifyContent: 'space-between', paddingHorizontal: 16, marginTop: 12}}>
                                        <TouchableOpacity 
                                            style={[styles.btn_tapIndex, tapIndex_Detail ? {backgroundColor: color.blue} : {}]}
                                            onPress={() => setTapIndexDetail(true)}
                                        >
                                            <Text style={tapIndex_Detail ? {color: color.white} : {}}>Phiếu yêu cầu</Text>
                                        </TouchableOpacity>
                                        <TouchableOpacity 
                                            style={[styles.btn_tapIndex,tapIndex_Detail ? {} : {backgroundColor: color.blue}]}
                                            onPress={() => {
                                                getData_Phieu_hotro_dichvu()
                                                setTapIndexDetail(false)
                                            }}
                                        >
                                            <Text style={tapIndex_Detail ? {} : {color: color.white}}>Phiếu hỗ trợ dịch vụ</Text>
                                        </TouchableOpacity>
                                    </View>
                                    {tapIndex_Detail ? 
                                        <View style={{marginTop: 24}}>
                                            <View style={{alignItems: 'center', marginBottom: 24}}>
                                                <Text style={styles.text_header_blue}>{masterDataDetail?.listServicePacket[0]?.name}</Text>
                                            </View>
                                            {masterDataDetail?.listDetail.map((item,index) => {
                                                return (
                                                    <View style={[styles.box_item,{flexDirection: 'column'}]}>
                                                        <Text style={[{fontWeight: '600',color: color.black}]}>{index + 1}. {item.optionName.split('--->')[1]}</Text>
                                                        {showDetailProperty(item)}
                                                        <View style={{flexDirection: 'row', justifyContent: 'space-between'}}>
                                                            <Text style={{color: color.black, marginTop: 10}}>Thành tiền: {formatNumber(calculatePriceVat(item))} vnd  <Text style={{color: color.orange}}>(VAT: {item?.vat ? item?.vat : 0} % )</Text></Text>
                                                            {masterDataDetail?.customerOrder?.statusOrder == 3 ? 
                                                            <TouchableOpacity
                                                                onPress={() => handleDeleteOption(item?.orderDetailId, false, masterDataDetail?.listDetail?.length)}
                                                            >
                                                                <Ionicons name='trash-outline' size={25} color={color.error} />
                                                            </TouchableOpacity>
                                                            : null }
                                                        </View>
                                                        
                                                        <Text style={[{position: 'absolute', right: 0, marginRight: 16, marginTop: 16}]}>{formatNumber(item?.priceInitial)} vnd</Text>
                                                    </View>
                                                )
                                            })}

                                            {masterDataDetail?.listOptionExten?.length > 0 ?
                                                <View>
                                                    <View style={[styles.box_item,{marginBottom: 0}]}>
                                                        <Image source={images.memo_pencil} style={{width: 20, height: 20}} />
                                                        <Text style={[styles.text,{fontWeight: '600'}]}>Yêu cầu bổ sung</Text>
                                                    </View>
                                                    <View style={{backgroundColor: color.white, paddingHorizontal: 16, paddingVertical: 10}}>
                                                            {masterDataDetail?.listOptionExten.map((item,index) => {
                                                                return (
                                                                    <View style={{}}>
                                                                        <View style={{flexDirection: 'row', alignItems: 'center'}}>
                                                                            <Ionicons name='ellipse' size={10} color={color.black} />
                                                                            <Text style={[styles.text,{marginLeft: 5}]}>{item?.name} {masterDataDetail?.customerOrder?.statusOrder == 3 ? <Text style={{color: color.orange}}>({item?.statusName})</Text> : null}</Text>
                                                                        </View>
                                                                        {masterDataDetail?.customerOrder?.statusOrder != 2 ?
                                                                            <View style={{flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center'}}>
                                                                                <Text>Giá: {formatNumber(item?.price)}</Text>
                                                                                {masterDataDetail?.customerOrder?.statusOrder == 3 ? 
                                                                                    <TouchableOpacity
                                                                                        onPress={() => handleDeleteOption(item?.id, true, index)}
                                                                                    >
                                                                                        <Ionicons name='trash-outline' size={25} color={color.error} />
                                                                                    </TouchableOpacity>
                                                                                : null }
                                                                            </View>
                                                                        : null }
                                                                    </View>   
                                                                )
                                                            })}
                                                    </View>
                                                </View>
                                                
                                            : null }
                                            <View style={[styles.box_item, {paddingTop: 24, flexDirection: 'column'}]}>
                                                <View style={styles.box_item_child}>
                                                    <Text style={styles.text_blue}>Số tiền</Text>
                                                    <Text style={styles.text_blue}>{formatNumber(calculateTotalPrice())} đ</Text>
                                                </View>
                                                {/* <View style={styles.box_item_child}>
                                                    <Text style={[styles.text_blue, {color: color.orange}]}>Giảm giá</Text>
                                                    <Text style={[styles.text_blue, {color: color.orange}]}>{formatNumber(calculateGiamGia())} đ</Text>
                                                </View> */}
                                                <View style={styles.box_item_child}>
                                                    <Text style={styles.text_blue}>Thanh toán</Text>
                                                    <Text style={styles.text_blue}>{formatNumber(calculateTotalPrice())} đ</Text>
                                                </View>
                                                <View style={styles.box_item_child}>
                                                    <Text style={styles.text_blue}>Hình thức thanh toán</Text>
                                                    <Text style={styles.text_blue}>{masterDataDetail?.customerOrder?.paymentMethodOrder == 1 ? 'Chuyển khoản' : 'Thanh toán qua VNPAY'}</Text>
                                                </View>
                                                <View style={styles.box_item_child}>
                                                    <Text style={styles.text_blue}>Trạng thái</Text>
                                                    <Text style={[styles.text_blue, {color: color.text_naunhat}]}>{params?.item?.orderStatusName}</Text>
                                                </View>
                                            </View>

                                            {/* Huỷ hoặc xác nhận phiếu yêu cầu */}
                                            <View style={[{flexDirection: 'row', justifyContent:'space-around', marginTop: 16}]}>
                                                {masterDataDetail?.listOptionExten?.length > 0 ?
                                                    [2,3].includes(masterDataDetail?.customerOrder?.statusOrder) ?
                                                        <TouchableOpacity 
                                                            style={{width: '40%', backgroundColor: color.error, paddingVertical: 12, borderRadius: 6, alignItems: 'center'}}
                                                            onPress={() => handleHuyPhieuYc()}
                                                        >
                                                            <Text style={{fontSize: 18, color: color.white, fontWeight: '600'}}>Huỷ</Text>
                                                        </TouchableOpacity>
                                                    : null
                                                : null }
                                                {masterDataDetail?.customerOrder?.statusOrder == 3 ? 
                                                    <TouchableOpacity 
                                                        style={{width: '40%', backgroundColor: color.blue, paddingVertical: 12, borderRadius: 6, alignItems: 'center'}}
                                                        onPress={() => handleSetShowModal("modal_thanh_toan", true)}
                                                    >
                                                        <Text style={{fontSize: 18, color: color.white, fontWeight: '600'}}>Xác nhận</Text>
                                                    </TouchableOpacity>
                                                : null }
                                            </View>
                                            
                                            {/* CHat với quản lý */}
                                            <View style={{alignItems: 'center'}}>
                                                <TouchableOpacity 
                                                    style={[styles.btn]}
                                                    onPress={chatWith_quanly}
                                                >
                                                    <Text style={[styles.text, {color: color.white}]}>Chat với quản lý</Text>
                                                </TouchableOpacity>
                                            </View>
                                            
                                                                              
                                        </View>
                                    :
                                        // phiếu hỗ trợ dịch vụ
                                        <View style={{marginTop: 24}}>
                                            <View style={styles.box}>
                                                <Text style={[styles.text_header_blue, {marginBottom: 10}]}>Nhân viên phụ trách</Text>
                                                {masterData_Phieu_hotro?.listCustomerOrderTask?.map((item) => {
                                                    return(
                                                        <View>
                                                            <View style={{flexDirection: 'row', justifyContent: 'space-between',  marginBottom: 7}}>
                                                                <Text style={styles.text}>{converName_Tuychon(item?.path) }</Text>
                                                                <Text style={styles.text}>{converName_nhanvien(item?.listEmpName)}</Text>
                                                            </View>
                                                            <View style={{flexDirection: 'row', justifyContent: 'space-between',  marginBottom: 7}}>
                                                                <Text style={styles.text}>Số điện thoại</Text>
                                                                <Text style={styles.text}>12345678</Text>
                                                            </View> 
                                                        </View>
                                                        

                                                    )
                                                })}
                                                
                                            </View>
                                            <View style={styles.box}>
                                                <Text style={[styles.text_header_blue, {marginBottom: 10}]}>Báo cáo tiến độ</Text>
                                                {masterData_Phieu_hotro?.listReportPoint?.map((item) => {
                                                    return (
                                                        <View style={{flexDirection: 'row',  marginBottom: 7}}>
                                                            <Ionicons name='ellipse' size={24} color={color.orange} />
                                                            <View style={{marginLeft: 10}}>
                                                                <Text style={[styles.text_header,{marginBottom: 16, fontSize: 15}]}>{item?.name}</Text>
                                                                <Text style={[styles.text, {marginBottom: 7}]}>Người phụ trách: {item?.empName}</Text>
                                                                <Text style={[styles.text, {marginBottom: 7}]}>Thời gian: {formatDate(item?.deadline, "dd/MM/YY - hh:mm")}</Text>
                                                                <Text style={[styles.text, {marginBottom: 7}]}>Nội dung: {item?.content}</Text>
                                                                {/* <TouchableOpacity style={{borderWidth: 1, paddingVertical: 5, alignItems: 'center', width: '45%', borderRadius: 7}}>
                                                                    <Text style={styles.text_blue}>Xem chi tiết</Text>
                                                                </TouchableOpacity> */}
                                                            </View>
                                                        </View>
                                                    )
                                                })}
                                                
                                            </View>
                                           
                                        </View>
                                    }
                                {/* </ScrollView> */}
                            </View>
                    </View>
            </View>
        );
    };

    return (
        <>
            {isLoading && <CenterSpinner/>}
            <Screen style={ROOT} preset="fixed">
                <View style={{flex: 1, backgroundColor: color.nau_nhat}}>
                    <Header headerText='Chi tiết' onLeftPress={() => navigation.goBack()} />
                    <FlatList
                        refreshing={isRefresh}
                        onRefresh={() => onRefresh()}
                        showsVerticalScrollIndicator={false}
                        showsHorizontalScrollIndicator={false}
                        style={{flex: 1}}
                        renderItem={null}
                        data={[]}
                        ListHeaderComponent={topComponent()}
                        keyExtractor={(item, index) => 'detail-yc-' + index + String(item)}
                    />
                </View>
                <Modal
                    animationType={"slide"}
                    transparent={true}
                    visible={showModal?.modal_thanh_toan}
                    onRequestClose={() => { }}
                >
                    <View style={styles.modal_container_thanh_toan}>
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
                                {/* <Text style={[styles.text_header,{fontSize: 16, marginTop: 5}]}>{data_Parent?.name}</Text> */}
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
                                {masterDataDetail?.listDetail.map((item,index) => {
                                    return (
                                        <View style={[styles.box_item,{flexDirection: 'column', paddingHorizontal: 0}]}>
                                            <Text style={[{fontWeight: '600',color: color.black}]}>{index + 1}. {masterDataDetail?.listDetail[0].optionName.split('--->')[1]}</Text>
                                            {showDetailProperty(item?.optionId, index + 1)}
                                            <Text style={{color: color.black, marginTop: 10}}>Thành tiền: {formatNumber(calculatePriceVat(item))} vnd  <Text style={{color: color.orange}}>(VAT: {item?.vat ? item?.vat : 0} % )</Text></Text>
                                            <Text style={[{position: 'absolute', right: 0, marginRight: 16, marginTop: 16}]}>{formatNumber(item?.priceInitial)} vnd</Text>
                                        </View>
                                    )
                                })}

                                {masterDataDetail?.listOptionExten?.length > 0 ?
                                    <View style={[styles.box_item,{paddingLeft: 0}]}>
                                        <Image source={images.memo_pencil} style={{width: 20, height: 20}} />
                                        <View style={{width: '95%',marginLeft: 5}}>
                                            <Text style={[styles.text,{fontWeight: '600'}]}>Yêu cầu bổ sung</Text>
                                            {masterDataDetail?.listOptionExten.map((item => {
                                                return (
                                                    <View style={{flexDirection: 'row'}}>
                                                        <Ionicons name='ellipse' size={12} color={color.black} />
                                                        <View style={{marginLeft: 5}}>
                                                            <Text style={styles.text}>{item?.name} {item?.status == 2 ? <Text style={{color: color.orange}}>({item?.statusName})</Text> : null}</Text>
                                                            <Text>Giá: {formatNumber(item?.price)}</Text>
                                                        </View>
                                                    </View>
                                                    
                                                        
                                                )
                                            }))}
                                            
                                        </View>
                                    </View>
                                : null }



                                <View style={[styles.box_flex,{marginTop: 16}]}>
                                    <Text style={[styles.text_header_2,{fontSize: 16}]}>Số tiền</Text>
                                    <Text>{formatNumber(calculateTotalPrice())} vnd</Text>
                                </View>
                                {/* <View style={[styles.box_flex]}>
                                    <Text style={[styles.text_header_2,{fontSize: 16, color: color.orange}]}>Giảm giá</Text>
                                    <Text style={{color: color.orange}}>{formatNumber(calculateGiamGia())} vnd</Text>
                                </View> */}
                                <View style={[styles.box_flex]}>
                                    <Text style={[styles.text_header_2,{fontSize: 16}]}>Thanh toán</Text>
                                    <Text>{formatNumber(calculateTotalPrice())} vnd</Text>
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
                                    onPress={() => handleXacNhanYc()}
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
                            
                        </View>
                    </Modal>
                </Modal>
            </Screen>
        </>
    );
});

const styles = StyleSheet.create({
    text_header_blue: {
        fontSize: 18,
        fontWeight: '700',
        color: color.blue
    },
    text: {
        marginBottom: 5,
        color: color.black,
        fontSize: 14
    },
    text_header: {
        fontSize: 20,
        fontWeight: '700',
        color: color.black
    },
    text_blue: {
        color: color.blue,
        fontSize: 16,
        fontWeight: '600'
    },
    text_header_2: {
        fontSize: 18,
        fontWeight: '700',
        color: color.blue
    },
    modal_detail_container: {
        backgroundColor: color.nau_nhat, 
        // height: layout.height - StatusBarHeight_2,
        // marginTop: StatusBarHeight_2
    },
    header_modal_detail: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        paddingHorizontal: 16,
        paddingVertical: 12
    },
    btn_tapIndex: {
        width: '49%',
        alignItems: 'center',
        paddingVertical: 12,
        borderRadius: 7,
        borderWidth: 1
    },
    box: {
        width: '100%', 
        marginBottom: 5, 
        backgroundColor: color.white, 
        paddingHorizontal: 16,
        paddingVertical: 10
    },
    box_item: {
        flexDirection: 'row',
        width: '100%', 
        marginBottom: 5, 
        backgroundColor: color.white, 
        paddingHorizontal: 16,
        paddingVertical: 10
    },
    box_item_child: {
        flexDirection: 'row', 
        justifyContent: 'space-between',
        width: '100%',
        marginBottom: 10
    },
    modal_container_thanh_toan: {
        backgroundColor: color.white, 
        height: layout.height,
    },
    box_flex: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        marginBottom: 10
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
    btn : {
        backgroundColor: color.blue,
        paddingVertical: 12,
        width: '50%',
        alignItems: 'center',
        borderRadius: 10,
        marginTop: 16
    }
});
