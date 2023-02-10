import React, {useEffect, useState} from 'react';
import {observer} from 'mobx-react-lite';
import {
    Animated, 
    Dimensions, 
    FlatList, 
    Image, 
    StyleSheet, 
    Text, 
    TouchableOpacity,
    View, 
    ViewStyle
} from 'react-native';
import {Header, Screen} from '../../components';
import {useIsFocused, useNavigation} from "@react-navigation/native"
// import { useStores } from "../../models"
import {color} from '../../theme';
import CenterSpinner from '../../components/center-spinner/center-spinner';
import Ionicons from 'react-native-vector-icons/Ionicons';
import { useStores } from '../../models';
import { images } from '../../images';
import { StatusBarHeight } from '../../services';
import { UnitOfWorkService } from '../../services/api/unitOfWork-service';

const _unitOfWork = new UnitOfWorkService()

const layout = Dimensions.get('window');

const ROOT: ViewStyle = {
    backgroundColor: color.white,
    flex: 1,
};

export const DashboardScreen = observer(function DashboardScreen() {
    const navigation = useNavigation();
    const {HDLTModel} = useStores();
    const [isLoading, setLoading] = useState(false);
    const isFocus = useIsFocused()
    const [isRefresh, setRefresh] = useState(false)
    const [userId, setUserId] = useState('')
    const [full_name, setFullName] = useState('')
    const [userAvatar, setUserAvatar] = useState('')
    const [listServicePacket, setListServicePacket] = useState([])


    useEffect(() => {
      fetchData();
    }, [isFocus,isRefresh]);
    const fetchData = async () => {
        setLoading(true)
        setRefresh(false)
        let _userId = await HDLTModel.getUserInfoByKey('userId')
        let userName = await HDLTModel.getUserInfoByKey('userFullName')
        let _userAvatar = await HDLTModel.getUserInfoByKey('userAvatar')
        setUserAvatar(_userAvatar)
        setFullName(userName)
        setUserId(_userId)

        console.log(_userId);
        

        let response = await _unitOfWork.user.getListServicePacket({"UserId": _userId})

        
        if(response?.statusCode == 200) setListServicePacket(response?.listServicePackageEntityModel)
        setLoading(false)
    };

    const goToPage = (page) => {
        navigation.navigate('MainScreen', {screen: page});
    };
    const onRefresh = () => {
        setRefresh(true)
    };

    const ItemView = ({item,index}) => {
        return (
            <TouchableOpacity 
                style={{alignItems: 'center'}}
                onPress={() => {
                    navigation.navigate('ChooseServiceScreen1',{
                        data: item
                    })
                }}
            >
                <View style={styles.box_item}>
                    <Image resizeMode='contain' source={{uri: item?.backgroundImage}} style={styles?.image} />
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
                <TouchableOpacity>
                    <Image resizeMode='stretch' source={images.Banner_QC} style={styles.image_banner_qc} />
                </TouchableOpacity>

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
                        {userId ? 
                        <View style={{marginLeft: 16}}>
                            <Text style={{color: color.black}}>Hi!</Text>
                            <Text style={{color: color.black, fontSize: 18, fontWeight:'700'}}>{full_name}</Text>
                        </View>
                        : null}
                    </View>
                    <View style={styles.header_right}>
                        <TouchableOpacity style={styles.header_right_icon}>
                            <Image resizeMode='contain' source={images.Icon_search} style={styles.image_header} />
                        </TouchableOpacity>
                        <TouchableOpacity
                            style={[styles.header_right_icon, {}]}
                            onPress={() => navigation.navigate('RoomsScreen')}
                        >
                            <Image resizeMode='contain' source={images.Icon_chat} style={styles.image_header} />
                        </TouchableOpacity>
                        <TouchableOpacity style={[styles.header_right_icon, {}]}>
                            <Image resizeMode='contain' source={images.Icon_notify} style={styles.image_header} />
                        </TouchableOpacity>
                        <TouchableOpacity 
                            style={styles.header_right_icon}
                            onPress={() => navigation.navigate('GioHangScreen')}
                        >
                            <Image resizeMode='contain' source={images.Icon_giohang}style={styles.image_header} />
                        </TouchableOpacity>
                    </View>
                </View>
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
                        keyExtractor={(item, index) => 'dashboard-screen' + index + String(item)}
                    />
                </View>
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
        marginTop: StatusBarHeight
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
        width: (layout.width - 32)/2 - 7,
        height: ((layout.width - 32)/2 - 10)*2/3,
        marginRight: 14,
        marginBottom: 11,
        alignItems: 'center',
        justifyContent: 'center',
        borderRadius: 16
    },
    image: {
        width: (layout.width - 28)/2 - 7,
        height: ((layout.width - 28)/2 - 10)*2/3,
        borderRadius: 16
    },
    image_icon: {
        width: 40,
        height: 40,
        borderRadius: 20
    },
    text: {
        color: color.white,
        fontSize: 17,
        fontWeight: '600'
    }

});
