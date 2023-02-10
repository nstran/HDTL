
import React, {createContext, useEffect, useState} from 'react';
// import 'react-native-gesture-handler';
import codePush from 'react-native-code-push';
import {AppNavigator} from './src/navigators';
import {Alert, Platform, StatusBar, StyleSheet, Text, View} from 'react-native';
// import CenterSpinner from './src/components/center-spinner/center-spinner';
import {color} from './src/theme';
import Toast, {BaseToast, ErrorToast} from 'react-native-toast-message';
import {RootStore, RootStoreProvider, setupRootStore} from './src/models';
// import {initFonts} from './src/theme/fonts';
// import {Text} from './src/components';
import * as eva from "@eva-design/eva"
import {ApplicationProvider, IconRegistry} from "@ui-kitten/components"
import {default as mapping} from "./mapping.json" // <-- import mapping
import {EvaIconsPack} from '@ui-kitten/eva-icons';
import CenterSpinner from './src/components/center-spinner/center-spinner';
// import { onAuthStateChanged } from 'firebase/auth';
import messaging from '@react-native-firebase/messaging';

import { LogBox } from 'react-native';

LogBox.ignoreLogs([
  "[react-native-gesture-handler] Seems like you\'re using an old API with gesture components, check out new Gestures system!",
]);
LogBox.ignoreAllLogs();
// AsyncStorage has been extracted from react-native core and will be removed in a future release. It can now be installed and imported from '@react-native-async-storage/async-storage'. Delete this warning.


let codePushOptions = {
    checkFrequency: codePush.CheckFrequency.MANUAL,
    // checkFrequency: codePush.CheckFrequency.ON_APP_START,
    installMode: codePush.InstallMode.IMMEDIATE,
};

// const AuthenticatedUserContext = createContext({});
// const AuthenticatedUserProvider = ({ children }) => {
//     const [user, setUser] = useState(null);
//   return (
//       <AuthenticatedUserContext.Provider value={{ user, setUser }}>
//         {children}
//       </AuthenticatedUserContext.Provider>
//     );
//   };

export const toastConfig = {
    success: (props) => (
        <BaseToast
            style={{borderLeftColor: color.green}}
            {...props}
            text1NumberOfLines={3}
            text2NumberOfLines={3}
        />
    ),
    error: (props) => (
        <ErrorToast
            style={{borderLeftColor: color.danger}}
            {...props}
            text1NumberOfLines={3}
            text2NumberOfLines={3}
        />
    ),
    // custom
    tomatoToast: ({text1, props}) => (
        <View style={{height: 60, width: '100%', backgroundColor: 'tomato'}}>
            <Text>{text1}</Text>
            <Text>{props.uuid}</Text>
        </View>
    )
};

function App() {
    // const [isLoadingComplete, setLoadingComplete] = useState<any>(true);
    const [isCodePushSync, setCodePushSync] = useState<any>(false);
    const [rootStore, setRootStore] = useState<RootStore | undefined>(undefined);

    // Kick off initial async loading actions, like loading fonts and RootStore
    useEffect(() => {
        (async () => {
            // await initFonts();
            setupRootStore().then(setRootStore);
            await codePush.sync({
                installMode: codePush.InstallMode.IMMEDIATE
            });
            setCodePushSync(true)
        })();
    }, []);

    // useEffect(() => {
    //     const unsubscribe = messaging().onMessage(async remoteMessage => {
    //       Alert.alert('A new FCM message arrived!', JSON.stringify(remoteMessage));
    //     });
    //     return unsubscribe;
    //   }, []);

    // if (!rootStore) return null;

    if (!rootStore || !isCodePushSync) {
    // if (!rootStore || !isLoadingComplete) {
        return (
            <View style={{flex: 1, backgroundColor: color.primary}}>
                <CenterSpinner/>
            </View>
        );
    } else {

        return (
            <View style={styles.container}>
                {/* {Platform.OS === 'ios' && <StatusBar barStyle="default"/>} */}
                <RootStoreProvider value={rootStore}>
                    {/* <IconRegistry icons={EvaIconsPack}/> */}
                    <ApplicationProvider {...eva} theme={eva.light} customMapping={mapping}>
                        {/* <AuthenticatedUserProvider> */}
                            <AppNavigator/>
                        {/* </AuthenticatedUserProvider> */}
                    </ApplicationProvider>
                </RootStoreProvider>
                <Toast config={toastConfig}/>
                {/* <Text style={{color: color.white}}>WellCome to app</Text> */}
            </View>
        );
    }
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: color.lightGrey,
    },
});

// export default codePush(codePushOptions)(App);
export default App;
