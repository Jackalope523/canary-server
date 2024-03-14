import * as React from 'react';
import { TextInput, StyleSheet, Text, View, Pressable } from 'react-native';

import Animated, {
  useAnimatedStyle,
  useSharedValue,
  withTiming,
} from 'react-native-reanimated';

import { globalStyles } from '../styles/GlobalStyles';
import { Spacing } from '../styles/SpacingStyles';
import { Colors } from '../styles/ColorStyles';

// In FIGMA: Design System -> Input fields and selectors -> Text Input (label)

// TODO clear text isn't clearing text, just unselecting the text input

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';

import TextInputMask from 'react-native-text-input-mask';

const Icon = createIconSetFromFontello(fontelloConfig);

// ! ||--------------------------------------------------------------------------------||
// ! ||                                     Types                                      ||
// ! ||--------------------------------------------------------------------------------||
interface TextInputSmallProps {
  type?: InputType;

  label?: string;
  description?: string;
  recommended?: boolean;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  clearButton?: boolean;

  text: string;
  setText: React.Dispatch<React.SetStateAction<string>>;

  valid: boolean;
  setValid: React.Dispatch<React.SetStateAction<boolean>>;

  autoComplete?: 'tel' | 'email';
  inputMode?:
    | 'none'
    | 'text'
    | 'decimal'
    | 'numeric'
    | 'tel'
    | 'search'
    | 'email'
    | 'url';
  maxLength?: number;
  mask?: string
}

