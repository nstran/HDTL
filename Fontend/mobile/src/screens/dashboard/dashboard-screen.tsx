import React, {useEffect, useState} from 'react';
import {observer} from 'mobx-react-lite';
import {
    Alert,
    Animated, 
    BackHandler, 
    Dimensions, 
    FlatList, 
    Image, 
    Modal, 
    SafeAreaView, 
    StyleSheet, 
    Text, 
    TouchableOpacity,
    View, 
    ViewStyle
} from 'react-native';
import {Header, Screen} from '../../components';
import {useFocusEffect, useIsFocused, useNavigation} from "@react-navigation/native"
// import { useStores } from "../../models"
import {color} from '../../theme';
import CenterSpinner from '../../components/center-spinner/center-spinner';
import Ionicons from 'react-native-vector-icons/Ionicons';
import { useStores } from '../../models';
import { images } from '../../images';
import { UnitOfWorkService } from '../../services/api/unitOfWork-service';
import { equalTo, firebaseDatabase, firebaseDatabaseRef, get, limitToLast, onValue, orderByChild, query } from '../../config/firebase';
import { toJS } from 'mobx';
import Carousel from 'react-native-snap-carousel';
import IframeRenderer, { iframeModel } from '@native-html/iframe-plugin';
import RenderHtml from "react-native-render-html";
import { IGNORED_TAGS } from 'react-native-render-html/src/HTMLUtils';
import { showToast, StatusBarHeight_2 } from '../../services';
import { ScrollView } from 'react-native-gesture-handler';
import { log } from 'react-native-reanimated';

const _unitOfWork = new UnitOfWorkService()

const layout = Dimensions.get('window');

const ROOT: ViewStyle = {
    backgroundColor: color.primary,
    flex: 1,
};

const renderers = {
    iframe: IframeRenderer
  };

const customHTMLElementModels = {
    iframe: iframeModel
};

