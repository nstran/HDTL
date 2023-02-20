import React, {useEffect, useState} from 'react';
import {observer} from 'mobx-react-lite';
import {
    Animated, 
    Dimensions, 
    FlatList, 
    StyleSheet, 
    View, 
    ViewStyle,
    Text,
    Image,
    TouchableOpacity,
    TextInput
} from 'react-native';
import {Header, Screen} from '../../components';
import {useNavigation} from "@react-navigation/native"
// import { useStores } from "../../models"
import {color} from '../../theme';
import CenterSpinner from '../../components/center-spinner/center-spinner';
import Ionicons from 'react-native-vector-icons/Ionicons';
import { images } from '../../images';

import { UnitOfWorkService } from '../../services/api/unitOfWork-service';
import { showToast } from '../../services';
const unitOfWork = new UnitOfWorkService()

const layout = Dimensions.get('window');

const ROOT: ViewStyle = {
    backgroundColor: color.white,
    flex: 1,
};

export const ForgotPasswordScreen = observer(function ForgotPasswordScreen() {
    const navigation = useNavigation();
    // const {movesModel} = useStores();
    const [isLoading, setLoading] = useState(false);
    const [email, setEmail] = useState('')
    const [placeholderEmail, setPlaceholderEmail] = useState('Email')

    // useEffect(() => {
    //   fetchData();
    // }, []);
    // const fetchData = async () => {
    // };

    const goToPage = (page) => {
        navigation.navigate(page);
    };

    const submit = async () => {
        if(email){
            setLoading(true)
            let response = await  unitOfWork.user.sendEmailForgotPass({
                "EmailAddress" :email
            })
            setLoading(false)
            console.log(response);
            
            if(response?.statusCode == 200){
                setEmail('')
                navigation.navigate('ChangePasswordScreen',{
                    data: {
                        email: email,
                        userName: response?.userName
                    }
                })
            }else{
                showToast('error',response?.messageCode)
            }
            
        }
    }

    const topComponent = () => {
        return (
            <View style={{paddingHorizontal: 30}}>
                <TouchableOpacity style={{marginTop: 30}} onPress={() => goToPage('LoginScreen')}>
                    <Ionicons name={"arrow-back-outline"} color={color.black} size={40}/>
                </TouchableOpacity>
                <View style={{alignItems: 'center'}}>
                    <Image source={images.forgot_password} style={styles.logo} />
                </View>
                <View style={{marginTop: 30}}>
                    <Text style={styles.text_header}>Quên mật khẩu?</Text>
                    <Text style={[styles.text,{marginTop: 20}]}>Nhập email của bạn</Text>
                    <View style={[styles.box_input,{marginTop: 25}]}>
                            <View style={styles.box_icon}>
                                <Ionicons name={"at-outline"} size={24} color={color.nau_nhat2}/>
                            </View>
                            <TextInput
                                style={styles.input}
                                placeholder={placeholderEmail}
                                onFocus={() => setPlaceholderEmail('')}
                                onBlur={() => {
                                    setPlaceholderEmail('Email')}}
                                autoCapitalize="none"
                                placeholderTextColor={color.nau_nhat2}
                                value={email}
                                onChangeText={(value) => setEmail(value)}
                            />
                    </View>
                    <TouchableOpacity style={styles.btn_blue} onPress={submit}>
                            <Text style={[styles.text_header,{color: color.white}]}>Gửi</Text>
                    </TouchableOpacity>
                </View>
                
            </View>
        );
    };

    return (
        <>
            {isLoading && <CenterSpinner/>}
            <Screen style={ROOT} preset="fixed">
                <View style={{flex: 1}}>
                    <FlatList
                        showsVerticalScrollIndicator={false}
                        showsHorizontalScrollIndicator={false}
                        style={{flex: 1}}
                        renderItem={null}
                        data={[]}
                        ListHeaderComponent={topComponent()}
                        keyExtractor={(item, index) => 'forgot-password-screen-' + index + String(item)}
                    />
                </View>
            </Screen>
        </>
    );
});

const styles = StyleSheet.create({
    logo:{
        width: layout.width/10*6,
        height: layout.width/10*6
    },
    text_header: {
        fontWeight: '600',
        fontSize: 24,
        color: color.black,
    },
    text: {
        fontSize: 15,
        fontWeight: '400',
        color: color.black
    },
    box_input: {
        flexDirection: 'row',
        borderRadius: 6,
        borderWidth: 1,
        borderColor: color.black,
        height: 50
    },
    box_icon: {
        marginTop: 10, 
        marginLeft: 10, 
    },
    input: {
        fontSize: 15,
        marginLeft: 10,
        color: color.black
    },
    btn_blue: {
        width: '100%',
        backgroundColor: color.blue,
        alignItems: 'center',
        justifyContent: 'center',
        marginTop: layout.width/10*2,
        paddingVertical: 10,
        borderRadius: 10
    },
});
