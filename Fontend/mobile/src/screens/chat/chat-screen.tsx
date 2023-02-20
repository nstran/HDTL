import React, {
    useCallback,
    useEffect,
    useLayoutEffect,
    useRef,
    useState,
  } from "react";
  import { observer } from "mobx-react-lite";
  import {
    Animated,
    Dimensions,
    FlatList,
    StyleSheet,
    View,
    ViewStyle,
    Text,
    Image,
    TextInput,
    TouchableOpacity,
    ScrollView,
    TouchableNativeFeedback,
  } from "react-native";
  import { Screen } from "../../components";
  import {
    useIsFocused,
    useNavigation,
    useRoute,
  } from "@react-navigation/native";
  // import { useStores } from "../../models"
  import { color } from "../../theme";
  import CenterSpinner from "../../components/center-spinner/center-spinner";
  import LinearGradient from "react-native-linear-gradient";
  import Ionicons from "react-native-vector-icons/Ionicons";
  import { images } from "../../images";
//   import { GiftedChat } from "react-native-gifted-chat";
  import { UnitOfWorkService } from "../../services/api/unitOfWork-service";
  import { StorageKey } from "../../services/storage/index";
  import { 
    firebaseDatabase,
    firebaseDatabaseRef,
    firebaseSet,
    child,
    get,
    onValue,
    query, 
    orderByChild, 
    equalTo, Database, limitToFirst, limitToLast, update, push
  } from '../../config/firebase';
  import { formatDate, formatDate_name } from "../../services";
//   import { startAt } from "firebase/firestore";
//   import firestore from "@react-native-firebase/firestore";
//   import { listenToMessages, createMessage } from "../../components/chat";
  import { TouchableWithoutFeedback } from "react-native-gesture-handler";
  import moment from "moment";
