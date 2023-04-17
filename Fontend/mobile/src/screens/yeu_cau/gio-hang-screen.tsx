import React, {useEffect, useState} from 'react';
import {observer} from 'mobx-react-lite';
import {Animated, Dimensions, FlatList, StyleSheet, View, ViewStyle, Image, TouchableOpacity, Text, Modal, Alert, ScrollView} from 'react-native';
import {Header, Screen} from '../../components';
import {useIsFocused, useNavigation} from "@react-navigation/native"
// import { useStores } from "../../models"
import {color} from '../../theme';
import CenterSpinner from '../../components/center-spinner/center-spinner';
import Ionicons from 'react-native-vector-icons/Ionicons';
import { images } from '../../images';
import { useStores } from '../../models';
import { UnitOfWorkService } from '../../services/api/unitOfWork-service';
import { formatDate, formatNumber, showToast, StatusBarHeight_2 } from '../../services';
import Toast from 'react-native-toast-message';
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

export const GioHangScreen = observer(function GioHangScreen() {
    const navigation = useNavigation();
    const {HDLTModel} = useStores();
    const [isLoading, setLoading] = useState(false);
    const [userID, setUserID] = useState();
    const isFocus = useIsFocused()
    const [isRefresh, setRefresh] = useState(false)
    const [masterData, setMasterData] = useState([])
    const [listDataOrder, setListDataOrder] = useState([])
    const [ListOrderSelect, setListOrderSelect] = useState([])
    const [modal_detail, setModalDetail] = useState(false)
    const [dataDetailYc , setDataDetailYc] = useState()
    const [Id_YC_select, setID_YC_select] = useState()
    const [showModal, setShowModal] = useState({
        modal_choose_dv: false,
        modal_thanh_toan: false,
        modal_chon_thanh_toan: false,
        show_detail_option: false,
        modal_thanh_toan_nhieu_yc: false
    })
    const [dataConfigPayment,setDataConfigPayment] = useState([])
    const [TypePayment,setTypePayment] = useState({})
    const [image_payment, setImage_payment] = useState('') 


    useEffect(() => {
      fetchData();
    }, [isFocus,isRefresh]);
    const fetchData = async () => {
        setRefresh(false)
        let customerID = await HDLTModel.getUserInfoByKey('customerId')
        let userId = await HDLTModel.getUserInfoByKey('userId')
        setUserID(userId)
        let response_payment = await _unitOfWork.user.takeMobileAppConfiguration({})
        if(response_payment?.statusCode == 200){
            setDataConfigPayment(response_payment?.listPayMent)
            setImage_payment(response_payment?.mobileAppConfigurationEntityModel?.paymentScreenIconTransfer)
        } 
        let response = await _unitOfWork.user.getListOrderOfCus({
            "CustomerId": customerID,
            "UserId": userId
        })
        console.log("response: ", response);
        
        if(response?.statusCode == 200){
            let data = [...response?.listOrder].filter(item => item?.statusOrder == 1 && item?.listCustomerOrderDetail?.length > 0)
            setListDataOrder(data)
            console.log("data: ", data);
            
            
        } 
    };

    const CalculatePrice = (listData) => {
        let price = 0;
        listData?.map((item) => {
            price += item?.priceInitial
            if(item?.vat) price += item?.priceInitial* item?.vat/100
        })
        return price
    }

    const handleSelectOption = (id) => {
        let data = [...ListOrderSelect]
        if(data.includes(id)){
            data = data.filter(item => item != id)
            setListOrderSelect(data)
        }else{
            data.push(id)
            setListOrderSelect(data)
        }
    }
    const getDetailYc = async ( id ) => {
        setID_YC_select(id)
        setLoading(true)
        let response = await _unitOfWork.user.getMasterDataOrderDetail(
            { OrderId: id,
              UserId: userID
        })
        
        if(response?.statusCode == 200){
            let response_master = await _unitOfWork.user.searchOptionOfPacketService(
                {   PacketServiceId: response?.customerOrder?.servicePacketId,
                    UserId: userID
                }
            )
            console.log("response_master: ", response_master);
                  
            if(response_master?.statusCode == 200) setMasterData(response_master)
            console.log("response YC: ", response);
            
            setDataDetailYc(response)
            setModalDetail(true)
        }
        setLoading(false)
    }

    const calculatePriceVat = (item) => {
        let price = item?.priceInitial
        if(item?.vat) price += item?.priceInitial*item?.vat / 100
        return price
    }

    const calculatePriceAll = () => {
        let price = 0;
        listDataOrder.map((item) => {
            if(ListOrderSelect.includes(item?.orderId)){
                price += CalculatePrice(item?.listCustomerOrderDetail)
            }
        })
        return price
    }

    const handleSubmit_One_Yc = async () => {
        setLoading(true) 
        if(TypePayment?.id) {  
            
            let res_change_status = await _unitOfWork.user.changeStatusCustomerOrder({
                    OrderId: dataDetailYc?.customerOrder?.orderId,
                    StatusOrder: 1,
                    UserId: userID,
                    PaymentMethod: TypePayment?.id
            })
            
            if(res_change_status?.statusCode == 200) {
                showToast('success', res_change_status?.messageCode)          
                navigation.navigate('MainScreen', {screen: 'YeuCauScreen'}) 
            }
        }else showToast('error', "Bạn chưa chọn hình thức thanh toán!")
        setLoading(false)
    }

    const handleDelete_One_Yc = async () => {
        Alert.alert(
            "Bạn muốn xoá phiếu yêu này",
            "",
            [
              {text: 'Cancel', onPress: () => console.log('Later button clicked')},
              {
                text: "OK",
                onPress: async () => {
                    let res_change_status = await _unitOfWork.user.changeStatusCustomerOrder({
                        OrderId: dataDetailYc?.customerOrder?.orderId,
                        StatusOrder: 6,
                        UserId: userID,
                    })
                    if(res_change_status?.statusCode == 200){
                        showToast("success", 'Xoá thành công!')
                        setModalDetail(false)
                        setRefresh(true)
                    }
                },
              },
            ]
          );
    }

    const handleDeleteOption = async ( id , isExten) => {
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
                    let response = await _unitOfWork.user.deleteOrderOptionByCus(payload)
                    if(response?.statusCode == 200){
                        showToast("success", 'Xoá thành công!')
                        getDetailYc(Id_YC_select)
                        setRefresh(true)
                    }
                },
              },
            ]
          );
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
            </View>
        );
    };

    const handleSetShowModal = (name, value) => {
        let _formData = {...showModal}
        _formData[name] = value
        setShowModal(_formData)
    }

    const showDetailProperty = (id_parent) => {
        let listAtrrOption = [...dataDetailYc?.listAtrrOption]
        dataDetailYc?.listAtrrOption.map((item,index) => {
            
            let data = masterData?.listOptionAttr.filter(item_2 => item_2?.id == item?.attributeConfigurationId)
            
            listAtrrOption[index].categoryName = data[0]?.categoryName
        })
     
        
        listAtrrOption = listAtrrOption.filter(item => item?.servicePacketMappingOptionsId == id_parent)

        return (
            <View style={{width: '95%', marginLeft: 15, marginTop: 10}}>
                {listAtrrOption.map((item) => {
                    return (
                        <Text style={styles.text}>{item?.categoryName}: {item?.dataType == 3 ? formatDate(item?.value) : item?.value}</Text>
                    )
                })}
            </View>
        )
    }

    const showName_Thong_tin_chung = (id_thong_tin_chung) => {
        let name = ''
        let find = masterData?.listAttrPacket?.find(i => i.id == id_thong_tin_chung)
        if(find) name = find.categoryName
        return name
    }

    const ItemView = ({item,index}) => {
        if(item?.listCustomerOrderDetail?.length == 0) return null
        return (
            <View>
                <View style={{height: 10, backgroundColor: color.nau_nhat}}></View>
                <View style={[{marginBottom: 0, backgroundColor: color.white, paddingHorizontal: 14, paddingVertical: 16}, ListOrderSelect?.includes(item?.orderId) ? {backgroundColor: color.blue_nhat} : {}]}>
                    <View style={{flexDirection: 'row'}}>
                        {/* <TouchableOpacity
                            style={{height: 25,width: 25, borderWidth: 2, borderColor: color.blue, borderRadius: 5,alignItems: 'center', justifyContent: 'center', marginRight: 10}}
                            onPress={() => handleSelectOption(item?.orderId)}
                        >
                            {ListOrderSelect?.includes(item?.orderId) ? 
                                <Ionicons name='checkmark-outline' size={20} color={color.blue}/>
                            : null }
                        </TouchableOpacity> */}
                        <Text style={[styles.text_header_blue]}>{item?.listPacketServiceName}</Text>
                    </View>
                   
                    <TouchableOpacity 
                        style={{flexDirection: 'row', justifyContent:'space-between', alignItems: 'center', paddingTop: 16, paddingBottom: 6}}
                        onPress={() => {
                            getDetailYc(item?.orderId)
                        }}
                    >
                        <View style={{width: '85%'}}>
                            {item?.listCustomerOrderDetail?.map((item_2) => {
                                return (
                                    <Text numberOfLines={2} style={styles.text}>Dịch vụ: {item_2?.optionName}</Text>
                                )
                            })}
                            {/* <Text numberOfLines={2} style={styles.text}>Dịch vụ: Dọn nhà</Text>
                            <Text numberOfLines={2} style={styles.text}>Dịch vụ: Nấu ăn</Text> */}
                            <Text numberOfLines={2} style={styles.text}>Số tiền: <Text style={{color: color.blue}}>{formatNumber(CalculatePrice(item?.listCustomerOrderDetail))} vnd</Text></Text>
                        </View>
                        <Ionicons name='caret-forward-outline' size={20} color='black' />
                    </TouchableOpacity>
                </View>
            </View>
            
        )
    }

    return (
        <>
            {isLoading && <CenterSpinner/>}
            <Screen style={ROOT} preset="fixed">
                <Header headerText='Giỏ hàng' onLeftPress={() => navigation.navigate( 'MainScreen', {screen:'DashboardScreen'})} style={modal_detail ? {opacity: 0} : {}}/>
                <View style={[{flex: 1, backgroundColor:color.white},  modal_detail ? {opacity: 0} : {}]}>
                    <FlatList
                        refreshing={isRefresh}
                        onRefresh={() => onRefresh()}
                        showsVerticalScrollIndicator={false}
                        showsHorizontalScrollIndicator={false}
                        style={{flex: 1}}
                        renderItem={ItemView}
                        data={listDataOrder}
                        ListHeaderComponent={topComponent()}
                        keyExtractor={(item, index) => 'gio-hang-screen-' + index + String(item)}
                    />
                </View>
                {/* <View style={[{marginVertical: 12, alignItems: 'center', position: 'absolute', bottom: 0, width: layout.width, borderTopColor: color.nau_nhat, borderTopWidth: 1}, modal_detail ? {opacity: 0} : {}]}>
                    <View style={{flexDirection: 'row', justifyContent: 'space-between', width: '90%', marginBottom: 10, marginTop: 12}}>
                        <Text style={[styles.text_blue]}>Lựa chọn: {ListOrderSelect?.length} phiếu</Text>
                        <Text style={[styles.text_blue]}>Số tiền: {formatNumber(calculatePriceAll())} đ</Text>
                    </View>
                    <TouchableOpacity 
                        style={{width: '90%', paddingVertical: 12, backgroundColor: color.blue, justifyContent: 'center', alignItems: 'center', borderRadius: 7}}
                        onPress={() => handleSetShowModal("modal_thanh_toan_nhieu_yc", true)}
                    >
                        <Text style={{fontSize: 18, color: color.white, fontWeight: '700'}}>Đặt dịch vụ</Text>
                    </TouchableOpacity>
                </View> */}

                <Modal
                    animationType={"slide"}
                    transparent={true}
                    visible={modal_detail}
                    onRequestClose={() => { }}
                >
                    <View style={{flex: 1, marginTop: StatusBarHeight_2}}>
                        <Header headerText='Chi tiết' onLeftPress={() => setModalDetail(false)}/>
                        <ScrollView style={{flex: 1, backgroundColor: color.white}}>
                                <View>
                                <View style={{justifyContent: 'center', alignItems: 'center', marginTop: 12}}>
                                    <Text style={[styles.text_header_black, {width: '60%', textAlign: 'center'}]}>{dataDetailYc?.listServicePacket[0]?.name}</Text>
                                </View>
                                <View style={{marginTop: 16, marginBottom: 10}}>
                                    <Text style={[styles.text_header_blue,{paddingLeft: 16, marginBottom: 10}]}>Dịch vụ</Text>
                                    <View style={[styles.box_item,{flexDirection: 'column'}]}>
                                        <Text style={[styles.text_header,{fontSize: 15, marginBottom: 5}]}>Thông tin chung</Text>
                                        {dataDetailYc?.listAtrrPacket?.map((AtrrPacket) => {
                                            return(
                                                <View style={{paddingLeft: 16}}>
                                                    <Text style={[styles.text]}>{showName_Thong_tin_chung(AtrrPacket.attributeConfigurationId)}: {AtrrPacket.dataType == 3 ? formatDate(AtrrPacket.value) : AtrrPacket.value}</Text>
                                                </View>
                                            )
                                        })}
                                    </View>
                                    {dataDetailYc?.listDetail?.map((item,index) => {
                                        return (
                                            <View style={[styles.box_item,{flexDirection: 'column', paddingHorizontal: 16}]}>
                                                <Text style={[{fontWeight: '600',color: color.black}]}>{index + 1}. {item.optionName.split('--->')[1]}</Text>
                                                {showDetailProperty(item?.optionId, index + 1)}
                                                <View style={{width: '95%', marginLeft: 15, marginTop: 10}}>
                                                    <Text style={styles.text}>Số lượng: {item.quantity}</Text>
                                                </View>
                                                <Text style={[{position: 'absolute', right: 0, marginRight: 16, marginTop: 16}]}>{formatNumber(item?.priceInitial)} vnd</Text>
                                                <View style={{flexDirection: 'row', justifyContent: 'space-between', marginTop: 10, borderTopColor: color.nau_nhat, borderTopWidth: 1, paddingTop: 10}}>
                                                    <Text style={{color: color.black}}>Thành tiền: {formatNumber(calculatePriceVat(item))} vnd  <Text style={{color: color.orange}}>(VAT: {item?.vat ? item?.vat : 0} % )</Text></Text>
                                                    <TouchableOpacity
                                                        key={'test3' + index}
                                                        onPress={() => handleDeleteOption(item?.orderDetailId, false)}
                                                    >
                                                        <Ionicons name='trash-outline' size={25} color={color.error} />
                                                    </TouchableOpacity>
                                                </View>
                                            </View>
                                        )
                                    })}
                                </View>
                                <View style={[styles.box,{justifyContent: 'space-between', alignItems: 'center'}]}>
                                    <View style={[{flexDirection: 'row'}]}>
                                        <Image source={images.icon_dv_bosung} style={{width: 22, height: 22}} />
                                        <Text style={[styles.text_header_black, {marginLeft: 16, color: color.blue}]}>Yêu cầu bổ sung</Text>
                                    </View>
                                </View>
                                <View>
                                    {dataDetailYc?.listOptionExten?.length > 0 ? 
                                        dataDetailYc?.listOptionExten?.map((item,index) => {
                                            return (
                                                <View style={{flexDirection: 'row', alignItems: 'center', paddingHorizontal: 16, marginVertical: 10}}>
                                                    <Ionicons name='ellipse' size={10} color={color.black} />
                                                    <View style={{ marginLeft: 5,flexDirection: 'row', justifyContent: 'space-between', width: '95%'}}>
                                                        <Text style={styles.text}>{item?.name}</Text>
                                                        <TouchableOpacity
                                                            key={'test4' + index}
                                                            onPress={() => handleDeleteOption(item?.id, true)}
                                                        >
                                                            <Ionicons name='trash-outline' size={25} color={color.error} />
                                                        </TouchableOpacity>
                                                    </View>
                                                </View>
                                            )
                                        })
                                    : 
                                        <Text style={{marginVertical: 10, paddingHorizontal: 16}}>Không có yêu cầu bổ sung</Text>
                                    }
                                </View>
                            </View>
                        </ScrollView>
                        
                        <View style={[{marginVertical: 12, alignItems: 'center', position: 'absolute', width: layout.width, borderTopColor: color.nau_nhat, borderTopWidth: 1, bottom: 30, justifyContent: 'space-around', flexDirection: 'row'}]}>
                            <TouchableOpacity 
                                style={{width: '45%', paddingVertical: 12, backgroundColor: color.blue, justifyContent: 'center', alignItems: 'center', borderRadius: 7}}
                                onPress={() => handleDelete_One_Yc()}
                            >
                                <Text style={{fontSize: 18, color: color.white, fontWeight: '700'}}>Xoá</Text>
                            </TouchableOpacity>
                            <TouchableOpacity 
                                style={{width: '45%', paddingVertical: 12, backgroundColor: color.blue, justifyContent: 'center', alignItems: 'center', borderRadius: 7}}
                                onPress={() => handleSetShowModal("modal_thanh_toan", true)}
                            >
                                <Text style={{fontSize: 18, color: color.white, fontWeight: '700'}}>Đặt ngay</Text>
                            </TouchableOpacity>
                        </View>
                    </View>
                    <Toast />
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
                                    </View>
                                    <View style={[styles.box_item,{flexDirection: 'column', paddingHorizontal: 16, width: layout.width, marginLeft: -16}]}>
                                        {dataDetailYc?.listDetail?.map((item,index) => {
                                            return (
                                                <View>
                                                    <Text style={[{fontWeight: '600',color: color.black, width: '70%'}]}>{index + 1}. {item.optionName.split('--->')[1]}<Text style={{color: color.orange}}>(VAT: {item?.vat ? item?.vat : 0} % )</Text></Text>
                                                    {showDetailProperty(item?.optionId, index + 1)}
                                                    <Text style={[{position: 'absolute', right: 0}]}>{formatNumber(item?.priceInitial)} vnd</Text>
                                                </View>
                                            )
                                        })}
                                        {dataDetailYc?.listOptionExten?.length > 0 ?
                                        <Text style={[styles.text,{marginTop: 10}]}>Yêu cầu bổ sung</Text>
                                        : null }
                                        {dataDetailYc?.listOptionExten?.length > 0 ? 
                                            dataDetailYc?.listOptionExten?.map((item) => {
                                                return (
                                                    <View style={{flexDirection: 'row', alignItems: 'center', paddingHorizontal: 16, marginVertical: 10}}>
                                                        <Ionicons name='ellipse' size={10} color={color.black} />
                                                        <View style={{ marginLeft: 5,flexDirection: 'row', justifyContent: 'space-between', width: '95%'}}>
                                                            <Text style={styles.text}>{item?.name}</Text>
                                                        </View>
                                                    </View>
                                                )
                                            })
                                        : 
                                            null
                                        }
                                    </View>

                                    <View style={[styles.box_flex,{marginTop: 16}]}>
                                        <Text style={[styles.text_header_2,{fontSize: 16}]}>Số tiền</Text>
                                        <Text>{formatNumber(CalculatePrice(dataDetailYc?.listDetail))} vnd</Text>
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
                                        onPress={() => handleSubmit_One_Yc()}
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
                                {dataConfigPayment.map((item,index) => {
                                    return (
                                        <TouchableOpacity 
                                            key={'test4-' + index}
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
                </Modal>

               
            </Screen>
        </>
    );
});

const styles = StyleSheet.create({
    text_header_blue: {
        fontSize: 16,
        fontWeight: '700',
        color: color.blue
    },
    text_header_2: {
        fontSize: 18,
        fontWeight: '700',
        color: color.blue
    },
    text_header: {
        fontSize: 20,
        fontWeight: '700',
        color: color.black
    },
    text_header_black: {
        fontSize: 18,
        fontWeight: '700',
        color: color.black
    },
    text: {
        marginBottom: 5,
        color: color.black,
        fontSize: 14
    },
    text_blue: {
        color: color.blue,
        fontSize: 15,
        fontWeight: '600'
    },
    box_item: {
        flexDirection: 'row',
        width: '100%', 
        marginBottom: 5, 
        backgroundColor: color.blue_nhat, 
        paddingHorizontal: 16,
        paddingVertical: 10,
    },
    box: {
        flexDirection: 'row',
        padding: 16,
        backgroundColor: color.nau_nhat,
    },
    modal_container_thanh_toan: {
        backgroundColor: color.white, 
        height: layout.height,
        marginTop: StatusBarHeight_2
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
