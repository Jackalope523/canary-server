import { Pressable, StyleSheet, Text, View } from 'react-native';
import React from 'react';

import { TextInput } from 'react-native-gesture-handler';

import { globalStyles } from '../../styles/GlobalStyles';
import { Spacing } from '../../styles/SpacingStyles';
import { Colors } from '../../styles/ColorStyles';
import Animated, {
  useAnimatedStyle,
  useSharedValue,
  withTiming,
} from 'react-native-reanimated';

interface OTPInputV1Props {
  code: string;
  setCode: React.Dispatch<React.SetStateAction<string>>;
  maxLength: number;
  setIsPinReady: React.Dispatch<React.SetStateAction<boolean>>;
}

export const OTPInputV1: React.FC<OTPInputV1Props> = ({
  code,
  setCode,
  maxLength,
  setIsPinReady,
}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Input                                      ||
  // ! ||--------------------------------------------------------------------------------||
  const [isFocused, setIsFocused] = React.useState(false);
  const [isInputContainerFocused, setIsInputContainerFocused] =
    React.useState(false);

  const textInputRef = React.useRef(null);
  const codeArray = new Array(maxLength).fill(0);

  const customOnPress = () => {
    setIsInputContainerFocused(true);
    textInputRef?.current?.focus();
  };

  const customOnBlur = () => {
    setIsInputContainerFocused(false);
  };

  React.useEffect(() => {
    setIsPinReady(code.length === maxLength);
    return () => setIsPinReady(false);
  }, [code]);

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                Code input const                                ||
  // ! ||--------------------------------------------------------------------------------||
  const toCodeInput = (_value, index) => {
    const emptyInputChar = ' ';
    const digit = code[index] || emptyInputChar;

    const isCurrentDigit = index === code.length;
    const isLastDigit = index === maxLength - 1;
    const isCodeFull = code.length === maxLength;

    const isDigitFocused = isCurrentDigit || (isLastDigit && isCodeFull);

    // ! ||--------------------------------------------------------------------------------||
    // ! ||                                   Animations                                   ||
    // ! ||--------------------------------------------------------------------------------||
    // TODO fix animations not working, probably because of the overlayed TextInput or Pressable components
    const bw = useSharedValue(0);

    const animatedInputStyle = useAnimatedStyle(() => {
      return {
        borderWidth: bw.value,
      };
    });

    React.useEffect(() => {
      bw.value = withTiming(isInputContainerFocused && isDigitFocused ? 4 : 2, {
        duration: 200,
      });
    }, [isInputContainerFocused, isDigitFocused]);

    return (
      <Animated.View
        style={[
          styles.inputContainer,
          styles.inputContainerEnabled,
          animatedInputStyle,
        ]}
        key={index}>
        <Text
          style={[
            globalStyles.buttonTextTwo,
            globalStyles.textDark,
            { textAlign: 'center' },
          ]}>
          {digit}
        </Text>
      </Animated.View>
    );
  };

  return (
    <View style={styles.OTPInputContainer}>
      <Pressable style={styles.inputGroupContainer} onPress={customOnPress}>
        {codeArray.map(toCodeInput)}
      </Pressable>
      <TextInput
        style={styles.hidden}
        value={code}
        onChangeText={setCode}
        maxLength={maxLength}
        textContentType="oneTimeCode"
        keyboardType="number-pad"
        returnKeyType="done"
      />
    </View>
  );
};

export default OTPInputV1;

// ! ||--------------------------------------------------------------------------------||
// ! ||                                     Styles                                     ||
// ! ||--------------------------------------------------------------------------------||
const styles = StyleSheet.create({
  // TEMP.
  OTPInputContainer: {
    alignItems: 'center',
  },

  inputGroupContainer: {
    flexDirection: 'row',
    columnGap: Spacing.sm,
  },

  // NEW STYLES
  hidden: {
    // borderColor: '#000',
    // borderWidth: 1,
    // padding: 12,
    // marginTop: 15,
    // width: 300,
    // color: '#000',

    position: 'absolute',
    // TODO make this dynamic if possible, without breaking the animations
    // inputContainer width * maxLength + tempInputContainer columnGap * (maxLength - 1)
    width: 58 * 4 + Spacing.sm * (4 - 1),
    // width: '100%',
    height: '100%',
    // opacity: 0,
    backgroundColor: 'rgba(23, 21, 84, 0.5)',
  },

  // ! ||--------------------------------------------------------------------------------||

  // INPUT STYLES
  container: {
    // rowGap: Spacing.xs,
    // width: '100%',
    // flex: 1,
  },

  inputContainer: {
    justifyContent: 'center',
    borderRadius: 8,
    backgroundColor: Colors.sparrowSand,
    // borderWidth: 2,

    height: 58,
    width: 58,

    // possibly a better styling option than the one above

    // minWidth: '16%',
    // padding: 12,
  },

  inputContainerEnabled: {
    borderColor: Colors.sparrowDarkBrown,
  },

  inputContainerDisabled: {
    borderColor: Colors.sand300,
  },

  input: {
    flex: 1,
    paddingRight: 16,
    color: Colors.sparrowDark,
  },
});
