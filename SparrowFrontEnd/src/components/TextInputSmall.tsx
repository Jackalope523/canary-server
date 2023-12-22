import * as React from 'react';
import { TextInput, StyleSheet, Text, View, Pressable } from 'react-native';

import Animated, {
  useAnimatedStyle,
  useSharedValue,
  Easing,
  withTiming,
  SharedValue,
} from 'react-native-reanimated';

import { inputStyles } from '../styles/InputStyles';
import { globalStyles } from '../styles/GlobalStyles';
import { Spacing } from '../styles/SpacingStyles';
import { Colors } from '../styles/ColorStyles';

// In FIGMA: Design System -> Input fields and selectors -> Text Input (label)

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';

const Icon = createIconSetFromFontello(fontelloConfig);

// ! ||--------------------------------------------------------------------------------||
// ! ||                                     Types                                      ||
// ! ||--------------------------------------------------------------------------------||
interface TextInputSmallProps {
  label?: string;
  description?: string;
  recommended?: boolean;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  value?: string | date;
  onChangeText?: (text: string | date) => void;

  autoComplete?: 'tel' | 'email';
}

export const TextInputSmall: React.FC<TextInputSmallProps> = ({
  label,
  description,
  recommended = false,
  required = false,
  disabled = false,
  placeholder,
  autoComplete,
}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                   Text input                                   ||
  // ! ||--------------------------------------------------------------------------------||
  const [isFocused, setIsFocused] = React.useState(false);

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

  return (
    <View style={styles.wrapper}>
      <View style={styles.labelWrapper}>
        <Text style={[globalStyles.labelTextTwoAsTyped, globalStyles.textDark]}>
          {label}
        </Text>
        {required && (
          <Text
            style={[
              globalStyles.labelTextTwoAsTyped,
              globalStyles.textError,
              styles.labelRequired,
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
            ]}>
            {' '}
            (recommended)
          </Text>
        )}
      </View>

      <Animated.View
        style={[
          styles.inputWrapper,
          styles.inputWrapperEnabled,
          animatedInputStyle,
          disabled && styles.inputWrapperDisabled,
        ]}>
        <TextInput
          value={text}
          onChangeText={setText}
          onFocus={() => setIsFocused(true)}
          onBlur={() => setIsFocused(false)}
          style={styles.input}
          placeholder={placeholder}
          placeholderTextColor={Colors.sand400}
          autoComplete={autoComplete}
          selectionColor={Colors.sparrowDarkBrown}
          editable={!disabled}
        />
        {isFocused && (
          <Pressable onPress={() => setText('')}>
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
          ]}>
          {description}
        </Text>
      )}
    </View>
  );
};

// ! ||--------------------------------------------------------------------------------||
// ! ||                                     Styles                                     ||
// ! ||--------------------------------------------------------------------------------||
const styles = StyleSheet.create({
  wrapper: {
    rowGap: Spacing.xs,
  },

  labelWrapper: {
    flexDirection: 'row',
    alignItems: 'center',
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

  inputWrapper: {
    // borderColor: Colors.sparrowDarkBrown,
    borderRadius: 8,
    backgroundColor: Colors.sparrowSand,
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingHorizontal: 16,
    height: 56,
  },

  inputWrapperEnabled: {
    borderColor: Colors.sparrowDarkBrown,
  },

  inputWrapperDisabled: {
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
});

export default TextInputSmall;