import { useStores } from "../../models";
import { onSnapshot } from "mobx-state-tree";
import { toJS } from "mobx";
  
  const layout = Dimensions.get("window");
  const ROOT: ViewStyle = {
    backgroundColor: color.primary,
    flex: 1,
  };

  const dbRef = firebaseDatabaseRef(firebaseDatabase)
  
  const _unitOfWork = new UnitOfWorkService();
  
  export const ChatScreen = observer(function ChatScreen() {
    const isFocus = useIsFocused()
    const navigation = useNavigation();
    const {HDLTModel} = useStores();
    const [isLoading, setLoading] = useState(false);
    const [isRefresh, setRefresh] = useState(false);
    const { params }: any = useRoute();
    const [textSend, setTextSend] = useState("");
    const [messages, setMessages] = useState([]);
    let yourRef = useRef();
    const [userId, setUserID] = useState('');
    const [friend, setFriend] = useState()
    const [timestamp_user, setTimeStamp_User] = useState(0)
    const [roomname, setRoomName] = useState('')
    const [nickname_user, setNickName_User] = useState('')

    useEffect(() => {

      fetchData();
    }, [isFocus,isRefresh]);

    useEffect(() => {
      onValueMessgaes_User();
      ConverMessageIsSeen()
      // onValueMessgaes_Reciver();
    }, [isFocus]);

    const fetchData = async () => {
      
      setRefresh(false)
      if(params){
        setRoomName(params?.data?.roomname)
      }
      
      let _userId = await HDLTModel.getUserInfoByKey('userId')
      let fullname = await HDLTModel.getUserInfoByKey('userFullName')
      setNickName_User(fullname)
      setUserID(_userId)
      setFriend(params?.data)

      getAllMessage()
    };

    const getAllMessage = async () => {
      let listMessage = []
      await get(query(firebaseDatabaseRef(firebaseDatabase, `chats/${params?.data?.roomname}`), limitToLast(10)))
      .then((snapshot) => {
        let value = snapshot.val()
        let keys = Object.keys(snapshot.val())
        keys.map((item) => {      
          let obj = {
            timestamp : value[item]?.date,
            message : value[item]?.message,
            senderID : value[item]?.senderId,
            isSeen : value[item]?.isSeen
          }
          listMessage.push(obj)
        })
      })
      // listMessage.sort((a,b) => {
      //   return b?.timestamp - a?.timestamp
      // })

      listMessage.reverse()
      setMessages(listMessage)
      await HDLTModel.thay_moi_listMessageChat(listMessage)
    }

    const onValueMessgaes_User = async () => { 
      let _userId = await HDLTModel.getUserInfoByKey('userId')
      return onValue(query(firebaseDatabaseRef(firebaseDatabase, `chats/${params?.data?.roomname}`), orderByChild("senderId"), equalTo(params?.data?.receiver), limitToLast(1)) , async (onSnapshot) => {
        console.log("onValueMessgaes_User");
        
        let _listMessageChat = await HDLTModel.getListMessage()
        let listMessageChat = toJS(_listMessageChat)
        if(onSnapshot.exists()){    
          let value = onSnapshot.val()
          let keys = Object.keys(onSnapshot.val())
          keys.map(async (item) => {   
            let obj = {
              timestamp : value[item]?.date,
              message :  value[item]?.message,
              senderID :  value[item]?.senderId,
              isSeen : value[item]?.isSeen
            }

            setMessages(listMessageChat)
            let check = false
            listMessageChat.map((item) => {
                if(item?.timestamp == obj?.timestamp) check=true
            })
            if(!check){      
              listMessageChat.unshift(obj)
              // listMessageChat.sort((a,b) => {
              //     return b?.timestamp - a?.timestamp
              // })
              setMessages(listMessageChat)
              await HDLTModel.thay_moi_listMessageChat(listMessageChat)
          }  
          })
        }
      })
    }

    const onSendMessage = async () => {
      let newDate = new Date().getTime()
      let nickname_user = await HDLTModel.getUserInfoByKey('userFullName')
      if(textSend){
        //   firebaseSet(firebaseDatabaseRef(firebaseDatabase, `chats/${roomname}/${newDate}`), {
        //   date: newDate ,
        //   message: textSend,
        //   isSeen: false,
        //   // message_type: 'text',
        //   nickname: nickname_user,
        //   roomname: roomname,
        //   senderId: userId,
        //   // receiverId: params?.data?.receiver
        // });

        await push(firebaseDatabaseRef(firebaseDatabase, `chats/${roomname}`), {
          date: newDate ,
          message: textSend,
          isSeen: false,
          // message_type: 'text',
          nickname: nickname_user,
          roomname: roomname,
          senderId: userId,
        })


        let listMessage = [...messages]
        let obj = {
          timestamp : newDate,
          message : textSend,
          senderID : userId,
          isSeen : false
        }
        listMessage.unshift(obj)
        // listMessage.sort((a,b) => {
        //   return b?.timestamp - a?.timestamp
        // })
        await HDLTModel.addMessageListChat(obj)
        setMessages(listMessage)
        setTextSend('')
      }
      
    }

    const ConverMessageIsSeen = async () => {
      let listMessage_ID : any = []
      let _userId = await HDLTModel.getUserInfoByKey('userId')
      await get(query(firebaseDatabaseRef(firebaseDatabase, `chats/${params?.data?.roomname}`), orderByChild("isSeen"), equalTo(false)))
      .then((snapshot) => {
        if(snapshot.exists()){
          let value = snapshot.val()
          let keys = Object.keys(snapshot.val())
          for(let i = 0; i < keys?.length; i++){
            if(value[keys[i]]?.senderId != _userId){
              update(firebaseDatabaseRef(firebaseDatabase, `chats/${params?.data?.roomname}/${keys[i]}`),{isSeen: true})
            }
          }
        }
      }) 
    }

    const ItemView = ({item,index}) => {
        return (
                <View >
                    {messages?.length > 0 ?
                    <View>
                        <TouchableOpacity style={[userId == item?.senderID ? styles.ChatRight : styles.ChatLeft]} onPress={() => {
                        }}>
                            <View style={[userId == item?.senderID ? styles.contentChatRight : styles.contentChat]}>
                                <Text style={[userId == item?.senderID ? styles.textChatRight : styles.textChatLeft]}>{item?.message}</Text>
                            </View>
                        </TouchableOpacity> 
                        <Text style={[userId == item?.senderID ? styles.timeChatRight : styles.timeChatLeft]}>{formatDate_name(item?.timestamp)}</Text>
                    </View>
                    : null }  
                </View>
        )
    }
    const ViewSendInput = () => {
      return (
        <View style={styles.bottomChat}>
          <View style={styles.containerChat}>
            {/* <Image style={{ marginLeft: "5%" }} source={images.iconGhim} /> */}
            <TextInput
              style={styles.textInput}
              placeholder="Message"
              placeholderTextColor={"rgba(0,0,0,0.5)"}
              value={textSend}
              onChangeText={(value) => setTextSend(value)}
              onFocus={ConverMessageIsSeen}
            />
            {/* <Image source={images.iconHappy} /> */}
          </View>
          <TouchableOpacity style={styles.containerEnter} onPress={onSendMessage}>
            <Image source={images.iconEnter} />
          </TouchableOpacity>
        </View>
      );
    };

    const TextImage = (text) => {
      let arr_name = text.split(" ")      
      return arr_name[0].charAt(0) + arr_name[arr_name?.length - 1].charAt(0)

  }
  
    const goToPage = (page: string, params: any) => {
      navigation.navigate(page, params);
    };

    const onRefresh = () => {
      setRefresh(true)
    };
  
    return (
      <>
        {isLoading && <CenterSpinner />}
            <Screen style={ROOT} preset="fixed">
                <View style={styles.container}>
                    <View style={styles.containerHeader}>
                        <TouchableOpacity
                            style={styles.iconBack}
                            onPress={async () => {
                                // setfirstPage(true)
                                // setListShowTime([])
                                // setMessCurrentSeen(0)
                                setMessages([])
                                setTextSend('')
                                // goToPage("RoomsScreen")
                                await HDLTModel.removeListMessageChat()
                                navigation.goBack()
                            } }
                        >
                            <Image source={images.iconBack} />
                        </TouchableOpacity>
                        {params?.data?.url_avatar ? 
                        <Image
                            style={{ marginLeft: '2%', height: 50, width: 50, borderRadius: 25 }}
                            source={{uri: params?.data?.url_avatar}} />
                        : 
                            <View style={{height: 50, width: 50, borderRadius: 25, backgroundColor: color.green, alignItems: 'center', justifyContent: 'center'}}>
                                <Text style={{fontSize: 24, fontWeight: '700', color: color.white}}>{params?.data?.text_name}</Text>
                            </View> 
                        }
                        <View style={styles.nameChat}>
                            <Text style={{ fontSize: 18, color: 'black', fontWeight: '600' }}>{params?.data?.nickname_reciver}</Text>
                        </View>
                    </View>
                    {messages?.length > 0 ?
                    <FlatList
                            inverted={true}
                            refreshing={isRefresh}
                            onRefresh={() => onRefresh()}
                            showsVerticalScrollIndicator={false}
                            showsHorizontalScrollIndicator={false}
                            style={{flex: 1}}
                            renderItem={ItemView}
                            data={messages}
                            keyExtractor={(item, index) => 'chat-obo-' + index + String(item)}
                        />
                    : <View style={{flex: 1}}></View> }
                    {ViewSendInput()}
                </View>
            </Screen>
      </>
    );
  });
  
  const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#ffffff',
    },
    containerHeader: {
        flexDirection: 'row',
        height: 70,
        backgroundColor: '#f9f9f9',
        justifyContent: 'flex-start',
        alignItems: 'center',
        borderBottomWidth: 1,
        borderBottomColor: '#dedcdc',
    },
    textHeader: {
        fontSize: 20,
        color: 'black',
        fontWeight: '500'
    },
    iconLogo: {
        height: 50,
        width: 50,
        borderRadius: 20,
        backgroundColor: '#efeff0',
        justifyContent: 'center',
        alignItems: 'center',
        marginRight: '2%',
        marginLeft: '5%',
    },
    iconBack: {
        width: 50,
        height: 50,
        justifyContent: 'center',
        alignItems: 'center'
    },
    nameChat: {
        flexDirection: 'column',
        marginLeft: '2%',
    },
    containerBoxChat: {
        width: '90%',
        marginTop: '2%',
        marginLeft: '5%',
        marginBottom: 5,
    },
    // left
    ChatLeft: {
        width: '70%',
        flexDirection: 'row',
        justifyContent: 'flex-start',
        marginLeft: 10
    },
    contentChat: {
        backgroundColor: '#E7E7E7',
        marginTop: '4%',
        borderRadius: 20,
        padding: 15,
    },
    textChatLeft: {
        fontSize: 15,
        color: '#303030',
        fontWeight: '400',
    },
    timeChatLeft: {
        marginLeft: 10,
        color: '#707070',
        fontSize: 12,
        fontWeight: '400'
    },
    // right
    ChatRight: {
        width: '50%',
        justifyContent: 'flex-end',
        alignSelf: 'flex-end',
        alignItems: 'flex-end',
        flexDirection: 'row',
        marginRight: 10
    },
    contentChatRight: {
        backgroundColor: '#41B1DF',
        marginTop: '4%',
        borderRadius: 20,
        padding: 15,
    },
    textChatRight: {
        fontSize: 15,
        color: '#303030',
        fontWeight: '400',
    },
    timeChatRight: {
        alignSelf: 'flex-end',
        marginRight: '5%',
        color: '#707070',
        fontSize: 12,
        fontWeight: '400'
    },
    bottomChat: {
        height: 80,
        width: '100%',
        backgroundColor: '#F8F8F8',
        justifyContent: 'space-between',
        alignItems: 'center',
        flexDirection: 'row',
        borderTopWidth: 1,
        borderTopColor: '#dedcdc',
    },
    containerChat: {
        backgroundColor: '#E6E6E6',
        width: '76%',
        height: '60%',
        borderRadius: 10,
        alignItems: 'center',
        flexDirection: 'row',
        justifyContent: 'flex-start',
        marginLeft: '4%',
    },
    textInput: {
        width: '100%',
        color: 'black',
        fontSize: 15,
        marginLeft: '3%',
        marginRight: '2%',
    },
    containerEnter: {
        backgroundColor: '#E6E6E6',
        width: 50,
        height: '60%',
        borderRadius: 10,
        justifyContent: 'center',
        alignItems: 'center',
        marginRight: '4%',
    }
});
  