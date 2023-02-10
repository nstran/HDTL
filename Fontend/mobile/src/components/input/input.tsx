// @ts-ignore
import Ionicons from 'react-native-vector-icons/Ionicons';
import * as React from 'react';
import {Animated, Dimensions, StyleSheet, TextInput, TouchableOpacity, View} from 'react-native';
import {color} from '../../theme';

const layout = Dimensions.get('window');

/**
 * For your text displaying needs.
 *
 * This component is a HOC over the built-in React Native one.
 */
export function Input(props: any) {
    // grab the props
    let {
        style,
        type,
        status,
        secureTextEntry,
        onChangeSecure,
        innerRef,
        ...rest
    } = props;

    let keyboardType = type || 'default';

    return (
        <View style={styles.container}>
            <TextInput
                ref={innerRef}
                secureTextEntry={secureTextEntry}
                autoCapitalize='none'
                keyboardType={keyboardType}
                placeholderTextColor={color.black}
                style={[styles.input,
                    status ? (
                        status == 'danger' ?
                            {borderColor: color.danger} :
                            status == 'none' ?
                                {borderWidth: 0} : {}
                    ) : {borderColor: color.white},
                    // secureTextEntry != null ? {paddingRight: 12 + 20 + 10} : {}
                ,style]}
                {...rest}
            />
            {secureTextEntry != null ?
                <TouchableOpacity style={styles.eye} onPress={onChangeSecure}>
                    <Ionicons name={secureTextEntry ? 'eye-outline' : 'eye-off-outline'} color="black" size={20}/>
                </TouchableOpacity>
                : null}
        </View>
    );
}

const styles = StyleSheet.create({
    container: {
        marginBottom: 16,
        borderRadius: 10,
        backgroundColor: color.lighterGrey
    },
    input: {
        width: layout.width * 3 / 4,
        height: 40,
        paddingVertical: 7,
        paddingHorizontal: 16,
        color: color.black,
        textAlign: 'center',
    },
    eye: {
        position: 'absolute',
        right: 12,
        top: 10,
    },
});
