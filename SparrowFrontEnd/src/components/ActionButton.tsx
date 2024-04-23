// #region imports
import * as React from 'react';
import { StyleSheet, Pressable, ViewStyle, View } from 'react-native';
import { globalStyles } from '../styles/GlobalStyles';
import { buttonStyles } from '../styles/ButtonStyles';
import Animated, {
  ReduceMotion,
  runOnJS,
  useAnimatedStyle,
  useSharedValue,
  withSpring,
  withTiming,
} from 'react-native-reanimated';
import { Colors } from '../styles/ColorStyles';
import { SvgProps } from 'react-native-svg';
import { ElementType } from 'react';
// #endregion

/*

This file is a copy of Button2.tsx

*/

// Types
export interface ActionButtonProps {
  onPress?: () => void;
  icon?: string;
  Icon?: React.FC<SvgProps> | ElementType;

  type: ActionButtonType;
  size: ActionButtonSize;
  display: ActionButtonDisplay;

  disabled: boolean;

  // Rest styles
  btnStyle?: ViewStyle[];
  btnIconStyle: string;

  // Active styles
  btnActiveStyle?: ViewStyle[];
  btnActiveIconStyle: string;

  // Disabled styles
  btnDisabledStyle?: ViewStyle[];
  btnDisabledIconStyle: string;

  btnIconSize?: number;

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

export const ActionButton: React.FC<ActionButtonProps> = ({
  // #region Props
  onPress = () => {},
  btnStyle = [],
  btnIconStyle,
  btnActiveStyle = [],
  btnActiveIconStyle,
  btnDisabledStyle = [],
  btnDisabledIconStyle,
  btnIconSize = 24,
  containerStyle = [],
  containerActiveStyle = [],
  containerDisabledStyle = [],
  btnShadowStyle = [],
  btnShadowActiveStyle = [],
  btnShadowDisabledStyle = [],

  disabled = false,
  type = null,
  size = null,
  display = null,
  icon,
  Icon,
  id = -1,
  current = -1,
  setCurrent = (x: number) => {},
  // #endregion
}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                 Animation                                      ||
  // ! ||--------------------------------------------------------------------------------||
  // #region Animation

  const sv = useSharedValue(6);

  const animatedButton = useAnimatedStyle(() => ({
    transform: [{ translateY: -sv.value }],
  }));

  const animationDuration = 220;

  const handlePressIn = () => {
    sv.value = withTiming(sv.value - 6 >= 0 ? sv.value - 6 : 0, {
      duration: animationDuration,
    });
    setTimeout(() => {
      sv.value = withTiming(6, { duration: animationDuration });
    }, 200);

    setCurrent(current === id ? -1 : id);
    onPress();
  };

  // #endregion
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Type                                      ||
  // ! ||--------------------------------------------------------------------------------||
  // #region Type
  switch (type) {
    case ActionButtonType.PrimaryLight:
      // Rest
      btnStyle = [buttonStyles.buttonPrimaryDark, animatedButton];
      btnIconStyle = Colors.sparrowSand;
      btnShadowStyle = [buttonStyles.shadowPrimaryLight];

      // Active
      btnActiveStyle = [buttonStyles.buttonPrimaryDarkSelected, animatedButton];
      btnActiveIconStyle = Colors.sparrowSand;
      btnShadowActiveStyle = [buttonStyles.shadowPrimaryLight];

      // Disabled
      btnDisabledStyle = [
        buttonStyles.buttonPrimaryDarkDisabled,
        animatedButton,
      ];
      btnDisabledIconStyle = Colors.sparrowSand;
      btnShadowDisabledStyle = [buttonStyles.shadowPrimaryLight];
      break;

    /*
    
    TODO fix SecondaryLight and SecondaryDark styles:
    1. rest and active styles seem to be swapped in dev, fix this
      - it's swapped for every button type but these two are noticable

    */
    case ActionButtonType.SecondaryLight:
      // Rest
      btnStyle = [buttonStyles.buttonSecondaryDark];
      btnIconStyle = Colors.sparrowDark;

      // Active
      btnActiveStyle = [buttonStyles.buttonSecondaryDarkSelected];
      btnActiveIconStyle = Colors.sparrowSand;

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonSecondaryDisabled];
      btnDisabledIconStyle = Colors.sand300;
      break;

    case ActionButtonType.SecondaryDark:
      // Rest
      btnStyle = [buttonStyles.buttonSecondaryLight];
      btnIconStyle = Colors.sparrowSand;

      // Active
      btnActiveStyle = [buttonStyles.buttonSecondaryLightSelected];
      btnActiveIconStyle = Colors.sparrowDark;

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonSecondaryDisabled];
      btnDisabledIconStyle = Colors.sand300;
      break;

