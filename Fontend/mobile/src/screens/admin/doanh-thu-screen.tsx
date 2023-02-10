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
import { useStores } from '../../models';
import { formatDate, formatNumber, formatNumber2 } from '../../services';
import DatePicker from 'react-native-date-picker';
import {
    LineChart,
  } from "react-native-chart-kit";

const layout = Dimensions.get('window');

import { UnitOfWorkService } from '../../services/api/unitOfWork-service';

const _unitOfWork = new UnitOfWorkService()

const ROOT: ViewStyle = {
    backgroundColor: color.white,
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

const data = [
    {
      name: "Seoul",
      population: 21500000,
      color: "rgba(131, 167, 234, 1)",
      legendFontColor: "#7F7F7F",
      legendFontSize: 15
    },
    {
      name: "Toronto",
      population: 2800000,
      color: "#F00",
      legendFontColor: "#7F7F7F",
      legendFontSize: 15
    },
    {
      name: "Beijing",
      population: 527612,
      color: color.black,
      legendFontColor: "#7F7F7F",
      legendFontSize: 15
    },
    {
      name: "New York",
      population: 8538000,
      color: color.orange,
      legendFontColor: "#7F7F7F",
      legendFontSize: 15
    },
    {
      name: "Moscow",
      population: 11920000,
      color: "rgb(0, 0, 255)",
      legendFontColor: "#7F7F7F",
      legendFontSize: 15,

    }
  ];

export const DoanhThuScreen = observer(function DoanhThuScreen() {
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
    const [dataLineChart, setDataLineChart] = useState()

    useEffect(() => {
      fetchData();
    }, [isFocus,isRefresh]);
    const fetchData = async () => {
        setRefresh(false)
        let _userId = await HDLTModel.getUserInfoByKey('userId')
        let response = await _unitOfWork.user.getDashboardDoanhthu({
                StartDate: dataDate?.FromDate,
                EndDate: dataDate?.ToDate,
                Count:5,
                UserId: _userId
        })
        if(response?.statusCode == 200) setMasterData_Doanhthu(response)

        
        let response_chothanhtoan = await _unitOfWork.user.getDashboardChoThanhToan({
            UserId: _userId
        })
        if(response_chothanhtoan?.statusCode == 200) setMasterData_cho_thanh_toan(response_chothanhtoan)
        handleGetDataLineChart(response)
    };

    const CaclutateTotalDoanhThuDaThanhToan = () => {
        let price = 0
        masterData_Doanhthu?.listRevenueStatisticServicePacketModel?.map((item) => {
            item?.listAmount?.map((item_2) => {
                price += item_2
            })
        })
        return price
    }

    const CaclutateTotalDoanhThuChoThanhToan = () => {
        let price = 0
        masterData_cho_thanh_toan?.listRevenueStatisticWaitPaymentModel?.map((item) => {    
            price += item?.amount
        })
        return price
    }

    const CacluteTotalArray = (data) => {
        let total = 0
        data?.map((item) => {
            total += item
        })
        return formatNumber(total)
    }

    const handleGetDataLineChart = (masterData) => {
        // const data = {
        //     labels: ["January", "February", "March", "April", "May", "June"],
        //     datasets: [
        //       {
        //         data: [20, 45, 28, 80, 99, 43],
        //         color: (opacity = 1) => color.blue, // optional
        //       }
        //     ],
        //   };
        let _data = {
            labels: [],
            datasets: [
                {
                    data: [],
                    color: (opacity = 1) => color.blue,
                }
            ]
        }
        let date = dataDate?.ToDate.getDate() - dataDate?.FromDate.getDate() + 1
        for(let i = 0; i < date; i++){
            let price = 0;
            masterData?.listRevenueStatisticServicePacketModel?.map((item) => {
                price += item.listAmount[i]
            })
            _data.labels.push(formatDate(new Date(dataDate.FromDate.getTime() + i*24*60*60*1000), "dd/MM"))
            _data.datasets[0]?.data.push(price)
        }
        setDataLineChart(_data)
    }

    const setChangeShowDate = (type,value) => {
        let _dataDate = {...dataDate}
        _dataDate[type] = value
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
                <View style={{ flexDirection: 'row', justifyContent: 'space-between'}}>
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
                                mode="date"
                                modal
                                open={dataDate?.showFromDate}
                                date={dataDate?.FromDate}
                                onConfirm={(date) => {
                                    setChangeShowDate('FromDate', date);        
                                    // setChangeShowDate('showFromDate', false);                             
                                }}
                                onCancel={() => {
                                    setChangeShowDate('showFromDate', false); 
                                }}
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
                                mode="time"
                                modal
                                open={dataDate?.showToDate}
                                date={dataDate?.ToDate}
                                onConfirm={(date) => {
                                    setChangeShowDate('ToDate', date);
                                    // setChangeShowDate('showToDate', false);
                                }}
                                onCancel={() => {
                                    setChangeShowDate('showToDate', false);
                                }}
                            />
                        </View>  
                    </View>
                </View>
                <Text style={[styles.text_header_black,{width: '100%', textAlign: 'center', marginTop: 20}]}>THỐNG KÊ DOANH THU</Text>
                <View style={[styles.box_flex,{marginTop: 20}]}>
                    <Text style={[styles.text_16, {color: '#08A305'}]} >Đã thanh toán</Text>
                    <Text style={[styles.text_16, {fontWeight: '700'}]}>{formatNumber(CaclutateTotalDoanhThuDaThanhToan())}</Text>
                </View>
                <View style={[styles.box_flex,{marginTop: 10}]}>
                    <Text style={[styles.text_16, {color: color.orange}]} >Chờ thanh toán</Text>
                    <Text style={[styles.text_16, {fontWeight: '700'}]}>{formatNumber(CaclutateTotalDoanhThuChoThanhToan())}</Text>
                </View>
                <View style={[styles.box_flex,{marginTop: 10}]}>
                    <Text style={[styles.text_16, {color: color.blue}]} >Tổng số</Text>
                    <Text style={[styles.text_16, {fontWeight: '700'}]}>{formatNumber(CaclutateTotalDoanhThuDaThanhToan() + CaclutateTotalDoanhThuChoThanhToan())}</Text>
                </View>

                {CaclutateTotalDoanhThuChoThanhToan() > 0 ? 
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
                            withVerticalLines={true}
                            withHorizontalLines={true}
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
                <View>
                    <Text style={[styles.text_header_black,{width: '100%', textAlign: 'center', marginTop: 20}]}>DOANH THU THEO DỊCH VỤ</Text>
                    {/* <PieChart
                        data={data}
                        width={layout.width - 32}
                        height={250}
                        chartConfig={chartConfig}
                        accessor={"population"}
                        backgroundColor={"transparent"}
                        paddingLeft={"15"}
                        // hasLegend={false}
                        // center={[(layout.width - 32)/5, 0]}
                        // absolute
                    /> */}
                    <Text style={[styles.text_header_black,{width: '100%', textAlign: 'center', marginTop: 20}]}>Chi tiết</Text>
                    <View style={{borderWidth: 1, borderColor: color.lighterGrey, borderTopStartRadius: 10, borderTopEndRadius: 10}}>
                        <View style={[styles.box_flex,{paddingVertical: 12, backgroundColor: color.lighterGrey, borderTopStartRadius: 10, borderTopEndRadius: 10}]}>
                            <Text style={[styles.text_black,{width: '70%', paddingLeft: 50}]}>Dịch vụ</Text>
                            <Text style={[styles.text_black,{width: '30%', textAlign: 'right', paddingRight: 10}]}>Doanh thu</Text>
                        </View>
                        {masterData_Doanhthu?.listRevenueStatisticServicePacketModelByServicePacket?.map((item) => {
                            return (
                                <View style={[styles.box_flex,{ borderBottomColor: color.lighterGrey, borderBottomWidth: 1}]}>
                                    <Text style={[styles.text_black,{width: '70%',paddingVertical: 12, paddingLeft: 26, borderRightWidth: 1, borderRightColor: color.lighterGrey}]}>{item?.productCategoryName}</Text>
                                    <Text style={[styles.text_black,{width: '30%',paddingVertical: 12, textAlign: 'right', paddingRight: 10}]}>{CacluteTotalArray(item?.listAmount)}</Text>
                                </View>
                            )
                        })}
                    </View>
                </View>
            </View>
        );
    };

    return (
        <>
            {isLoading && <CenterSpinner/>}
            <Screen style={ROOT} preset="fixed">
            <Header 
                headerText='Doanh thu' 
                onLeftPress={() => navigation.navigate( 'MainAdminScreen', {screen:'DashBoardAdminScreen'})}
                rightText='Lọc'
            />
                <View style={{flex: 1}}>
                    <FlatList
                        refreshing={isRefresh}
                        onRefresh={() => onRefresh()}
                        showsVerticalScrollIndicator={false}
                        showsHorizontalScrollIndicator={false}
                        style={{flex: 1}}
                        renderItem={null}
                        data={[]}
                        ListHeaderComponent={topComponent()}
                        keyExtractor={(item, index) => 'doanh-thu-screen-' + index + String(item)}
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
    }
});
