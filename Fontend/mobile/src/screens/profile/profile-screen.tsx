import React, {useEffect, useState} from 'react';
import {observer} from 'mobx-react-lite';
import {
    Animated, 
    Dimensions, 
    FlatList, 
    StyleSheet, 
    Text, 
    View, 
    ViewStyle,
    TouchableOpacity,
    Image,
    TextInput,
    Modal
} from 'react-native';
import {Header, Screen} from '../../components';
import {useIsFocused, useNavigation} from "@react-navigation/native"
// import { useStores } from "../../models"
import {color} from '../../theme';
import CenterSpinner from '../../components/center-spinner/center-spinner';
import Ionicons from 'react-native-vector-icons/Ionicons';
import {launchImageLibrary} from 'react-native-image-picker';
import { useStores } from '../../models';
import { images } from '../../images';
import { showToast } from '../../services';
import { toJS } from 'mobx';
import RNPickerSelect from 'react-native-picker-select';

import { UnitOfWorkService } from '../../services/api/unitOfWork-service';
import { firebaseDatabase, firebaseDatabaseRef, firebaseSet, get, query } from '../../config/firebase';

const _unitOfWork = new UnitOfWorkService()

const layout = Dimensions.get('window');

const ROOT: ViewStyle = {
    backgroundColor: color.primary,
    flex: 1,
};

