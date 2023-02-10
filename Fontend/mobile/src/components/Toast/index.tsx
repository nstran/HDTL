import React from 'react';
import {View, Text, StyleProp, ViewStyle} from 'react-native'
import Animated,{FadeInUp,FadeOutUp} from 'react-native-reanimated'

interface IToast {
    overrideStyle?: StyleProp<ViewStyle>;
    bgColor: string;
    toastWidth?: number;
    content?: string;
    overrideTextStyle?: StyleProp<ViewStyle>;
}

export const Toast = (props: IToast) => {
    const {overrideStyle, bgColor, toastWidth, content, overrideTextStyle} = props;
    const defaultStyle = {
        toastStyle:{
            width: toastWidth ? toastWidth : '90%',
            backgroundColor: bgColor,
            position: "absolute",
            padding: 20,
            alignItems: "center",
            top: 50,
            left: 20,
            right: 20,
            borderRadius: 8,
            zIndex: 1000
        },
        textAlignStyle: {
            color: 'white',
            fontSize: 24
        }
    }
    return (
        <Animated.View
            entering={FadeInUp}
            exiting={FadeOutUp}
            style={[defaultStyle.toastStyle,overrideStyle]}
        >
            <Text style={[defaultStyle.textAlignStyle, overrideTextStyle]}>{content}</Text>
        </Animated.View>
    );
  };