//#region imports
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
import { globalStyles } from '../styles/GlobalStyles';
import { buttonStyles } from '../styles/ButtonStyles';
import { Gap, Spacing } from '../styles/SpacingStyles';
import { Colors } from '../styles/ColorStyles';
import { AnimatedView } from 'react-native-reanimated/lib/typescript/reanimated2/component/View';
import Animated from 'react-native-reanimated';
//#endregion

// Types
export interface ButtonProps {
  onPress?: () => void;
  text?: string;
  Icon?: React.FC<any> | string | any;

  type?: ButtonType;
  size?: ButtonSize;
  display?: ButtonDisplay;
  displayIcon?: boolean;

  disabled?: boolean;

  self?: number;
  status?: number;
  changeState?: (myNumber: number) => void;

  // Rest styles
  btnStyle?: ViewStyle[];
  btnTextStyle?: TextStyle[];
  btnIconStyle: string;

  // Active styles
  btnActiveStyle?: ViewStyle[];
  btnActiveTextStyle?: TextStyle[];
  btnActiveIconStyle: string;

  // Disabled styles
  btnDisabledStyle?: ViewStyle[];
  btnDisabledTextStyle?: TextStyle[];
  btnDisabledIconStyle: string;

  // Exclusive Button Support
  id?: number;
  current?: number;
  setCurrent?: React.Dispatch<React.SetStateAction<number>>;
}

