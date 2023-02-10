// /**
//  * This is the navigator you will modify to display the logged-in screens of your app.
//  * You can use RootNavigator to also display an auth flow or other user flows.
//  *
//  * You'll likely spend most of your time in this file.
//  */
// import React, { useEffect, useState } from 'react';
// import {
//     createDrawerNavigator,
// } from '@react-navigation/drawer';
// import { SidebarDrawer } from './sidebar-drawer';
// // import BottomTabsScreen from './bottom-tab-navigator';
// import { NavigationContainer } from '@react-navigation/native';
// import { useBackButtonHandler } from './navigation-utilities';
// import {
//     LoginScreen,
//     DashboardScreen,
//     RegisterScreen,
//     JobDetailScreen,
//     NotifyScreen,
//     NotifyDetailScreen,
//     WellcomeScreen,
//     CategoryScreen,
//     LocationScreen,
//     ChatScreen,
//     ChatObo,
//     JobSearchScreen,
//     SettingScreen,
//     NewDetailScreen,
//     ForgotPWScreen,
//     NewPWScreen,
// } from '../screens';
// import { Dimensions } from 'react-native';
// import Main from './main-bottom';


// const layout = Dimensions.get('window');
// import { createNativeStackNavigator  } from '@react-navigation/native-stack'

// const Drawer = createDrawerNavigator();
// const Stack = createNativeStackNavigator();

// export function PrimaryNavigator() {

//     useBackButtonHandler(canExit);

//     // useEffect(() => {
//     // }, []);

//     return (
//         <NavigationContainer independent={true} >
//             {/* <Drawer.Navigator initialRouteName="Main" screenOptions={{ headerShown: false }}> */}
//             <Drawer.Navigator
//                 initialRouteName="Main"
//                 screenOptions={{
//                     headerShown: false,
//                     swipeEdgeWidth: 0,
//                     drawerPosition: 'right',
//                     drawerStyle: { width: layout.width }
//                 }}
//                 drawerContent={(props) => <SidebarDrawer {...props} />}
//             >
//                 <Drawer.Screen name="MainScreen" component={Main} options={{ headerShown: false }} />
//             {/* </Drawer.Navigator> */}
//             </Drawer.Navigator>
//         </NavigationContainer>
//     );
// }
// const exitRoutes = ['Trang chủ'];
// export const canExit = (routeName: string) => exitRoutes.includes(routeName);