    // TODO remove tertiary when it's removed from ALL use cases and
    // and all button components have been updated

    // case ActionButtonType.Tertiary:
    //   // Rest
    //   btnStyle = [buttonStyles.buttonTertiary];
    //   btnIconStyle = [globalStyles.textLight];

    //   // Active
    //   btnActiveStyle = [buttonStyles.buttonTertiary];
    //   btnActiveIconStyle = [globalStyles.textLight];

    //   // Disabled
    //   btnDisabledStyle = [buttonStyles.buttonTertiaryDisabled];
    //   btnDisabledIconStyle = [globalStyles.textLight];
    //   break;

    case ActionButtonType.Success:
      // Rest
      btnStyle = [buttonStyles.buttonSuccess, animatedButton];
      btnIconStyle = Colors.green700;
      btnShadowStyle = [buttonStyles.shadowSuccess];

      // Active
      btnActiveStyle = [buttonStyles.buttonSuccess, animatedButton];
      btnActiveIconStyle = Colors.green700;
      btnShadowActiveStyle = [buttonStyles.shadowSuccess];

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonSuccessDisabled, animatedButton];
      btnDisabledIconStyle = Colors.green300;
      btnShadowDisabledStyle = [buttonStyles.shadowSuccess];
      break;

    case ActionButtonType.Warning:
      // Rest
      btnStyle = [buttonStyles.buttonWarning, animatedButton];
      btnIconStyle = Colors.orange700;
      btnShadowStyle = [buttonStyles.shadowWarning];

      // Active
      btnActiveStyle = [buttonStyles.buttonWarning, animatedButton];
      btnActiveIconStyle = Colors.orange700;
      btnShadowActiveStyle = [buttonStyles.shadowWarning];

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonWarningDisabled, animatedButton];
      btnDisabledIconStyle = Colors.orange300;
      btnShadowDisabledStyle = [buttonStyles.shadowWarning];
      break;

    case ActionButtonType.Error:
      // Rest
      btnStyle = [buttonStyles.buttonError, animatedButton];
      btnIconStyle = Colors.red700;
      btnShadowStyle = [buttonStyles.shadowError];

      // Active
      btnActiveStyle = [buttonStyles.buttonError, animatedButton];
      btnActiveIconStyle = Colors.red700;
      btnShadowActiveStyle = [buttonStyles.shadowError];

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonErrorDisabled, animatedButton];
      btnDisabledIconStyle = Colors.red300;
      btnShadowDisabledStyle = [buttonStyles.shadowError];
      break;

    case ActionButtonType.Function:
      // Rest
      btnStyle = [buttonStyles.buttonFunction, animatedButton];
      btnIconStyle = Colors.turqoise700;
      btnShadowStyle = [buttonStyles.shadowFunction];

      // Active
      btnActiveStyle = [buttonStyles.buttonFunction, animatedButton];
      btnActiveIconStyle = Colors.turqoise700;
      btnShadowActiveStyle = [buttonStyles.shadowFunction];

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonFunctionDisabled, animatedButton];
      btnDisabledIconStyle = Colors.turqoise300;
      btnShadowDisabledStyle = [buttonStyles.shadowFunction];
      break;
  }
  // #endregion

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Size                                      ||
  // ! ||--------------------------------------------------------------------------------||
  // #region Size
  switch (size) {
    case ActionButtonSize.Small:
      //Rest
      btnStyle = [...btnStyle, buttonStyles.actionButtonSmall];
      btnIconSize = 24;
      btnShadowStyle = [...btnShadowStyle, buttonStyles.actionButtonSmall];

      // Active
      btnActiveStyle = [...btnActiveStyle, buttonStyles.actionButtonSmall];
      btnIconSize = 24;
      btnShadowActiveStyle = [
        ...btnShadowActiveStyle,
        buttonStyles.actionButtonSmall,
      ];

      // Disabled
      btnDisabledStyle = [...btnDisabledStyle, buttonStyles.textButtonSmall];
      btnIconSize = 24;
      btnShadowDisabledStyle = [
        ...btnShadowDisabledStyle,
        buttonStyles.actionButtonSmall,
      ];
      break;

    case ActionButtonSize.Medium:
      // Rest
      btnStyle = [...btnStyle, buttonStyles.actionButtonMedium];
      btnIconSize = 24;
      btnShadowStyle = [...btnShadowStyle, buttonStyles.actionButtonMedium];

      // Active
      btnActiveStyle = [...btnActiveStyle, buttonStyles.actionButtonMedium];
      btnIconSize = 24;
      btnShadowActiveStyle = [
        ...btnShadowActiveStyle,
        buttonStyles.actionButtonMedium,
      ];

      // Disabled
      btnDisabledStyle = [...btnDisabledStyle, buttonStyles.actionButtonMedium];
      btnIconSize = 24;
      btnShadowDisabledStyle = [
        ...btnShadowDisabledStyle,
        buttonStyles.actionButtonMedium,
      ];

      break;

    case ActionButtonSize.Large:
      // Rest
      btnStyle = [...btnStyle, buttonStyles.actionButtonLarge];
      btnIconSize = 24;
      btnShadowStyle = [...btnShadowStyle, buttonStyles.actionButtonLarge];

      // Active
      btnActiveStyle = [...btnActiveStyle, buttonStyles.actionButtonLarge];
      btnIconSize = 24;
      btnShadowActiveStyle = [
        ...btnShadowActiveStyle,
        buttonStyles.actionButtonLarge,
      ];

      // Disabled
      btnDisabledStyle = [...btnDisabledStyle, buttonStyles.actionButtonLarge];
      btnIconSize = 24;
      btnShadowDisabledStyle = [
        ...btnShadowDisabledStyle,
        buttonStyles.actionButtonLarge,
      ];

      break;
  }
  // #endregion

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Display                                    ||
  // ! ||--------------------------------------------------------------------------------||
  // #region Display
  switch (display) {
    case ActionButtonDisplay.Contained:
      //----------------[ NEW ]----------------
      containerStyle = [buttonStyles.buttonContained];
      containerActiveStyle = [buttonStyles.buttonContained];
      containerDisabledStyle = [buttonStyles.buttonContained];

      break;

    case ActionButtonDisplay.Full:
      //----------------[ NEW ]----------------
      containerStyle = [buttonStyles.buttonFull];
      containerActiveStyle = [buttonStyles.buttonFull];
      containerDisabledStyle = [buttonStyles.buttonFull];
      break;
  }
  // #endregion

  return (
    <Pressable
      onPress={() => {
        handlePressIn();
      }}
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
        {/* Animated press */}
        <Animated.View
          style={[
            // animatedButton,
            disabled
              ? btnDisabledStyle
              : current == id
              ? btnActiveStyle
              : btnStyle,
          ]}>
          <Icon
            name={icon}
            height={btnIconSize}
            width={btnIconSize}
            fill={
              disabled
                ? btnDisabledIconStyle
                : current == id
                ? btnActiveIconStyle
                : btnIconStyle
            }
          />
        </Animated.View>

        {/* Shadow */}
        {type !== ActionButtonType.SecondaryLight &&
          type !== ActionButtonType.SecondaryDark && (
            <Animated.View
              style={[
                buttonStyles.shadow,
                StyleSheet.absoluteFill,
                disabled
                  ? btnShadowDisabledStyle
                  : current == id
                  ? btnShadowActiveStyle
                  : btnShadowStyle,
              ]}
            />
          )}
      </View>
    </Pressable>
  );
};

export default ActionButton;

// ! ||--------------------------------------------------------------------------------||
// ! ||                                 Exported Enums                                 ||
// ! ||--------------------------------------------------------------------------------||
// #region Exported Enums
export enum ActionButtonType {
  PrimaryLight,
  SecondaryLight,
  SecondaryDark,
  Success,
  Warning,
  Error,
  Function,
}

export enum ActionButtonSize {
  Large,
  Medium,
  Small,
}

export enum ActionButtonDisplay {
  Contained,
  Full,
}
// #endregion