const TextInputSmall: React.FC<TextInputSmallProps> = ({
  type = null,
  label,
  description,
  recommended = false,
  required = false,
  disabled = false,
  clearButton = true,
  placeholder,
  autoComplete,
  inputMode,
  maxLength,
  text,
  setText,
  valid,
  setValid,
  mask = ''

}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                   Text input                                   ||
  // ! ||--------------------------------------------------------------------------------||
  const [isFocused, setIsFocused] = React.useState(false);
  const textInput: React.MutableRefObject<TextInput | undefined> =
    React.useRef();
  const locked: React.MutableRefObject<boolean> = React.useRef(false);

  // Animations
  const bw = useSharedValue(0);

  const animatedInputStyle = useAnimatedStyle(() => {
    return {
      borderWidth: bw.value,
    };
  });

  React.useEffect(() => {
    bw.value = withTiming(isFocused ? 4 : 2, {
      // TODO create an AnimationStyles file (maybe?) - to organize animation values
      duration: 200,
    });
  }, [isFocused]);

  const customOnFocus = () => {
    setIsFocused(true);
  };

  const customOnBlur = () => {
    handleSubmit();
    locked.current ? textInput.current?.focus() : setIsFocused(false);
    locked.current = false;
  };

  const handleSubmit = () => {
    validateInput();
  };

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                Clear text button                               ||
  // ! ||--------------------------------------------------------------------------------||

  // Animations
  const iconOpacity = useSharedValue(0);

  const animatedIconStyle = useAnimatedStyle(() => {
    return {
      opacity: iconOpacity.value,
    };
  });

  React.useEffect(() => {
    iconOpacity.value = withTiming(isFocused ? 1 : 0, {
      duration: 200,
    });
  }, [isFocused]);

  // On press
  // TODO stop keyboard from abruptly closing and opening when pressing the clear button
  const clearButtonPress = () => {
    setText('');
    locked.current = true;
  };

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                   Validation                                   ||
  // ! ||--------------------------------------------------------------------------------||
  const [error, setError] = React.useState('');

  const validateInput = () => {

    let currentValidity = false;
    let currentError = '';

    switch (type) {
      case InputType.FirstName:
        const firstNameRegex = /^[a-zA-Z'-]+$/;

        if (text.length === 0) {
          currentError = 'First name field cannot be empty.';
        } else if (!firstNameRegex.test(text)) {
          currentError = 'First name can only contain letters.';
        } else {
          currentValidity = true;
        }
        break;

      case InputType.Email:
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (text.length === 0) {
          currentError = 'Email field cannot be empty.';
        } else if (!emailRegex.test(text)) {
          currentError = 'Please enter a valid email address.';
        } else {
          currentValidity = true;
        }
        break;

      case InputType.PhoneNumber:
        const phoneNumberRegex = /^\+\d{1,2}\s?\(\d{3}\)\s?\d{3}-\d{4}$/;

        if (text.length === 0) {
          currentError = 'Phone number field cannot be empty.';
        } else if (!phoneNumberRegex.test(text)) {
          currentError = 'Please enter a valid phone number.';
        } else {
          currentValidity = true;
        }
        break;

      case InputType.Day:
        const dayRegex = /^[1-31]{1,2}$/;

        if (!dayRegex.test(text)) {
          currentError = 'Invalid.';
        } else {
          currentValidity = true;
        }
        break;

      case InputType.Year:
        const yearRegex = /^\d{4}$/;

        const currentYear = new Date().getFullYear();
        const maxAge = 100;
        const minAge = 18;
        const minYear = currentYear - maxAge;
        const maxYear = currentYear - minAge;

        if (!yearRegex.test(text) || (parseInt(text) < minYear || parseInt(text) > maxYear)) {
          currentError = 'Invalid.';
        }
        else {
          currentValidity = true;
        }
        break;

      // Default
      default:
        maxLength = undefined;
        currentValidity = false;
    }

    setValid(currentValidity);
    setError(currentError);
  };

  return (
    <View style={styles.container}>
      <View style={styles.labelContainer}>
        <Text
          style={[
            globalStyles.labelTextTwoAsTyped,
            globalStyles.textDark,
            disabled && globalStyles.textDisabled,
          ]}>
          {label}
        </Text>
        {required && (
          <Text
            style={[
              globalStyles.labelTextTwoAsTyped,
              globalStyles.textError,
              styles.labelRequired,
              disabled && globalStyles.textDisabled,
            ]}>
            {' '}
            *
          </Text>
        )}
        {recommended && (
          <Text
            style={[
              globalStyles.labelTextTwoItalic,
              globalStyles.textDark,
              styles.labelRecommended,
              disabled && globalStyles.textDisabled,
            ]}>
            {' '}
            (recommended)
          </Text>
        )}
      </View>

      <Animated.View
        style={[
          styles.inputContainer,
          styles.inputContainerEnabled,
          animatedInputStyle,
          disabled && styles.inputContainerDisabled,
        ]}>
       
        <TextInputMask
          ref={textInput}
          mask={mask}
          placeholder={mask}
          keyboardType="numeric"
          placeholderTextColor="grey"
          type={type}
          value={text}
          onChangeText={(formatted, extracted) => setText(formatted)}
          onFocus={customOnFocus}
          onBlur={customOnBlur}
          style={[styles.input, globalStyles.bodyTextOne]}
          autoComplete={autoComplete}
          selectionColor={Colors.sparrowDarkBrown}
          editable={!disabled}
          inputMode={inputMode}
          maxLength={maxLength}
          returnKeyType="done"
          onSubmitEditing={handleSubmit}
          testID="input"
        />

        {clearButton && isFocused && (
          <Pressable onPress={clearButtonPress}>
            <Animated.View style={animatedIconStyle}>
              <CloseOutline
                height={24}
                width={24}
                fill={Colors.sparrowDarkBrown}
                style={styles.icon}
              />
            </Animated.View>
          </Pressable>
        )}
      </Animated.View>

      {/* TODO description can be made hidden on default and only shown when TextInput is focused, just an idea I'm noting down here for now */}
      {description && (
        <Text
          style={[
            globalStyles.bodyTextTwo,
            globalStyles.textDark,
            styles.description,
          ]}
          testID="error">
          {description}
        </Text>
      )}

      {/* TODO used for Jest testing - remove later or make it work with TextInput's onSubmitEditing  */}
      {/* <Pressable onPress={handleSubmit} testID="button">
        <Text>Submit</Text>
      </Pressable> */}

      {error ? (
        <View style={styles.errorContainer}>
          <ErrorFill
            width={24}
            height={24}
            fill={Colors.sparrowDarkBrown}
            style={styles.icon}
          />
          <Text style={[globalStyles.bodyTextTwo, globalStyles.textError]}>
            {error}
          </Text>
        </View>
      ) : null}
    </View>
  );
};

// ! ||--------------------------------------------------------------------------------||
// ! ||                                     Styles                                     ||
// ! ||--------------------------------------------------------------------------------||
const styles = StyleSheet.create({
  container: {
    rowGap: Spacing.xs,
    // width: '100%',
    // flex: 1,
  },

  labelContainer: {
    flexDirection: 'row',
    // alignItems: 'center',
  },

  labelRecommended: {
    paddingLeft: Spacing.xs,
  },

  labelRequired: {
    left: -2,
  },

  description: {
    paddingTop: Spacing.xs,
    // textAlign: 'center',
  },

  inputContainer: {
    // borderColor: Colors.sparrowDarkBrown,
    borderRadius: 8,
    backgroundColor: Colors.sparrowSand,
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',

    // TODO FIX: possibly what's messing up the input's width - BUG: text getting cut off
    paddingHorizontal: 16,
    height: 56,
    width: '100%',
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

  icon: {
    flex: 0,
  },

  errorContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingTop: Spacing.xs,
    columnGap: Spacing.sm,
  },
});

export enum InputType {
  FirstName,
  Email,
  PhoneNumber,
  Day,
  Year,
}

export default TextInputSmall;
