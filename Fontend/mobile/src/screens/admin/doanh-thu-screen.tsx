import React, {useEffect, useState} from 'react';
import {observer} from 'mobx-react-lite';
import {Animated, Dimensions, FlatList, StyleSheet, View, ViewStyle, Image, Text, TouchableOpacity, Platform, Modal} from 'react-native';
import {Header, Screen} from '../../components';
import {useIsFocused, useNavigation, useRoute} from "@react-navigation/native"
// import { useStores } from "../../models"
import {color} from '../../theme';
import CenterSpinner from '../../components/center-spinner/center-spinner';
import Ionicons from 'react-native-vector-icons/Ionicons';
import { images } from '../../images';
import { useStores } from '../../models';
import { formatDate, formatNumber, formatNumber2 } from '../../services';
import DatePicker from 'react-native-date-picker';
import {
    LineChart,
  } from "react-native-chart-kit";

import { PieChart } from "react-native-gifted-charts";

const layout = Dimensions.get('window');

import { UnitOfWorkService } from '../../services/api/unitOfWork-service';
import { log } from 'react-native-reanimated';
import Toast from 'react-native-toast-message';
import { CONSTANTS } from '@firebase/util';

const _unitOfWork = new UnitOfWorkService()

const ROOT: ViewStyle = {
    backgroundColor: color.primary,
    flex: 1,
};

const chartConfig = {
    backgroundColor: color.white,
    backgroundGradientFrom:color.white,
    backgroundGradientTo: color.white,
    fillShadowGradientFrom: '#0000FF',
    fillShadowGradientTo: color.white,
    color: (opacity = 1) => color.blue,
    strokeWidth: 1,
    barPercentage: 0.5,
    useShadowColorFromDataset: false,
    decimalPlaces: 0,
    labelColor: (opacity = 1) => color.black,
    propsForDots: {
        r: "3",
        strokeWidth: "2",
        stroke: color.blue
    },
    propsForLabels: {
        
    }
};


