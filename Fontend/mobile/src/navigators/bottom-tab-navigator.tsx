import React, { Component, useEffect, useState } from 'react'
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import Ionicons from "react-native-vector-icons/Ionicons";

import {
    DashboardScreen,
    ProfileScreen, 
    ServicesScreen,
    ChooseServiceScreen1,
    YeuCauScreen,
    DashBoardAdminScreen,
    RoomsScreen, 
    GioHangScreen,

} from '../screens';
import { Alert, Platform, StyleSheet, Text, TouchableOpacity, View } from 'react-native';
import { color } from '../theme/color';
import { useIsFocused, useNavigation } from '@react-navigation/native';
import { useStores } from '../models';
import { showToast } from '../services';

const getTabBarIcon = (name) => ({color, size}: { color: string; size: number }) => (
    <Ionicons name={name} color={color} size={27}/>
);


const Tab = createBottomTabNavigator();

const TabButton = (props) => {
    let {item, onPress, accessibilityState, userID} = props
    let focus = accessibilityState.selected
    let arr = ['DashboardScreen','ServicesScreen']
    const navigation = useNavigation();


    return (
        <TouchableOpacity
            onPress={() => {
                if(userID) onPress()
                else {
                    if(arr.includes(item?.name)) onPress()
                    else {
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
                }
            }}
            style={styles.container}
        >
            <View
                style={[styles.btn]}
            >
                <Ionicons name={item?.icon} size={27} color={focus ? '#2C79BD' : '#808089'}/>
                <Text style={[{fontSize: 17},focus ? {color: '#2C79BD'} : {color: '#808089'}]}>{item?.lable}</Text>
            </View>
            
        </TouchableOpacity>
    )
}

export default function BottomTapScreen(props: any) {
    const isFocus = useIsFocused()
    const {HDLTModel} = useStores();
    const [screens, setScreens] = useState([
        {name: 'DashboardScreen', component: DashboardScreen, icon: 'home-outline', lable: 'Trang chủ'},
        {name: 'ServicesScreen', component: ServicesScreen, icon: 'layers-outline', lable: 'Dịch vụ'},
        {name: 'YeuCauScreen', component: YeuCauScreen, icon: 'reader-outline', lable: 'Yêu cầu'},
        {name: 'ProfileScreen', component: ProfileScreen, icon: 'person-circle-outline', lable: 'Tài khoản'},
        // {name: 'GioHangScreen', component: GioHangScreen, icon: 'person-circle-outline', lable: ''},
        // {name: 'RoomsScreen', component: RoomsScreen, icon: 'person-circle-outline', lable: 'Chat'},
    ])
    const [userID, setUserID] = useState()
    useEffect(() => {
      fetchData();
    }, [isFocus]);
    const fetchData = async () => {    
        let _userId = await HDLTModel.getUserInfoByKey('userId')  
        setUserID(_userId)
    };
    // const screens = [
    //     {name: 'DashboardScreen', component: DashboardScreen, icon: 'home-outline', lable: 'Trang chủ'},
    //     {name: 'ProfileScreen', component: ProfileScreen, icon: 'person-circle-outline', lable: 'Tài khoản'},
    // ]
        return (
            <Tab.Navigator 
                initialRouteName='DashboardScreen' 
                screenOptions={{ 
                    headerShown: false,
                }}
            >
                {screens.map(item => {
                    return(
                        <Tab.Screen
                            key={item?.name}
                            name={item?.name} 
                            component={item?.component}
                            options={{
                                tabBarShowLabel: true,
                                tabBarLabel: item?.lable,
                                tabBarLabelStyle: {
                                    fontSize: 15,
                                },
                                tabBarStyle: {
                                   height: Platform?.OS == 'ios' ? 90 : 70,
                                   backgroundColor: color.nau_nhat,
                                   borderTopColor: color.black,
                                   borderTopWidth: 1
                                },
                                tabBarActiveTintColor: '#2C79BD',
                                tabBarInactiveTintColor: '#808089',
                                tabBarItemStyle: {
                                    display: ['DashboardScreen','ServicesScreen','YeuCauScreen', 'ProfileScreen'].includes(item.name) ? undefined : 'none',
                                    backgroundColor: color.nau_nhat,
                                    top: -1,
                                    paddingVertical: 7,
                                },
                                tabBarIcon: getTabBarIcon(item.icon),
                                tabBarButton: (props) => <TabButton {...props} item={item} userID={userID}/>
                            }} 
                        />
                    )
                })}
            </Tab.Navigator>
        )
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center'
    },
    btn: {
        alignItems: 'center',
        justifyContent: 'center'
    },
    tabbar: {
        backgroundColor: color.white,
        height: 60,
        position: 'absolute',
        bottom: 16,
        right: 10,
        left: 10,
        borderRadius: 10
    },
    activeBackground: {
        position: 'absolute',
    },
})