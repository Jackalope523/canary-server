import React, {useState, useEffect} from 'react';
import { 
  TextInput, 
  StyleSheet,  
  View, 
  Pressable,
  Keyboard,  
} from 'react-native';

import Animated, {
  useAnimatedStyle,
  useSharedValue,
  withTiming,
} from 'react-native-reanimated';

import { globalStyles } from '../../styles/GlobalStyles';
import { Spacing } from '../../styles/SpacingStyles';
import { Colors } from '../../styles/ColorStyles';

import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
import { buttonStyles } from '../../styles/ButtonStyles';

const Icon = createIconSetFromFontello(fontelloConfig);

// ! ||--------------------------------------------------------------------------------||
// ! ||                                     Types                                      ||
// ! ||--------------------------------------------------------------------------------||
interface TextInputSmallProps {
  placeholder?: string;
  disabled?: boolean;
  text: string;
  maxLength?: number;
  setText: React.Dispatch<React.SetStateAction<string>>;
  isFocused: boolean;
  setIsFocused: React.Dispatch<React.SetStateAction<boolean>>;
}

const SearchBar: React.FC<TextInputSmallProps> = ({
  disabled = false,
  placeholder,
  maxLength,
  text,
  setText,
  isFocused,
  setIsFocused
}) => {
  const textInput: React.MutableRefObject<TextInput | undefined> = React.useRef();
  const locked: React.MutableRefObject<boolean> = React.useRef(false);

  // Animations
  const bw = useSharedValue(0);

  const animatedInputStyle = useAnimatedStyle(() => {
    return {
      borderWidth: bw.value,
    };
  });

  useEffect(() => {
    bw.value = withTiming(isFocused ? 4 : 2, {
      duration: 200,
    });
  }, [isFocused]);

  const customOnFocus = () => {
    setIsFocused(true);
  };

  const customOnBlur = () => {
    handleSubmit();
    
    if (locked.current) {
      textInput.current?.focus()
    }
    else {
      setIsFocused(false);
    }

    locked.current = false;
  };

  const handleSubmit = () => {

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

  useEffect(() => {
    iconOpacity.value = withTiming(isFocused ? 1 : 0, {
      duration: 200,
    });
  }, [isFocused]);

  const clearButtonPress = () => {
    setText('');
    locked.current = true;
  };

  return (
    <View style={styles.container}>
      <Animated.View
        style={[
          styles.inputContainer,
          styles.inputContainerEnabled,
          animatedInputStyle,
          disabled && styles.inputContainerDisabled,
        ]}>
        <Icon
          name="search-outline"
          style={buttonStyles.buttonIconSmallDark}
        />     
        <TextInput
          ref={textInput}
          color={Colors.sparrowDark}
          placeholder={placeholder}
          placeholderTextColor={Colors.sparrowDark}
          value={text}
          onChangeText={setText}
          onFocus={customOnFocus}
          onBlur={customOnBlur}
          style={[styles.input, globalStyles.bodyTextOne]}
          selectionColor={Colors.sparrowDarkBrown}
          editable={!disabled}
          maxLength={maxLength}
          returnKeyType="done"
          onSubmitEditing={handleSubmit}
          autoCorrect={false}
          autoCapitalize="none"
        />

        {isFocused && (
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
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    rowGap: Spacing.xs,
    flex: 3,
    paddingTop: 10,
    paddingRight: 23,
    paddingLeft: 23
  },

  labelContainer: {
    flexDirection: 'row',
  },

  labelRecommended: {
    paddingLeft: Spacing.xs,
  },

  labelRequired: {
    left: -2,
  },

  description: {
    paddingTop: Spacing.xs,
  },

  inputContainer: {
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

export default SearchBar;