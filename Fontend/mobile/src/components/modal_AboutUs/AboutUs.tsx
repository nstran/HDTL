// @ts-ignore
import Ionicons from 'react-native-vector-icons/Ionicons';
import React, { useState } from 'react';
import { View, ViewStyle, TextStyle, Image, TouchableOpacity, StyleSheet, Modal, FlatList,Text, Dimensions} from 'react-native';
import { AboutProps } from './AboutUs.props';
// import {Button} from '../button/button';
// import {Text} from '../text/text';
import { color, spacing } from '../../theme';
// import {translate} from '../../i18n/';
import { DrawerActions, useNavigation } from '@react-navigation/native';
import { images } from '../../images';
import { useStores } from '../../models';
import { observer } from 'mobx-react-lite';
// import codePush from "react-native-code-push";
import { replaceHTTP } from '../../services';

// static styles
const ROOT: ViewStyle = {
    flexDirection: 'row',
    paddingHorizontal: spacing[4],
    alignItems: 'center',
    paddingTop: spacing[3],
    paddingBottom: spacing[4],
    justifyContent: 'space-between',
};
const TITLE: TextStyle = { textAlign: 'center' };
const TITLE_MIDDLE: ViewStyle = { flex: 1, justifyContent: 'center', alignItems: 'center' };
const LEFT: ViewStyle = { width: 32 };
const RIGHT: ViewStyle = { width: 32 };

const CENTER_IMG: any = {
    height: 24,
    resizeMode: 'contain',
};

const layout = Dimensions.get('window');

const LEFT_IMG: any = {
    width: 40,
    resizeMode: 'contain',
};

const RIGHT_IMG: any = {
    width: 20,
    resizeMode: 'contain',
};

const RIGHT_IMG_AVATAR: any = {
    width: 28,
    height: 28,
    borderRadius: 28 / 2,
    backgroundColor: color.lightGrey
};

/**
 * Header that appears on many screens. Will hold navigation buttons and screen title.
 */
export const AboutUs = observer(function AboutUs(props: AboutProps) {
    const {
        model_about_us,
        closeModal,
        // onRightPress,
        // rightIcon,
        // leftIcon,
        // headerText,
        // headerTx,
        style,
        // titleStyle,
    } = props;

    const HandlecloseModal = () => {
        closeModal()
    }

    return (
        <View>
            <Modal
                     animationType={"slide"}
                    transparent={true}
                    visible={model_about_us}
                    onRequestClose={() => {
                    }}
                >
                    <View style={styles.modal_container}>
                        <FlatList
                            contentContainerStyle={{flexGrow: 1}}
                            showsVerticalScrollIndicator={false}
                            showsHorizontalScrollIndicator={false}
                            style={{flex: 1,}}
                            renderItem={null}
                            data={[]}
                            ListEmptyComponent={() => {
                                return (
                                    <View style={{padding:10, alignItems: 'center'}}>
                                        <View style={{width: layout.width - 30}}>
                                            <Image resizeMode='stretch'  source={images.images_about} style={{width: layout.width - 30, height:182}}/>
                                        </View>
                                        <Text style={[{fontWeight: '900',fontSize: 32, marginTop: '5%', color: color.xanh_xam, marginBottom: '5%'}, styles.text]}>About Us</Text>
                                        <Text style={[{lineHeight: 23, fontSize: 15, textAlign:'justify', marginHorizontal: 5, color:'black'},styles.text]}>
                                            <Text style={{color: color.xanh_xam}}>WORKSIFY</Text> is a non-profit project and its mission is to help connecting job seekers and recruiters coming together. Use of Worksify website and mobile application are free. We do expect to have your support by sharing and spreading Worksify to everyone who want either to share the jobs they are hiring or the ones who are looking for job opportunities. Hope we can help everyone !
                                        </Text>
                                        <TouchableOpacity 
                                            style={{paddingHorizontal: 75,height:54, backgroundColor: '#41B1DF', borderRadius: 30, marginTop:'5%', justifyContent:'center', alignItems:'center'}}
                                            onPress={() => HandlecloseModal()}>
                                            <Text style={[{fontWeight:'700', fontSize: 20, color: color.white},styles.text]}>OK</Text>
                                        </TouchableOpacity>
                                    </View>
                                )
                            }}
                            keyExtractor={(item, index) => 'about-us-modal' + index + String(item)}
                        />
                    </View>
                </Modal>
        </View>
    );
});

const styles = StyleSheet.create({
    modal_container: {
        backgroundColor: color.white, 
        width: layout.width - 30,
        height: layout.height/10 * 8,
        minHeight: 550,
        marginTop: layout.height/20,
        marginLeft: 15,
        borderRadius: 20

    },

});
