// import { AsyncStorage } from 'react-native';
import AsyncStorage from '@react-native-async-storage/async-storage';
export const StorageKey = {
    TOKEN: 'TOKEN'
}
export class Storage {
    async clearStore() {
        // await Promise.all(Object.keys(StoreKey).map(key => {
        //     removeItem(key);
        // }))
        AsyncStorage.clear();
    }

    async getToken() {
        var access_token = await String(AsyncStorage.getItem(StorageKey.TOKEN));
        return 'Bearer ' + access_token;
    }

    async getItem(key: string) {
        try {
            const value = await AsyncStorage.getItem(key);
            if (value !== null) {
                // We have data!!
                return JSON.parse(value);
            }
        } catch (error) {
            return null;
            // Error retrieving data
        }
    }
    async removeItem(key: string) {
        await AsyncStorage.removeItem(key)
    }
    async logout() {
        await AsyncStorage.removeItem(StorageKey.TOKEN);

    }
    async setItem(key: string, val: any) {
 
        
        try {
            await AsyncStorage.removeItem(key)
            await AsyncStorage.setItem(key, JSON.stringify(val))
        } catch (error) {

        }

    }
    async setItemNumber({ key, val }: { key: string; val: any; }) {
        try {
            await AsyncStorage.removeItem(key)
            await AsyncStorage.setItem(key, JSON.stringify(val))
        } catch (error) {

        }
    }
}
