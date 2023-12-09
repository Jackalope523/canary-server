import * as React from 'react';
import {
  StyleSheet,
  Text,
  Pressable,
  StyleProp,
  ViewStyle,
  TextStyle,
} from 'react-native';
import { globalStyles } from '../styles/Global';
import { buttonStyles } from '../styles/Buttons';
import { Gap } from '../styles/Spacing';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';

const Icon = createIconSetFromFontello(fontelloConfig);

interface ButtonProps {
  onPress?: () => void;
  btnText?: string;
  btnIcon?: string;

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
}

export const Button: React.FC<ButtonProps> = ({
  onPress = null,
  btnText = 'NULL',
  btnStyle = [],
  btnTextStyle = [],
  btnIconStyle = [],
  btnIcon = null,
  btnActiveStyle = [],
  btnActiveTextStyle = [],
  btnActiveIconStyle = [],
  self = null,
  status = null,
  changeState = null,
  disabled = null,
  type = null,
  size = null,
  display = null,
}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Type                                      ||
  // ! ||--------------------------------------------------------------------------------||

  switch (type) {
    case ButtonType.PrimaryDark:
      btnStyle = [buttonStyles.buttonPrimaryDark];
      btnTextStyle = [globalStyles.textLight];
      btnIconStyle = [globalStyles.textLight];
      btnActiveStyle = [buttonStyles.buttonPrimaryDark];
      btnActiveTextStyle = [globalStyles.textLight];
      btnActiveIconStyle = [globalStyles.textLight];
      break;

    case ButtonType.SecondaryDark:
      btnStyle = [buttonStyles.buttonSecondaryDark];
      btnTextStyle = [globalStyles.textDark];
      btnIconStyle = [globalStyles.textDark];
      btnActiveStyle = [buttonStyles.buttonSecondaryDarkSelected];
      btnActiveTextStyle = [globalStyles.textLight];
      btnActiveIconStyle = [globalStyles.textLight];
      break;

    case ButtonType.SecondaryLight:
      btnStyle = [buttonStyles.buttonSecondaryLight];
      btnTextStyle = [globalStyles.textDark];
      btnIconStyle = [globalStyles.textDark];
      btnActiveStyle = [buttonStyles.buttonSecondaryLightSelected];
      btnActiveTextStyle = [globalStyles.textDark];
      btnActiveIconStyle = [globalStyles.textDark];
      break;

    case ButtonType.Tertiary:
      btnStyle = [buttonStyles.buttonTertiary];
      btnTextStyle = [globalStyles.textLight];
      btnIconStyle = [globalStyles.textLight];
      btnActiveStyle = [buttonStyles.buttonTertiary];
      btnActiveTextStyle = [globalStyles.textLight];
      btnActiveIconStyle = [globalStyles.textLight];
      break;

    case ButtonType.Success:
      btnStyle = [buttonStyles.buttonSuccess];
      btnTextStyle = [globalStyles.textDark];
      btnIconStyle = [globalStyles.textDark];
      btnActiveStyle = [buttonStyles.buttonSuccess];
      btnActiveTextStyle = [globalStyles.textDark];
      btnActiveIconStyle = [globalStyles.textDark];
      break;

    case ButtonType.Warning:
      btnStyle = [buttonStyles.buttonWarning];
      btnTextStyle = [globalStyles.textDark];
      btnIconStyle = [globalStyles.textDark];
      btnActiveStyle = [buttonStyles.buttonWarning];
      btnActiveTextStyle = [globalStyles.textDark];
      btnActiveIconStyle = [globalStyles.textDark];
      break;

    case ButtonType.Error:
      btnStyle = [buttonStyles.buttonError];
      btnTextStyle = [globalStyles.textDark];
      btnIconStyle = [globalStyles.textDark];
      btnActiveStyle = [buttonStyles.buttonError];
      btnActiveTextStyle = [globalStyles.textDark];
      btnActiveIconStyle = [globalStyles.textDark];
      break;

    case ButtonType.Function:
      btnStyle = [buttonStyles.buttonFunction];
      btnTextStyle = [globalStyles.textDark];
      btnIconStyle = [globalStyles.textDark];
      btnActiveStyle = [buttonStyles.buttonFunction];
      btnActiveTextStyle = [globalStyles.textDark];
      btnActiveIconStyle = [globalStyles.textDark];
      break;
  }

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Size                                      ||
  // ! ||--------------------------------------------------------------------------------||

  switch (size) {
    case ButtonSize.ExtraSmall:
      btnStyle = [
        ...btnStyle,
        buttonStyles.textButtonExtraSmall,
        Gap.gapMedium,
      ];
      btnTextStyle = [...btnTextStyle, globalStyles.buttonTextThree];
      btnActiveStyle = [
        ...btnActiveStyle,
        buttonStyles.textButtonExtraSmall,
        Gap.gapMedium,
      ];
      btnActiveTextStyle = [
        ...btnActiveTextStyle,
        globalStyles.buttonTextThree,
      ];
      break;

    case ButtonSize.Small:
      btnStyle = [...btnStyle, buttonStyles.textButtonSmall, Gap.gapMedium];
      btnTextStyle = [...btnTextStyle, globalStyles.buttonTextTwo];
      btnActiveStyle = [
        ...btnActiveStyle,
        buttonStyles.textButtonSmall,
        Gap.gapMedium,
      ];
      btnActiveTextStyle = [...btnActiveTextStyle, globalStyles.buttonTextTwo];
      break;

    case ButtonSize.Medium:
      btnStyle = [...btnStyle, buttonStyles.textButtonMedium, Gap.gapMedium];
      btnTextStyle = [...btnTextStyle, globalStyles.buttonTextOne];
      btnActiveStyle = [
        ...btnActiveStyle,
        buttonStyles.textButtonMedium,
        Gap.gapMedium,
      ];
      btnActiveTextStyle = [...btnActiveTextStyle, globalStyles.buttonTextOne];
      break;

    case ButtonSize.Large:
      btnStyle = [...btnStyle, buttonStyles.textButtonLarge, Gap.gapMedium];
      btnTextStyle = [...btnTextStyle, globalStyles.buttonTextOne];
      btnActiveStyle = [
        ...btnActiveStyle,
        buttonStyles.textButtonLarge,
        Gap.gapMedium,
      ];
      btnActiveTextStyle = [...btnActiveTextStyle, globalStyles.buttonTextOne];
      break;
  }

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Display                                    ||
  // ! ||--------------------------------------------------------------------------------||

  switch (display) {
    case ButtonDisplay.Contained:
      btnStyle = [...btnStyle, buttonStyles.buttonContained];
      btnActiveStyle = [...btnActiveStyle, buttonStyles.buttonContained];
      break;

    case ButtonDisplay.Full:
      btnStyle = [...btnStyle, buttonStyles.buttonFull];
      btnActiveStyle = [...btnActiveStyle, buttonStyles.buttonFull];
      break;
  }

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                   Button                                       ||
  // ! ||--------------------------------------------------------------------------------||

  // TODO button needs to reset state back to rest (isPressed = false), when the user has
  // left the screen / doesn't see the button anymore.

  const [isPressed, setIsPressed] = React.useState(false);

  const handlePressIn = () => {
    if (self != null && status != null && changeState != null) {
      if (status == self) changeState(-1);
      else changeState(self);
    } else {
      setIsPressed(true);
      if (onPress != null) {
        onPress();
      }
    }
  };

  return (
    <Pressable
      onPress={() => {
        handlePressIn();
        setIsPressed(!isPressed);
      }}
      style={isPressed ? btnStyle : btnActiveStyle}
      disabled={disabled}>
      <Text style={isPressed ? btnTextStyle : btnActiveTextStyle}>
        {btnText}
      </Text>
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
  },
});

export default Button;
