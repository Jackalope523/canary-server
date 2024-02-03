import React, { useState, useEffect, useRef } from 'react';
import { View, Pressable, TextInput, Text, StyleSheet } from 'react-native';
import { Spacing } from '../styles/SpacingStyles';
import { Colors } from '../styles/ColorStyles';
import Animated, { useAnimatedStyle, useSharedValue, withTiming } from 'react-native-reanimated';

interface OTPInputProps 
{
    codeLength : number;
    code : string;
    setCode : React.Dispatch<React.SetStateAction<string>>;
    setCodeReady : React.Dispatch<React.SetStateAction<boolean>>;  
}

const OTPInput: React.FC<OTPInputProps> = 
(
    {
        codeLength,
        code,
        setCode,
        setCodeReady
    }
) => 
{  
    const codeDigitsArray = new Array(codeLength).fill(-1);
    const textInputRef = useRef();
    const [isFocused, setIsFocused] = useState(false);

    useEffect(() => 
    {
        setCodeReady(code.length === codeLength);
        return () => setCodeReady(false);
    }, [code]);

    const handleOnPress = () => 
    {
        setIsFocused(true);
        textInputRef.current?.focus();
    };

    const handleOnBlur = () => 
    {
        setIsFocused(false);
    };

    const toCodeDigitInput = (_value : number, index : number) => 
    {
        let digit;
        if (parseInt(code[index]) !== -1) digit = code[index];
        else digit = ' ';
        
        const isCurrentDigit = index === code.length;
        const isLastDigit = index === codeLength - 1;
        const isCodeFull = index === codeLength;

        const isDigitFocused = isCurrentDigit || (isLastDigit && isCodeFull);

        // Animation___________________________________________________
        const bw = useSharedValue(2);
        const animatedInputStyle = useAnimatedStyle(() => {
        return {
             borderWidth: bw.value,
        };
        });

        bw.value = withTiming( isFocused && isDigitFocused ? 4 : 2, {
            duration: 200,
          });
        //_____________________________________________________________

        return (
            <Animated.View key = {index} style={[styles.inputContainer, animatedInputStyle]}>
                <Text style = {styles.text}>{digit}</Text>
            </Animated.View>
        );
    };

    return (
    <View>
        <Pressable onPress={handleOnPress} style = {styles.container}>
            {codeDigitsArray.map(toCodeDigitInput)}
        </Pressable>
        <TextInput 
            ref = {textInputRef}
            style = {styles.hiddenTextInput}
            value = {code}
            maxLength={codeLength}
            keyboardType='number-pad'
            returnKeyType='done'
            textContentType='oneTimeCode'
            onBlur={handleOnBlur}
            onChangeText={setCode}
        />
    </View>
    );
}

const styles = StyleSheet.create({
    container: {
      flexDirection: 'row',
      columnGap: Spacing.sm,
    },
  
    inputContainer: {
      borderRadius: 8,
      backgroundColor: Colors.sparrowSand,
      alignItems: 'center',
      justifyContent: 'center',
      height: 58,
      width: 58,
      borderColor: Colors.sparrowDarkBrown,
    },

    hiddenTextInput: {
        height: 0,
        width: 0,
        opacity: 0,
    },

    text: {
        color: Colors.sparrowDarkBrown
    }
  });

export default OTPInput;