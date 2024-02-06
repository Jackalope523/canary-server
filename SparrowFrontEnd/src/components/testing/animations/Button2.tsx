import * as React from 'react';
import {
  StyleSheet,
  Text,
  Pressable,
  StyleProp,
  ViewStyle,
  TextStyle,
  View,
} from 'react-native';
import { globalStyles } from '../../../styles/GlobalStyles';
import { buttonStyles } from '../../../styles/ButtonStyles';
import { Gap, Spacing } from '../../../styles/SpacingStyles';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../../config.json';
import { Colors } from '../../../styles/ColorStyles';
import Animated, {
  ReduceMotion,
  runOnJS,
  useAnimatedStyle,
  useSharedValue,
  withSpring,
  withTiming,
} from 'react-native-reanimated';

/*

This file is a copy of Button.tsx with additional features:
- animated button
- button shadow

*/

const Icon = createIconSetFromFontello(fontelloConfig);

// Types
export interface ButtonProps {
  onPress?: () => void;
  text?: string;
  icon?: string;

  type?: ButtonType;
  size?: ButtonSize;
  display?: ButtonDisplay;

  disabled?: boolean;

  self?: number;
  status?: number;
  changeState?: (myNumber: number) => void;

  // Rest styles
  btnStyle?: ViewStyle[];
  btnTextStyle?: TextStyle[];
  btnIconStyle?: TextStyle[];

  // Active styles
  btnActiveStyle?: ViewStyle[];
  btnActiveTextStyle?: TextStyle[];
  btnActiveIconStyle?: TextStyle[];

  // Disabled styles
  btnDisabledStyle?: ViewStyle[];
  btnDisabledTextStyle?: TextStyle[];
  btnDisabledIconStyle?: TextStyle[];

  // Exclusive Button Support
  id?: number;
  current?: number;
  setCurrent?: React.Dispatch<React.SetStateAction<number>>;

  //----------------[ NEW ]----------------
  containerStyle?: ViewStyle[];
  containerActiveStyle?: ViewStyle[];
  containerDisabledStyle?: ViewStyle[];

  btnShadowStyle?: ViewStyle[];
  btnShadowActiveStyle?: ViewStyle[];
  btnShadowDisabledStyle?: ViewStyle[];
}

