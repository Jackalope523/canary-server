import React from 'react';
import {
  FlatList,
  Pressable,
  StyleProp,
  StyleSheet,
  Text,
  View,
  ViewStyle,
} from 'react-native';
import { Colors } from '../styles/ColorStyles';
import { globalStyles } from '../styles/GlobalStyles';
import Animated, {
  useAnimatedStyle,
  useSharedValue,
  Easing,
  withTiming,
  SharedValue,
} from 'react-native-reanimated';
import { Spacing } from '../styles/SpacingStyles';

/*

Turns out this dropdown isn't good for accessibility so
investigate and make a better one later, when there's time

*/

// Dropdown content width
import { Dimensions } from 'react-native';
const screenWidth = Dimensions.get('window').width;

interface DropdownProps {
  label: string;
  disabled?: boolean;
  initialValue?: string;
  data: any[];

  onTextChange: React.Dispatch<React.SetStateAction<string>>;

  // Use for aligning the dropdown content with alignSelf
  dropdownContentAlignment?: StyleProp<ViewStyle>;

  // Use for specifying flex externally
  containerFlexValue?: StyleProp<ViewStyle>;
}

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';
import { CustomDimensions } from '../styles/CustomDimensionStyles';

const Icon = createIconSetFromFontello(fontelloConfig);

export const Dropdown: React.FC<DropdownProps> = ({
  label,
  disabled = false,
  initialValue = 'Initial text',
  data,
  dropdownContentAlignment,
  containerFlexValue,
  onTextChange
}) => {
  // Selection
  // const [selected, setSelected] = React.useState(initialValue);
  const [selected, setSelected] = React.useState('Select');
  const [isFocused, setIsFocused] = React.useState(false);
  // const [data, setData] = React.useState(months);

  // Animations
  const bw = useSharedValue(0);

  // TODO rotate chevron icon 180 degrees on press
  // const animatedIconStyle = useAnimatedStyle(() => {
  //   return {
  //     transform: [{ rotate: '180deg' }],
  //   };
  // });

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

    console.log('pressed');
  }, [isFocused]);

  return (
    <View style={[styles.container, containerFlexValue]}>
      <Text
        style={[
          globalStyles.labelTextTwoAsTyped,
          globalStyles.textDark,
          disabled && globalStyles.textDisabled,
        ]}>
        {label}
      </Text>
      <Pressable onPress={() => setIsFocused(!isFocused)}>
        <Animated.View
          style={[
            styles.inputContainer,
            styles.inputContainerEnabled,
            animatedInputStyle,
            disabled && styles.inputContainerDisabled,
          ]}>
          <Text style={[globalStyles.textDark, globalStyles.bodyTextOne]}>
            {selected}
          </Text>
          <Icon
            name="chevron-outline"
            size={24}
            height={24}
            width={24}
            style={styles.icon}
          />

          {/* <TextInput
          type={type}
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
          inputMode={inputMode}
          maxLength={maxLength}
          returnKeyType="done"
          onSubmitEditing={handleSubmit}
          testID="input"
        /> */}
        </Animated.View>
      </Pressable>
      {isFocused ? (
        <Animated.View
          style={[
            styles.dropdownContent,
            dropdownContentAlignment,
            animatedInputStyle,
          ]}>
          <FlatList
            ItemSeparatorComponent={() => (
              <View style={{ height: Spacing.md }} />
            )}
            style={{ flex: 1 }}
            scrollEnabled={false}
            data={data}
            renderItem={({ item }) => {
              return (
                <Pressable
                  style={styles.dropdownItem}
                  onPress={() => {
                    setSelected(item);
                    onTextChange(item);
                    setIsFocused(false);
                  }}>
                  <Text
                    style={[globalStyles.textDark, globalStyles.bodyTextOne]}>
                    {item}
                  </Text>
                </Pressable>
              );
            }}
          />
        </Animated.View>
      ) : null}
    </View>
  );
};

export default Dropdown;

const styles = StyleSheet.create({
  // TODO make the dropdown hover over the screen content
  // TODO FlatList should scroll on default
  dropdownContent: {
    flex: 1,
    width: screenWidth - 48,
    borderRadius: 8,
    marginTop: 16,
    backgroundColor: Colors.sparrowSand,
    // alignSelf: 'center',

    // borderWidth: 4,
    // borderColor: Colors.sparrowDarkBrown,

    paddingVertical: Spacing.md,
  },

  dropdownItem: {
    width: '100%',
    paddingHorizontal: Spacing.md,
    justifyContent: 'center',
    alignContent: 'center',

    // backgroundColor: Colors.fuchsia200,
  },

  container: {
    rowGap: Spacing.xs,
    // flex: 1,
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

    columnGap: Spacing.md,
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
