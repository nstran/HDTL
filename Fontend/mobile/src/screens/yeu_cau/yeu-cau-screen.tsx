import React, {useEffect, useState} from 'react';
import {observer} from 'mobx-react-lite';
import {Animated, Dimensions, FlatList, StyleSheet, View, ViewStyle, Image, Text, TouchableOpacity, Modal, ScrollView, KeyboardAvoidingView, Platform} from 'react-native';
import {Header, Screen} from '../../components';
import {useIsFocused, useNavigation} from "@react-navigation/native"
// import { useStores } from "../../models"
import {color} from '../../theme';
import CenterSpinner from '../../components/center-spinner/center-spinner';
import Ionicons from 'react-native-vector-icons/Ionicons';
import { images } from '../../images';
import { formatDate, formatNumber, showToast, StatusBarHeight, StatusBarHeight_2 } from '../../services';
import { useStores } from '../../models';
import { UnitOfWorkService } from '../../services/api/unitOfWork-service';
import { TextInput } from 'react-native-gesture-handler';
import {KeyboardAwareScrollView} from 'react-native-keyboard-aware-scroll-view';
import { log } from 'react-native-reanimated';

const _unitOfWork = new UnitOfWorkService()

const layout = Dimensions.get('window');

const ROOT: ViewStyle = {
    backgroundColor: color.primary,
    flex: 1,
};

const data = [1,1,1]

