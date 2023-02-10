import React, {useEffect, useRef, useState} from 'react';
import {observer} from 'mobx-react-lite';
import {Animated, 
    Dimensions, 
    FlatList, 
    StyleSheet, 
    Text, 
    View, 
    ViewStyle,
    TextInput,
    Image,
    TouchableOpacity,
    Alert,
} from 'react-native';
import {Header, Screen} from '../../components';
import {useIsFocused, useNavigation, useRoute} from "@react-navigation/native"
// import { useStores } from "../../models"
import {color} from '../../theme';
import CenterSpinner from '../../components/center-spinner/center-spinner';
import { images } from '../../images';
import Ionicons from 'react-native-vector-icons/Ionicons';
import { showToast } from '../../services';
import RNPickerSelect from 'react-native-picker-select';

import { UnitOfWorkService } from '../../services/api/unitOfWork-service';
import { useStores } from '../../models';
import { 
    firebaseDatabase,
    firebaseDatabaseRef,
    firebaseSet,
    createUserWithEmailAndPassword,
    auth
} from '../../config/firebase';
import { getDeviceToken } from '../../services/notify';


const unitOfWork = new UnitOfWorkService()

const layout = Dimensions.get('window');

const ROOT: ViewStyle = {
    backgroundColor: color.white,
    flex: 1,
};

