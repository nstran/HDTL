// @ts-ignore
import Ionicons from 'react-native-vector-icons/Ionicons';
import React, { useEffect, useState } from 'react';
import { View, ViewStyle, TextStyle, Image, TouchableOpacity, StyleSheet, Text } from 'react-native';
import { HeaderProps } from './header.props';
// import {Button} from '../button/button';
// import {Text} from '../text/text';
import { color, spacing } from '../../theme';
// import {translate} from '../../i18n/';
import { DrawerActions, useIsFocused, useNavigation } from '@react-navigation/native';
import { images } from '../../images';
import { useStores } from '../../models';
import { observer } from 'mobx-react-lite';
// import codePush from "react-native-code-push";
import { replaceHTTP } from '../../services';
import { UnitOfWorkService } from '../../services/api/unitOfWork-service';
import { StorageKey } from '../../services/storage/index';


// static styles
const ROOT: ViewStyle = {
    flexDirection: 'row',
    paddingHorizontal: spacing[4],
    alignItems: 'center',
    paddingTop: spacing[3],
    paddingBottom: spacing[3],
    justifyContent: 'space-between',
};

const _unitOfWork = new UnitOfWorkService()

export const Header = observer(function Header(props: HeaderProps) {
    const isFocused = useIsFocused();
    const {
        onLeftPress,
        onRightPress,
        // rightIcon,
        // leftIcon,
        headerText,
        rightText,
        // headerTx,
        style,
        onPressBtnNotify
        // titleStyle,
    } = props;
    // const header = headerText || (headerTx && translate(headerTx)) || '';
    const navigation = useNavigation();

    useEffect(() => {   
        fetchData()
    }, [isFocused])

    const fetchData = async () => {
    }
    const goToPage = (page) => {
        navigation.navigate(page);
    };
    return (
        <View style={[ROOT, style, styles.wrapper]}>
            <TouchableOpacity onPress={onLeftPress}>
                <Ionicons name='chevron-back-outline' size={30} color={color.black} />
            </TouchableOpacity>
            
            <Text style={styles.header_text}>{headerText}</Text>

            <TouchableOpacity style={{width: '15%'}} onPress={onRightPress}>
                <Text style={styles.right_text} >{rightText ? rightText : ''}</Text>
            </TouchableOpacity>
            
            
        </View>
    );
});

const styles = StyleSheet.create({
    wrapper: {
        backgroundColor: color.white,
        borderBottomColor: color.nau_nhat2,
        borderBottomWidth: 1
    },
    header_text: {
        fontWeight: '700',
        fontSize: 18,
        textAlign:'center',
        color: color.black
    },
    right_text: {
        fontWeight: '700',
        fontSize: 17,
        textAlign:'center',
        color: color.blue,
    }
});
