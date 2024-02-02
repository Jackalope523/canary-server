import React from 'react';
import { Pressable, TextInput, Text, StyleSheet } from 'react-native';
import { View } from 'react-native-reanimated/lib/typescript/Animated';
import { Spacing } from '../styles/SpacingStyles';
import { Colors } from '../styles/ColorStyles';

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
    const textInputRef : React.MutableRefObject<TextInput | undefined> = React.useRef();

    const [isFocused, setIsFocused] = React.useState(false);

    const handleOnPress = () => 
    {
        setIsFocused(true);
        textInputRef.current?.focus();
    };

    const handleOnBlur = () => 
    {
        setIsFocused(false);
    };

    const codeDigitsArray = new Array(codeLength).fill(-1);

    const toCodeDigitInput = (_value : number, index : number) => 
    {
        const emptyInputChar = ' ';
        const digit = code[index] || emptyInputChar;

        return (
            <View key = {index}>
                <Text>{digit}</Text>
            </View>
        );
    };

    return (
    <View>
        <Pressable onPress={handleOnPress}>
            {codeDigitsArray.map(toCodeDigitInput)}
        </Pressable>
        <TextInput 
            ref = {textInputRef}
            value = {code}
            maxLength={codeLength}
            keyboardType='number-pad'
            returnKeyType='done'
            textContentType='oneTimeCode'
            onBlur={handleOnBlur}
        />
    </View>
    );
}

const styles = StyleSheet.create({
    // TEMP. STYLES
    container: {
      flexDirection: 'row',
      columnGap: Spacing.sm,
  
      // width: '100%',
      // justifyContent: 'space-between',
    },
  
    inputContainer: {
      borderRadius: 8,
      backgroundColor: Colors.sparrowSand,
      alignItems: 'center',
      justifyContent: 'center',
      // paddingHorizontal: 16,
      // paddingVertical: 4,
      height: 58,
      width: 58,
      borderColor: Colors.sparrowDarkBrown,
      // flexDirection: 'row',
      // justifyContent: 'space-between',
    },
  });

export default OTPInput;