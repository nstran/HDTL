import React, {useEffect, useState} from 'react';
import {observer} from 'mobx-react-lite';
import {Animated, Dimensions, FlatList, StyleSheet, View, ViewStyle, Image, Text} from 'react-native';
import {Header, Screen} from '../../components';
import {useIsFocused, useNavigation} from "@react-navigation/native"
// import { useStores } from "../../models"
import {color} from '../../theme';
import CenterSpinner from '../../components/center-spinner/center-spinner';
import Ionicons from 'react-native-vector-icons/Ionicons';
import { images } from '../../images';
import { useStores } from '../../models';
import { equalTo, firebaseDatabase, firebaseDatabaseRef, get, limitToLast, orderByChild, query, update } from '../../config/firebase';

const layout = Dimensions.get('window');

const ROOT: ViewStyle = {
    backgroundColor: color.primary,
    flex: 1,
};

export const NotifiScreen = observer(function NotifiScreen() {
    const navigation = useNavigation();
    const {HDLTModel} = useStores();
    const [isLoading, setLoading] = useState(false);
    const isFocus = useIsFocused()
    const [isRefresh, setRefresh] = useState(false)
    const [list_Notification, setList_Notification] = useState([])


    useEffect(() => {
      fetchData();
      ConverNotificationIsSeen()
    }, [isFocus,isRefresh]);
    const fetchData = async () => {     
        setRefresh(false)
        let customerId = await HDLTModel.getUserInfoByKey('customerId')
        let _list_Notification = []
        await get(query(firebaseDatabaseRef(firebaseDatabase, `notification/${customerId}`), orderByChild("employeeId"), equalTo(customerId)))
        .then((onsnapshot) => {
      
            if(onsnapshot.exists()){
                let value = onsnapshot.val()
                let keys = Object.keys(onsnapshot.val())
                keys.map((item, index) => {
                    _list_Notification.push({
                        content: value[item].content,
                        date: value[item].date,
                        status: value[item].status
                    })
                })

                _list_Notification.reverse()

                setList_Notification(_list_Notification)
            }
        })
    };

    const ConverNotificationIsSeen = async () => {
        let customerId = await HDLTModel.getUserInfoByKey('customerId')
        await get(query(firebaseDatabaseRef(firebaseDatabase, `notification/${customerId}`), orderByChild("status"), equalTo(false)))
        .then((snapshot) => {
          if(snapshot.exists()){
            let keys = Object.keys(snapshot.val())
            for(let i = 0; i < keys?.length; i++){
                update(firebaseDatabaseRef(firebaseDatabase, `notification/${customerId}/${keys[i]}`),{status: true})
            }
          }
        }) 
      }

    const calculateTime = (date) => {
        let newDate = converStringToDate(date)

        let time_chenh_lech = new Date().getTime() - new Date(newDate).getTime()
        
        let phut =  Math.floor(time_chenh_lech/(60*1000))
 
        
        if(phut < 60) {
            return phut + ' phút'
        }
        if(60 <= phut && phut < 24*60){
            let gio = Math.floor(phut/(60))
            return gio + ' giờ'
        }
        if(24*60 <= phut){
            let ngay = Math.floor(phut/(24*60))
            return ngay + ' ngay'
        }
    }

    const converStringToDate = (data) => {
        let string_date = data.split(' ')[0]
        let string_time = data.split(' ')[1]
        let arr_date = string_date.split('/')
        let arr_time = string_time.split(':')
        return new Date(arr_date[2], parseFloat( arr_date[1]) - 1,arr_date[0],arr_time[0],arr_time[1],arr_time[2]).getTime()
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

    const ItemView = ({item, index}) => {

        return (
            <View style={[{flexDirection: 'row', paddingHorizontal: 16, paddingVertical: 19, alignItems: 'center'}, item?.status ? {} : {backgroundColor: color.blue_nhat}]}>
                <View style={{width: 46, height: 46, borderRadius: 23, alignItems: 'center', justifyContent: 'center', borderWidth: 1, borderColor: color.xanh_nhat}}>
                    <Image source={images.icon_notifi_detail} style={{width: 18, height: 18}}/>
                </View>
                <View style={{marginLeft: 16,width: '80%'}}>
                    <Text numberOfLines={2} style={{color: color.black}}>{item?.content}</Text>
                    <Text style={{marginTop: 5, color: color.nau_nhat2}}>{calculateTime(item?.date)}</Text>
                </View>
                <Ionicons name='ellipsis-vertical-outline' size={25} />
                
            </View>
        )  
    }

    return (
        <>
            {isLoading && <CenterSpinner/>}
            <Screen style={ROOT} preset="fixed">
                <Header headerText='Thông báo' onLeftPress={() => navigation.goBack() } />
                <View style={{flex: 1, backgroundColor: color.white}}>
                    {list_Notification.length > 0 ? 
                        <FlatList
                            refreshing={isRefresh}
                            onRefresh={() => onRefresh()}
                            showsVerticalScrollIndicator={false}
                            showsHorizontalScrollIndicator={false}
                            style={{flex: 1}}
                            renderItem={ItemView}
                            data={list_Notification}
                            ListHeaderComponent={topComponent()}
                            keyExtractor={(item, index) => 'NotifyScreen-' + index + String(item)}
                        />
                    : null }
                </View>
            </Screen>
        </>
    );
});

const styles = StyleSheet.create({});
