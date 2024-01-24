import * as React from 'react';
import { TextInput, StyleSheet, Text, View, Pressable } from 'react-native';

import Animated, {
  useAnimatedStyle,
  useSharedValue,
  Easing,
  withTiming,
  SharedValue,
} from 'react-native-reanimated';

import { globalStyles } from '../styles/GlobalStyles';
import { Spacing } from '../styles/SpacingStyles';
import { Colors } from '../styles/ColorStyles';

// In FIGMA: Design System -> Input fields and selectors -> Text Input (label)

// TODO clear text isn't clearing text, just unselecting the text input

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';

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
  value?: string | date;
  onChangeText: React.Dispatch<React.SetStateAction<string>>;

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
}

export const TextInputSmall: React.FC<TextInputSmallProps> = ({
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
  onChangeText
}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                   Text input                                   ||
  // ! ||--------------------------------------------------------------------------------||
  const [isFocused, setIsFocused] = React.useState(false);
  const textInput: React.MutableRefObject<TextInput | undefined> =
    React.useRef();
  let locked: React.MutableRefObject<boolean> = React.useRef(false);

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
    locked.current = false;
  }
  
  const customOnBlur = () => 
  {    
    handleSubmit();
    locked.current ?  textInput.current?.focus() : setIsFocused(false);
    locked.current = false;
  }

  const handleSubmit = () => {
    validateInput();
    if (isValid) {
      setError('');
      onChangeText(text);
    }
  };

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                Clear text button                               ||
  // ! ||--------------------------------------------------------------------------------||
  const [text, setText] = React.useState('');

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
  const clearButtonPress = () => {
    setText('');
    onChangeText(text);
    locked.current = true;
  };


  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                   Validation                                   ||
  // ! ||--------------------------------------------------------------------------------||
  const [isValid, setIsValid] = React.useState(false);
  const [error, setError] = React.useState('');

  // TODO remove error message when the user inputs the right value after failure

  const validateInput = () => {
    let errors = '';

    switch (type) {
      // First name
      // TODO update first name regex if necessary
      case InputType.FirstName:
        const firstNameRegex = /^[a-zA-Z'-]+$/;

        if (text.length === 0) {
          setError('First name field cannot be empty.');
        } else if (!firstNameRegex.test(text)) {
          setError('First name can only contain letters.');
        } else {
          Object.keys(errors).length === 0;
          setIsValid(true);
        }
        break;

      // Email
      // TODO update email regex if necessary
      case InputType.Email:
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (text.length === 0) {
          setError('Email field cannot be empty.');
        } else if (!emailRegex.test(text)) {
          setError('Please enter a valid email address.');
        } else {
          Object.keys(errors).length === 0;
          setIsValid(true);
        }
        break;

      // Phone number
      // TODO update phone number regex if necessary
      case InputType.PhoneNumber:
        const phoneNumberRegex = /^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$/;

        if (text.length === 0) {
          setError('Phone number field cannot be empty.');
        } else if (!phoneNumberRegex.test(text)) {
          setError('Please enter a valid phone number.');
        } else {
          Object.keys(errors).length === 0;
          setIsValid(true);
        }
        break;

      // Used for an external date of birth component
      // Day
      // TODO use a more accurate regex range according to selected month, probably already got something on the backend
      case InputType.Day:
        const dayRegex = /^[1-31]{1,2}$/;

        if (!dayRegex.test(text)) {
          setError('Invalid.');
        } else {
          Object.keys(errors).length === 0;
          setIsValid(true);
        }
        break;

      // Year
      case InputType.Year:
        const yearRegex = /^\d{4}$/;
        const currentYear = new Date().getFullYear();
        const maxAge = 150;
        const minAge = 18;
        const minYear = currentYear - maxAge;
        const maxYear = currentYear - minAge;
        maxLength = 4;

        // TODO make minYear and maxYear work
        if (!yearRegex.test(text)) {
          setError('Invalid.');
        } else {
          Object.keys(errors).length === 0;
          setIsValid(true);
        }
        break;

      // Default
      default:
        maxLength = undefined;
        Object.keys(errors).length === 0;
        setIsValid(true);
    }
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
        <TextInput
          ref={textInput}
          type={type}
          value={text}
          onChangeText={(val) => setText(val)}
          onFocus={customOnFocus}
          onBlur={customOnBlur}
          style={[styles.input, globalStyles.bodyTextOne]}
          placeholder={placeholder}
          placeholderTextColor={Colors.sand400}
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
              <Icon
                name="close-outline"
                size={24}
                height={24}
                width={24}
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
          <Icon
            name="error-fill"
            size={24}
            height={24}
            width={24}
            style={styles.iconError}
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
    color: Colors.sparrowDark,
    flex: 0,
  },

  iconError: {
    color: Colors.red400,
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
