import React, {useEffect, useState} from 'react';
import {observer} from 'mobx-react-lite';
import {Animated, Dimensions, FlatList, StyleSheet, View, ViewStyle, Image, Text, TouchableOpacity, Platform} from 'react-native';
import {Header, Screen} from '../../components';
import {useIsFocused, useNavigation} from "@react-navigation/native"
// import { useStores } from "../../models"
import {color} from '../../theme';
import CenterSpinner from '../../components/center-spinner/center-spinner';
import Ionicons from 'react-native-vector-icons/Ionicons';
import { images } from '../../images';
import DatePicker from 'react-native-date-picker';
import { formatDate, formatNumber, StatusBarHeight_2 } from '../../services';

const layout = Dimensions.get('window');

import { UnitOfWorkService } from '../../services/api/unitOfWork-service';
import { useStores } from '../../models';
import { log } from 'react-native-reanimated';

const _unitOfWork = new UnitOfWorkService()

const ROOT: ViewStyle = {
    backgroundColor: color.primary,
    flex: 1,
};

export const DashBoardAdminScreen = observer(function DashBoardAdminScreen() {
    const navigation = useNavigation();
    const {HDLTModel} = useStores();
    const [isLoading, setLoading] = useState(false);
    const isFocus = useIsFocused()
    const [isRefresh, setRefresh] = useState(false)
    const [dataDate,setDataDate] = useState({
        FromDate: new Date(new Date().getTime() - 7*24*60*60*1000),
        ToDate: new Date(), 
        showFromDate: false,
        showToDate: false
    })
    const [masterData_Doanhthu, setMasterData_Doanhthu] = useState()
    const [masterData_cho_thanh_toan, setMasterData_cho_thanh_toan] = useState()
    const [masterData_danhgia, setMasterData_danhgia] = useState()
    const [masterData_phieuYc, setMasterData_phieuYc] = useState()


    useEffect(() => {
      fetchData();
    }, [isFocus,isRefresh]);
    const fetchData = async () => {
        setRefresh(false)
        setLoading(true)
        if(isFocus){
            let _userId = await HDLTModel.getUserInfoByKey('userId')
            // let response = await _unitOfWork.user.getDashboardDoanhthu({
            //         StartDate: dataDate?.FromDate,
            //         EndDate: dataDate?.ToDate,
            //         Count:5,
            //         UserId: _userId
            // })
            // if(response?.statusCode == 200) setMasterData_Doanhthu(response)
            // let response_chothanhtoan = await _unitOfWork.user.getDashboardChoThanhToan({
            //         UserId: _userId
            // })
            // if(response_chothanhtoan?.statusCode == 200) setMasterData_cho_thanh_toan(response_chothanhtoan)

            // let respons_danhgia = await _unitOfWork.user.takeRatingStatisticDashboard({
            //     StartDate: dataDate?.FromDate,
            //     EndDate: dataDate?.ToDate,
            //     Count:5,
            //     UserId: _userId
            // })
            // if(respons_danhgia?.statusCode == 200) setMasterData_danhgia(respons_danhgia)

            let respon_phieuYc = await _unitOfWork.user.takeStatisticServiceTicketDashboard({
                UserId: _userId
            })
            if(respon_phieuYc?.statusCode == 200) setMasterData_phieuYc(respon_phieuYc)

            await Promise.all([
                _unitOfWork.user.getDashboardDoanhthu({
                    StartDate: dataDate?.FromDate,
                    EndDate: dataDate?.ToDate,
                    Count:5,
                    UserId: _userId
                }),
                _unitOfWork.user.getDashboardChoThanhToan({
                    UserId: _userId
                }),
                _unitOfWork.user.takeRatingStatisticDashboard({
                    StartDate: dataDate?.FromDate,
                    EndDate: dataDate?.ToDate,
                    Count:5,
                    UserId: _userId
                }),
                _unitOfWork.user.takeStatisticServiceTicketDashboard({
                    UserId: _userId
                })
            ])
            .then((response) => {
                
                let res_doanh_thu = response[0]
                if(res_doanh_thu?.statusCode == 200) setMasterData_Doanhthu(res_doanh_thu)
 
                let response_chothanhtoan = response[1]
                if(response_chothanhtoan?.statusCode == 200) setMasterData_cho_thanh_toan(response_chothanhtoan)

                let respons_danhgia = response[2]
                if(respons_danhgia?.statusCode == 200) setMasterData_danhgia(respons_danhgia)

                let respon_phieuYc = response[3]
                if(respon_phieuYc?.statusCode == 200) setMasterData_phieuYc(respon_phieuYc)

            });
        }
        setLoading(false)
        
    };

    const CaclutateTotalDoanhThuDaThanhToan = () => {
        let price = 0
        masterData_Doanhthu?.listRevenueStatisticServicePacketModel?.map((item) => {
            item?.listAmount?.map((item_2) => {
                price += item_2
            })
        })
        return formatNumber(price)
    }

    const CaclutateTotalDoanhThuChoThanhToan = () => {
        let price = 0
        masterData_cho_thanh_toan?.listRevenueStatisticWaitPaymentModel?.map((item) => {    
            price += item?.amount
        })
        return formatNumber(price)
    }

    const CaclutateTotal_Review = (type) => {
        let total = 0
        if(masterData_danhgia?.listRatingStatisticStarServicePacketModel?.length > 0){
            let data_danhgia = [...masterData_danhgia?.listRatingStatisticStarServicePacketModel]
            data_danhgia?.map((item) => {
                if(item?.rateStar == type) total += item?.rateStar
            })
        }
        return total
    }

    const CaclutateTotal_PhieuYc = (type) => {
        let total = 0
        if(masterData_phieuYc){
            if(type == 1){
                let data_phieuYc = [...masterData_phieuYc?.listNewStatus]
                data_phieuYc.map((item) => {
                    total += item?.count
                })
            }
            if(type == 2){
                let data_phieuYc = [...masterData_phieuYc?.listProgressStatus]
                data_phieuYc.map((item) => {
                    total += item?.count
                })
            }
            if(type == 3){
                let data_phieuYc = [...masterData_phieuYc?.listDoneStatus]
                data_phieuYc.map((item) => {
                    total += item?.count
                })
            }
            if(type == 4){
                let data_phieuYc = [...masterData_phieuYc?.listCancelStatus]
                data_phieuYc.map((item) => {
                    total += item?.count
                })
            }
        }
        return total
    }

    const goToPage = (page) => {
        navigation.navigate(page);
    };
    const onRefresh = () => {
        setRefresh(true)
    };

    const setChangeShowDate = (type,value) => {
        let _dataDate = {...dataDate}
        _dataDate[type] = value
        if(type == 'FromDate') _dataDate['showFromDate'] = false
        if(type == 'ToDate') _dataDate['showToDate'] = false
        
        setDataDate({..._dataDate})
    }

    const topComponent = () => {
        return (
            <View style={{paddingHorizontal: 16}}>
                {/* Chọn ngày */}
                <View style={{ flexDirection: 'row', flexWrap: 'wrap', justifyContent: 'space-between'}}>
                    <View style={{width: '48%'}}>
                        <Text style={[styles.text_black,{marginVertical: 16}]}>Từ ngày</Text>
                        <View>
                            <TouchableOpacity style={styles.inputDate} onPress={() => {setChangeShowDate('showFromDate', true)}}>
                                <Text>{formatDate(dataDate?.FromDate)}</Text>
                                <Ionicons name={'calendar-outline'} color="black" size={24}/>
                            </TouchableOpacity>
                            <DatePicker
                                // minimumDate={lastUpload}
                                // maximumDate={new Date()}
                                textColor={Platform?.OS == 'ios' ? color.black : color.black}
                                mode='date'
                                modal
                                open={dataDate?.showFromDate}
                                date={dataDate?.FromDate}
                                onConfirm={(date) => {
                                    setChangeShowDate('FromDate', date);    
                                    setRefresh(true)    
                                    // setChangeShowDate('showFromDate', false);                             
                                }}
                                onCancel={() => {
                                    setChangeShowDate('showFromDate', false); 
                                }}
                                locale='vi'
                                confirmText='Xác nhận'
                                cancelText='Hủy'
                                title={null}
                            />
                        </View>   
                    </View>
                    <View style={{width: '48%'}}>
                        <Text style={[styles.text_black,{marginVertical: 16}]}>Đến ngày</Text>
                        <View>
                            <TouchableOpacity style={styles.inputDate} onPress={() => setChangeShowDate('showToDate', true)}>
                                <Text>{formatDate(dataDate?.ToDate)}</Text>
                                <Ionicons name={'calendar-outline'} color="black" size={24}/>
                            </TouchableOpacity>
                            <DatePicker
                                maximumDate={new Date()}
                                textColor={Platform?.OS == 'ios' ? color.black : color.black}
                                mode="date"
                                modal
                                open={dataDate?.showToDate}
                                date={dataDate?.ToDate}
                                onConfirm={(date) => {
                                    setChangeShowDate('ToDate', date);
                                    setRefresh(true)
                                    // setChangeShowDate('showToDate', false);
                                }}
                                onCancel={() => {
                                    setChangeShowDate('showToDate', false);
                                }}
                                locale='vi'
                                confirmText='Xác nhận'
                                cancelText='Hủy'
                                title={null}
                            />
                        </View>  
                    </View>
                </View>

                {/* Doanh thu */}
                <View style={styles.box}>
                    <Text style={[styles.title_black, {marginBottom: 16}]}>Doanh Thu</Text>
                    <Image source={images.image_doanh_thu} style={styles.image} />
                    <View style={[styles.row,{marginTop: 30}]}>
                        <View>
                            <Text style={[styles.text_number_green]}>{CaclutateTotalDoanhThuDaThanhToan()} đ</Text>
                            <Text style={[styles.text_black]}>Đã thanh toán</Text>
                        </View>
                        <View>
                            <Text style={[styles.text_number_green,{color: color.orange}]}>{CaclutateTotalDoanhThuChoThanhToan()}đ</Text>
                            <Text style={[styles.text_black]}>Chờ thanh toán</Text>
                        </View>
                    </View>
                    <TouchableOpacity 
                        style={styles.btn}
                        // onPress={() => navigation.navigate('MainAdminScreen', {screen: 'DoanhThuScreen'},{
                        //     data: {
                        //         FromDate: dataDate?.FromDate.getTime(),
                        //         ToDate: dataDate?.ToDate.getTime()
                        //     }
                        // })}
                        onPress={() => {
                                navigation.navigate('DoanhThuScreen',{
                                    FromDate: dataDate?.FromDate.getTime(),
                                    ToDate: dataDate?.ToDate.getTime()
                                })
                            }
                        }
                    >
                            <Image source={images.icon_note} style={{width: 25,height: 25, marginRight: 7}} />
                            <Text style={[styles.text_black,{fontWeight: '400'}]}>Xem chi tiết doanh thu</Text>
                    </TouchableOpacity>
                </View>

                {/* Phiếu yêu cầu dịch vụ */}
                <View style={styles.box}>
                    <Text style={[styles.title_black, {marginBottom: 16}]}>Phiếu yêu cầu dịch vụ</Text>
                    <Image source={images.image_phieu_yc} style={styles.image} />
                    <View style={[styles.row,{marginTop: 30}]}>
                        <View>
                            <Text style={[styles.text_number_green]}>{CaclutateTotal_PhieuYc(1)}</Text>
                            <Text style={[styles.text_black]}>Mới</Text>
                        </View>
                        <View>
                            <Text style={[styles.text_number_green,{color: color.orange}]}>{CaclutateTotal_PhieuYc(2)}</Text>
                            <Text style={[styles.text_black]}>Đang thực hiện</Text>
                        </View>
                        <View>
                            <Text style={[styles.text_number_green,{color: color.blue}]}>{CaclutateTotal_PhieuYc(3)}</Text>
                            <Text style={[styles.text_black]}>Hoàn thành</Text>
                        </View>
                        <View>
                            <Text style={[styles.text_number_green,{color: color.orange}]}>{CaclutateTotal_PhieuYc(4)}</Text>
                            <Text style={[styles.text_black]}>Huỷ</Text>
                        </View>
                    </View>
                    <TouchableOpacity 
                        style={styles.btn}
                    >
                            <Image source={images.icon_note} style={{width: 25,height: 25, marginRight: 7}} />
                            <Text style={[styles.text_black,{fontWeight: '400'}]}>Xem chi tiết phiếu yêu cầu dịch vụ</Text>
                    </TouchableOpacity>
                </View>

                {/* Đánh giá */}
                <View style={styles.box}>
                    <Text style={[styles.title_black, {marginBottom: 16}]}>Đánh giá</Text>
                    <Image source={images.image_danh_gia} style={styles.image} />
                    <View style={[styles.row,{marginTop: 30}]}>
                        <View>
                            <Text style={[styles.text_number_green,{color: color.blue}]}>{CaclutateTotal_Review(5)}</Text>
                            <Text style={[styles.text_black,{fontSize: 13}]}>Tuyệt vời</Text>
                        </View>
                        <View>
                            <Text style={[styles.text_number_green,{color: color.blue}]}>{CaclutateTotal_Review(4)}</Text>
                            <Text style={[styles.text_black,{fontSize: 13}]}>Hài lòng</Text>
                        </View>
                        <View>
                            <Text style={[styles.text_number_green,{color: color.tabbar}]}>{CaclutateTotal_Review(3)}</Text>
                            <Text style={[styles.text_black,{fontSize: 13}]}>Bình thường</Text>
                        </View>
                        <View>
                            <Text style={[styles.text_number_green,{color: color.tabbar}]}>{CaclutateTotal_Review(2)}</Text>
                            <Text style={[styles.text_black,{fontSize: 13}]}>Chưa hài lòng</Text>
                        </View>
                        <View>
                            <Text style={[styles.text_number_green,{color: color.tabbar}]}>{CaclutateTotal_Review(1)}</Text>
                            <Text style={[styles.text_black,{fontSize: 13}]}>Tệ</Text>
                        </View>
                    </View>
                    <TouchableOpacity style={styles.btn}>
                            <Image source={images.icon_note} style={{width: 25,height: 25, marginRight: 7}} />
                            <Text style={[styles.text_black,{fontWeight: '400'}]}>Xem chi tiết đánh giá</Text>
                    </TouchableOpacity>
                </View>
            </View>
        );
    };

    return (
        <>
            {isLoading && <CenterSpinner/>}
            <Screen style={ROOT} preset="fixed">
                <View style={{flex: 1, backgroundColor: color.white}}>
                    <FlatList
                        // refreshing={isRefresh}
                        // onRefresh={() => onRefresh()}
                        showsVerticalScrollIndicator={false}
                        showsHorizontalScrollIndicator={false}
                        style={{flex: 1}}
                        renderItem={null}
                        data={[]}
                        ListHeaderComponent={topComponent()}
                        keyExtractor={(item, index) => 'dashboard-admin-' + index + String(item)}
                    />
                </View>
            </Screen>
        </>
    );
});

const styles = StyleSheet.create({
    text_black: {
        color: color.black,
        fontSize: 15,
        fontWeight: '500', 
    },
    inputDate: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
        marginRight: 8,
        borderWidth: 1,
        borderColor: color.lighterGrey,
        // paddingHorizontal: 12,
        backgroundColor: color.white,
        paddingLeft: 16,
        paddingRight: 8,
        paddingVertical: 8,
        borderRadius: 4
    },
    title_black: {
        fontWeight: '700',
        fontSize: 16,
        color: color.black
    },
    box: {
        padding: 14,
        borderWidth: 1,
        borderColor: color.lighterGrey,
        borderRadius: 12,
        marginVertical: 16

    },
    image: {
        width: 70,
        height: 80
    },
    text_number_green: {
        fontWeight: '700',
        fontSize: 22,
        color: color.green
    },
    row: {
        flexDirection: 'row',
        justifyContent: 'space-between'
    },
    btn: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
        width: '100%',
        marginTop: 16,
        borderRadius: 7,
        paddingVertical: 7,
        borderWidth: 1,
        borderColor: color.lighterGrey,
    }
});
