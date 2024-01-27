import {
  NativeSyntheticEvent,
  Pressable,
  StyleSheet,
  Text,
  TextInputKeyPressEventData,
  View,
} from 'react-native';
import React, { ComponentClass, ComponentType } from 'react';

import { TextInput } from 'react-native-gesture-handler';

import { globalStyles } from '../../styles/GlobalStyles';
import { Spacing } from '../../styles/SpacingStyles';
import { Colors } from '../../styles/ColorStyles';
import Animated, {
  useAnimatedStyle,
  useSharedValue,
  withTiming,
} from 'react-native-reanimated';

interface OTPInputV2Props {
  length: number;
  value: Array<string>;
  disabled?: boolean;
  // onChange?: (value: string) => void;
  onChange(value: Array<string>): void;
}

export const OTPInputV2: React.FC<OTPInputV2Props> = ({
  length,
  value,
  disabled,
  onChange,
}) => {
  const inputRef = React.useRef<Array<ComponentType<any> | TextInput>>([]);
  const [isFocused, setIsFocused] = React.useState(false);

  const textInput: React.MutableRefObject<TextInput | undefined> =
    React.useRef();
  let locked: React.MutableRefObject<boolean> = React.useRef(false);

  // const onChangeValue = (text: string, index: number) => {
  //   const newValue = value.map((item, valueIndex) => {
  //     if (valueIndex === index) {
  //       return text;
  //     }
  //     return item;
  //   });

  //   onChange(newValue);
  // };

  const handleChange = (text: string, index: number) => {
    if (text.length !== 0) {
      return inputRef?.current[index + 1]?.focus();
    }
    return inputRef?.current[index - 1]?.focus();
  };
  // if input is clear, skip to next input
  const handleBackspace = (
    event: NativeSyntheticEvent<TextInputKeyPressEventData>,
    index: number,
  ) => {
    const { nativeEvent } = event;

    if (nativeEvent.key === 'Backspace') {
      handleChange('', index);
    }
  };

  const customOnFocus = () => {
    locked.current ? textInput.current?.focus() : setIsFocused(true);
    locked.current = false;
  };

  const customOnBlur = () => {
    locked.current ? textInput.current?.focus() : setIsFocused(false);
    locked.current = false;
  };

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                   Animations                                   ||
  // ! ||--------------------------------------------------------------------------------||
  // TODO FIX: animate focus on selected input
  const bw = useSharedValue(0);

  const animatedInputStyle = useAnimatedStyle(() => {
    return {
      borderWidth: bw.value,
    };
  });

  React.useEffect(() => {
    bw.value = withTiming(isFocused ? 4 : 2, {
      duration: 200,
    });
  }, [isFocused]);

  return (
    <View style={styles.container}>
      {[...new Array(length)].map((item, index) => (
        <Animated.View style={[styles.inputContainer, animatedInputStyle]}>
          <TextInput
            style={[globalStyles.buttonTextTwo, globalStyles.textDark]}
            ref={(ref) => {
              if (ref && !inputRef.current.includes(ref)) {
                inputRef.current = [...inputRef.current, ref];
              }
            }}
            key={index}
            maxLength={1}
            contextMenuHidden
            editable={!disabled}
            keyboardType="number-pad"
            textContentType="oneTimeCode"
            autoComplete="one-time-code"
            testID={`OTPInput-${index}`}
            onChangeText={(text) => handleChange(text, index)}
            onKeyPress={(event) => handleBackspace(event, index)}
            onFocus={customOnFocus}
            onBlur={customOnBlur}
            textAlign="center"
            selectTextOnFocus={false}
            caretHidden
          />
        </Animated.View>
      ))}
    </View>
  );
};

export default OTPInputV2;

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
