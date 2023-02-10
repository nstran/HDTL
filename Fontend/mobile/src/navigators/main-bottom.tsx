import React, { Component } from 'react'
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import Ionicons from "react-native-vector-icons/Ionicons";

import {
    LoginScreen,
    DashboardScreen,
    RegisterScreen,
    JobDetailScreen,
    NotifyScreen,
    NotifyDetailScreen,
    WellcomeScreen,
    CategoryScreen,
    LocationScreen,
    ChatScreen,
    ChatObo,
    JobSearchScreen,
    SettingScreen,
    NewDetailScreen,
    ForgotPWScreen,
    NewPWScreen,
    NewsScreen
} from '../screens';
import { Platform, Text } from 'react-native';
import { color } from '../theme/color';

const getTabBarIcon = (name) => ({color, size}: { color: string; size: number }) => (
    <Ionicons name={name} color="white" size={24}/>
);


const Tab = createBottomTabNavigator();

export default function Main(props: any) {
    const screens = [
        {name: 'NewsScreen', component: NewsScreen, icon: 'newspaper-outline', lable: 'News'},
        {name: 'DashboardScreen', component: DashboardScreen, icon: 'briefcase-outline', lable: 'jobs'},
        {name: 'NewDetailScreen', component: NewDetailScreen, icon: 'newspaper-outline', lable: 'New Detail'},
        {name: 'SettingScreen', component: SettingScreen, icon: 'settings-outline', lable: 'Settings'},
        {name: 'JobDetailScreen', component: JobDetailScreen, icon: 'newspaper-outline', lable: 'Job Detail'},
    ]
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
                                    fontSize: 16,
                                },
                                tabBarStyle: {
                                   height: Platform?.OS == 'ios' ? 120 : 70,
                                   backgroundColor: color.lightGrey,
                                },
                                tabBarActiveTintColor: "black",
                                tabBarInactiveTintColor: "white",
                                tabBarItemStyle: {
                                    display: ['NewsScreen', 'DashboardScreen', 'SettingScreen'].includes(item.name) ? undefined : 'none',
                                    backgroundColor: color.lightGrey,
                                    top: -1,
                                    paddingVertical: 7
                                },
                                tabBarIcon: getTabBarIcon(item.icon),
                            }} 
                        />
                    )
                })}
            </Tab.Navigator>
        )
}