export const ProfileScreen = observer(function ProfileScreen() {
    const navigation = useNavigation();
    const {HDLTModel} = useStores();
    const [isLoading, setLoading] = useState(false);
    const isFocus = useIsFocused()
    const [isRefresh, setRefresh] = useState(false)
    const [formData, setFormData] = useState({
        userId: '',
        phone: '',
        userFullName: '',
        userAvatar: '',
        email: '',
        address: '',
        fileName: '',
        gender: '',
        provinceId: ''
    })
    const [dataPassword, setDataPassword] = useState({
        newPassword: '',
        oldPassword: '',
        passwordNewSecure: true,
        passwordOldSecure: true,
    })
    const [modal_chang_password, setModalChangePassword] = useState(false)
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
    const [listData_KhuVuc, setListData_KhuVuc] = useState([])


    useEffect(() => {
      fetchData();
    }, [isFocus,isRefresh]);
    const fetchData = async () => {
        let _UserInfo = await HDLTModel.getUserInfo()
        let userInfo = toJS(_UserInfo)  

        let _formData = {...formData}
        _formData.userId = userInfo?.userId
        _formData.userFullName = userInfo?.userFullName
        _formData.phone = userInfo?.phone
        _formData.email = userInfo?.email
        _formData.address = userInfo?.address
        _formData.userAvatar = userInfo?.userAvatar
        _formData.gender = userInfo?.gender
        _formData.provinceId = userInfo?.provinceId
        setFormData(_formData)
        

        let res = await _unitOfWork.user.getListProvince()
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

    const setChangeText = (type, value) => {
        let _formData = {...formData};
        _formData[type] = value;
        setFormData(_formData);
    }

    const setChangeText_Password = (type, value) => {
        let _formData = {...dataPassword};
        _formData[type] = value;
        setDataPassword(_formData);
    };

    const goToPage = (page) => {
        navigation.navigate(page);
    };
    const onRefresh = () => {
        setRefresh(true)
    };

    const imagePicker = async () => {
        let options = {
            mediaType: 'photo',
            quality: 0.5,
            includeBase64: true,
        };
        await launchImageLibrary(options, (response) => {
            if (response && response?.assets?.length) {
               
                let _formData = {...formData};
                _formData['fileName'] = response?.assets[0]?.fileName;
                _formData['userAvatar'] = "data:image/png;base64," + response?.assets[0]?.base64?.toString();
                setFormData(_formData);
            }
        });
    };

    const ChangeProfile = async () => {
        let customer_Id = await HDLTModel.getUserInfoByKey('customerId')
        let userID = await HDLTModel.getUserInfoByKey('userId')
        let payload = {
            CustomerId: customer_Id,
            FirstNameLastName: formData?.userFullName,
            PhoneNumber: formData?.phone,
            Email: formData?.email,
            ProvinceId: formData?.provinceId,
            Address: formData?.address,
            Gender: formData?.gender,
            UserId: userID,
            AvatarUrl: formData?.userAvatar
        }

        if(!payload?.FirstNameLastName) {
            showToast('error', 'Họ và tên không được để trống!')
            return
        } 
        if(!payload?.Gender) {
            showToast('error', 'Giới tính không được để trống!')
            return
        } 
        if(!payload?.PhoneNumber) {
            showToast('error', 'Số điện thoại không được để trống!')
            return
        }
        if(!payload?.Email) {
            showToast('error', 'Email không được để trống!')
            return
        }
        if(!payload?.ProvinceId) {
            showToast('error', 'Khu vực không được để trống!')
            return
        }
        if(!payload?.Address) {
            showToast('error', 'Địa chỉ không được để trống!')
            return
        }
        
        setLoading(true)
        
        let response = await _unitOfWork.user.signUp(payload)
        

        if(response?.statusCode == 200) {
            await HDLTModel.setUserInfo({
                userFullName: formData?.userFullName,
                userAvatar: formData?.userAvatar,
                phone: formData?.phone,
                email: formData?.email,
                address: formData?.address,
                gender: formData?.gender,
                provinceId: formData?.provinceId,
                // role: 'CUS'
            })
            showToast("success", response?.message)
            setDataPassword({
                newPassword: '',
                oldPassword: '',
                passwordNewSecure: true,
                passwordOldSecure: true,
            })
        }else{
            showToast("error", response?.messageCode)
        }
        setLoading(false) 

    }

    const changePassword = async () => {
        let _userId = await HDLTModel.getUserInfoByKey('userId')
        let payload = {
            "UserId": _userId,
            "OldPassword": dataPassword?.oldPassword,
            "NewPassword":  dataPassword?.newPassword
        }
        let response = await _unitOfWork.user.changePassword(payload)
        
        if(response?.statusCode == 200){
            setModalChangePassword(false)
            showToast("success",response?.messageCode )
        }else{
            showToast("error",response?.messageCode )
        }
        
    }

    const chatWithAdmin = async () => {
        let _userId = await HDLTModel.getUserInfoByKey('userId')
        let fullname = await HDLTModel.getUserInfoByKey('userFullName')
        let response = await _unitOfWork.user.createDataFireBase({
            RoomName:`${_userId}_f54811af-84cc-4d1e-b5fd-5865f6ebb2b6`,
            OtherId:"f54811af-84cc-4d1e-b5fd-5865f6ebb2b6",
            UserId:_userId
        })
        if(response?.statusCode == 200){
            await get(query(firebaseDatabaseRef(firebaseDatabase, `rooms/${response?.roomname}`)))
            .then((snapshot) => {
                if(!snapshot?.exists()){
                    firebaseSet(firebaseDatabaseRef(firebaseDatabase, `rooms/${response?.roomname}`), {
                        receiver: "f54811af-84cc-4d1e-b5fd-5865f6ebb2b6",
                        receiverName: 'Hoai Son',
                        roomname: response?.roomname,
                        userCreate: _userId,
                        userCreateName: fullname
                    });
                }
            })
             
        }
        
        // firebaseSet(firebaseDatabaseRef(firebaseDatabase, `Rooms/${_userId}_${id_admin}`), {
        //     receiver: id_admin,
        //     receiverName: 'Hoai Son',
        //     roomName: _userId + "_" + id_admin,
        //     userCreate: _userId,
        //     userCreateName: fullname
        // });
        // await get(query(firebaseDatabaseRef(firebaseDatabase, "RoomUsers/f54811af-84cc-4d1e-b5fd-5865f6ebb2b6")))
        // .then((snapshot) => {
        //     console.log("snapshot: ", snapshot.val());
        //     if(snapshot.val()){
        //         navigation.navigate('ChatScreen',{
        //             data: {...snapshot.val()}
        //         })
        //     }
            
        // })
    }


    const topComponent = () => {
        return (
            <View style={{}}>
                <View style={styles.box_content_avatar}>
                    <View style={styles.box_avatar}>
                    <View style={styles.box_image_camera}>
                        <TouchableOpacity style={styles.box_image_camera_2} 
                            onPress={() => imagePicker()}
                        >
                            <Image source={images.icon_camera} style={styles.image_camera} />
                        </TouchableOpacity>    
                    </View>
                    {formData?.userAvatar ? 
                        formData?.fileName ?
                                <Image style={styles.avatar} resizeMode={"cover"}
                                       source={{uri:formData?.userAvatar}}/> :
                                <Image style={styles.avatar} resizeMode={"cover"}
                                       source={{uri: formData?.userAvatar}}/>
                    : 
                        <Image style={styles.avatar} resizeMode={"cover"}
                            source={images.image_avatar}/> 
                    }
                    </View>
                </View>
                <View style={{marginTop: 16}}>
                    <View style={styles.box_input}>
                        <Text style={[styles.text]}>Họ và tên</Text>
                        <TextInput
                            style={styles.input}
                            autoCapitalize="none"
                            placeholderTextColor={color.nau_nhat2}
                            value={formData?.userFullName}
                            onChangeText={(value) => setChangeText('userFullName', value)}
                        />
                    </View>
                    <View style={[styles.box_input,{paddingVertical: 5, alignItems: 'center'}]}>
                        <Text style={[styles.text]}>Giới tính</Text>
                        {/* <TextInput
                            style={styles.input}
                            autoCapitalize="none"
                            placeholderTextColor={color.nau_nhat2}
                            value={formData?.gender}
                            onChangeText={(value) => setChangeText('gender', value)}
                        /> */}
                        <RNPickerSelect
                            placeholder={{}}
                            items={listData_Gender}
                            onValueChange={value => {
        
                                if(value != undefined){
                                    setChangeText('gender', value)
                                }
                                
                            }}
                            useNativeAndroidPickerStyle={false}
                            value={formData?.gender}
                            style={ {inputAndroid: styles.inputAndroid, inputIOS: styles.inputIOS}}
                            Icon={() => {
                                return <View style={{marginTop: 10,marginRight: 5}}><Ionicons name={"chevron-down-outline"} color={color.nau_nhat2} size={24}/></View>
                            }}
                            // ref={ref_input_location}
                        />
                    </View>
                    <View style={styles.box_input}>
                        <Text style={[styles.text]}>Số điện thoại</Text>
                        <TextInput
                            editable={false}
                            style={styles.input}
                            placeholder={'Số điện thoại'}
                            autoCapitalize="none"
                            placeholderTextColor={color.nau_nhat2}
                            value={formData?.phone}
                            onChangeText={(value) => setChangeText('phone', value)}
                        />
                        {/* <Text>{formData?.phone}</Text> */}
                    </View>
                    <View style={styles.box_input}>
                        <Text style={[styles.text]}>Email</Text>
                        <TextInput
                            style={styles.input}
                            placeholder={'Email'}
                            autoCapitalize="none"
                            placeholderTextColor={color.nau_nhat2}
                            value={formData?.email}
                            onChangeText={(value) => setChangeText('email', value)}
                        />
                    </View>

                    <View style={[styles.box_input, {paddingVertical: 5, alignItems: 'center'}]}>
                        <Text style={[styles.text,{}]}>Khu vực</Text>
                   
                        <RNPickerSelect
                            placeholder={{}}
                            items={listData_KhuVuc}
                            onValueChange={value => {
                                setChangeText('provinceId', value)
                            }}
                            useNativeAndroidPickerStyle={false}
                            value={formData?.provinceId}
                            style={ {inputAndroid: styles.inputAndroid, inputIOS: styles.inputIOS}}
                            Icon={() => {
                                return <View style={{marginTop: 10,marginRight: 5}}><Ionicons name={"chevron-down-outline"} color={color.nau_nhat2} size={24}/></View>
                            }}
                            // ref={ref_input_location}
                        />
                    </View>
                    <View style={[styles.box_input, {}]}>
                        <Text style={[styles.text,{marginBottom: 16}]}>Địa chỉ</Text>
                        <TextInput
                            style={styles.input}
                            placeholder={'Address'}
                            autoCapitalize="none"
                            placeholderTextColor={color.nau_nhat2}
                            value={formData?.address}
                            onChangeText={(value) => setChangeText('address', value)}
                        />
                    </View>

                    <TouchableOpacity style={[styles.box_btn]} onPress={() => setModalChangePassword(true)}>
                        <Text style={[styles.text_btn]}>Đổi mật khẩu</Text>
                    </TouchableOpacity>
                    <TouchableOpacity style={[styles.box_btn,{marginTop: 0}]} onPress={async () =>{
                        let res = await _unitOfWork.user.removeDeviceId({
                            UserId: formData?.userId
                        })
                        await HDLTModel.logout()
                        goToPage('LoginScreen')
                    } }>
                        <Text style={[styles.text_btn]}>Đăng xuất</Text>
                    </TouchableOpacity>
                    {/* <TouchableOpacity style={[styles.box_btn,{marginTop: 0}]} onPress={async () =>{
                        chatWithAdmin()
                    } }>
                        <Text style={[styles.text_btn]}>Chat with admin</Text>
                    </TouchableOpacity> */}
                </View>
                
            </View>
        );
    };

    return (
        <>
            {isLoading && <CenterSpinner/>}
            <Screen style={ROOT} preset="fixed">
                <Header
                    style={modal_chang_password ? {opacity: 0.3} : {}}
                    headerText='Thông tin tài khoản' 
                    rightText='Lưu'
                    onRightPress={ChangeProfile}
                    onLeftPress={() => navigation.goBack()} />
                <View style={[{flex: 1,backgroundColor: color.white},modal_chang_password ? {opacity: 0.3} : {}]}>
                    <FlatList
                        refreshing={isRefresh}
                        onRefresh={() => onRefresh()}
                        showsVerticalScrollIndicator={false}
                        showsHorizontalScrollIndicator={false}
                        style={{flex: 1}}
                        renderItem={null}
                        data={[]}
                        ListHeaderComponent={topComponent()}
                        keyExtractor={(item, index) => 'profile-' + index + String(item)}
                    />
                </View>
                <Modal
                    animationType={"slide"}
                    transparent={true}
                    visible={modal_chang_password}
                    onRequestClose={() => { }}
                >
                    <View style={styles.modal_container}>
                        <View style={{flexDirection: 'row', alignItems: 'center', width: '100%', justifyContent: 'center',marginBottom: 16}}>
                            <Text style={[styles.text,{width: '30%'}]}>Mật khẩu cũ</Text>
                            <View style={{width: '60%', alignItems: 'center', justifyContent: 'center'}}>
                                <TextInput 
                                    style={styles.input_pass}
                                    placeholder={'Mật khẩu cũ'}
                                    autoCapitalize="none"
                                    placeholderTextColor={color.nau_nhat2}
                                    value={dataPassword?.oldPassword}
                                    onChangeText={(value) => {
                                        setChangeText_Password('oldPassword', value)
                                    }}
                                    secureTextEntry={dataPassword?.passwordOldSecure}
                                />
                                <TouchableOpacity style={[styles.eye]} onPress={()=>setChangeText_Password('passwordOldSecure', !dataPassword?.passwordOldSecure)}>
                                    <Ionicons name={dataPassword?.passwordOldSecure ? 'eye-outline' : 'eye-off-outline'} color={color.nau_nhat2} size={24}/>
                                </TouchableOpacity>
                            </View>
                            
                        </View>
                        <View style={{flexDirection: 'row', alignItems: 'center', width: '100%', justifyContent: 'center', marginBottom: 16}}>
                            <Text style={[styles.text,{width: '30%'}]}>Mật khẩu mới</Text>
                            <View style={{width: '60%',alignItems: 'center', justifyContent: 'center'}}>
                               <TextInput
                                    style={styles.input_pass}
                                    placeholder={'Mật khẩu mới'}
                                    autoCapitalize="none"
                                    placeholderTextColor={color.nau_nhat2}
                                    value={dataPassword?.newPassword}
                                    onChangeText={(value) => {
                                        setChangeText_Password('newPassword', value)
                                    }}
                                    secureTextEntry={dataPassword?.passwordNewSecure}
                                /> 
                                <TouchableOpacity style={[styles.eye,{marginTop: 35}]} onPress={()=>setChangeText_Password('passwordNewSecure', !dataPassword?.passwordNewSecure)}>
                                    <Ionicons name={dataPassword?.passwordNewSecure ? 'eye-outline' : 'eye-off-outline'} color={color.nau_nhat2} size={24}/>
                                </TouchableOpacity>
                            </View>
                            
                        </View>
                        <TouchableOpacity style={[styles.btn, {marginTop: 20}]} onPress={changePassword}>
                            <Text style={[styles.text_btn,{color: color.white}]}>Lưu</Text>
                        </TouchableOpacity>
                        <TouchableOpacity style={styles.btn} onPress={() =>{
                            setDataPassword({
                                newPassword: '',
                                oldPassword: '',
                                passwordNewSecure: true,
                                passwordOldSecure: true,
                            })
                            setModalChangePassword(false)
                        } }>
                            <Text style={[styles.text_btn,{color: color.white}]}>Huỷ</Text>
                        </TouchableOpacity>
                    
                    </View>
                </Modal>
            </Screen>
        </>
    );
});

const styles = StyleSheet.create({
    box_content_avatar: {
        marginTop:16,
        alignItems: 'center'
    },
    box_avatar: {
        width: layout.width/10*3.5 + 10,
        height: layout.width/10*3.5 + 10,
        backgroundColor: '#E0E0E0',
        alignItems: 'center',
        justifyContent: 'center',
        borderRadius: (layout.width/10*3.5 + 10)/2
    },
    avatar: {
        width: layout.width/10*3.5,
        height: layout.width/10*3.5,
        borderRadius: (layout.width/10*3.5)/2
    },
    box_image_camera: {
        position: 'absolute',
        width: 50,
        height: 50,
        borderRadius: 35,
        zIndex: 999,
        alignItems: 'center',
        justifyContent: 'center',
        backgroundColor: color.white,
        bottom: 0,
        right: 0
    },
    box_image_camera_2: {
        width: 45,
        height: 45,
        borderRadius: 25,
        backgroundColor: color.nau_nhat2,
        alignItems: 'center',
        justifyContent: 'center',
    },
    image_camera: {
        width: 25,
        height: 20
    },
    box_input: {
        padding: 16,
        borderTopColor: color.lighterGrey,
        borderTopWidth: 1,
        // borderBottomColor: color.lighterGrey,
        // borderBottomWidth: 1,
        flexDirection:'row',
        justifyContent: 'space-between'
    },
    text: {
        // height: 100,
        // width: 200,
        width: '35%',
        fontSize: 17,
        color: color.black,
    },
    input: {
        padding: 0,
        width: '60%',
        color: color.black
    },
    box_btn: {
        marginVertical: 16,
        padding: 16,
        borderTopColor: color.lighterGrey,
        borderTopWidth: 1,
        borderBottomColor: color.lighterGrey,
        borderBottomWidth: 1,
        alignItems: 'center',
        justifyContent: 'center'
    },
    text_btn: {
        fontSize: 19, 
        fontWeight:'700',
        width: '100%', 
        textAlign:'center',
        color: color.black
    },
    modal_container: {
        height: layout.height*4/10,
        marginTop: layout.height*2/10,
        backgroundColor: color.white,
        width: layout.width*9/10,
        marginLeft: layout.width/20,
        borderRadius: 20,
        alignItems: 'center',
        justifyContent: 'center'
    },
    input_pass: {
        borderWidth: 1,
        borderColor: color.black,
        paddingVertical: 7,
        borderRadius: 7,
        width: '95%',
        marginLeft: 10,
        paddingHorizontal: 7, 
        height: 50
    },
    btn: {
        width: '50%',
        paddingVertical: 10,
        borderRadius: 10,
        alignItems: 'center',
        backgroundColor: color.blue,
        marginBottom: 16
    },
    eye: {
        position: 'absolute',
        right: 5,
    },
    inputAndroid: {
        width: (layout.width)/100*60,
        height: 50,
        fontSize: 16,
        paddingHorizontal: 20,
        // paddingVertical: 8,
        borderRadius: 5,
        color: 'black',
      },
    inputIOS : {
        width: (layout.width)/100*60,
        height: 50,
        fontSize: 16,
        paddingHorizontal: 20,
        // paddingVertical: 8,
        borderRadius: 5,
        color: 'black'
    }
});
