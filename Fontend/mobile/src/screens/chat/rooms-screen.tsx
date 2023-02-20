import React, {useEffect, useState} from 'react';
import {observer} from 'mobx-react-lite';
import {Animated, Dimensions, FlatList, StyleSheet, View, ViewStyle, Image, TouchableOpacity, Text, ImageBackground, TextInput} from 'react-native';
import {Header, Screen} from '../../components';
import {useIsFocused, useNavigation} from "@react-navigation/native"
// import { useStores } from "../../models"
import {color} from '../../theme';
import CenterSpinner from '../../components/center-spinner/center-spinner';
import Ionicons from 'react-native-vector-icons/Ionicons';
import { images } from '../../images';
import { 
  firebaseDatabase,
  firebaseDatabaseRef,
  firebaseSet,
  child,
  get,
  onValue,
  query, 
  orderByChild, 
  equalTo, Database,
  limitToFirst,
  limitToLast
} from '../../config/firebase';
import { useStores } from '../../models';
import { formatDate } from '../../services';
import { toJS } from 'mobx';

const layout = Dimensions.get('window');

const ROOT: ViewStyle = {
    backgroundColor: color.primary,
    flex: 1,
};

interface UserChat {
  user_id: string,
  nickname: string
}

export const RoomsScreen = observer(function RoomsScreen() {
    const navigation = useNavigation();
    const {HDLTModel} = useStores();
    const [isLoading, setLoading] = useState(false);
    const isFocus = useIsFocused()
    const [isRefresh, setRefresh] = useState(false)
    const [userID, setUserID] = useState('')
    const [listUserChat, setListUserChat] = useState([])


    useEffect(() => {
      fetchData();
    }, [isFocus,isRefresh]);

    // useEffect(() => {
    //   onLoadData();
    // }, []);

    const onLoadData = async (listAllUsers) => { 
      let _userId = await HDLTModel.getUserInfoByKey('userId')
      let listRooms = []
      listAllUsers?.map((item,index) => {
        listRooms.push(item?.roomname)
      })
      for(let i = 0; i < listRooms?.length ; i++){
        onValue(query(firebaseDatabaseRef(firebaseDatabase, `chats/${listRooms[i]}`), limitToLast(1)) , async (onSnapshot) => {
          if(onSnapshot.exists()){
            let _listUsers_current = await HDLTModel.getListUserChats()
            let listUsers_current = toJS(_listUsers_current)

            let value = onSnapshot.val()
            let keys = Object.keys(onSnapshot.val())
            let listUser = [...listUsers_current]
        
            for(let j = 0; j < listUser?.length; j++){
              if(listUser[j]?.roomname == value[keys[0]]?.roomname){
                if(value[keys[0]]?.date != listUser[j].timestamp){
                  listUsers_current[j].timestamp = value[keys[0]]?.date
                  listUsers_current[j].message_last = value[keys[0]]?.message
                  listUsers_current[j].numberMessageNotSeen += 1
                }
              }
            }
            listUsers_current.sort((a,b) => {
              return b?.timestamp - a?.timestamp
            })
            setListUserChat(listUsers_current)
            await HDLTModel.thay_moi_listUserChats(listUsers_current)
          }
        })
      }
      // return onValue(query(firebaseDatabaseRef(firebaseDatabase, `chats`), orderByChild("receiverId"), equalTo(_userId), limitToLast(1)) , async (onSnapshot) => {
      //   if(onSnapshot.exists()){
      //     setRefresh(true)
      //   }
      // })
    }

    const fetchData = async () => {
      setLoading(true)
      setRefresh(false)
      let _userId = await HDLTModel.getUserInfoByKey('userId')
      setUserID(_userId)
      let listAllUsers = await getListUserChat()
      getMessagelast(listAllUsers)
      onLoadData(listAllUsers)
      setLoading(false)
    };

    const getListUserChat = async () => {
      let _userId = await HDLTModel.getUserInfoByKey('userId')
      let allUser = []
      await get(query(firebaseDatabaseRef(firebaseDatabase, "rooms"), orderByChild("userCreate"), equalTo(_userId)))
      .then((snapshot) => {
        if(snapshot.exists()){
          let value = snapshot.val()
          let keys = Object.keys(snapshot.val())
          keys.map((item,index) => {  
            let obj = {
              timestamp: 0,
              receiver: value[item]?.receiver,
              nickname_reciver: value[item]?.receiverName,
              roomname:  value[item]?.roomname,
              numberMessageNotSeen: 0,
              message_last: ''
            }
            allUser.push(obj)
          });
        }
      });

      await get(query(firebaseDatabaseRef(firebaseDatabase, "rooms"), orderByChild("receiver"), equalTo(_userId)))
      .then((snapshot) => {
        if(snapshot.exists()){
          let value = snapshot.val()
          let keys = Object.keys(snapshot.val())
          keys.map((item,index) => {  
            let obj = {
              timestamp: 0,
              receiver: value[item]?.userCreate,
              nickname_reciver: value[item]?.userCreateName,
              roomname:  value[item]?.roomname,
              numberMessageNotSeen: 0,
              message_last: ''
            }
            allUser.push(obj)
          });
        }
      });
      return allUser
    }

    const getMessagelast = async (dataUsers) => {
      setListUserChat(dataUsers)
      let _userId = await HDLTModel.getUserInfoByKey('userId')
      
      let allDataUser = [...dataUsers]
      for(let i = 0; i < dataUsers?.length; i++){
  
        let numberMessageNotSeen = 0
        
        await get(query(firebaseDatabaseRef(firebaseDatabase, `chats/${dataUsers[i]?.roomname}`),orderByChild('isSeen'), equalTo(false)))
        .then((snapshot) => {
          if(snapshot.exists()){
            let value = snapshot.val()
            let keys = Object.keys(snapshot.val())
            keys.map((item) => {      
              if(value[item]?.senderId != _userId){
                numberMessageNotSeen++
              }
            })
            allDataUser[i].numberMessageNotSeen = numberMessageNotSeen
          }  
        })

        await get(query(firebaseDatabaseRef(firebaseDatabase, `chats/${dataUsers[i]?.roomname}`), limitToLast(1)))
        .then((snapshot) => {
          if(snapshot.exists()){
            let value = snapshot.val()
            let keys = Object.keys(snapshot.val())
            allDataUser[i].message_last = value[keys[keys?.length - 1]]?.message
            allDataUser[i].timestamp = value[keys[keys?.length - 1]]?.date
          }  
        })
      }

      allDataUser.sort((a,b) => {
        return b?.timestamp - a?.timestamp
      })
      await HDLTModel.thay_moi_listUserChats(allDataUser)
      setListUserChat(allDataUser)
    
    }

    const goToPage = (page) => {
        navigation.navigate(page);
    };
    const onRefresh = () => {
        setRefresh(true)
    };

    const renderAvatar = (item) => {
        return (
          <ImageBackground
            // source={{ uri: item?.imageUrl  ? item.imageUrl : images.image_avatar }}
            source={images.image_avatar}
            style={[
              styles.roomImage
            ]}
          >
            {!item.imageUrl ? (
              <Text style={styles.userInitial}>
                {/* {name ? name.charAt(0).toUpperCase() : ''} */}
              </Text>
            ) : null}
          </ImageBackground>
        )
      }

    const renderItem = ({ item }) => {
      
      return (
        <TouchableOpacity
          onPress={() => navigation.navigate('ChatScreen', {data: item})}
        >
          <View style={styles.roomContainer}>
            <Image source={images.image_avatar} style={styles.roomImage} />
            <Text>{item.nickname ?? 'vinh'}</Text>
          </View>
        </TouchableOpacity>
    )
    }

    const ViewItem = ({item, intex}) => { 
      let url_avatar = ''
      let arr_name = item?.nickname_reciver.split(" ")      
      let text_name = arr_name[0]?.charAt(0) + arr_name[arr_name?.length - 1]?.charAt(0)
      return (
          <TouchableOpacity
              onPress={() => {
                  // setListUser([])
                  navigation.navigate("ChatScreen", {
                      data: {...item, url_avatar, text_name},
                  })
                  // goToPage('ChatObo')
              }
              }
              style={styles.containerChat}>
              <View style={{width:'15%'}}>
                  {url_avatar ? 
                      <Image
                          style={{ marginLeft: '2%', height: 50, width: 50, borderRadius: 25 }}
                          source={{uri: url_avatar}} />
                  : 
                      <View style={{height: 50, width: 50, borderRadius: 25, backgroundColor: color.green, alignItems: 'center', justifyContent: 'center'}}>
                          <Text style={{fontSize: 24, fontWeight: '700', color: color.white}}>{text_name}</Text>
                      </View> 
                  }
              </View>
              
              <View style={styles.Chat}>
                  <Text style={styles.chatName}>{item?.nickname_reciver}</Text>
                  <Text numberOfLines={1} style={styles.chatContent}>{item?.message_last}</Text>
                  <View style={styles.containTime}>
                        <Text style={[styles.textTimes,{color: color.black}]}>{formatDate(item?.timestamp, "dd/MM/YY hh:mm")}</Text>
                        {item?.numberMessageNotSeen > 0 ? 
                            <View style={styles.containerTimes}>
                                <Text style={styles.textTimes}>{item?.numberMessageNotSeen}</Text>
                            </View>
                        : null }
                    </View>
              </View>
              {/* <View style={styles.containTime}>
                   <Text style={[styles.textTimes,{color: color.black}]}>{formatDate(item?.timestamp, "dd-MM")}</Text>
                  {item?.numberMessageNotSeen > 0 ? 
                      <View style={styles.containerTimes}>
                          <Text style={styles.textTimes}>{item?.numberMessageNotSeen}</Text>
                      </View>
                  : null }
              </View> */}
          </TouchableOpacity>
      )
    } 

    const topComponent = () => {
        
      return (
          // <View style={styles.containerSearch}>
          //     {/* <Image
          //         style={{ marginRight: '2%' }}
          //         source={images.Icon_search} />
          //     <TextInput
          //         style={{}}
          //         placeholder='Search'
          //         // placeholderTextColor={'smokewhite'}
          //     /> */}
          // </View>
          <></>

      );
    };

    return (
      <>
      {isLoading && <CenterSpinner />}
      <Screen style={ROOT} preset="fixed">
          <View style={styles.containerHeader}>
              <TouchableOpacity
                  style={styles.iconBack}
                  onPress={() => navigation.goBack()}
              >
                  <Image source={images.iconBack} />
              </TouchableOpacity>
              <Text style={styles.textHeader}>Chat</Text>
              <TouchableOpacity
                  // onPress={() => goToPage('DashboardScreen')}
                  style={styles.iconHome}
              >
                  {/* <Image source={images.iconHome} /> */}
              </TouchableOpacity>
          </View>
          <View style={{flex: 1, backgroundColor: color.white}}>
            <FlatList
              // refreshing={isRefresh}
              // onRefresh={() => onRefresh()}
              showsVerticalScrollIndicator={false}
              showsHorizontalScrollIndicator={false}
              style={{flex: 1}}
              renderItem={ViewItem}
              data={listUserChat}
              ListHeaderComponent={topComponent()}
              // ListFooterComponent={FooterComponent()}
              keyExtractor={(item, index) => 'chat-screen' + index + String(item)}
            />
          </View>
         
      </Screen>
  </>
    );
});

