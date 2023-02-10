import React, {useEffect, useState} from 'react';
import {observer} from 'mobx-react-lite';
import {Animated, Dimensions, FlatList, StyleSheet, View, ViewStyle, Image} from 'react-native';
import {Header, Screen} from '../../components';
import {useIsFocused, useNavigation} from "@react-navigation/native"
// import { useStores } from "../../models"
import {color} from '../../theme';
import CenterSpinner from '../../components/center-spinner/center-spinner';
import Ionicons from 'react-native-vector-icons/Ionicons';
import { images } from '../../images';
import { useStores } from '../models';

const layout = Dimensions.get('window');

const ROOT: ViewStyle = {
    backgroundColor: color.white,
    flex: 1,
};

export const WelcomeScreen = observer(function WelcomeScreen() {
    const navigation = useNavigation();
    const {HDLTModel} = useStores();
    const [isLoading, setLoading] = useState(false);
    const isFocus = useIsFocused()
    const [isRefresh, setRefresh] = useState(false)


    useEffect(() => {
      fetchData();
    }, [isFocus,isRefresh]);
    const fetchData = async () => {
    };

    const goToPage = (page) => {
        navigation.navigate(page);
    };
    const onRefresh = () => {
        setRefresh(true)
    };

    const topComponent = () => {
        return (
            <View>
            </View>
        );
    };

    return (
        <>
            {isLoading && <CenterSpinner/>}
            <Screen style={ROOT} preset="fixed">
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
                        keyExtractor={(item, index) => 'profile-' + index + String(item)}
                    />
                </View>
            </Screen>
        </>
    );
});

const styles = StyleSheet.create({});
