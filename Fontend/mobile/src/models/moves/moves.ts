import {Instance, SnapshotOut, types} from 'mobx-state-tree';

/**
 * Model description here for TypeScript hints.
 */

const UserInfoModel = types.model('UserInfo').props({
    username: types.maybe(types.string),
    token: types.maybe(types.string),
    firstApp: types.maybe(types.boolean)
});

const AppInfoModel = types.model('AppInfo').props({
    tabIndex: types.maybe(types.number),
    version: types.maybe(types.number),
});

// const SetAppInfo = types.model('AppInfo').props({
//     tabIndex: types.maybe(types.number),
//     version: types.maybe(types.number),
// });


const LocationModel = types.model('location').props({
    ID: types.maybe(types.number),
    CATEGORY_TYPE_ID: types.maybe(types.number),
    NAME_CATEGORY: types.maybe(types.string),
    CODE_CATEGORY: types.maybe(types.string),
});
const LocationModelSelect = types.model('locationSelect').props({
    ID: types.maybe(types.number),
    CATEGORY_TYPE_ID: types.maybe(types.number),
    NAME_CATEGORY: types.maybe(types.string),
    CODE_CATEGORY: types.maybe(types.string),
});
const CategoryModel = types.model('category').props({
    ID: types.maybe(types.number),
    CATEGORY_TYPE_ID: types.maybe(types.number),
    NAME_CATEGORY: types.maybe(types.string),
    CODE_CATEGORY: types.maybe(types.string),
});

const CategoryModelSelect = types.model('categorySelect').props({
    ID: types.maybe(types.number),
    CATEGORY_TYPE_ID: types.maybe(types.number),
    NAME_CATEGORY: types.maybe(types.string),
    CODE_CATEGORY: types.maybe(types.string),
});

export const WorkSifyModel = types
    .model('WorkSify')
    .props({
        userInfo: types.optional(types.maybe(UserInfoModel), {
            username: '',
            token: '',
            firstApp: true
        }),
        appInfo: types.optional(types.maybe(AppInfoModel), {
            tabIndex: 1,
            version: 1,
        }),
        // setAppInfo: types.optional(types.maybe(SetAppInfo), {
        //     tabIndex: 1,
        //     version: 1,
        // }),
        category: types.optional(types.array(CategoryModel), []),
        location: types.optional(types.array(LocationModel), []),
        categorySelect: types.optional(types.array(CategoryModelSelect), []),
        locationSelect: types.optional(types.array(LocationModelSelect), []),
    })
    .views((self) => ({})) // eslint-disable-line @typescript-eslint/no-unused-vars
    .actions((self: any) => ({
        getCategory() {
            return self.category
        },
        addCategory(value) {
            let data = [...self.category]
            let check = true
            for(let i = 0; i < data?.length; i++){
                if(data[i]?.ID == value?.ID) check = false
            }
            if(check){
                self.category = [...self.category, value];
            }
            
        },
        updateCategory(index, value) {
            self.category[index] = value
        },
        removeCategory(index) {
            self.category.splice(index, 1);
        },
        clearCategory() {
            self.category = []
        },
        getCategorySelect() {
            return self.categorySelect
        },
        addCategorySelect(value) {
            let data = [...self.categorySelect]
            let check = true
            for(let i = 0; i < data?.length; i++){
                if(data[i]?.ID == value?.ID) check = false
            }
            if(check){
                self.categorySelect = [...self.categorySelect, value];
            }
            
        },
        updateCategorySelect(index, value) {
            self.categorySelect[index] = value
        },
        removeCategorySelect(index) {
            self.categorySelect.splice(index, 1);
        },
        clearCategorySelect() {
            self.categorySelect = []
        },
        getLocation() {
            return self.location
        },
        addLocation(value) {
            let data = [...self.location]
            let check = true
            for(let i = 0; i < data?.length; i++){
                if(data[i]?.ID == value?.ID) check = false
            }
            if(check){
                self.location = [...self.location, value];
            }
        },
        updateLocation(index, value) {
            self.location[index] = value
        },
        removeLocation(index) {
            self.location.splice(index, 1);
        },
        clearLocation() {
            self.location = []
        },
        getLocationSelect() {
            return self.locationSelect
        },
        addLocationSelect(value) {
            let data = [...self.locationSelect]
            let check = true
            for(let i = 0; i < data?.length; i++){
                if(data[i]?.ID == value?.ID) check = false
            }
            if(check){
                self.locationSelect = [...self.locationSelect, value];
            }
        },
        updateLocationSelect(index, value) {
            self.locationSelect[index] = value
        },
        removeLocationSelect(index) {
            self.locationSelect.splice(index, 1);
        },
        clearLocationSelect() {
            self.locationSelect = []
        },
        getUserInfo() {
            return self.userInfo;
        },
        getUserInfoByKey(key) {
            return self.userInfo[key];
        },
        // getAppInfo() {
        //     return self.appInfo;
        // },
        setUserInfo(value: any) {
            
            if (value?.username != null) {
                self.userInfo.username = value?.username;
            }
            if (value?.token != null) {
                self.userInfo.token = value?.token;
            }
            if (value?.firstApp != null) {
                self.userInfo.firstApp = value?.firstApp;
            }

        },
        // setAppInfo(value: any) {
        //     if (value?.tabIndex != null) {
        //         self.appInfo.tabIndex = value?.tabIndex;
        //     }
        //     if (value?.version != null) {
        //         self.appInfo.version = value?.version;
        //     }
        // },
        // logout() {
        //     self.userInfo.username = '';
        //     self.activity = []
        // },
    })); // eslint-disable-line @typescript-eslint/no-unused-vars

type WorkSifyType = Instance<typeof WorkSifyModel>

export interface WorkSify extends WorkSifyType {
}

type MovesSnapshotType = SnapshotOut<typeof WorkSifyModel>

export interface MovesSnapshot extends MovesSnapshotType {
}

export const createMovesDefaultModel = () => types.optional(WorkSifyModel, {});