const styles = StyleSheet.create({
  container: {
    backgroundColor: '#ffffff',
    // padding: 10,
  },
  containerHeader: {
      flexDirection: 'row',
      height: '10%',
      backgroundColor: '#f9f9f9',
      justifyContent: 'space-between',
      alignItems: 'center',
      borderBottomWidth: 1,
      borderBottomColor: '#dedcdc',
  },
  textHeader: {
      fontSize: 20,
      color: 'black',
      fontWeight: '500'
  },
  iconHome: {
      height: 35,
      width: 35,
      borderRadius: 20,
      // backgroundColor: '#efeff0',
      justifyContent: 'center',
      alignItems: 'center',
      marginRight: '5%',
  },
  iconBack: {
      width: 50,
      height: 50,
      justifyContent: 'center',
      alignItems: 'center'
  },
  containerSearch: {
      backgroundColor: '#efeff0',
      borderRadius: 22,
      alignItems: 'center',
      height: 44,
      width: '90%',
      marginLeft: '5%',
      marginTop: '3%',
      color: 'black',
      flexDirection: 'row',
      paddingHorizontal: 10,
      fontSize: 17,
      fontWeight: '400'
  },
  containerBoxChat: {
      // flexDirection:'column',
      width: '90%',
      marginTop: '2%',
      marginLeft: '5%',
      marginBottom: 5,
  },
  containerChat: {
      flexDirection: 'row',
      height: 66,
      width: '100%',
      alignItems: 'center',
      borderRadius: 10,
      // borderWidth:1,
      marginBottom: 5,
      borderBottomWidth: 1,
      borderBottomColor: '#efeff0',
      marginTop: '2%',
      paddingHorizontal: 10,
  },
  Chat: {
      justifyContent: 'flex-start',
      flexDirection: 'column',
      width: '80%',
      marginLeft: 7
  },
  chatName: {
      fontSize: 16,
      fontWeight: '600',
      color: 'black'
  },
  chatContent: {
      fontSize: 14,
      marginTop: 10,
      color: color.text_naunhat,
      width: '90%'
  },
  containTime: {
      position: 'absolute',
      right: 0,
      alignItems: 'flex-end',
  },
  containerTimes: {
      height: 18,
      width: 18,
      borderRadius: 10,
      backgroundColor: 'red',
      alignItems: 'center',
      marginTop: 10
  },
  textTimes: {
      fontSize: 13,
      color: color.black,
  },
  bottom: {
      marginTop: 16,
      flexDirection: 'column',
      height: 80,
      justifyContent: 'space-between',
      marginLeft: '5%',
  },
  textBottom: {
      color: '#797979',
      fontSize: 14,
  }
});
