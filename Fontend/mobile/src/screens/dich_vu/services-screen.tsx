import React, {useEffect, useState} from 'react';
import {observer} from 'mobx-react-lite';
import {
    Animated, 
    Dimensions, 
    FlatList, 
    StyleSheet, 
    Text, 
    TextInput, 
    View, 
    ViewStyle,
    Image,
    Modal,
    TouchableOpacity
} from 'react-native';
import {Header, Screen} from '../../components';
import {useIsFocused, useNavigation} from "@react-navigation/native"
// import { useStores } from "../../models"
import {color} from '../../theme';
import CenterSpinner from '../../components/center-spinner/center-spinner';
import Ionicons from 'react-native-vector-icons/Ionicons';
import { images } from '../../images';
import { UnitOfWorkService } from '../../services/api/unitOfWork-service';
import { useStores } from '../../models';

const _unitOfWork = new UnitOfWorkService()

const layout = Dimensions.get('window');

const ROOT: ViewStyle = {
    backgroundColor: color.white,
    flex: 1,
};

const data = [1,1,1,1,1,1,1,1,1,1]

export const ServicesScreen = observer(function ServicesScreen() {
    const navigation = useNavigation();
    const {HDLTModel} = useStores();
    const [isLoading, setLoading] = useState(false);
    const isFocus = useIsFocused()
    const [isRefresh, setRefresh] = useState(false)
    const [modal_option, setModal_Option] = useState(false)
    const [listServicePacket, setListServicePacket] = useState([])
    const [listProducCategory, setListProducCategory] = useState([])
    const [listProvince, setListProvince] = useState([])
    const [listProduce_select, setListProduce_Select] = useState([])
    const [listProvince_select, setListProvince_Select] = useState([])
    const [list_ID_Packet_search, setList_ID_Packet_search] = useState([])
    const [textSearch, setTextSearch] = useState('')

    useEffect(() => {
      fetchData();
    }, [isFocus,isRefresh]);
    const fetchData = async () => {
        setList_ID_Packet_search([])
        setTextSearch('')
        setRefresh(false)
        setLoading(true)
        let user_id = await HDLTModel.getUserInfoByKey('userId')
        let response = await _unitOfWork.user.getListServicePacket({"UserId": user_id})
        let listCategory : any = []
        let _listProvince : any = []
        if(response?.statusCode == 200){
            setListServicePacket(response?.listServicePackageEntityModel)
            response?.listServicePackageEntityModel.map((item, index) => {
                let objCategory = {
                    productCategoryId : item?.productCategoryId,
                    productCategoryName : item?.productCategoryName,
                    icon: item?.icon
                }
                let check_Category = true
                listCategory.map((item_2,index_2) => {
                    if(item_2?.productCategoryId == item?.productCategoryId) check_Category = false
                })
                if(check_Category) listCategory.push(objCategory)

                let objProvince = {
                    provinceId: item?.provinceId,
                    provinceName: item?.provinceName
                }
                let check_Province = true
                _listProvince.map((item_2,index_2) => {
                    if(item_2?.provinceId == item?.provinceId) check_Province = false
                })
                if(check_Province) _listProvince.push(objProvince)
            })
        }
        setListProducCategory(listCategory)
        setListProvince(_listProvince)
        setLoading(false) 
    };

    const chooseOption = (type, id) => {

        let _listProduce_select = [...listProduce_select]
        let _listProvince_select = [...listProvince_select]
        
        if(type == 0){
            if(_listProduce_select.includes(id)){
                let new_arr = _listProduce_select.filter(item => item !== id);
                setListProduce_Select(new_arr)
            }else {
                _listProduce_select.push(id)
                setListProduce_Select(_listProduce_select)
            }
        }
        if(type == 1){
            if(_listProvince_select.includes(id)){
                let new_arr = _listProvince_select.filter(item => item !== id);
                setListProvince_Select(new_arr)
            }else {
                _listProvince_select.push(id)
                setListProvince_Select(_listProvince_select)
            }
        }
    }

    const searchProducePacket = async () => {
        setLoading(true)
        let user_id = await HDLTModel.getUserInfoByKey('userId')
        let response = await _unitOfWork.user.getListServicePacket({UserId: user_id, FilterText: textSearch})
        
        let list_id : any =  []
        response?.listServicePackageEntityModel.map((item, index) => {
            list_id.push(item?.id)
        })
        
        setList_ID_Packet_search(list_id)
        setLoading(false)
        
    }

    const goToPage = (page) => {
        navigation.navigate(page);
    };
    const onRefresh = () => {
        setList_ID_Packet_search([])
        setTextSearch('')
        setRefresh(true)
    };

    const Item_View_Child = (item_2) => {
        return (
            <TouchableOpacity 
                style={{ borderWidth: 1, borderColor: color.nau_nhat2, borderRadius: 12, marginBottom: 10, width: layout.width - 35}}
                onPress={() => {
                    navigation.navigate('ChooseServiceScreen1',{
                        data: item_2
                    })
                }}
            >
                <Image
                    resizeMode='stretch'
                    source={{uri: item_2?.mainImage}} 
                    style={{width: layout.width - 35,  height: (layout.width - 35)*2/3, borderRadius: 12, marginBottom: 12}}
                />
                <View style={{paddingHorizontal: 10}}>
                    <Text style={[styles.title_2]}>{item_2?.name}</Text>
                    <View  style={[styles.box_item,{justifyContent: 'space-between', marginTop: 10}]}>
                        <Text>{item_2?.countOption} dịch vụ</Text>
                        <Text>{item_2?.provinceName}</Text>
                    </View>
                </View>  
            </TouchableOpacity>
        )
    }

    const ItemView = ({item, index}) => {
        if(listProduce_select?.length == 0){
                return (
                    <View style={{padding: 16}} onPress={() => goToPage('ChooseServiceScreen1')}>
                            <View style={styles.box_item}>
                                <Image
                                    source={images.icon_yte} style={{width: 25, height: 25}}
                                />
                                <Text style={[styles.title]}>{item?.productCategoryName}</Text>
                            </View>
                            {listServicePacket && listServicePacket.map((item_2, index_2) => {
                                if(item_2?.productCategoryId == item?.productCategoryId) {
                                    if(list_ID_Packet_search?.length > 0 && list_ID_Packet_search.includes(item_2?.id)){ 
                                        if(listProvince_select?.length == 0){
                                            return Item_View_Child(item_2)
                                        }else{
                                            if(listProvince_select.includes(item_2?.provinceId)){
                                                Item_View_Child(item_2)
                                            }
                                        }
                                    }
                                    if(list_ID_Packet_search?.length == 0){                                   
                                        if(listProvince_select?.length == 0){
                                             return Item_View_Child(item_2)
                                        }else{
                                            if(listProvince_select.includes(item_2?.provinceId)){
                                                return Item_View_Child(item_2)
                                            }
                                        }
                                    }     
                                }
                            })}
                            
                    </View>
                )
        }else{
            if(listProduce_select.includes(item?.productCategoryId)){
                return (
                    <TouchableOpacity style={{padding: 16}} onPress={() => goToPage('ChooseServiceScreen1')}>
                            <View style={styles.box_item}>
                                <Image
                                    source={images.icon_yte} style={{width: 25, height: 25}}
                                />
                                <Text style={[styles.title]}>{item?.productCategoryName}</Text>
                            </View>
                            {listServicePacket && listServicePacket.map((item_2, index_2) => {
                                if(item_2?.productCategoryId == item?.productCategoryId) {
                                    if(list_ID_Packet_search?.length > 0 && list_ID_Packet_search.includes(item_2?.id)){
                                        if(listProvince_select?.length == 0){
                                            return Item_View_Child(item_2)
                                        }else{
                                            if(listProvince_select.includes(item_2?.provinceId)){
                                                return Item_View_Child(item_2)
                                            }
                                        }
                                    }
                                    if(list_ID_Packet_search?.length == 0){
                                        if(listProvince_select?.length == 0){
                                            return Item_View_Child(item_2)
                                        }else{
                                            if(listProvince_select.includes(item_2?.provinceId)){
                                                return Item_View_Child(item_2) 
                                            }
                                        }
                                    }
                                   
                                }
                            })}
                            
                    </TouchableOpacity>
                )
            }
        }
        
    }

    const topComponent = () => {
        return (
            <View>
                <View style={{flex: 1}}>
                <FlatList
                        refreshing={isRefresh}
                        onRefresh={() => onRefresh()}
                        showsVerticalScrollIndicator={false}
                        showsHorizontalScrollIndicator={false}
                        style={{flex: 1}}
                        renderItem={ItemView}
                        data={listProducCategory}
                        keyExtractor={(item, index) => 'services-screen' + index + String(item)}
                />

                </View>
                {/* <View style={{padding: 16}}>
                    <View style={styles.box_item}>
                        <Image
                            source={images?.icon_yte} style={{width: 25, height: 25}}
                        />
                        <Text style={[styles.title]}>Y tế / Chăm sóc sức khoẻ</Text>
                    </View>
                    <View style={{padding: 12, borderWidth: 1, borderColor: color.nau_nhat2, borderRadius: 12}}>
                        <Image
                            source={images?.banner_yte} style={{width: layout.width - 58, height: (layout.width - 32)*2/3 - 30, borderRadius: 12, marginBottom: 12}}
                        />
                        <Text style={[styles.title_2]}>Đưa / đón người nhà đi bệnh viện</Text>
                        <View  style={[styles.box_item,{justifyContent: 'space-between', marginTop: 12}]}>
                            <Text>3 dịch vụ</Text>
                            <Text>Toàn quốc</Text>
                        </View>
                    </View>
                    
      
                </View> */}
            </View>
        );
    };

    return (
        <>
            {isLoading && <CenterSpinner/>}
            <Screen style={ROOT} preset="fixed">
                <View style={{flex: 1}}>
                    <View style={styles.header}>
                        <TouchableOpacity onPress={() => goToPage('DashboardScreen')}>
                            <Ionicons name='chevron-back-outline' size={30} color={color.black} />
                        </TouchableOpacity>
                        
                        <TextInput 
                            style={styles.text_input}
                            placeholder='Nhập dịch vụ cần tìm kiếm'
                            onChangeText={(value) => setTextSearch(value)}
                            onSubmitEditing={() => searchProducePacket()}
                            value={textSearch}
                        />
                        <TouchableOpacity onPress={() => setModal_Option(true)}>
                            <Ionicons name='options-outline' size={30} color={color.blue} />
                        </TouchableOpacity>
                        
                    </View>
                    <FlatList
                        refreshing={isRefresh}
                        onRefresh={() => onRefresh()}
                        showsVerticalScrollIndicator={false}
                        showsHorizontalScrollIndicator={false}
                        style={{flex: 1}}
                        renderItem={null}
                        data={[]}
                        ListHeaderComponent={topComponent()}
                        keyExtractor={(item, index) => 'services-screen' + index + String(item)}
                    />
                </View>
                <Modal
                    animationType={"slide"}
                    transparent={true}
                    visible={modal_option}
                    onRequestClose={() => { }}
                >
                    <View style={styles.modal_container}>
                        <View style={{}}>
                            <View style={styles.header_apply}>
                                <TouchableOpacity style={{marginRight: 10, marginTop: 10}} onPress={() => setModal_Option(false)}>
                                    <Ionicons name={'close-outline'} color='black' size={30} />
                                </TouchableOpacity>
                            </View>
                            <View style={{ paddingHorizontal: 20 }}>
                                <FlatList
                                    showsVerticalScrollIndicator={false}
                                    showsHorizontalScrollIndicator={false}
                                    style={{ height: layout.height*9/10 - 170}}
                                    renderItem={null}
                                    data={[]}
                                    ListHeaderComponent={() => {
                                        return (
                                            <View style={styles.modal_content}>
                                                <Text style={[styles.title_3,{marginBottom: 16}]}>Chọn nhóm dịch vụ</Text>
                                                {listProducCategory.map((item,index) => {
                                                    return (
                                                        <TouchableOpacity style={{ flexDirection: 'row', marginBottom: 16, alignItems: 'center' }} onPress={() => chooseOption(0,item?.productCategoryId )}>
                                                            <Ionicons name={listProduce_select.includes(item?.productCategoryId) ? 'radio-button-on-outline' : 'radio-button-off-outline'}color={color.xanh_nhat} size={25} />
                                                            <Text style={{ marginLeft: 17, fontSize: 15, color: color.lightGrey }}>{item?.productCategoryName}</Text>
                                                        </TouchableOpacity>
                                                    )
                                                })}
                                                <Text style={[styles.title_3,{marginBottom: 16}]}>Địa điểm</Text>
                                                {listProvince.map((item,index) => {
                                                    return (
                                                        <TouchableOpacity style={{ flexDirection: 'row', marginBottom: 16, alignItems: 'center' }} onPress={() => chooseOption(1,item?.provinceId )}>
                                                            <Ionicons name={listProvince_select.includes(item?.provinceId) ? 'radio-button-on-outline' : 'radio-button-off-outline'}color={color.xanh_nhat} size={25} />
                                                            <Text style={{ marginLeft: 17, fontSize: 15, color: color.lightGrey }}>{item?.provinceName}</Text>
                                                        </TouchableOpacity>
                                                    )
                                                })}
                                            </View>
                                        )
                                    }}
                                    keyExtractor={(item, index) => 'modal-option-' + index + String(item)}
                                />
                            </View>
                        </View>
                        <View style={{position: 'absolute', width: layout.width*8/10, bottom: 0, marginBottom: 50, marginLeft: layout.width/10 }}>
                            <TouchableOpacity style={[{ width: '100%', borderRadius: 25, paddingVertical: 14, backgroundColor: color.blue, alignItems: 'center' }]} onPress={() => setModal_Option(false)}>
                                <Text style={[{fontSize: 18, fontWeight: '700', color: color.white}]}>OK</Text>
                            </TouchableOpacity>
                        </View>
                    </View>
                </Modal>
            </Screen>
        </>
    );
});

const styles = StyleSheet.create({
    header: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        paddingHorizontal: 12,
        paddingVertical: 10,
        borderBottomColor: color.nau_nhat2,
        borderBottomWidth: 1
    },
    text_input: {
        paddingHorizontal: 10,
        paddingVertical: 5,
        backgroundColor: color.nau_nhat,
        width: '75%'
    },
    box: {
        padding: 12
    },
    box_item: {
        flexDirection: 'row', 
        marginBottom: 10
    },
    title: {
        fontSize: 18,
        fontWeight: '700',
        color: color.blue,
        marginLeft: 16
    },
    title_2: {
        fontSize: 15,
        fontWeight: '700',
        color: color.black
    },
    title_3: {
        fontSize: 17,
        fontWeight: '700',
        color: '#182954'
    },
    modal_container: {
        backgroundColor: '#E5E5E5', 
        height: layout.height/10*9,
        minHeight: 650,
        marginTop:  layout.height/10,
        borderRadius: 20
    },
    header_apply: {
        flexDirection: 'row',
        justifyContent: 'flex-end',
        borderTopRightRadius: 20,
        borderTopLeftRadius: 20,
        height: 50,
    },
    modal_content: {
        // height: layout.height,
        // backgroundColor: color.blue
    }
});
