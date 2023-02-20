import React, {useEffect, useState} from 'react';
import {observer} from 'mobx-react-lite';
import {Animated, Dimensions, FlatList, StyleSheet, View, ViewStyle, Image, Text, TouchableOpacity, Modal, ScrollView} from 'react-native';
import {Header, Screen} from '../../components';
import {useIsFocused, useNavigation} from "@react-navigation/native"
// import { useStores } from "../../models"
import {color} from '../../theme';
import CenterSpinner from '../../components/center-spinner/center-spinner';
import Ionicons from 'react-native-vector-icons/Ionicons';
import { images } from '../../images';
import { formatDate, formatNumber, StatusBarHeight, StatusBarHeight_2 } from '../../services';
import { useStores } from '../../models';
import { UnitOfWorkService } from '../../services/api/unitOfWork-service';

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
    const [modal_detail, setModal_Detail] = useState(false)
    const [tapIndex_Detail, setTapIndexDetail] = useState(true)
    const [masterData, setMasterData] = useState([])

    useEffect(() => {
      fetchData();
    }, [isFocus,isRefresh]);
    const fetchData = async () => {
        setLoading(true)
        setRefresh(false)
        let customerID = await HDLTModel.getUserInfoByKey('customerId')
        let userId = await HDLTModel.getUserInfoByKey('userId')
        let response = await _unitOfWork.user.getListOrderOfCus({
            "CustomerId": customerID,
            "UserId": userId
        })
        if(response?.statusCode == 200){
            let data = [...response?.listOrder].filter(item => item?.statusOrder != 1)
            data.sort((a,b) => {
                return new Date(b?.createdDate).getTime() - new Date(a?.createdDate).getTime()
            })
            console.log("Response: ", response);
            setMasterData(data)
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
                {item?.statusOrderActionName ? 
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
                : null }
                <TouchableOpacity style={{flexDirection: 'row', justifyContent:'space-between', alignItems: 'center', paddingTop: 10}}>
                    <Text numberOfLines={2} style={[styles.text,{width: '85%'} ]}>Phiếu yêu cầu: <Text style={{color: color.orange}}>{item?.orderStatusName}</Text></Text>
                    <Ionicons name='caret-forward-outline' size={20} color='black' />
                </TouchableOpacity>
            </View>
        )
    }

    return (
        <>
            {isLoading && <CenterSpinner/>}
            <Screen style={ROOT} preset="fixed">
                <Header headerText='Quản lý đặt dịch vụ' rightText='' onRightPress={() =>{}} onLeftPress={() => goToPage('DashboardScreen')}/>
                <View style={{flex: 1,paddingTop: 16, backgroundColor: color.nau_nhat}}>
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
        backgroundColor: '#E5E5E5', 
        height: layout.height/10*7,
        minHeight: 650,
        marginTop:  layout.height*3/10,
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
    }
});
