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
    TouchableOpacity,
    Image
} from 'react-native';
import {Header, Screen} from '../../components';
import {useIsFocused, useNavigation} from "@react-navigation/native"
// import { useStores } from "../../models"
import {color} from '../../theme';
import CenterSpinner from '../../components/center-spinner/center-spinner';
import Ionicons from 'react-native-vector-icons/Ionicons';
import Video from 'react-native-video';
import { UnitOfWorkService } from '../../services/api/unitOfWork-service';
import { images } from '../../images';
import { log } from 'react-native-reanimated';

const _unitOfWork = new UnitOfWorkService()

const layout = Dimensions.get('window');

const ROOT: ViewStyle = {
    backgroundColor: color.black,
    flex: 1,
};

export const WelcomeScreen = observer(function WelcomeScreen() {
    const navigation = useNavigation();
    // const {movesModel} = useStores();
    const [isLoading, setLoading] = useState(false);
    const [isRefesh, setRefesh] = useState(false);
    const [tapIndex, setTapIndex] = useState(0);
    const isFocus = useIsFocused()
    const [data, setData] = useState();

    useEffect(() => {
      fetchData();
    }, [isFocus, tapIndex]);

    const fetchData = async () => {
        setLoading(true)
        if(tapIndex == 0) {
            let response = await _unitOfWork.user.TakeMobileAppConfigurationIntro({}) 
            console.log("response: ", response)
            
            if(response?.statusCode == 200){
                setData(response?.mobileAppConfigurationEntityModel)
            }
        }else{
            let response = await _unitOfWork.user.TakeMobileAppConfigurationLoginAndRegister({}) 
            if(response?.statusCode == 200){
                setData(response?.mobileAppConfigurationEntityModel)
            }
        }
        
        setLoading(false)
    };

    const goToPage = (page) => {
        navigation.navigate(page);
    };

    const topComponent = () => {
        return (
            <View style={{height: layout.height}}>
                <View style={{justifyContent: 'center', alignItems: 'center', width: layout.width}}>
                    {/* <Video
                        source={{ uri: 'http://d23dyxeqlo5psv.cloudfront.net/big_buck_bunny.mp4' }}
                        rate={1.0}
                        volume={1.0}
                        isMuted={false}
                        resizeMode="cover"
                        shouldPlay
                        isLooping
                        style={{width: layout.width, height: layout.width}}
                    /> */}
                    {tapIndex == 0 ? 
                      data?.introduceImageOrVideo ? 
                        <Image source={{uri: data?.introduceImageOrVideo}} style={{width: layout.height/2, height: layout.width}} resizeMode='stretch' />
                        : null
                    : 
                        data?.loginAndRegisterScreenImage ? 
                        <Image source={{uri: data?.loginAndRegisterScreenImage}} style={{width: layout.width, height: layout.width}} resizeMode='stretch' />
                        : null
                    }
                </View>
                <View style={{paddingHorizontal: 24, position: 'absolute', bottom: 100}}>
                    <Text style={{fontSize: 25, fontWeight: '700', color: color.black}}>Hãy để tôi lo</Text>
                    <Text style={{marginTop: 10, fontSize: 18, fontWeight: '400', color: color.black, lineHeight: 27}}>Chúng tôi luôn lắng nghe, tiếp nhận mong muốn của khách hàng</Text>
                    {tapIndex == 0 ?
                    <View style={{flexDirection: 'row', marginTop: 70, alignItems: 'center', justifyContent: 'space-between'}}>
                        <View style={{ flexDirection: 'row'}}>
                            <View style={{width: 17, height: 4, backgroundColor: color.blue, borderRadius: 2}}></View>
                            <View style={{marginLeft: 5,width: 4, height: 4, backgroundColor: color.lightGrey, borderRadius: 2}}></View>
                        </View>
                        <View style={{}}>
                            <TouchableOpacity 
                                style={{paddingHorizontal: 24, paddingVertical: 20, backgroundColor: color.blue, borderRadius: 16, flexDirection: 'row'}}
                                // onPress={() => setTapIndex(1)}
                                onPress={() =>  navigation.navigate('MainScreen',{screen: 'DashboardScreen'})}
                            >
                                <Text style={[styles.textWhile,{marginRight: 47}]}>Tiếp theo</Text>
                                <Ionicons name='arrow-forward-outline' color={color.white} size={25}/>
                            </TouchableOpacity>
                        </View>
                    </View>
                : 
                    <View style={{flexDirection: 'row', marginTop: 70, alignItems: 'center', justifyContent: 'space-between'}}>
                        <View style={{ flexDirection: 'row'}}>
                            <View style={{width: 4, height: 4, backgroundColor: color.lightGrey, borderRadius: 2}}></View>
                            <View style={{marginLeft: 5,width: 17, height: 4, backgroundColor: color.blue, borderRadius: 2}}></View>
                        </View>
                        <View style={{flexDirection: 'row'}}>
                            <TouchableOpacity 
                                style={{width: 130,paddingHorizontal: 16, paddingVertical: 20, borderRadius: 16, flexDirection: 'row', justifyContent: 'center', borderWidth: 1, borderColor: color.blue}}
                                onPress={() => goToPage('LoginScreen')}
                            >
                                <Text style={[styles.textWhile,{color: color.blue}]}>Đăng nhập</Text>
                            </TouchableOpacity>
                            <TouchableOpacity 
                                style={{width: 130,paddingHorizontal: 16, paddingVertical: 20, backgroundColor: color.blue, borderRadius: 16, flexDirection: 'row', marginLeft: 12, justifyContent: 'center'}}
                                onPress={() => navigation.navigate('LoginScreen',{isSingin : true})}
                            >
                                <Text style={[styles.textWhile]}>Đăng kí</Text>
                            </TouchableOpacity>
                        </View>
                    </View>
                 }
                </View>
            </View>
            
        );
    };

    return (
        <>
            {isLoading && <CenterSpinner/>}
            <Screen style={[ROOT]} preset="fixed">
                <View style={[{flex: 1}, data?.introduceColor ? {backgroundColor: data?.introduceColor} : {backgroundColor: color.white}]}>
                    <FlatList
                        showsVerticalScrollIndicator={false}
                        showsHorizontalScrollIndicator={false}
                        style={{flex: 1}}
                        renderItem={null}
                        data={[]}
                        ListHeaderComponent={topComponent()}
                        keyExtractor={(item, index) => 'wellcome-' + index + String(item)}
                    />
                </View>
            </Screen>
        </>
    );
});

const styles = StyleSheet.create({
    backgroundVideo: {},
    textWhile: {
        fontSize: 18,
        fontWeight: '400',
        color: color.white
    }
});
