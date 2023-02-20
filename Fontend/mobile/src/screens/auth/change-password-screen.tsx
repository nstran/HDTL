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
import {useIsFocused, useNavigation, useRoute} from "@react-navigation/native"
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

export const ChangePasswordScreen = observer(function ChangePasswordScreen() {
    const navigation = useNavigation();
    // const {movesModel} = useStores();
    const [isLoading, setLoading] = useState(false);
    const [isRefresh, setRefresh] = useState(false)
    const isFocus = useIsFocused()
    const [formData, setFormData] = useState<any>({
        code: '',
        password: '',
        password_confirm: '',
        passwordSecure: true,
        passwordSecure_confirm: true
    });
    const { params }: any = useRoute();

    const [placeholder, setPlaceholder] = useState<any>({
        code: 'Mã xác minh email',
        password: 'Mật khẩu mới',
        password_confirm: 'Xác nhận mật khẩu mới'
    });

    useEffect(() => {
      fetchData();
    }, [isFocus,isRefresh ]);
    const fetchData = async () => {
        console.log("params: ", params);
        
    };

    const setChangeText = (type, value) => {
        let _formData = {...formData};
        _formData[type] = value;
        setFormData(_formData);
    };
    const setChangePlaceholder = (type, value) => {
        let _formData = {...placeholder};
        _formData[type] = value;
        setPlaceholder(_formData);
    };


    const goToPage = (page) => {       
        navigation.navigate(page);
    };

    const handleChangePass = async () => {
        if(!formData?.code){
            showToast("error", 'Mã xác minh không được để trống')
            return
        }
        if(!formData?.password){
            showToast("error", 'Mật khẩu không được để trống')
            return
        }
        if(!formData?.password_confirm){
            showToast("error", 'Xác nhận mật khẩu không được để trống')
            return
        }
        let response = await unitOfWork.user.changePasswordForgot({
            "code": formData?.code,
            "userName": params?.data?.userName,
            "newPassword": formData?.password,
            "confirmPassword": formData?.password_confirm
        })
        console.log("response: ", response);
        
        if(response?.statusCode == 200){
            showToast("success", response?.message)
            navigation.navigate('LoginScreen')
        }else{
            showToast("error", response?.message)
        }

    }

    const topComponent = () => {
        return (
            <View style={{paddingHorizontal: 30}}>
                <TouchableOpacity style={{marginTop: 30}} onPress={() => goToPage('ForgotPasswordScreen')}>
                    <Ionicons name={"arrow-back-outline"} color={color.black} size={40}/>
                </TouchableOpacity>
                <View style={{alignItems: 'center'}}>
                    <Image source={images.change_password} style={styles.logo} />
                </View>
                <View style={{marginTop: 30}}>
                    <Text style={styles.text_header}>Mật khẩu mới</Text>
                    <View style={{alignItems: 'center'}}>
                        <Text style={[styles.text,{marginTop: 20, fontSize: 16}]}>Mã xác minh đã được gửi tới email:</Text>
                        <Text style={[styles.text,{marginTop: 5,fontSize: 16, color: color.blue}]}>{params?.data?.email}</Text>
                    </View>
                    
                    <View style={[styles.box_input,{marginTop: 25}]}>
                            <View style={styles.box_icon}>
                                <Ionicons name={"at-outline"} size={24} color={color.nau_nhat2}/>
                            </View>
                            <TextInput
                                style={styles.input}
                                placeholder={placeholder?.code}
                                onFocus={() => setChangePlaceholder('code','')}
                                onBlur={() => {
                                    setChangePlaceholder('code', 'Mã xác minh email')}}
                                autoCapitalize="none"
                                placeholderTextColor={color.nau_nhat2}
                                value={formData?.code}
                                onChangeText={(value) => setChangeText('code', value)}
                            />
                    </View>
                    <View style={[styles.box_input,{marginTop: 25}]}>
                            <View style={styles.box_icon}>
                                <Ionicons name={"at-outline"} size={24} color={color.nau_nhat2}/>
                            </View>
                            <TextInput
                                style={styles.input}
                                placeholder={placeholder?.password}
                                onFocus={() => setChangePlaceholder('password','')}
                                onBlur={() => {
                                    setChangePlaceholder('password', 'Mật khẩu mới')}}
                                autoCapitalize="none"
                                placeholderTextColor={color.nau_nhat2}
                                value={formData?.password}
                                onChangeText={(value) => setChangeText('password', value)}
                                secureTextEntry={formData?.passwordSecure}
                            />
                            <TouchableOpacity style={styles.eye} onPress={()=>setChangeText('passwordSecure', !formData?.passwordSecure)}>
                                <Ionicons name={formData?.passwordSecure ? 'eye-outline' : 'eye-off-outline'} color={color.nau_nhat2} size={24}/>
                            </TouchableOpacity>
                    </View>
                    <View style={[styles.box_input,{marginTop: 25}]}>
                            <View style={styles.box_icon}>
                                <Ionicons name={"at-outline"} size={24} color={color.nau_nhat2}/>
                            </View>
                            <TextInput
                                style={styles.input}
                                placeholder={placeholder?.password_confirm}
                                onFocus={() => setChangePlaceholder('password_confirm','')}
                                onBlur={() => {
                                    setChangePlaceholder('password_confirm', 'Mã xác minh email')}}
                                autoCapitalize="none"
                                placeholderTextColor={color.nau_nhat2}
                                value={formData?.password_confirm}
                                onChangeText={(value) => setChangeText('password_confirm', value)}
                                secureTextEntry={formData?.passwordSecure_confirm}
                            />
                            <TouchableOpacity style={styles.eye} onPress={()=>setChangeText('passwordSecure_confirm', !formData?.passwordSecure_confirm)}>
                                <Ionicons name={formData?.passwordSecure_confirm ? 'eye-outline' : 'eye-off-outline'} color={color.nau_nhat2} size={24}/>
                            </TouchableOpacity>
                    </View>
                    <TouchableOpacity style={styles.btn_blue} onPress={handleChangePass}>
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
                        keyExtractor={(item, index) => 'change-password-screen-' + index + String(item)}
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
        width: '80%',
        fontSize: 15,
        marginLeft: 10,
        color: color.black
    },
    btn_blue: {
        width: '100%',
        backgroundColor: color.blue,
        alignItems: 'center',
        justifyContent: 'center',
        marginTop: 30,
        paddingVertical: 10,
        borderRadius: 10
    },
    eye: {
        position: 'absolute',
        right: 12,
        top: 10,
    },
});