export const LoginScreen = observer(function LoginScreen() {
    const navigation = useNavigation();
    const [isRefresh, setRefresh] = useState(false)
    const {HDLTModel} = useStores();
    const [isLoading, setLoading] = useState(false);
    const [tapIndex, setTapIndex] = useState('Login')
    const isFocus = useIsFocused()
    const { params }: any = useRoute();

    const [listData_KhuVuc, setListData_KhuVuc] = useState([])
    const [listData_Gender, setListData_Gender] = useState([
        {
            label: 'Nam',
            value: 'NAM'
        },
        {
            label: 'Nữ',
            value: 'NU'
        }
    ])

    const [formData, setFormData] = useState<any>({
        phone: '',
        password: '',
        passwordSecure: true,
    });
    const [formDataSingUp, setFormDataSingUp] = useState<any>({
        name: '',
        phone: '',
        email: '',
        address: '',
        location: 0,
        password: '',
        passwordSecure: true,
        Gender: ''
    });
    const [placeholderLogin, setPlaceholderLogin] = useState<any>({
        phone: 'Số điện thoại',
        password: 'Mật khẩu'
    });

    const [placeholderSingup, setPlaceholderSingup] = useState<any>({
        name: 'Họ và tên',
        phone: 'Số điện thoại',
        password: 'Mật khẩu',
        email: 'Email',
        address: 'Địa chỉ',
        location: 'Khu vực',
        gender: 'Giới tính'
    });

    const ref_input_password = useRef();

    const ref_input_name = useRef()
    const ref_input_phone = useRef();
    const ref_input_password_singup = useRef()
    const ref_input_email = useRef();
    const ref_input_address = useRef();
    const ref_input_location = useRef()
    const [token, setToken] = useState('1')

    useEffect(() => {
      fetchData();
    }, [isFocus, isRefresh]);
    const fetchData = async () => {
        console.log('para: ', params);
        if(params){
            if(params?.isSingin) setTapIndex('Singup')
        }

        let device_token = await getDeviceToken()
        console.log("device_token: ", device_token);
        setToken(device_token)
        
        setRefresh(false)
        let res = await unitOfWork.user.getListProvince()
        let list_Khu_vuc : any = []
        if(res?.statusCode == 200) {
            res?.listProvinceEntityModel.map((item, index) => {
                list_Khu_vuc.push({
                    label: item?.provinceName,
                    value: item?.provinceId
                })
            })
        }
        setListData_KhuVuc(list_Khu_vuc)
        
    };

    const goToPage = (page) => {
        navigation.navigate(page);
    };

    const setChangeText = (type, value) => {
        let _formData = {...formData};
        _formData[type] = value;
        setFormData(_formData);
    };
    const setChangeText_Singup = (type, value) => {
        let _formData = {...formDataSingUp};
        _formData[type] = value;
        setFormDataSingUp(_formData);
    };
    const setChangePlaceholderLogin = (type, value) => {
        let _formData = {...placeholderLogin};
        _formData[type] = value;
        setPlaceholderLogin(_formData);
    };
    const setChangePlaceholderSingup = (type, value) => {
        let _formData = {...placeholderSingup};
        _formData[type] = value;
        setPlaceholderSingup(_formData);
    };
    const onRefresh = () => {
        setRefresh(true)
    };

    const resetData = () => {
        setFormData({
            phone: '',
            password: '',
            passwordSecure: true,
        })
        setFormDataSingUp({
            name: '',
            phone: '',
            email: '',
            address: '',
            location: 0,
            password: '',
            passwordSecure: true,
        })
    }


    const submit = async () => {
        // navigation.navigate('MainScreen',{isAdmin: true,screen: 'DashboardScreen'})
        // setTapIndex("Login")
        if(tapIndex == 'Login'){
            if(!formData?.phone){
            showToast('error', 'Số điện thoại không được để trống!')
            return
            }
            if(!formData?.password){
                showToast('error', 'Mật khẩu không được để trống!')
                return
            }
            setLoading(true)
            let payload = {
                User: {
                    UserId: null,
                    Password: formData?.password,
                    UserName: formData?.phone,
                    EmployeeId: "",
                    EmployeeCode: "",
                    Disabled: false,
                    CreatedById: "DE2D55BF-E224-4ADA-95E8-7769ECC494EA",
                    CreatedDate: null,
                    UpdatedById: null,
                    UpdatedDate: null,
                    Active: true,
                    DeviceId: token
                }
            }
            console.log("payload: ", payload);
            
            let response_login = await unitOfWork.user.login(payload)
            setLoading(false)        
            if(response_login?.statusCode == 200){

                // await firebaseSet(firebaseDatabaseRef(
                //     firebaseDatabase,
                //     `users/${response_login?.currentUser?.userId}`
                // ),{
                //     nickname: response_login?.contactEntityModel?.firstName
                // }
                let _userId = response_login?.currentUser?.userId
                let _userFullName = response_login?.contactEntityModel?.firstName
                console.log(_userFullName);
                
                firebaseSet(firebaseDatabaseRef(firebaseDatabase, `RoomUsers/${_userId}`), {
                    nickname: `${_userFullName}`,
                    user_ID: _userId
                });



                await HDLTModel.setUserInfo({
                    userId: response_login?.currentUser?.userId,
                    customerId: response_login?.currentUser?.employeeId,
                    token: response_login?.currentUser?.token,
                    userFullName: response_login?.contactEntityModel?.firstName,
                    userAvatar: response_login?.userAvatar,
                    phone: response_login?.contactEntityModel?.phone,
                    email: response_login?.contactEntityModel?.email,
                    address: response_login?.contactEntityModel?.address,
                    gender: response_login?.contactEntityModel?.gender,
                    provinceId: response_login?.contactEntityModel?.provinceId,
                    role: response_login?.contactEntityModel?.objectType
                    // role: 'CUS'
                })
                await unitOfWork.storage.setItem('TOKEN', response_login?.currentUser?.token)
                resetData()
                if(response_login?.contactEntityModel?.objectType){
                    if(response_login?.contactEntityModel?.objectType == 'CUS') navigation.navigate('MainScreen',{isAdmin: true, screen: 'DashboardScreen'})
                   else navigation.navigate('MainAdminScreen',{isAdmin: true, screen: 'DashBoardAdminScreen'})
                }
            }else{
                showToast('error', response_login?.messageCode)
            }
        }
        else {
            if(!formDataSingUp?.name){
                showToast('error', 'Họ và tên không được để trống!')
                return
            }
            if(!formDataSingUp?.location){
                showToast('error', 'Khu vực không được để trống!')
                return
            }
            if(!formDataSingUp?.phone){
                showToast('error', 'Số điện thoại không được để trống!')
                return
            }
            if(!formDataSingUp?.Gender){
                showToast('error', 'Giới tính không được để trống!')
                return
            }
            if(!formDataSingUp?.password){
                showToast('error', 'Mật Khẩu không được để trống!')
                return
            }
            if(!formDataSingUp?.email){
                showToast('error', 'Email không được để trống!')
                return
            }
            if(!formDataSingUp?.email){
                showToast('error', 'Địa chỉ không được để trống!')
                return
            }

            let payload = {
                firstNameLastName: formDataSingUp?.name,
                password: formDataSingUp?.password,
                phoneNumber:formDataSingUp?.phone,
                email: formDataSingUp?.email,
                address: formDataSingUp?.address,
                provinceId: formDataSingUp?.location,
                Gender: formDataSingUp?.Gender
            }
            setLoading(true)
            
            let respone_singup = await unitOfWork.user.signUp(payload)
     
            
            if(respone_singup?.statusCode == 200) {
                showToast("success", "Đăng kí thành công!")
                resetData()
                setTapIndex('Login')
            }else{
                showToast('error', respone_singup?.messageCode)
            }
            setLoading(false)
            
        }
        
    }

    const topComponent = () => {
        return (
            <View style={{paddingHorizontal: 30, paddingBottom: 16}}>
                <View style={styles.header}>
                    <Image source={images.logo_1} style={styles.logo} />
                    <Text style={{color: color.nau_nhat2, fontSize: 14, marginTop: 14}}>Dựng tài năng, xây hạnh phúc</Text>
                </View>
                {tapIndex == 'Login' ? 
                    <View>
                        <Text style={[styles.text_header,{marginTop: 30}]}>Đăng nhập</Text>
                        <View style={[styles.box_input,{marginTop: 25}]}>
                            <View style={styles.box_icon}>
                                <Ionicons name={"phone-portrait-outline"} size={24} color={color.nau_nhat2}/>
                            </View>
                            <TextInput
                                onSubmitEditing={() => ref_input_password.current?.focus()}
                                // onChangeText={(value) => {
                                //     setChangeText("username", value)
                                // }}
                                // value={formData?.userName}
                                style={styles.input}
                                placeholder={placeholderLogin?.phone}
                                onFocus={() => setChangePlaceholderLogin('phone','')}
                                onBlur={() => {
                                    setChangePlaceholderLogin('phone','Số điện thoại')}}
                                autoCapitalize="none"
                                placeholderTextColor={color.nau_nhat2}
                                // keyboardType='numeric'
                                value={formData?.phone}
                                // value={token}
                                onChangeText={(value) => setChangeText('phone', value)}
                            />
                        </View>
                        <View style={[styles.box_input,{marginTop:20}]}>
                            <View style={styles.box_icon}>
                                <Ionicons name={"lock-open-outline"} size={24} color={color.nau_nhat2} />
                            </View>
                            <TextInput
                                ref={ref_input_password}
                                // onChangeText={(value) => {
                                //     setChangeText("username", value)
                                // }}
                                // value={formData?.userName}
                                onSubmitEditing={() => submit()}
                                style={styles.input}
                                placeholder={placeholderLogin?.password}
                                onFocus={() => setChangePlaceholderLogin('password','')}
                                onBlur={() => {
                                    setChangePlaceholderLogin('password','Mật Khẩu')}}
                                autoCapitalize="none"
                                placeholderTextColor={color.nau_nhat2}
                                secureTextEntry={formData?.passwordSecure}
                                value={formData?.password}
                                onChangeText={(value) => setChangeText('password', value)}
                            />
                            <TouchableOpacity style={styles.eye} onPress={()=>setChangeText('passwordSecure', !formData?.passwordSecure)}>
                                <Ionicons name={formData?.passwordSecure ? 'eye-outline' : 'eye-off-outline'} color={color.nau_nhat2} size={24}/>
                            </TouchableOpacity>
                        </View>
                        <View style={{width: '100%', alignItems: 'flex-end', marginTop: 25}}>
                            <TouchableOpacity 
                                onPress={() => goToPage('ForgotPasswordScreen')}
                            >
                                <Text style={styles.text_2}>Quên mật khẩu?</Text>
                            </TouchableOpacity>
                        </View>
                      
                        
                        <TouchableOpacity style={styles.btn_blue} onPress={submit}>
                            <Text style={[styles.text_header,{color: color.white}]}>Đăng nhập</Text>
                        </TouchableOpacity>
                        <View style={{width: '100%', alignItems: 'center', marginTop: 30}}>
                            <Text>Chưa có tài khoản? <Text onPress={() => setTapIndex('Singup')} style={styles.text_2}>Đăng kí ngay</Text></Text>
                        </View>
                    
                    </View>
                : 
                    <View style={{marginTop: 30}}>

                        <View style={{flexDirection: 'row', justifyContent: 'space-between', marginTop: 10}}>
                            <View style={{width: '47%'}}>
                                <Text style={[styles.text_2]}>Họ và tên <Text style={styles.text_red}>*</Text></Text>
                                <View style={[styles.box_input,{marginTop: 10}]}>
                                    <TextInput
                                        onSubmitEditing={() => ref_input_phone.current?.focus()}
                                        style={styles.input}
                                        placeholder={placeholderSingup?.name}
                                        onFocus={() => setChangePlaceholderSingup('name','')}
                                        onBlur={() => {
                                            setChangePlaceholderSingup('name','Họ và tên')}}
                                        autoCapitalize="none"
                                        placeholderTextColor={color.nau_nhat2}
                                        value={formDataSingUp?.name}
                                        onChangeText={(value) => setChangeText_Singup('name', value)}
                                    />
                                </View>
                            </View>
                            <View style={{width: '47%'}}>
                                <Text style={[styles.text_2]}>Khu vực <Text style={styles.text_red}>*</Text></Text>
                                <View style={[styles.box_input,{marginTop: 10}]}>
                                    <RNPickerSelect
                                        placeholder={"Khu vực"}
                                        items={listData_KhuVuc}
                                        onValueChange={value => {
                                            setChangeText_Singup('location', value)
                                        }}
                                        useNativeAndroidPickerStyle={false}
                                        value={formDataSingUp?.location}
                                        style={ {inputAndroid: styles.inputAndroid, inputIOS: styles.inputIOS}}
                                        Icon={() => {
                                            return <View style={{marginTop: 10,marginRight: 5}}><Ionicons name={"chevron-down-outline"} color={color.nau_nhat2} size={24}/></View>
                                        }}
                                        // ref={ref_input_location}
                                    />
                                </View>
                            </View>
                        </View>

                        <View style={{flexDirection: 'row', justifyContent: 'space-between', marginTop: 10}}>
                            <View style={{width: '47%'}}>
                                <Text style={[styles.text_2]}>Số điện thoại <Text style={styles.text_red}>*</Text></Text>
                                <View style={[styles.box_input,{marginTop: 10}]}>
                                    <TextInput
                                        ref={ref_input_phone}
                                        onSubmitEditing={() => ref_input_password_singup.current?.focus()}
                                        style={styles.input}
                                        placeholder={placeholderSingup?.phone}
                                        onFocus={() => setChangePlaceholderSingup('phone','')}
                                        onBlur={() => {
                                            setChangePlaceholderSingup('phone','Số điện thoại')}}
                                        autoCapitalize="none"
                                        keyboardType='numeric'
                                        placeholderTextColor={color.nau_nhat2}
                                        value={formDataSingUp?.phone}
                                        onChangeText={(value) => setChangeText_Singup('phone', value)}
                                    />
                                </View>
                            </View>
                            <View style={{width: '47%'}}>
                                <Text style={[styles.text_2]}>Giới tính <Text style={styles.text_red}>*</Text></Text>
                                <View style={[styles.box_input,{marginTop: 10}]}>
                                    <RNPickerSelect
                                        placeholder={"Giới tính"}
                                        items={listData_Gender}
                                        onValueChange={value => {
                                            setChangeText_Singup('Gender', value)
                                        }}
                                        useNativeAndroidPickerStyle={false}
                                        value={formDataSingUp?.Gender}
                                        style={ {inputAndroid: styles.inputAndroid, inputIOS: styles.inputIOS}}
                                        Icon={() => {
                                            return <View style={{marginTop: 10,marginRight: 5}}><Ionicons name={"chevron-down-outline"} color={color.nau_nhat2} size={24}/></View>
                                        }}
                                        // ref={ref_input_location}
                                    />
                                </View>
                            </View>
                        </View>

                        <Text style={[styles.text_2,{marginTop: 10}]}>Mật khẩu <Text style={styles.text_red}>*</Text></Text>
                        <View style={[styles.box_input,{marginTop: 10}]}>
                            <TextInput
                                ref={ref_input_password_singup}
                                onSubmitEditing={() => ref_input_email.current?.focus()}
                                style={[styles.input, {width: '70%'}]}
                                placeholder={placeholderSingup?.password}
                                onFocus={() => setChangePlaceholderSingup('password','')}
                                onBlur={() => {
                                    setChangePlaceholderSingup('password','Mật khẩu')}}
                                autoCapitalize="none"
                                secureTextEntry={formDataSingUp?.passwordSecure}
                                placeholderTextColor={color.nau_nhat2}
                                value={formDataSingUp?.password}
                                onChangeText={(value) => setChangeText_Singup('password', value)}
                            />
                            <TouchableOpacity style={[styles.eye,{}]} onPress={()=>setChangeText_Singup('passwordSecure', !formDataSingUp?.passwordSecure)}>
                                    <Ionicons name={formDataSingUp?.passwordSecure ? 'eye-outline' : 'eye-off-outline'} color={color.nau_nhat2} size={24}/>
                            </TouchableOpacity>
                        </View>

                        <Text style={[styles.text_2,{marginTop: 10}]}>Email <Text style={styles.text_red}>*</Text></Text>
                        <View style={[styles.box_input,{marginTop: 10}]}>
                            <TextInput
                                onSubmitEditing={() => ref_input_address.current?.focus()}
                                ref={ref_input_email}
                                // onChangeText={(value) => {
                                //     setChangeText("username", value)
                                // }}
                                // value={formData?.userName}
                                style={styles.input}
                                placeholder={placeholderSingup?.email}
                                onFocus={() => setChangePlaceholderSingup('email','')}
                                onBlur={() => {
                                    setChangePlaceholderSingup('email','Email')}}
                                autoCapitalize="none"
                                placeholderTextColor={color.nau_nhat2}
                                value={formDataSingUp?.email}
                                onChangeText={(value) => setChangeText_Singup('email', value)}
                            />
                        </View>

                        <Text style={[styles.text_2,{marginTop: 10}]}>Địa chỉ <Text style={styles.text_red}>*</Text></Text>
                        <View style={[styles.box_input,{marginTop: 10}]}>
                            <TextInput
                                onSubmitEditing={() => ref_input_address.current?.focus()}
                                ref={ref_input_address}
                                style={styles.input}
                                placeholder={placeholderSingup?.address}
                                onFocus={() => setChangePlaceholderSingup('address','')}
                                onBlur={() => {
                                    setChangePlaceholderSingup('address','Địa chỉ')}}
                                autoCapitalize="none"
                                placeholderTextColor={color.nau_nhat2}
                                value={formDataSingUp?.address}
                                onChangeText={(value) => setChangeText_Singup('address', value)}
                            />
                        </View>
                        <TouchableOpacity style={styles.btn_blue} onPress={submit}>
                            <Text style={[styles.text_header,{color: color.white}]}>Đăng kí</Text>
                        </TouchableOpacity>
                        <View style={{width: '100%', alignItems: 'center', marginTop: 20}}>
                            <Text>Bạn đã có tài khoản? <Text onPress={() => setTapIndex('Login')} style={styles.text_2}>Đăng nhập</Text></Text>
                        </View>
                    </View>
                }
            </View>           
        );
    };

    return (
        <>
            {isLoading && <CenterSpinner/>}
            <Screen style={ROOT} preset="fixed">
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
                        keyExtractor={(item, index) => 'login-screen-' + index + String(item)}
                    />
                </View>
            </Screen>
        </>
    );
});