export const Button2: React.FC<ButtonProps> = ({
  onPress = null,
  text = 'NULL',
  btnStyle = [],
  btnTextStyle = [],
  btnIconStyle = [],
  btnActiveStyle = [],
  btnActiveTextStyle = [],
  btnActiveIconStyle = [],
  btnDisabledStyle = [],
  btnDisabledTextStyle = [],
  btnDisabledIconStyle = [],
  self = null,
  status = null,
  changeState = null,
  disabled = false,
  type = null,
  size = null,
  display = null,
  icon = false,
  id = -1,
  current = -1,
  setCurrent = null,

  //----------------[ NEW ]----------------
  containerStyle = [],
  containerActiveStyle = [],
  containerDisabledStyle = [],

  btnShadowStyle = [],
  btnShadowActiveStyle = [],
  btnShadowDisabledStyle = [],
}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Type                                      ||
  // ! ||--------------------------------------------------------------------------------||

  switch (type) {
    case ButtonType.PrimaryDark:
      // Rest
      btnStyle = [buttonStyles.buttonPrimaryDark];
      btnTextStyle = [globalStyles.textLight];
      btnIconStyle = [globalStyles.textLight];

      // Active
      btnActiveStyle = [buttonStyles.buttonPrimaryDarkSelected];
      btnActiveTextStyle = [globalStyles.textLight];
      btnActiveIconStyle = [globalStyles.textLight];

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonPrimaryDarkDisabled];
      btnDisabledTextStyle = [globalStyles.textLight];
      btnDisabledIconStyle = [globalStyles.textLight];
      break;

    case ButtonType.SecondaryDark:
      // Rest
      btnStyle = [buttonStyles.buttonSecondaryDark];
      btnTextStyle = [globalStyles.textDark];
      btnIconStyle = [globalStyles.textDark];

      // Active
      btnActiveStyle = [buttonStyles.buttonSecondaryDarkSelected];
      btnActiveTextStyle = [globalStyles.textLight];
      btnActiveIconStyle = [globalStyles.textLight];

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonSecondaryDisabled];
      btnDisabledTextStyle = [globalStyles.textDisabled];
      btnDisabledIconStyle = [globalStyles.textDisabled];
      break;

    case ButtonType.SecondaryLight:
      // Rest
      btnStyle = [buttonStyles.buttonSecondaryLight];
      btnTextStyle = [globalStyles.textLight];
      btnIconStyle = [globalStyles.textLight];

      // Active
      btnActiveStyle = [buttonStyles.buttonSecondaryLightSelected];
      btnActiveTextStyle = [globalStyles.textDark];
      btnActiveIconStyle = [globalStyles.textDark];

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonSecondaryDisabled];
      btnDisabledTextStyle = [globalStyles.textDisabled];
      btnDisabledIconStyle = [globalStyles.textDisabled];
      break;

    case ButtonType.Tertiary:
      // Rest
      btnStyle = [buttonStyles.buttonTertiary];
      btnTextStyle = [globalStyles.textLight];
      btnIconStyle = [globalStyles.textLight];

      // Active
      btnActiveStyle = [buttonStyles.buttonTertiary];
      btnActiveTextStyle = [globalStyles.textLight];
      btnActiveIconStyle = [globalStyles.textLight];

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonTertiaryDisabled];
      btnDisabledTextStyle = [globalStyles.textLight];
      btnDisabledIconStyle = [globalStyles.textLight];
      break;

    case ButtonType.Success:
      // Rest
      btnStyle = [buttonStyles.buttonSuccess];
      btnTextStyle = [buttonStyles.buttonSuccessText];
      btnIconStyle = [buttonStyles.buttonSuccessText];

      // Active
      btnActiveStyle = [buttonStyles.buttonSuccess];
      btnActiveTextStyle = [buttonStyles.buttonSuccessText];
      btnActiveIconStyle = [buttonStyles.buttonSuccessText];

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonSuccessDisabled];
      btnDisabledTextStyle = [buttonStyles.buttonSuccessDisabledText];
      btnDisabledIconStyle = [buttonStyles.buttonSuccessDisabledText];

      //----------------[ NEW ]----------------
      btnShadowStyle = [styles.successShadow];
      btnShadowActiveStyle = [styles.successShadow];
      btnShadowDisabledStyle = [styles.successShadow];

      break;

    case ButtonType.Warning:
      // Rest
      btnStyle = [buttonStyles.buttonWarning];
      btnTextStyle = [buttonStyles.buttonWarningText];
      btnIconStyle = [buttonStyles.buttonWarningText];

      // Active
      btnActiveStyle = [buttonStyles.buttonWarning];
      btnActiveTextStyle = [buttonStyles.buttonWarningText];
      btnActiveIconStyle = [buttonStyles.buttonWarningText];

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonWarningDisabled];
      btnDisabledTextStyle = [buttonStyles.buttonWarningDisabledText];
      btnDisabledIconStyle = [buttonStyles.buttonWarningDisabledText];
      break;

    case ButtonType.Error:
      // Rest
      btnStyle = [buttonStyles.buttonError];
      btnTextStyle = [buttonStyles.buttonErrorText];
      btnIconStyle = [buttonStyles.buttonErrorText];

      // Active
      btnActiveStyle = [buttonStyles.buttonError];
      btnActiveTextStyle = [buttonStyles.buttonErrorText];
      btnActiveIconStyle = [buttonStyles.buttonErrorText];

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonErrorDisabled];
      btnDisabledTextStyle = [buttonStyles.buttonErrorDisabledText];
      btnDisabledIconStyle = [buttonStyles.buttonErrorDisabledText];
      break;

    case ButtonType.Function:
      // Rest
      btnStyle = [buttonStyles.buttonFunction];
      btnTextStyle = [buttonStyles.buttonFunctionText];
      btnIconStyle = [buttonStyles.buttonFunctionText];

      // Active
      btnActiveStyle = [buttonStyles.buttonFunction];
      btnActiveTextStyle = [buttonStyles.buttonFunctionText];
      btnActiveIconStyle = [buttonStyles.buttonFunctionText];

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonErrorDisabled];
      btnDisabledTextStyle = [buttonStyles.buttonErrorDisabledText];
      btnDisabledIconStyle = [buttonStyles.buttonErrorDisabledText];
      break;
  }

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Size                                      ||
  // ! ||--------------------------------------------------------------------------------||

  switch (size) {
    case ButtonSize.ExtraSmall:
      // Rest
      btnStyle = [
        ...btnStyle,
        buttonStyles.textButtonExtraSmall,
        Gap.gapMedium,
      ];
      btnTextStyle = [...btnTextStyle, globalStyles.buttonTextThree];

      // Active
      btnActiveStyle = [
        ...btnActiveStyle,
        buttonStyles.textButtonExtraSmall,
        Gap.gapMedium,
      ];
      btnActiveTextStyle = [
        ...btnActiveTextStyle,
        globalStyles.buttonTextThree,
      ];

      // Disabled
      btnDisabledStyle = [
        ...btnDisabledStyle,
        buttonStyles.textButtonExtraSmall,
        Gap.gapMedium,
      ];
      btnDisabledTextStyle = [
        ...btnDisabledTextStyle,
        globalStyles.buttonTextThree,
      ];
      break;

    case ButtonSize.Small:
      //Rest
      btnStyle = [...btnStyle, buttonStyles.textButtonSmall, Gap.gapMedium];
      btnTextStyle = [...btnTextStyle, globalStyles.buttonTextTwo];

      // Active
      btnActiveStyle = [
        ...btnActiveStyle,
        buttonStyles.textButtonSmall,
        Gap.gapMedium,
      ];
      btnActiveTextStyle = [...btnActiveTextStyle, globalStyles.buttonTextTwo];

      // Disabled
      btnDisabledStyle = [
        ...btnDisabledStyle,
        buttonStyles.textButtonSmall,
        Gap.gapMedium,
      ];
      btnDisabledTextStyle = [
        ...btnDisabledTextStyle,
        globalStyles.buttonTextTwo,
      ];
      break;

    case ButtonSize.Medium:
      // Rest
      btnStyle = [...btnStyle, buttonStyles.textButtonMedium, Gap.gapMedium];
      btnTextStyle = [...btnTextStyle, globalStyles.buttonTextOne];

      //----------------[ NEW ]----------------
      btnShadowStyle = [...btnShadowStyle, buttonStyles.textButtonMedium];

      // Active
      btnActiveStyle = [
        ...btnActiveStyle,
        buttonStyles.textButtonMedium,
        Gap.gapMedium,
      ];
      btnActiveTextStyle = [...btnActiveTextStyle, globalStyles.buttonTextOne];

      //----------------[ NEW ]----------------
      btnShadowActiveStyle = [
        ...btnShadowActiveStyle,
        buttonStyles.textButtonMedium,
      ];

      // Disabled
      btnDisabledStyle = [
        ...btnDisabledStyle,
        buttonStyles.textButtonMedium,
        Gap.gapMedium,
      ];
      btnDisabledTextStyle = [
        ...btnDisabledTextStyle,
        globalStyles.buttonTextOne,
      ];

      //----------------[ NEW ]----------------
      btnShadowDisabledStyle = [
        ...btnShadowDisabledStyle,
        buttonStyles.textButtonMedium,
      ];

      break;

    case ButtonSize.Large:
      // Rest
      btnStyle = [...btnStyle, buttonStyles.textButtonLarge, Gap.gapMedium];
      btnTextStyle = [...btnTextStyle, globalStyles.buttonTextOne];

      // Active
      btnActiveStyle = [
        ...btnActiveStyle,
        buttonStyles.textButtonLarge,
        Gap.gapMedium,
      ];
      btnActiveTextStyle = [...btnActiveTextStyle, globalStyles.buttonTextOne];

      // Disabled
      btnDisabledStyle = [
        ...btnDisabledStyle,
        buttonStyles.textButtonLarge,
        Gap.gapMedium,
      ];
      btnDisabledTextStyle = [
        ...btnDisabledTextStyle,
        globalStyles.buttonTextOne,
      ];
      break;
  }

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Display                                    ||
  // ! ||--------------------------------------------------------------------------------||

  switch (display) {
    case ButtonDisplay.Contained:
      //----------------[ NEW ]----------------
      containerStyle = [buttonStyles.buttonContained];
      containerActiveStyle = [buttonStyles.buttonContained];
      containerDisabledStyle = [buttonStyles.buttonContained];

      break;

    case ButtonDisplay.Full:
      //----------------[ NEW ]----------------
      containerStyle = [buttonStyles.buttonFull];
      containerActiveStyle = [buttonStyles.buttonFull];
      containerDisabledStyle = [buttonStyles.buttonFull];
      break;
  }

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                   Button                                       ||
  // ! ||--------------------------------------------------------------------------------||

  // TODO button needs to reset state back to rest (isPressed = false), when the user has
  // left the screen / doesn't see the button anymore.

  // -------------------------------[ NEW ]-----------------------------------------------

  const sv = useSharedValue(6);

  const animatedButton = useAnimatedStyle(() => ({
    transform: [{ translateY: -sv.value }],
  }));

  const animationDuration = 220;

  // ! ||--------------------------------------------------------------------------------||

  /*

  TODO button needs to:

  1. Animate down when pressed (DONE)
  2. Animate up when released (DONE)
  3. Not be able to press again until it has fully animated up

  */

  const handlePressIn = () => {
    console.log('Pressed');

    runOnJS(() => {
      sv.value = withTiming(sv.value - 6, { duration: animationDuration });
      console.log('Press-in triggered');
      setTimeout(() => {
        sv.value = withTiming(6, { duration: animationDuration });
      }, 200);
    })();

    if (self != null && status != null && changeState != null) {
      if (status == self) changeState(-1);
      else changeState(self);
    } else {
      if (onPress != null) {
        onPress();
      }
      if (setCurrent != null && current == id) {
        setCurrent(-1);
      }
      if (setCurrent != null && current != id) {
        setCurrent(id);
      }
    }
  };

  return (
    <Pressable
      onPress={() => {
        handlePressIn();
      }}
      // onPressIn={() => {
      //   handlePressIn();
      // }}
      // onPressOut={() => {
      //   handlePressOut();
      // }}
      disabled={disabled}>
      {/* Container */}
      <View
        style={
          disabled
            ? containerDisabledStyle
            : current == id
            ? containerActiveStyle
            : containerStyle
        }>
        {/* Button */}
        <Animated.View
          style={[
            animatedButton,
            disabled
              ? btnDisabledStyle
              : current == id
              ? btnActiveStyle
              : btnStyle,
          ]}>
          <View style={styles.btnBase}>
            {icon && (
              <Icon
                name={icon}
                size={24}
                height={24}
                width={24}
                style={
                  disabled
                    ? btnDisabledIconStyle
                    : current == id
                    ? btnActiveIconStyle
                    : btnIconStyle
                }
              />
            )}
            <Text
              style={
                disabled
                  ? btnDisabledTextStyle
                  : current == id
                  ? btnActiveTextStyle
                  : btnTextStyle
              }>
              {text}
            </Text>
          </View>
        </Animated.View>
        {/* Shadow */}
        <Animated.View
          style={[
            styles.shadow,
            StyleSheet.absoluteFill,
            disabled
              ? btnShadowDisabledStyle
              : current == id
              ? btnShadowActiveStyle
              : btnShadowStyle,
          ]}
        />
      </View>
    </Pressable>
  );
};

// ! ||--------------------------------------------------------------------------------||
// ! ||                                 Exported Enums                                 ||
// ! ||--------------------------------------------------------------------------------||

export enum ButtonType {
  PrimaryDark,
  SecondaryDark,
  SecondaryLight,
  Tertiary,
  Success,
  Warning,
  Error,
  Function,
}

export enum ButtonSize {
  Large,
  Medium,
  Small,
  ExtraSmall,
}

export enum ButtonDisplay {
  Contained,
  Full,
}

const styles = StyleSheet.create({
  btnBase: {
    flexDirection: 'row',
    alignItems: 'baseline',
    columnGap: Spacing.sm,
  },

  shadow: {
    // Shadow size on Figma + border size
    // transform: [{ translateY: 4 + 2 }],
    zIndex: -1,
  },

  successShadow: {
    backgroundColor: Colors.green700,
    borderColor: Colors.green700,
  },
});

export default Button2;
