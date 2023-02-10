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
    GioHangScreen
} from '../screens';
import { Platform, Text } from 'react-native';
import { color } from '../theme/color';
import { useIsFocused } from '@react-navigation/native';
import { useStores } from '../models';

const getTabBarIcon = (name) => ({color, size}: { color: string; size: number }) => (
    <Ionicons name={name} color={color} size={27}/>
);


const Tab = createBottomTabNavigator();

export default function BottomTapScreen(props: any) {
    const isFocus = useIsFocused()
    const {HDLTModel} = useStores();
    const [screens, setScreens] = useState([
        {name: 'DashboardScreen', component: DashboardScreen, icon: 'home-outline', lable: 'Trang chủ'},
        {name: 'ServicesScreen', component: ServicesScreen, icon: 'layers-outline', lable: 'Dịch vụ'},
        {name: 'YeuCauScreen', component: YeuCauScreen, icon: 'reader-outline', lable: 'Yêu cầu'},
        {name: 'ProfileScreen', component: ProfileScreen, icon: 'person-circle-outline', lable: 'Tài khoản'},
        // {name: 'RoomsScreen', component: RoomsScreen, icon: 'person-circle-outline', lable: 'Chat'},
    ])
    useEffect(() => {
      fetchData();
    }, [isFocus]);
    const fetchData = async () => {      
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
                                    display: ['DashboardScreen','ServicesScreen','YeuCauScreen', 'ProfileScreen','DashBoardAdminScreen'].includes(item.name) ? undefined : 'none',
                                    backgroundColor: color.nau_nhat,
                                    top: -1,
                                    paddingVertical: 7,
                                },
                                tabBarIcon: getTabBarIcon(item.icon),
                            }} 
                        />
                    )
                })}
            </Tab.Navigator>
        )
}