const styles = StyleSheet.create({
    header: {
        marginTop: 30,
        justifyContent: 'center',
        alignItems: 'center',
    },
    logo:{
        width: layout.width/10*3,
        height: layout.width/10*3
    },
    text_header: {
        fontWeight: '600',
        fontSize: 24,
        color: color.black,
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
        color: color.black,
        width: '75%',
    },
    btn_blue: {
        width: '100%',
        backgroundColor: color.blue,
        alignItems: 'center',
        justifyContent: 'center',
        marginTop: 25,
        paddingVertical: 15,
        borderRadius: 10
    },
    text_2: {
        fontSize: 16, 
        fontWeight:'700', 
        color: color.blue
    },
    eye: {
        position: 'absolute',
        right: 12,
        top: 10,
    },
    text_red: {
        color: color.error
    },
    inputAndroid: {
        width: (layout.width - 60)/100*47,
        height: 50,
        fontSize: 16,
        paddingHorizontal: 10,
        paddingVertical: 8,
        borderRadius: 5,
        color: 'black',
        paddingRight: 30, // to ensure the text is never behind the icon
      },
    inputIOS : {
        width: (layout.width - 60)/100*47,
        height: 50,
        fontSize: 16,
        paddingHorizontal: 10,
        paddingVertical: 8,
        borderRadius: 5,
        color: 'black',
        paddingRight: 30,
    }
});