export const Button: React.FC<ButtonProps> = ({
  onPress = null,
  text = 'NULL',
  Icon = null,
  btnStyle = [],
  btnTextStyle = [],
  btnIconStyle,
  btnActiveStyle = [],
  btnActiveTextStyle = [],
  btnActiveIconStyle,
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
  displayIcon = false,
  id = -1,
  current = -1,
  setCurrent = null,
}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Type                                      ||
  // ! ||--------------------------------------------------------------------------------||
  //#region Type

  switch (type) {
    case ButtonType.PrimaryDark:
      // Rest
      btnStyle = [buttonStyles.buttonPrimaryDark];
      btnTextStyle = [globalStyles.textLight];
      btnIconStyle = Colors.sparrowSand;

      // Active
      btnActiveStyle = [buttonStyles.buttonPrimaryDarkSelected];
      btnActiveTextStyle = [globalStyles.textLight];
      btnActiveIconStyle = Colors.sparrowSand;

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonPrimaryDarkDisabled];
      btnDisabledTextStyle = [globalStyles.textLight];
      btnDisabledIconStyle = Colors.sparrowSand;
      break;

    case ButtonType.SecondaryDark:
      // Rest
      btnStyle = [buttonStyles.buttonSecondaryDark];
      btnTextStyle = [globalStyles.textDark];
      btnIconStyle = Colors.sparrowDark;

      // Active
      btnActiveStyle = [buttonStyles.buttonSecondaryDarkSelected];
      btnActiveTextStyle = [globalStyles.textLight];
      btnActiveIconStyle = Colors.sparrowSand;

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonSecondaryDisabled];
      btnDisabledTextStyle = [globalStyles.textDisabled];
      btnDisabledIconStyle = Colors.sand300;
      break;

    case ButtonType.SecondaryLight:
      // Rest
      btnStyle = [buttonStyles.buttonSecondaryLight];
      btnTextStyle = [globalStyles.textLight];
      btnIconStyle = Colors.sparrowSand;

      // Active
      btnActiveStyle = [buttonStyles.buttonSecondaryLightSelected];
      btnActiveTextStyle = [globalStyles.textDark];
      btnActiveIconStyle = Colors.sparrowDark;

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonSecondaryDisabled];
      btnDisabledTextStyle = [globalStyles.textDisabled];
      btnDisabledIconStyle = Colors.sand300;
      break;

    case ButtonType.Tertiary:
      // Rest
      btnStyle = [buttonStyles.buttonTertiary];
      btnTextStyle = [globalStyles.textLight];
      btnIconStyle = Colors.sparrowSand;

      // Active
      btnActiveStyle = [buttonStyles.buttonTertiary];
      btnActiveTextStyle = [globalStyles.textLight];
      btnActiveIconStyle = Colors.sparrowSand;

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonTertiaryDisabled];
      btnDisabledTextStyle = [globalStyles.textLight];
      btnDisabledIconStyle = Colors.sparrowSand;
      break;

    case ButtonType.Success:
      // Rest
      btnStyle = [buttonStyles.buttonSuccess];
      btnTextStyle = [buttonStyles.buttonSuccessText];
      btnIconStyle = Colors.green700;

      // Active
      btnActiveStyle = [buttonStyles.buttonSuccess];
      btnActiveTextStyle = [buttonStyles.buttonSuccessText];
      btnActiveIconStyle = Colors.green700;

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonSuccessDisabled];
      btnDisabledTextStyle = [buttonStyles.buttonSuccessDisabledText];
      btnDisabledIconStyle = Colors.green300;
      break;

    case ButtonType.Warning:
      // Rest
      btnStyle = [buttonStyles.buttonWarning];
      btnTextStyle = [buttonStyles.buttonWarningText];
      btnIconStyle = Colors.orange700;

      // Active
      btnActiveStyle = [buttonStyles.buttonWarning];
      btnActiveTextStyle = [buttonStyles.buttonWarningText];
      btnActiveIconStyle = Colors.orange700;

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonWarningDisabled];
      btnDisabledTextStyle = [buttonStyles.buttonWarningDisabledText];
      btnDisabledIconStyle = Colors.orange300;
      break;

    case ButtonType.Error:
      // Rest
      btnStyle = [buttonStyles.buttonError];
      btnTextStyle = [buttonStyles.buttonErrorText];
      btnIconStyle = Colors.red700;

      // Active
      btnActiveStyle = [buttonStyles.buttonError];
      btnActiveTextStyle = [buttonStyles.buttonErrorText];
      btnActiveIconStyle = Colors.red700;

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonErrorDisabled];
      btnDisabledTextStyle = [buttonStyles.buttonErrorDisabledText];
      btnDisabledIconStyle = Colors.red300;
      break;

    case ButtonType.Function:
      // Rest
      btnStyle = [buttonStyles.buttonFunction];
      btnTextStyle = [buttonStyles.buttonFunctionText];
      btnIconStyle = Colors.turqoise700;

      // Active
      btnActiveStyle = [buttonStyles.buttonFunction];
      btnActiveTextStyle = [buttonStyles.buttonFunctionText];
      btnActiveIconStyle = Colors.turqoise700;

      // Disabled
      btnDisabledStyle = [buttonStyles.buttonFunctionDisabled];
      btnDisabledTextStyle = [buttonStyles.buttonFunctionDisabledText];
      btnDisabledIconStyle = Colors.turqoise300;
      break;
  }
  //#endregion

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Size                                      ||
  // ! ||--------------------------------------------------------------------------------||
  //#region Size

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

      // Active
      btnActiveStyle = [
        ...btnActiveStyle,
        buttonStyles.textButtonMedium,
        Gap.gapMedium,
      ];
      btnActiveTextStyle = [...btnActiveTextStyle, globalStyles.buttonTextOne];

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
  //#endregion 

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Display                                    ||
  // ! ||--------------------------------------------------------------------------------||
  //#region Display

  switch (display) {
    case ButtonDisplay.Contained:
      btnStyle = [...btnStyle, buttonStyles.buttonContained];
      btnActiveStyle = [...btnActiveStyle, buttonStyles.buttonContained];
      btnDisabledStyle = [...btnDisabledStyle, buttonStyles.buttonContained];
      break;

    case ButtonDisplay.Full:
      btnStyle = [...btnStyle, buttonStyles.buttonFull];
      btnActiveStyle = [...btnActiveStyle, buttonStyles.buttonFull];
      btnDisabledStyle = [...btnDisabledStyle, buttonStyles.buttonFull];
      break;
  }
  //#endregion

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                   Button                                       ||
  // ! ||--------------------------------------------------------------------------------||
  



  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Animation                                  ||
  // ! ||--------------------------------------------------------------------------------||
  //#region Animation

  
  //#endregion

  // TODO button needs to reset state back to rest (isPressed = false), when the user has
  // left the screen / doesn't see the button anymore.

  //#region Buttons
  const handlePressIn = () => {
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
      onPressIn={() => {
        handlePressIn();
      }}
      disabled={disabled}>
      <Animated.View style={[styles.btnBase, disabled ? btnDisabledStyle : current == id ? btnActiveStyle : btnStyle]}>
        {displayIcon && (
          <Icon
            height={24}
            width={24}
            fill={
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
      </Animated.View>
    </Pressable>
  );
};
//#endregion

// ! ||--------------------------------------------------------------------------------||
// ! ||                                 Exported Enums                                 ||
// ! ||--------------------------------------------------------------------------------||
//#region Exported Enums

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
//#endregion

const styles = StyleSheet.create({
  btnBase: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.sm,
  },
});

export default Button;
