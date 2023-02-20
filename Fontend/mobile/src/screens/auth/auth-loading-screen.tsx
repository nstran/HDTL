import React, {useEffect, useState} from 'react';
import {observer} from 'mobx-react-lite';
import {Animated, Dimensions, FlatList, StyleSheet, Text, TouchableOpacity, View, ViewStyle} from 'react-native';
import {Header, Screen} from '../../components';
import {useIsFocused, useNavigation} from "@react-navigation/native"
// import { useStores } from "../../models"
import {color} from '../../theme';
import CenterSpinner from '../../components/center-spinner/center-spinner';
import Ionicons from 'react-native-vector-icons/Ionicons';
import { useStores } from '../../models';
import {getDeviceToken, requestNotificationPermission} from '../../services/notify'
import messaging, {
    FirebaseMessagingTypes,
} from '@react-native-firebase/messaging'
import { setLogout } from '../../services';

const layout = Dimensions.get('window');

const ROOT: ViewStyle = {
    backgroundColor: color.white,
    flex: 1,
};

export const AuthLoadingScreen = observer(function AuthLoadingScreen() {
    const navigation = useNavigation();
    const {HDLTModel} = useStores();
    const [isLoading, setLoading] = useState(false);
    const isFocus = useIsFocused()
    const [isRefresh, setRefresh] = useState(false)
    const [token, setToken] = useState('')

    useEffect(() => {
      fetchData();
    }, [isFocus,isRefresh]);
    const fetchData = async () => {

        await requestNotificationPermission()

        // let device_token = await getDeviceToken()
        // console.log("device_token: ", device_token);
        // setToken(device_token)

        setLogout(() => navigation.navigate('LoginScreen'))
        
        let userId = await HDLTModel.getUserInfoByKey('userId')
        let role = await HDLTModel.getUserInfoByKey('role')
        
        if(userId){
            if(role == 'CUS') goToPage('MainScreen')
            if(role == 'EMP') goToPage('MainAdminScreen')
        }else{
            goToPage('WelcomeScreen')
        }
    };

    useEffect(() => {
        messaging().setBackgroundMessageHandler(onMessageReceived);
        messaging().onNotificationOpenedApp(remoteMessage => {
            
            console.log(
              'Notification caused app to open from background state:',
              remoteMessage.notification,
            );
            navigation.navigate( 'MainScreen', {screen:'ServicesScreen'});
          });
      }, []);

    const onMessageReceived = async (message) => {
        // do sth
    };

    const handleSendNotification = () => {
        return;
      };
      

    // const handleSetTheAlarm = async () => {
    //     await fetch(`http://127.0.0.1:3000/alarm`, {
    //       method: 'POST',
    //       body: JSON.stringify({
    //         token,
    //       }),
    //     });
    //   };

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

    return (
        <>
            {/* <TouchableOpacity>
                <Text>SEND NOTIFICATION</Text>
            </TouchableOpacity>
            <TouchableOpacity onPress={() => handleSetTheAlarm()}>
                <Text>SET ALARM</Text>
            </TouchableOpacity> */}
        </>
    );
});

const styles = StyleSheet.create({});