export const YeuCauScreen = observer(function YeuCauScreen() {
    const navigation = useNavigation();
    const {HDLTModel} = useStores();
    const [isLoading, setLoading] = useState(false);
    const isFocus = useIsFocused()
    const [isRefresh, setRefresh] = useState(false)
    const [modal_option, setModal_Option] = useState(false)
    const [modal_danh_gia, setModal_Danh_Gia] = useState(false)
    const [modal_detail, setModal_Detail] = useState(false)
    const [tapIndex_Detail, setTapIndexDetail] = useState(true)
    const [masterData, setMasterData] = useState([])
    const [ID_dich_vu_select, setID_dich_vu_select] = useState()
    const [Star_Select, setStar_Select] = useState(0)
    const [noiDung_DanhGia,setNoiDung_DanhGia] = useState('')

    const [listNhanVien_danhGia, setListNhanVien_DanhGia] = useState([])
    const [nhan_vien_select, setNhan_vien_select] = useState('') // nhân viên được chọn xem đánh giá
    const [payload_nhanvien_danhGia, setPayload_nhanvien_danhGia] = useState([])

    const star = [1,2,3,4,5]

    useEffect(() => {
      fetchData();
    }, [isFocus,isRefresh]);
    const fetchData = async () => {
        setLoading(true)
        setRefresh(false)

        if(isFocus){
            let customerID = await HDLTModel.getUserInfoByKey('customerId')
            let userId = await HDLTModel.getUserInfoByKey('userId')
            let response = await _unitOfWork.user.getListOrderOfCus({
                "CustomerId": customerID,
                "UserId": userId
            })
            
            console.log({
                "CustomerId": customerID,
                "UserId": userId
            });
            
            if(response?.statusCode == 200){
                let data = [...response?.listOrder].filter(item => item?.statusOrder != 1)
                data.sort((a,b) => {
                    return new Date(b?.createdDate).getTime() - new Date(a?.createdDate).getTime()
                })
                console.log("Response: ", response);
                setMasterData(data)
            }
        }
        

        setLoading(false)
        
    };

    const CalculatePrice = (listData) => {
        let price = 0;
        
        listData?.listCustomerOrderDetail.map((item) => {
            price += item?.priceInitial*item?.quantity
            if(item?.vat) price += item?.priceInitial*item?.quantity*item?.vat/100
        })

        listData?.listCustomerOrderDetailExten?.map((item) => {
            price += item?.price
        })

        return price
    }

    const handle_danh_gia = async () => {
        if(Star_Select == 0){
            showToast('error', 'Bạn chưa chọn sao đánh giá!')
            return
        }
        console.log("payload :", {
            OrderId : ID_dich_vu_select,
            RateStar : Star_Select,
            RateContent : noiDung_DanhGia,
            ListEmployeeRatingStar: payload_nhanvien_danhGia
        })
        setLoading(true)
        let respose = await _unitOfWork.user.ratingOrder({
            OrderId : ID_dich_vu_select,
            RateStar : Star_Select,
            RateContent : noiDung_DanhGia,
            ListEmployeeRatingStar: payload_nhanvien_danhGia
            
        })
        console.log("Res đánh giá: ", respose);
        if(respose?.statusCode == 200){
            showToast('success', respose?.messageCode)
            setNoiDung_DanhGia('')
            setStar_Select(0)
            setModal_Danh_Gia(false)
            setRefresh(true)
        }
        setLoading(false)
        
    }

    const goToPage = (page, params) => {
        navigation.navigate(page, params);
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

    const Net_dut = () => {
        return (
            <View>
                <Text numberOfLines={1} ellipsizeMode="clip"  style={{color: color.lightGrey, width: '100%', opacity: 0.5}}>-------------------------------------------------------------------------------------------------------------------------</Text>
            </View>
            
        )
    }

    const handle_choose_danhGia = async (item: any) => {
        setNhan_vien_select('')
        setListNhanVien_DanhGia([])
        setPayload_nhanvien_danhGia([])
        setID_dich_vu_select(item.orderId)
        setStar_Select(item.rateStar)
        setNoiDung_DanhGia(item.rateConent ? item.rateConent : '')

        console.log("item : ", item)

        let list_NhanVien = []
        let payload_Nv_danhGia = []
        item?.listEmployeeEntityModel?.map((data) => {
            let obj = {
                label: data?.employeeName,
                value: data?.employeeId
            }
            list_NhanVien.push(obj)

            // tạo payload nhân viên đánh giá
            let obj_2 = {
                EmployeeId: data?.employeeId,
                OrderProcessId : item.orderId,
                RateContent : data?.rateContent ? data.rateContent : ''
            }
            payload_Nv_danhGia.push(obj_2)
        })
        if(list_NhanVien?.length > 0) setNhan_vien_select(list_NhanVien[0].value)
        setListNhanVien_DanhGia(list_NhanVien)

        setPayload_nhanvien_danhGia(payload_Nv_danhGia)

        setModal_Danh_Gia(true)
    }

    const getNameNhanVienDanhGia = (id_nv) => {
        let find = listNhanVien_danhGia.find(i => i.value == id_nv)
        if(find) return find?.label
        return ''
    }

    const ItemView = ({item,index}) => {   
        return (
            <View style={{marginBottom: 16, backgroundColor: color.white, paddingHorizontal: 14, paddingVertical: 16}}>
                <Text style={[styles.text_header_blue]}>{item?.listPacketServiceName}</Text>
                <TouchableOpacity 
                    style={{flexDirection: 'row', justifyContent:'space-between', alignItems: 'center', paddingTop: 16, paddingBottom: 6}}
                    onPress={() => goToPage('DetailYeuCauScreen', {item : item})}
                >
                    <View style={{width: '85%'}}>
                        {item?.listCustomerOrderDetail.map((item_2) => {
                            return (
                                <Text numberOfLines={2} style={styles.text}>Dịch vụ: {item_2?.optionName}</Text>
                            )
                        })}
                        {/* <Text numberOfLines={2} style={styles.text}>Dịch vụ: Dọn nhà</Text>
                        <Text numberOfLines={2} style={styles.text}>Dịch vụ: Nấu ăn</Text> */}
                        <Text numberOfLines={2} style={styles.text}>Số tiền: <Text style={{color: color.blue}}>{formatNumber(CalculatePrice(item))} vnd</Text></Text>
                    </View>
                    <Ionicons name='caret-forward-outline' size={20} color='black' />
                </TouchableOpacity>
                {/* {item?.statusOrderActionName ? 
                <TouchableOpacity style={{flexDirection: 'row', justifyContent:'space-between', alignItems: 'center', paddingTop: 10}}>
                    <Text numberOfLines={2} style={[styles.text,{width: '85%'} ]}>Phiếu hỗ trợ dịch vụ: <Text style={{color: color.orange}}>{item?.statusOrderActionName}</Text></Text>
                    <Ionicons name='caret-forward-outline' size={20} color='black' />
                </TouchableOpacity>
                : null }
                {item?.statusOrderActionName ? 
                <TouchableOpacity style={{flexDirection: 'row', justifyContent:'space-between', alignItems: 'center', paddingTop: 10}}>
                    <Text numberOfLines={2} style={[styles.text,{width: '85%'} ]}>Phiếu yêu cầu bổ sung dịch vụ: <Text style={{color: color.orange}}>{item?.statusOrderActionName}</Text></Text>
                    <Ionicons name='caret-forward-outline' size={20} color='black' />
                </TouchableOpacity>
                : null } */}
                <TouchableOpacity style={{flexDirection: 'row', justifyContent:'space-between', alignItems: 'center', paddingTop: 10}}>
                    <Text numberOfLines={2} style={[styles.text,{width: '85%'} ]}>Phiếu yêu cầu: <Text style={{color: color.orange}}>{item?.orderStatusName}</Text></Text>
                    <Ionicons name='caret-forward-outline' size={20} color='black' />
                </TouchableOpacity>

                {item?.orderStatusName == "Đã thanh toán" ?
                <TouchableOpacity 
                    style={{backgroundColor: "#D4480C", width: 80, borderRadius: 5, alignItems: 'center', paddingVertical: 5, justifyContent: 'center', marginTop: 10}}
                    onPress={() => {
                        handle_choose_danhGia(item)
                    }}
                >
                    <Text style={{color: color.white}}>Đánh giá</Text>
                </TouchableOpacity>
                : null}
            </View>
        )
    }

    return (
        <>
            {isLoading && <CenterSpinner/>}
            <Screen style={ROOT} preset="fixed">
                <Header headerText='Quản lý đặt dịch vụ' style={modal_danh_gia ? {opacity: 0.6}: {}} rightText='' onRightPress={() =>{}} onLeftPress={() => goToPage('DashboardScreen')}/>
                <View style={[{flex: 1,paddingTop: 16, backgroundColor: color.nau_nhat}, modal_danh_gia ? {opacity: 0.6}: {}]}>
                    <FlatList
                        refreshing={isRefresh}
                        onRefresh={() => onRefresh()}
                        showsVerticalScrollIndicator={false}
                        showsHorizontalScrollIndicator={false}
                        style={{flex: 1}}
                        renderItem={ItemView}
                        data={masterData}
                        ListHeaderComponent={topComponent()}
                        keyExtractor={(item, index) => 'yc-screen-' + index + String(item)}
                    />
                </View>
                <Modal
                    animationType={"slide"}
                    transparent={true}
                    visible={modal_option}
                    onRequestClose={() => { }}
                >
                    <View style={styles.modal_container}>
                        <View style={{}}>
                            <View style={styles.header_apply}>
                                <TouchableOpacity style={{marginRight: 10, marginTop: 10}} onPress={() => setModal_Option(false)}>
                                    <Ionicons name={'close-outline'} color='black' size={30} />
                                </TouchableOpacity>
                            </View>
                            <View style={{ paddingHorizontal: 20 }}>
                                <FlatList
                                    showsVerticalScrollIndicator={false}
                                    showsHorizontalScrollIndicator={false}
                                        style={{ height: layout.height*7/10 - 170}}
                                    renderItem={null}
                                    data={[]}
                                    ListHeaderComponent={() => {
                                        return (
                                            <View style={{}}>
                                                <Text style={[styles.title,{marginBottom: 16}]}>Trang thái phiếu yêu cầu</Text>
                                                {data.map((item,index) => {
                                                    return (
                                                        <TouchableOpacity style={{ flexDirection: 'row', marginBottom: 16, alignItems: 'center' }} onPress={() => {}}>
                                                            <Ionicons name='radio-button-on-outline'color={color.xanh_nhat} size={25} />
                                                            <Text style={{ marginLeft: 17, fontSize: 15, color: color.lightGrey }}>Đang thực hiện</Text>
                                                        </TouchableOpacity>
                                                    )
                                                })}
                                                
                                            </View>
                                        )
                                    }}
                                    keyExtractor={(item, index) => 'modal-option-2-' + index + String(item)}
                                />
                            </View>
                        </View>
                        <View style={{position: 'absolute', width: layout.width*8/10, bottom: 0, marginBottom: 120, marginLeft: layout.width/10}}>
                            <TouchableOpacity style={[{ width: '100%', borderRadius: 25, paddingVertical: 14, backgroundColor: color.blue, alignItems: 'center' }]} onPress={() => setModal_Option(false)}>
                                <Text style={[{fontSize: 18, fontWeight: '700', color: color.white}]}>OK</Text>
                            </TouchableOpacity>
                        </View>
                    </View>
                </Modal>

                <Modal
                    animationType={"slide"}
                    transparent={true}
                    visible={modal_danh_gia}
                    onRequestClose={() => { }}
                >
                    <View style={styles.modal_container}>
                            <View style={[styles.box_item_child,{paddingHorizontal: 20, paddingTop: 16, marginBottom: 0}]}>
                                    <Text style={[styles.text_header_blue, {color: '#182954'}]}>Đánh giá</Text>
                                    <TouchableOpacity onPress={() => setModal_Danh_Gia(false)}>
                                        <Ionicons name='close-outline' size={30} color={'#182954'} />
                                    </TouchableOpacity>
                                    
                            </View>
                                <View style={{flex: 1, paddingBottom: 80}}>
                                <KeyboardAwareScrollView>
                                        <View style={{padding: 16}}>
                                            <Text style={[styles.text_header_blue]}>Đánh giá dịch vụ</Text>
                                            <Net_dut />
                                            <View style={{flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center'}}>
                                                <View>
                                                    <Text style={styles.text}>Chất lượng dịch vụ</Text>
                                                    <Text style={styles.text}>(Từ 1 đến 5)</Text>
                                                </View>
                                                <View>
                                                    <View style={{flexDirection: 'row'}}>
                                                        {star.map((item) => {
                                                            return(
                                                                <TouchableOpacity 
                                                                    style={{marginLeft: 7}}
                                                                    onPress={() => setStar_Select(item)}
                                                                >
                                                                    <Ionicons name={item <= Star_Select ? 'star' : 'star-outline'} color={color.orange} size={25} />
                                                                </TouchableOpacity>
                                                            )
                                                        })}
                                                    </View>
                                                    <Text style={[styles.text, {color: color.orange, textAlign: 'right', marginTop: 7}]}>{Star_Select == 1 ? 'Tệ' : Star_Select == 2 ? 'Chưa hài lòng' : Star_Select == 3 ? 'Bình thường' : Star_Select == 4 ? 'Hài lòng' : Star_Select == 5 ? 'Tuyệt vời' : ''}</Text>
                                                </View>
                                            </View>
                                            <Net_dut />
                                            <Text style={styles.text}>Nhận xét</Text>
                                            <View style={[{paddingHorizontal: 10, borderWidth: 1, borderColor: color.lightGrey, marginTop: 7, borderRadius: 5, height: 100}]}>
                                                <TextInput 
                                                    multiline
                                                    placeholder='Nhập nội dung đánh giá'
                                                    onChangeText={(str) => setNoiDung_DanhGia(str)}
                                                    value={noiDung_DanhGia}
                                                    autoCorrect={false}
                                                />
                                            </View>

                                            <Text style={[styles.text_header_blue,{marginVertical: 10}]}>Đánh giá nhân viên</Text>
                                            {/* <Text style={styles.text}>Chọn nhân viên</Text> */}
                                            {/* <View style={[{paddingHorizontal: 10, borderWidth: 1, borderColor: color.lightGrey,borderRadius: 5, marginVertical: 10}]}>
                                                <RNPickerSelect
                                                    placeholder={"Chọn nhân viên"}
                                                    items={listNhanVien_danhGia}
                                                    onValueChange={value => {
                                                        // setChangeText_Singup('Gender', value)
                                                        setNhan_vien_select(value)
                                                    }}
                                                    useNativeAndroidPickerStyle={false}
                                                    value={nhan_vien_select}
                                                    style={ {inputAndroid: styles.inputAndroid, inputIOS: styles.inputIOS}}
                                                    Icon={() => {
                                                        return <View style={{marginTop: 10,marginRight: 5}}><Ionicons name={"chevron-down-outline"} color={color.nau_nhat2} size={24}/></View>
                                                    }}
                                                    // ref={ref_input_location}
                                                />
                                            </View> */}
                                            {payload_nhanvien_danhGia?.map((item,index) => {
                                                // if(item?.EmployeeId == nhan_vien_select){
                                                    return(
                                                        <View style={{marginTop: 10}}>
                                                            <Text style={styles.text}>{getNameNhanVienDanhGia(item?.EmployeeId)}</Text>
                                                            <View style={[{paddingHorizontal: 10, borderWidth: 1, borderColor: color.lightGrey, marginTop: 7, borderRadius: 5, height: 90}]}>
                                                                <TextInput 
                                                                    multiline
                                                                    placeholder='Nhập nội dung đánh giá'
                                                                    onChangeText={(str) => {
                                                                        let _payload_nhanvien_danhGia = [...payload_nhanvien_danhGia]
                                                                        _payload_nhanvien_danhGia[index].RateContent = str
                                                                        setPayload_nhanvien_danhGia(_payload_nhanvien_danhGia)
                                                                    }}
                                                                    value={item?.RateContent}
                                                                    autoCorrect={false}
                                                                />
                                                            </View>
                                                        </View>
                                                    )
                                                // }
                                            })}

                                        </View>
                                 </KeyboardAwareScrollView>
                                </View>
                           
                       <TouchableOpacity 
                            style={[{backgroundColor: color.blue, marginTop: 26, paddingVertical: 10, borderRadius: 10, alignItems: 'center', position: 'absolute', width: layout.width - 32, bottom: 30, left: 16}]}
                            onPress={handle_danh_gia}
                        >
                            <Text style={[styles.text_header_blue,{color:color.white}]}>Gửi</Text>
                        </TouchableOpacity>
                       
                    </View>
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
    text_blue: {
        color: color.blue,
        fontSize: 16,
        fontWeight: '600'
    },
    modal_container: {
        backgroundColor: color.white, 
        height: layout.height/10*9,
        minHeight: 650,
        marginTop:  layout.height/10,
        borderRadius: 20
    },
    header_apply: {
        flexDirection: 'row',
        justifyContent: 'flex-end',
        borderTopRightRadius: 20,
        borderTopLeftRadius: 20,
        height: 50,
    },
    title: {
        fontSize: 17,
        fontWeight: '700',
        color: '#182954'
    },
    modal_detail_container: {
        backgroundColor: color.nau_nhat, 
        height: layout.height - StatusBarHeight_2,
        marginTop: StatusBarHeight_2
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
        flexDirection:'row', 
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
    inputAndroid: {
        width: '80%',
        height: 50,
        fontSize: 14,
        // paddingHorizontal: 5,
        // paddingVertical: 5,
        borderRadius: 5,
        color: 'black',
        paddingRight: 30, // to ensure the text is never behind the icon
      },
    inputIOS : {
        width: '80%',
        height: 50,
        fontSize: 14,
        // paddingHorizontal: 5,
        // paddingVertical: 5,
        borderRadius: 5,
        color: 'black',
        paddingRight: 30,
    }
});
