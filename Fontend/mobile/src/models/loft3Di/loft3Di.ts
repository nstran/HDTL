import {Instance, SnapshotOut, types} from 'mobx-state-tree';

const UserInfoModel = types.model('UserInfo').props({
    username: types.maybe(types.string),
    token: types.maybe(types.string),
    surname: types.maybe(types.string),
    forename: types.maybe(types.string),
    id: types.maybe(types.string),
    exp: types.maybe(types.number),
    email: types.maybe(types.string),
    phone: types.maybe(types.string),
    avatar: types.maybe(types.string),
    createdDate: types.maybe(types.Date),
});

export const Loft3DiModel = types
    .model('Loft3Di')
    .props({
        userInfo:types.optional(types.maybe(UserInfoModel),
        {
            username: '',
            token: '',
            surname: '',
            forename: '',
            id: '',
            exp: 0,
            email: '',
            phone: '',
            avatar: '',
            createdDate: 0,
        })
    })
    .views((self) => ({}))
    .actions((self) => ({
        getUserInfo() {
            return self.userInfo;
        },
        setUserInfo(value: any) {
            if (value?.username != null) {
                self.userInfo.username = value?.username;
            }
            if (value?.token != null) {
                self.userInfo.token = value?.token;
            }
            if (value?.surname != null) {
                self.userInfo.surname = value?.surname;
            }
            if (value?.forename != null) {
                self.userInfo.forename = value?.forename;
            }
            if (value?.id != null) {
                self.userInfo.id = value?.id;
            }
            if (value?.exp != null) {
                self.userInfo.exp = value?.exp;
            }
            if (value?.email != null) {
                self.userInfo.email = value?.email;
            }
            if (value?.phone != null) {
                self.userInfo.phone = value?.phone;
            }
            if (value?.avatar != null) {
                self.userInfo.avatar = value?.avatar;
            }
            if (value?.createdDate != null) {
                self.userInfo.createdDate = value?.createdDate;
            }
        },
        getUserInfoByKey(key) {
            return self.userInfo[key];
        },
        logout() {
            self.userInfo.username = '';
            // self.userInfo.password = '';
            self.userInfo.token = '';
            self.userInfo.surname = '';
            self.userInfo.forename = '';
            self.userInfo.id = '';
            self.userInfo.exp = 0;
            self.userInfo.email = '';
            self.userInfo.phone = '';
            self.userInfo.avatar = '';
        },
    }));

type Loft3DiType = Instance<typeof Loft3DiModel>

export interface Moves extends Loft3DiType {
}

type Loft3DiSnapshotType = SnapshotOut<typeof Loft3DiModel>
export interface Loft3DiSnapshot extends Loft3DiSnapshotType {
}
export const createLoft3DiDefaultModel = () => types.optional(Loft3DiModel, {});
