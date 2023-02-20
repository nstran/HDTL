import {Instance, SnapshotOut, types} from 'mobx-state-tree';

/**
 * Model description here for TypeScript hints.
 */

const UserInfoModel = types.model('UserInfo').props({
    userId: types.maybe(types.string),
    customerId: types.maybe(types.string),
    phone: types.maybe(types.string),
    token: types.maybe(types.string),
    userFullName: types.maybe(types.string),
    userAvatar: types.maybe(types.string),
    email: types.maybe(types.string),
    address: types.maybe(types.string),
    gender: types.maybe(types.string),
    provinceId: types.maybe(types.string),
    role: types.maybe(types.string)
});

const MessageChatModel = types.model('MessageChatModal').props({
    timestamp : types.maybe(types.number),
    message : types.maybe(types.string),
    senderID : types.maybe(types.string),
    isSeen : types.maybe(types.boolean)
});

const ListUserChatModel = types.model('ListUserChatModel').props({
    timestamp: types.maybe(types.number),
    receiver: types.maybe(types.string),
    nickname_reciver: types.maybe(types.string),
    roomname:  types.maybe(types.string),
    numberMessageNotSeen: types.maybe(types.number),
    message_last: types.maybe(types.string)
})

const CountChatsNotSeen = types.model('CountChatsNotSeen').props({
    roomname : types.maybe(types.string),
});

const AppInfoModel = types.model('AppInfo').props({
    tabIndex: types.maybe(types.number),
    version: types.maybe(types.number),
});


export const HDLTModel = types
    .model('HDLT')
    .props({
        userInfo: types.optional(types.maybe(UserInfoModel), {
            userId: '',
            customerId: '',
            phone: '',
            token: '',
            userFullName: '',
            userAvatar: '',
            email: '',
            address: '',
            gender: '',
            provinceId: '',
            role: ''
        }),
        appInfo: types.optional(types.maybe(AppInfoModel), {
            tabIndex: 1,
            version: 1,
        }),
       
        listMessageChats : types.optional(types.array(MessageChatModel), []),
        listUserChats : types.optional(types.array(ListUserChatModel), []),
        CountChatsNotSeen: types.optional(types.array(CountChatsNotSeen), []),
    })
    .views((self) => ({})) // eslint-disable-line @typescript-eslint/no-unused-vars
    .actions((self: any) => ({
        getListMessage() {
            return self.listMessageChats
        },
        thay_moi_listMessageChat(value) {
            self.listMessageChats = value
        },
        addMessageListChat(value) {
            let listmessage = [...self.listMessageChats]; 
            listmessage.unshift(value)
            self.listMessageChats = [...listmessage];      
        },
        removeListMessageChat() {
            self.listMessageChats = []
        },
        getListUserChats() {
            return self.listUserChats
        },
        thay_moi_listUserChats(value) {
            self.listUserChats = value
        },
        add_User_listUserChats(value){
            self.listUserChats = [...self.listUserChats, value]
        },
        
        getCountChatsNotSeen(){
            return self.CountChatsNotSeen
        },
        thay_moi_CountChatsNotSeen(value){
            self.CountChatsNotSeen = value
        },
        add_CountChatsNotSeen(value){
            self.CountChatsNotSeen = [...self.CountChatsNotSeen, value]
        },
        clear_CountChatsNotSeen(){
            self.CountChatsNotSeen = []
        },

        getUserInfo() {
            return self.userInfo;
        },
        getUserInfoByKey(key) {
            return self.userInfo[key];
        },
        setUserInfo(value: any) {        
            if (value?.userId != null) {
                self.userInfo.userId = value?.userId;
            }
            if (value?.customerId != null) {
                self.userInfo.customerId = value?.customerId;
            }
            if (value?.phone != null) {
                self.userInfo.phone = value?.phone;
            }
            if (value?.token != null) {
                self.userInfo.token = value?.token;
            }
            if (value?.userFullName != null) {
                self.userInfo.userFullName = value?.userFullName;
            }
            if (value?.userAvatar != null) {
                self.userInfo.userAvatar = value?.userAvatar;
            }
            if (value?.email != null) {
                self.userInfo.email = value?.email;
            }
            if (value?.address != null) {
                self.userInfo.address = value?.address;
            }
            if (value?.gender != null) {
                self.userInfo.gender = value?.gender;
            }
            if (value?.provinceId != null) {
                self.userInfo.provinceId = value?.provinceId;
            }
            if (value?.role != null) {
                self.userInfo.role = value?.role;
            }
        },

        logout() {
            self.userInfo.userId = '',
            self.userInfo.phone = '',
            self.userInfo.token = '',
            self.userInfo.userFullName = '',
            self.userInfo.userAvatar = '',
            self.userInfo.email = '',
            self.userInfo.address = '',
            self.userInfo.role = ''
        },
    })); // eslint-disable-line @typescript-eslint/no-unused-vars

type HDLTType = Instance<typeof HDLTModel>

export interface HDLT extends HDLTType {
}

type MovesSnapshotType = SnapshotOut<typeof HDLTModel>

export interface MovesSnapshot extends MovesSnapshotType {
}

export const createMovesDefaultModel = () => types.optional(HDLTModel, {});