export const DoanhThuScreen = observer(function DoanhThuScreen() {
    const navigation = useNavigation();
    const {HDLTModel} = useStores();
    const [isLoading, setLoading] = useState(false);
    const isFocus = useIsFocused()
    const { params }: any = useRoute();
    const [isRefresh, setRefresh] = useState(false)
    const [dataDate,setDataDate] = useState({
        FromDate: new Date(new Date().getTime() - 7*24*60*60*1000),
        ToDate: new Date(), 
        showFromDate: false,
        showToDate: false
    })
    const [masterData_Doanhthu, setMasterData_Doanhthu] = useState()
    const [masterData_cho_thanh_toan, setMasterData_cho_thanh_toan] = useState()
    const [dataLineChart, setDataLineChart] = useState()
    const [dataPieChart, setDataPieChart] = useState()
    const [showModalOption, setShowModalOption] = useState(false)
    const [list_nhom_dv, setList_nhom_dv] = useState([])
    const [type_select_dv, setType_Select_Dv] = useState('Tất cả')

    useEffect(() => {
      fetchData();
    }, [isFocus,isRefresh]);
    const fetchData = async () => {
        setType_Select_Dv('Tất cả')
        setLoading(true)
        setRefresh(false)
        let _userId = await HDLTModel.getUserInfoByKey('userId')

        let payload = {
            StartDate: dataDate?.FromDate,
            EndDate: dataDate?.ToDate,
            Count:5,
            UserId: _userId
        }

        if(params){
            let _dataDate = {...dataDate}
            _dataDate.FromDate = new Date(params?.FromDate) 
            _dataDate.ToDate = new Date(params?.ToDate)
            setDataDate(_dataDate)
            payload.StartDate = _dataDate?.FromDate
            payload.EndDate = _dataDate?.ToDate
        }
        
        let response = await _unitOfWork.user.getDashboardDoanhthu(payload)
        if(response?.statusCode == 200) {
            setMasterData_Doanhthu(response)

            // tao list tuy chọn trong lọc
            let _list_nhom_dv = []
            _list_nhom_dv.push({
                name: 'Tất cả',
                status: type_select_dv == 'Tất cả' ? true : false
            })
            response?.listRevenueStatisticServicePacketModelByServicePacket.map((item) => {
                _list_nhom_dv.push({
                    name: item?.productCategoryName,
                    status: item?.productCategoryName == type_select_dv ? true : false
                })
            })
            setList_nhom_dv(_list_nhom_dv)
        }
        handleGetDataPieChart(response)
        handleGetDataLineChart(response, type_select_dv)
        let response_chothanhtoan = await _unitOfWork.user.getDashboardChoThanhToan({
            UserId: _userId
        })
        if(response_chothanhtoan?.statusCode == 200) setMasterData_cho_thanh_toan(response_chothanhtoan)
        setLoading(false)
    };


    const fetchData2 = async (fromDate, toDate) => {
        setLoading(true)
        let _userId = await HDLTModel.getUserInfoByKey('userId')
        let response = await _unitOfWork.user.getDashboardDoanhthu({
                StartDate: fromDate,
                EndDate: toDate,
                Count:5,
                UserId: _userId
        })
        if(response?.statusCode == 200) {
            setMasterData_Doanhthu(response)

            // tao list tuy chọn trong lọc
            let _list_nhom_dv = []
            _list_nhom_dv.push({
                name: 'Tất cả',
                status: type_select_dv == 'Tất cả' ? true : false
            })
            response?.listRevenueStatisticServicePacketModelByServicePacket.map((item) => {
                _list_nhom_dv.push({
                    name: item?.productCategoryName,
                    status: item?.productCategoryName == type_select_dv ? true : false
                })
            })
            setList_nhom_dv(_list_nhom_dv)
        }
        handleGetDataPieChart(response)
        handleGetDataLineChart(response, type_select_dv)
        let response_chothanhtoan = await _unitOfWork.user.getDashboardChoThanhToan({
            UserId: _userId
        })
        if(response_chothanhtoan?.statusCode == 200) setMasterData_cho_thanh_toan(response_chothanhtoan)
        setLoading(false)
    };

    const CaclutateTotalDoanhThuDaThanhToan = (type) => {
        let price = 0
        if(type == 'Tất cả'){
            masterData_Doanhthu?.listRevenueStatisticServicePacketModel?.map((item) => {
                item?.listAmount?.map((item_2) => {
                    price += item_2
                })
            }) 
        }else{
            masterData_Doanhthu?.listRevenueStatisticServicePacketModelByServicePacket?.map((item) => {
                if(item?.productCategoryName == type){
                    item?.listAmount?.map((item_2) => {
                        price += item_2
                    })
                }
                // console.log("price: ", price, type)
            })
        }
        
        return price
    }

    const CaclutateTotalDoanhThuChoThanhToan = (type) => {
        let price = 0
        if(type == 'Tất cả'){
            masterData_cho_thanh_toan?.listRevenueStatisticWaitPaymentModel?.map((item) => {    
                price += item?.amount
            })
        }else{
            masterData_cho_thanh_toan?.listRevenueStatisticWaitPaymentModel?.map((item) => {
                if(item?.productCategoryName == type){
                    price += item?.amount
                }  
            })
        }
        
        return price
    }

    const CacluteTotalArray = (data) => {
        let total = 0
        data?.map((item) => {
            total += item
        })
        return formatNumber(total)
    }

    const handleGetDataLineChart = (masterData, type) => { 
        let _data = {
            labels: [],
            datasets: [
                {
                    data: [],
                    color: (opacity = 1) => color.blue,
                }
            ]
        }
        let date = ((dataDate?.ToDate.getTime() - dataDate?.FromDate.getTime())/(24*60*60*1000)) + 1
        
        if(type == 'Tất cả'){
            for(let i = 0; i < date; i++){
                let price = 0;
                masterData?.listRevenueStatisticServicePacketModel?.map((item) => {
                    price += item?.listAmount[i]
                })
                if(price >= 0){
                    _data.labels.push(formatDate(new Date(dataDate.FromDate.getTime() + i*24*60*60*1000), "dd/MM"))
                    _data.datasets[0]?.data.push(price)
                }   
            }
        }else{
            for(let i = 0; i < date; i++){
                let price = 0;
                masterData?.listRevenueStatisticServicePacketModel?.map((item) => {
                    if(item?.productCategoryName == type){
                        price += item?.listAmount[i]
                    }
                })
                if(price >= 0){
                    _data.labels.push(formatDate(new Date(dataDate.FromDate.getTime() + i*24*60*60*1000), "dd/MM"))
                    _data.datasets[0]?.data.push(price)
                }   
            }
        }
        
        
     
        setDataLineChart(_data)
    }

    const handleGetDataPieChart = (masterData) => {
        let _data = [];
        let total = 0;
        let colors = ['#177AD5','#79D2DE','#ED6665', '#f88111','#00FF00', '#33CCFF', '#FF66FF' , '#0099FF', '#CCFFFF', '#BBBBBB' ]
        masterData?.listRevenueStatisticServicePacketModelByServicePacket?.map((item) => {
            item?.listAmount?.map((item2) => total += item2)
        })

        masterData?.listRevenueStatisticServicePacketModelByServicePacket?.map((item) => {
            let doanh_thu_con = 0
            item?.listAmount?.map((item2) => doanh_thu_con += item2)
            
            let tinh_phantram = ((doanh_thu_con / total) * 100).toFixed(1)
            _data.push({
                value: doanh_thu_con, 
                color: colors[_data?.length], 
                text: `${tinh_phantram}%`,
            })
        })

        setDataPieChart(_data)

    }

    const handleChooseOption_DV = (index) => {
        let _list_nhom_dv = [...list_nhom_dv]
        list_nhom_dv.map((item2, index2) => {
            if(index2 == index) {
                _list_nhom_dv[index2].status = true
                setType_Select_Dv(_list_nhom_dv[index2].name)
                handleGetDataLineChart(masterData_Doanhthu, _list_nhom_dv[index2].name )
            }
            else _list_nhom_dv[index2].status = false
        })
        setList_nhom_dv(_list_nhom_dv)
    }

    const setChangeShowDate = (type,value) => {
        let _dataDate = {...dataDate}
        _dataDate[type] = value
        if(type == 'FromDate') _dataDate['showFromDate'] = false
        if(type == 'ToDate') _dataDate['showToDate'] = false
        setDataDate(_dataDate)
    }

    const goToPage = (page) => {
        navigation.navigate(page);
    };
    const onRefresh = () => {
        setRefresh(true)
    };

    const topComponent = () => {
        return (
            <View style={{paddingHorizontal: 16}}>
                <Text style={[styles.text_header_black,{width: '100%', textAlign: 'center', marginTop: 20}]}>THỐNG KÊ DOANH THU</Text>
                <View style={[styles.box_flex,{marginTop: 20}]}>
                    <Text style={[styles.text_16, {color: '#08A305'}]} >Đã thanh toán</Text>
                    <Text style={[styles.text_16, {fontWeight: '700'}]}>{formatNumber(CaclutateTotalDoanhThuDaThanhToan(type_select_dv))}</Text>
                </View>
                <View style={[styles.box_flex,{marginTop: 10}]}>
                    <Text style={[styles.text_16, {color: color.orange}]} >Chờ thanh toán</Text>
                    <Text style={[styles.text_16, {fontWeight: '700'}]}>{formatNumber(CaclutateTotalDoanhThuChoThanhToan(type_select_dv))}</Text>
                </View>
                <View style={[styles.box_flex,{marginTop: 10}]}>
                    <Text style={[styles.text_16, {color: color.blue}]} >Tổng số</Text>
                    <Text style={[styles.text_16, {fontWeight: '700'}]}>{formatNumber(CaclutateTotalDoanhThuDaThanhToan(type_select_dv) + CaclutateTotalDoanhThuChoThanhToan(type_select_dv))}</Text>
                </View>

                {CaclutateTotalDoanhThuDaThanhToan(type_select_dv) > 0 ? 
                    <View>
                        <Text style={[styles.text_header_black,{width: '100%', textAlign: 'center', marginTop: 20}]}>BIỂU ĐỒ DOANH THU</Text>
                        <Text style={[styles.text_16, {marginTop: 10}]}>Đơn vị - triệu đồng</Text>
                        {dataLineChart ? 
                            <LineChart
                                style={styles.lineChart}
                                // withVerticalLabels={true}
                                withHorizontalLabels={true}
                                // segments={4}
                                fromZero={true}
                                withInnerLines={true}
                                withShadow={true}
                                withOuterLines={false}
                                withVerticalLines={false}
                                withHorizontalLines={false}
                                data={dataLineChart}
                                width={layout.width}
                                // width={dataChart?.labels?.length*50 + 50}
                                height={layout.width * 4.5 / 10}
                                chartConfig={chartConfig}
                                bezier
                                getDotColor={(dataPoint, dataPointIndex) => {
                                    return color.white;
                                }}
                                formatYLabel={(lable) => {
                                    return formatNumber2(lable)
                                }}
                            />
                        : null }
                    </View>
                : null }
                {CaclutateTotalDoanhThuDaThanhToan(type_select_dv) > 0 && type_select_dv == 'Tất cả' ?
                    <View>
                        <Text style={[styles.text_header_black,{width: '100%', textAlign: 'center', marginTop: 20}]}>DOANH THU THEO DỊCH VỤ</Text>
                        <View style={{alignItems: 'center', marginTop: 16, paddingLeft: 25}}>
                            {dataPieChart ? 
                                <PieChart
                                    donut
                                    data={dataPieChart}
                                    showText
                                    textColor="black"
                                    radius={150}
                                    textSize={15}
                                    showValuesAsLabels
                                    // showTextBackground
                                    // textBackgroundRadius={20}
                                />
                            : null }
                        </View>
                        
                        <Text style={[styles.text_header_black,{width: '100%', textAlign: 'center', marginBottom: 16}]}>Chi tiết</Text>
                        <View style={{borderWidth: 1, borderColor: color.lighterGrey, borderTopStartRadius: 10, borderTopEndRadius: 10, marginBottom: 30}}>
                            <View style={[styles.box_flex,{paddingVertical: 12, backgroundColor: color.lighterGrey, borderTopStartRadius: 10, borderTopEndRadius: 10}]}>
                                <Text style={[styles.text_black,{width: '70%', paddingLeft: 50}]}>Dịch vụ</Text>
                                <Text style={[styles.text_black,{width: '30%', textAlign: 'right', paddingRight: 10}]}>Doanh thu</Text>
                            </View>
                            {dataPieChart && masterData_Doanhthu?.listRevenueStatisticServicePacketModelByServicePacket?.map((item,index) => {
                                return (
                                    <View style={[styles.box_flex,{ borderBottomColor: color.lighterGrey, borderBottomWidth: 1}]}>
                                        <View style={{width: '70%',paddingVertical: 12, paddingLeft: 10, borderRightWidth: 1, borderRightColor: color.lighterGrey, flexDirection: 'row'}}>
                                            <View style={[{width: 20, height: 20, borderRadius: 10}, dataPieChart[index]?.color ? {backgroundColor: dataPieChart[index]?.color} : {}]}></View>
                                            <Text style={[styles.text_black,{ marginLeft: 10}]}>{item?.productCategoryName}</Text>  
                                        </View>
                                        
                                        <Text style={[styles.text_black,{width: '30%',paddingVertical: 12, textAlign: 'right', paddingRight: 10}]}>{CacluteTotalArray(item?.listAmount)}</Text>
                                    </View>
                                )
                            })}
                        </View>
                    </View>
                : null }
            </View>
        );
    };

    return (
        <>
            {isLoading && <CenterSpinner/>}
            <Screen style={ROOT} preset="fixed">
                <View style={{flex: 1, backgroundColor: color.white}} >
                    <Header 
                        headerText='Doanh thu' 
                        onLeftPress={() => navigation.navigate( 'MainAdminScreen', {screen:'DashBoardAdminScreen'})}
                        rightText='Lọc'
                        onRightPress={() => setShowModalOption(true)}
                    />
                    <View style={{flex: 1}}>
                        <FlatList
                            // refreshing={isRefresh}
                            // onRefresh={() => onRefresh()}
                            showsVerticalScrollIndicator={false}
                            showsHorizontalScrollIndicator={false}
                            style={{flex: 1}}
                            renderItem={null}
                            data={[]}
                            ListHeaderComponent={topComponent()}
                            keyExtractor={(item, index) => 'doanh-thu-screen-2-' + index + String(item)}
                        />
                    </View>
                </View>

                <Modal
                    animationType={"slide"}
                    transparent={true}
                    visible={showModalOption}
                    onRequestClose={() => { }}
                >
                    <View style={styles.container_modal}>
                        <View style={[styles.box_flex, {marginBottom: 30}]}>
                            <Text style={[styles.text_header_black,{fontSize: 18}]}>Lọc doanh thu</Text>
                            <TouchableOpacity onPress={() => setShowModalOption(false)}>
                                <Ionicons name='close-outline' size={35} color={color.black}/>
                            </TouchableOpacity>
                            
                        </View>

                        <Text style={[styles.text_header_black, {color: color.blue}]}>Theo thời gian</Text>
                        <View style={{marginTop: 17}}>
                            <View style={{width: '100%', flexDirection: 'row', justifyContent: 'space-between'}}>
                                <Text style={[styles.text_black,{marginVertical: 16, fontSize: 16}]}>Từ ngày</Text>
                                <View style={{width: '70%'}}>
                                    <TouchableOpacity style={styles.inputDate} onPress={() => {setChangeShowDate('showFromDate', true)}}>
                                        <Text style={styles.text_black}>{formatDate(dataDate?.FromDate)}</Text>
                                        <Ionicons name={'calendar-outline'} color={color.blue} size={24}/>
                                    </TouchableOpacity>
                                    <DatePicker
                                        // minimumDate={lastUpload}
                                        // maximumDate={new Date()}
                                        textColor={Platform?.OS == 'ios' ? color.black : color.black}
                                        mode="date"
                                        modal
                                        open={dataDate?.showFromDate}
                                        date={dataDate?.FromDate}
                                        onConfirm={(date) => {
                                            setChangeShowDate('FromDate', date); 
                                            fetchData2(date, dataDate?.ToDate)
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
                            <View style={{width: '100%', flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center'}}>
                                <Text style={[styles.text_black,{marginVertical: 16, fontSize: 16}]}>Đến ngày</Text>
                                <View style={{width: '70%'}}>
                                    <TouchableOpacity style={styles.inputDate} onPress={() => setChangeShowDate('showToDate', true)}>
                                        <Text style={styles.text_black}>{formatDate(dataDate?.ToDate)}</Text>
                                        <Ionicons name={'calendar-outline'} color={color.blue} size={24}/>
                                    </TouchableOpacity>
                                    <DatePicker
                                        // maximumDate={new Date()}
                                        textColor={Platform?.OS == 'ios' ? color.black : color.black}
                                        mode="date"
                                        modal
                                        open={dataDate?.showToDate}
                                        date={dataDate?.ToDate}
                                        onConfirm={(date) => {
                                            setChangeShowDate('ToDate', date);
                                            fetchData2(dataDate?.FromDate, date)
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

                        <Text style={[styles.text_header_black, {color: color.blue, marginTop: 30, marginBottom: 16}]}>Theo nhóm dịch vụ</Text>
                        {list_nhom_dv && list_nhom_dv.map((item,index) => {
                            return (
                                <TouchableOpacity style={{ flexDirection: 'row', marginBottom: 16, alignItems: 'center' }} onPress={() => handleChooseOption_DV(index)}>
                                    <Ionicons name={item?.status ? 'radio-button-on-outline' : 'radio-button-off-outline'}color={color.xanh_nhat} size={25} />
                                    <Text style={{ marginLeft: 17, fontSize: 15, color: color.lightGrey }}>{item?.name}</Text>
                                </TouchableOpacity>
                            )
                        })}

                    </View>
                </Modal>
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
        borderWidth: 1,
        borderColor: color.lighterGrey,
        // paddingHorizontal: 12,
        backgroundColor: color.white,
        paddingLeft: 16,
        paddingRight: 8,
        paddingVertical: 8,
        borderRadius: 4
    },
    text_header_black: {
        fontSize: 16,
        fontWeight: '700',
        color: color.black
    },
    box_flex: {
        flexDirection: 'row',
        justifyContent: 'space-between'
    },
    text_16: {
        fontSize: 16,
        fontWeight: '400',
        color: color.black
    },
    lineChart: {
        marginLeft: -16,
        marginTop: 16,
    },
    container_modal: {
        height: layout.height - 20,
        marginTop: 20,
        backgroundColor: color.nau_nhat,
        borderTopEndRadius: 15,
        borderTopStartRadius: 15,
        paddingHorizontal: 30,
        paddingVertical: 26
    }

});