export const DashboardScreen = observer(function DashboardScreen() {
    const navigation = useNavigation();
    const {HDLTModel} = useStores();
    const [isLoading, setLoading] = useState(false);
    const isFocus = useIsFocused()
    const [isRefresh, setRefresh] = useState(false)
    const [modal_quang_cao, setModal_quang_cao] = useState(false)
    const [userId, setUserId] = useState('')
    const [full_name, setFullName] = useState('')
    const [userAvatar, setUserAvatar] = useState('')
    const [listServicePacket, setListServicePacket] = useState([])
    const [countMessage, setCountMessage] = useState([])
    const [countNotification, setCountNotification] = useState(0)

    const [list_qc, setList_qc] = useState([])
    const [quang_cao_detail, setQuang_cao_detail] = useState()

    const isCarousel = React.useRef(null)

    useFocusEffect(
        React.useCallback(() => {
          const onBackPress = () => {
            return true;
          };
    
          BackHandler.addEventListener('hardwareBackPress', onBackPress);
    
          return () =>
            BackHandler.removeEventListener('hardwareBackPress', onBackPress);
        }, []),
      );


    useEffect(() => {
      fetchData();
      getCountMessageNotSeen()
      getNotificationNotSeen()
    }, [isFocus]);

    // useEffect(() => {
    //     getCountMessageNotSeen()
    //   }, []);
    const fetchData = async () => {
        console.log("isFocus : ", isFocus)
        setLoading(true)
        // setRefresh(false)
        let _userId = await HDLTModel.getUserInfoByKey('userId')
        let userName = await HDLTModel.getUserInfoByKey('userFullName')
        let _userAvatar = await HDLTModel.getUserInfoByKey('userAvatar')
        setUserAvatar(_userAvatar)
        setFullName(userName)
        setUserId(_userId)

        console.log({"UserId": _userId})

        if(isFocus){
            await Promise.all([
                _unitOfWork.user.getListServicePacket({"UserId": "00000000-0000-0000-0000-000000000000"}),
                _unitOfWork.user.takeListAdvertisementConfiguration({}),
            ])
            .then((response) => {
                console.log("res all: ",response[0], response[1])
                let res_list_service = response[0]
                let res_list_qc = response[1]

                if(res_list_service?.statusCode == 200){
                    let arrList = [...res_list_service?.listServicePackageEntityModel].filter(item => item.subjectsApplication == true)
                    setListServicePacket(arrList)
                }

                if(res_list_qc?.statusCode == 200) setList_qc(res_list_qc?.listAdvertisementConfigurationEntityModel)

            });
        }
        

        

        setLoading(false)
    };

    const getCountMessageNotSeen = async () => {
        await HDLTModel.clear_CountChatsNotSeen()
        let _userId = await HDLTModel.getUserInfoByKey('userId')
        if(_userId){
            let countUserNotSeen = []
            let allUser = []

            // Lấy danh sách roomname đang chat
            await get(query(firebaseDatabaseRef(firebaseDatabase, "rooms"), orderByChild("userCreate"), equalTo(_userId)))
            .then((snapshot) => {
                if(snapshot.exists()){
                let value = snapshot.val()
                let keys = Object.keys(snapshot.val())
                keys.map((item,index) => {  
                    allUser.push(value[item]?.roomname)
                });
                }
            });
            await get(query(firebaseDatabaseRef(firebaseDatabase, "rooms"), orderByChild("receiver"), equalTo(_userId)))
            .then((snapshot) => {
                if(snapshot.exists()){
                let value = snapshot.val()
                let keys = Object.keys(snapshot.val())
                keys.map((item,index) => {  
                    allUser.push(value[item]?.roomname)
                });
                }
            });

            // lấy số người chưa đọc
            for(let i = 0; i < allUser?.length ; i++){
                await get(query(firebaseDatabaseRef(firebaseDatabase, `chats/${allUser[i]}`), orderByChild("isSeen"), equalTo(false), limitToLast(1)))
                .then((snapshot) => {
                    if(snapshot.exists()){
                        let value = snapshot.val()
                        let keys = Object.keys(snapshot.val())
                        keys.map((item,index) => {  
                            if(value[keys[0]].senderId != _userId) countUserNotSeen.push({
                                roomname: value[keys[0]].roomname
                            })
                        });
                    }
                });
            }

            setCountMessage(countUserNotSeen)

            await HDLTModel.thay_moi_CountChatsNotSeen(countUserNotSeen)

            // lắng nghe cập nhật số người chưa đọc
            for(let i = 0; i < allUser?.length ; i++){
                onValue(query(firebaseDatabaseRef(firebaseDatabase, `chats/${allUser[i]}`), orderByChild('isSeen'),equalTo(false), limitToLast(1)) , async (onSnapshot) => {
                    if(onSnapshot.exists()){
                        let _countUserNotSeen = await HDLTModel.getCountChatsNotSeen()
                        let user_ID = await HDLTModel.getUserInfoByKey('userId')
                
                        
                        let arrayUserNotSeen = toJS(_countUserNotSeen)
                        
                        let value = onSnapshot.val()
                        let keys = Object.keys(onSnapshot.val())
                        if(value[keys[0]].senderId != user_ID){
                            let check = false
                            arrayUserNotSeen.map(item => {
                                if(item?.roomname == value[keys[0]].roomname) check = true
                            })
                            
                            if(!check) arrayUserNotSeen.push({roomname: value[keys[0]].roomname})
                        }        
                        setCountMessage(arrayUserNotSeen)
                        await HDLTModel.thay_moi_CountChatsNotSeen(arrayUserNotSeen)
                    }
                    })
            }
        }
        
    }

    const getNotificationNotSeen = async () => {
        let _userId = await HDLTModel.getUserInfoByKey('userId')
        if(_userId){
            setCountNotification(0)
                let employeeId = await HDLTModel.getUserInfoByKey('customerId')
                onValue(query(firebaseDatabaseRef(firebaseDatabase, `notification/${employeeId}`), orderByChild('status'),equalTo(false)) , async (onSnapshot) => {
                    if(onSnapshot.exists()){
                        let keys = Object.keys(onSnapshot.val())
                        setCountNotification(keys?.length)
                }
            })
        }
        
        
    }

    const goToPage = (page) => {
        navigation.navigate('MainScreen', {screen: page});
    };
    const onRefresh = () => {
        // setRefresh(true)
        fetchData()
    };

    const checkLogin = () => {
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

    const ItemView = ({item,index}) => {
        return (
            <TouchableOpacity 
                style={{alignItems: 'center', marginBottom: 10}}
                onPress={() => {
                    navigation.navigate('ChooseServiceScreen1',{
                        data: item
                    })
                }}
            >
                <View style={[styles.box_item]}>
                    <Image resizeMode='stretch' source={{uri: item?.backgroundImage}} style={styles?.image} />
                    {/* <Text style={styles.text}>{item?.productCategoryName}</Text> */}
                </View>
                <View style={{ height: '100%', width: (layout.width - 32)/2 - 7, position: 'absolute',alignItems: 'center', justifyContent: 'center', left: 0}}>
                    <Image source={{uri: item?.icon}} style={styles?.image_icon} />
                    <Text numberOfLines={2}adjustsFontSizeToFit={true} style={[styles.text,{marginTop: 10, width: '100%', paddingHorizontal: 5,textAlign: 'center'}]}>{item?.name}</Text>
                </View>
            </TouchableOpacity>
        )
    }

    const topComponent = () => {
        return (
            <View style={{padding: 16}}>
                {/* <TouchableOpacity onPress={() => setModal_quang_cao(true)}>
                    <Image resizeMode='stretch' source={images.Banner_QC} style={styles.image_banner_qc} />
                </TouchableOpacity> */}
                <Carousel
                    ref={isCarousel}
                    layout='default'
                    data={list_qc}
                    renderItem={({item, index}) => {
                        return (
                            <TouchableOpacity onPress={() =>{
                                setQuang_cao_detail(item)
                                 setModal_quang_cao(true)
                            }}>
                                <Image resizeMode='stretch' source={{uri: item?.image}} style={styles.image_banner_qc} />
                            </TouchableOpacity>
                        )
                    }}
                    sliderWidth={layout.width - 32}
                    itemWidth={layout.width - 32}
                    // initialScrollIndex={1}
                    onSnapToItem={async (index) => {
            
                        isCarousel.snapToItem(index)
                    }}
                    // autoplay={true}
                    // loop={true}
                    useScrollView={true}
                /> 

                <View style={{flex: 1, marginTop: 16}}>
                    <FlatList
                            numColumns={2}
                            showsVerticalScrollIndicator={false}
                            showsHorizontalScrollIndicator={false}
                            style={{flex: 1}}
                            renderItem={ItemView}
                            data={listServicePacket}
                            keyExtractor={(item, index) => 'item-dv-' + index + String(item)}
                    />
                </View>

            </View>
        );
    };

    return (
        <>
            {isLoading && <CenterSpinner/>}
            <Screen style={ROOT} preset="fixed">
                {/* <Header /> */}
                <View style={[styles.header]}>
                    <View style={styles.header_left}>
                        {userAvatar ? 
                            <Image resizeMode='cover' source={{uri: userAvatar}} style={{width: 60, height: 60, borderRadius: 30}} />
                        : 
                            <Image resizeMode='cover' source={images.image_avatar} style={{width: 60, height: 60, borderRadius: 30}} />
                        }
                        
                        
                        <View style={{marginLeft: 16}}>
                            <Text style={{color: color.black}}>Hi!</Text>
                            <Text style={{color: color.black, fontSize: 18, fontWeight:'700'}}>{userId ? full_name : ''}</Text>
                        </View>
                    </View>
                    <View style={styles.header_right}>
                        <TouchableOpacity 
                            style={styles.header_right_icon}
                            onPress={() => navigation.navigate('MainScreen', {screen: 'ServicesScreen'})}
                        >
                            <Image resizeMode='contain' source={images.Icon_search} style={styles.image_header} />
                        </TouchableOpacity>
                        <TouchableOpacity
                            style={[styles.header_right_icon, {}]}
                            onPress={() => {
                                if(userId) navigation.navigate('RoomsScreen')
                                else{
                                    checkLogin()
                                }
                               
                            } }
                        >
                            {countMessage?.length > 0 ? 
                                <View style={{width: 24, height: 24, borderRadius: 12, backgroundColor: color.angry, alignItems: 'center', position: 'absolute', right: -10, top: -10}}>
                                    <Text style={{color: color.white, fontSize: 17}}>{countMessage?.length}</Text>
                                </View>
                            : null}
                            <Image resizeMode='contain' source={images.Icon_chat} style={styles.image_header} />
                        </TouchableOpacity>
                        <TouchableOpacity 
                            style={[styles.header_right_icon, {}]}
                            onPress={() => {
                                if(userId) navigation.navigate('NotifiScreen')
                                else{
                                    checkLogin()
                                }
                            }}
                        >
                            {countNotification > 0 ? 
                                <View style={{width: 24, height: 24, borderRadius: 12, backgroundColor: color.angry, alignItems: 'center', position: 'absolute', right: -10, top: -10}}>
                                    <Text style={{color: color.white, fontSize: 17}}>{countNotification}</Text>
                                </View>
                            : null}
                            <Image resizeMode='contain' source={images.Icon_notify} style={styles.image_header} />
                        </TouchableOpacity>
                        <TouchableOpacity 
                            style={styles.header_right_icon}
                            onPress={() => {
                                if(userId) navigation.navigate('GioHangScreen')
                                else{
                                    checkLogin()
                                }
                            }}
                        >
                            <Image resizeMode='contain' source={images.Icon_giohang}style={styles.image_header} />
                        </TouchableOpacity>
                    </View>
                </View>
                <View style={{flex: 1,backgroundColor: color.white}}>
                    <FlatList
                        refreshing={isRefresh}
                        onRefresh={() => onRefresh()}
                        showsVerticalScrollIndicator={false}
                        showsHorizontalScrollIndicator={false}
                        style={{flex: 1}}
                        renderItem={null}
                        data={[]}
                        ListHeaderComponent={topComponent()}
                        keyExtractor={(item, index) => 'dashboard-screen' + index + String(item)}
                    />
                </View>

                <Modal
                    animationType={"slide"}
                    transparent={true}
                    visible={modal_quang_cao}
                >
                    <View style={{flex: 1, backgroundColor: color.white, marginTop: StatusBarHeight_2}}>
                        <Header headerText='Giới thiệu - quảng cáo' onLeftPress={() => setModal_quang_cao(false)} />
                        <ScrollView style={{flex: 1}}
                            showsHorizontalScrollIndicator={false}
                            showsVerticalScrollIndicator={true}
                        >
                            <View style={{paddingHorizontal:30, alignItems: 'center', flex: 1}}>
                                <RenderHtml
                                    renderers={renderers}
                                    source={{
                                        html: `<div>${quang_cao_detail?.title}</div>`,
                                    }}
                                    customHTMLElementModels={customHTMLElementModels}
                                    tagsStyles={{
                                    }}
                                    renderersProps={{
                                    }}
                                />
                                <RenderHtml
                                    renderers={renderers}
                                    source={{
                                        html: `<div>${quang_cao_detail?.content}</div>`,
                                    }}
                                    customHTMLElementModels={customHTMLElementModels}
                                    // defaultWebViewProps={webViewProps}
                                    tagsStyles={{
                                    // p: {
                                    //     color: "#797979"
                                    // }
                                    }}
                                    renderersProps={{
                                        iframe: {
                                        scalesPageToFit: true,
                                        webViewProps: {
                                            /* Any prop you want to pass to iframe WebViews */
                                        }
                                        }
                                    }}
                                />
                            </View>
                                
                            
                        </ScrollView>
                       
                    </View>
                </Modal>
            </Screen>
        </>
    );
});

const styles = StyleSheet.create({
    header: {
        flexDirection: 'row',
        borderBottomColor: color.nau_nhat2,
        borderBottomWidth: 1,
        paddingVertical: 10,
        paddingHorizontal: 10,
        justifyContent: 'space-between',
        // marginTop: StatusBarHeight,
        backgroundColor: color.white
    },
    header_left: {
        width: '30%',
        flexDirection: 'row',
        alignItems: 'center'
    },
    header_right: {
        width: '55%',
        flexDirection: 'row',
        justifyContent: 'space-around',
        alignItems: 'center'
    },
    image_header: {
        width: 20
    },
    header_right_icon: {
        width: 40,
        height: 40,
        backgroundColor: color.lighterGrey,
        borderRadius: 20,
        alignItems: 'center',
        justifyContent: 'center'
    },
    image_banner_qc: {
        width: layout.width - 32,
        height: (layout.width - 32)*1/3 + 50,
        borderRadius: 10
    },
    box_item: {
        width: (layout.width - 28)/2 - 7,
        height: ((layout.width - 32)/2 - 7)*2/3,
        marginRight: 14,
        alignItems: 'center',
        // borderRadius: 16   justifyContent: 'center',
     
    },
    image: {
        width: (layout.width - 28)/2 - 7,
        height: ((layout.width - 28)/2 - 7)*2/3,
    },
    image_icon: {
        width: 40,
        height: 40,
    },
    text: {
        color: color.white,
        fontSize: 17,
        fontWeight: '600'
    }

});
