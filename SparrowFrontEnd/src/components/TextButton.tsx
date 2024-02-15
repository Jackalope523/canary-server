import * as React from 'react';
import {
  StyleSheet,
  Text,
  Pressable,
  StyleProp,
  ViewStyle,
  TextStyle,
} from 'react-native';
import { globalStyles } from '../styles/GlobalStyles';
import { buttonStyles } from '../styles/ButtonStyles';
import { Gap, Spacing } from '../styles/SpacingStyles';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';
import { Colors } from '../styles/ColorStyles';

const Icon = createIconSetFromFontello(fontelloConfig);

// Types
interface TextButtonProps {
  text?: string;
  btnTextStyle?: TextStyle[];
  btnDisabledTextStyle?: TextStyle[];
  disabled?: boolean;
  type?: TextButtonType;
  variant?: TextButtonVariant;
  displayIcon: boolean;
  btnIconStyle?: string;
  btnDisabledIconStyle?: string;
  Icon?: React.FC<any> | string | any;
  onPress?: () => void;
}

export const TextButton: React.FC<TextButtonProps> = ({
  text = 'NULL',
  btnTextStyle = [],
  btnDisabledTextStyle = [],
  disabled = null,
  type = null,
  variant = null,
  displayIcon = false,
  btnIconStyle,
  btnDisabledIconStyle,
  Icon = null,
  onPress = null,
}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Type                                      ||
  // ! ||--------------------------------------------------------------------------------||

  switch (type) {
    case TextButtonType.Dark:
      btnTextStyle = [globalStyles.textDark];
      btnIconStyle = Colors.sparrowDarkBrown;

      // Disabled
      btnDisabledTextStyle = [globalStyles.textDisabled];
      btnDisabledIconStyle = Colors.sand300;
      break;

    case TextButtonType.Light:
      btnTextStyle = [globalStyles.textLight];
      btnIconStyle = Colors.sparrowSand;

      // Disabled
      btnDisabledTextStyle = [globalStyles.textDisabled];
      btnDisabledIconStyle = Colors.sand300;
      break;

    case TextButtonType.Success:
      btnTextStyle = [globalStyles.textSuccess];
      btnIconStyle = Colors.green700;

      // Disabled
      btnDisabledTextStyle = [buttonStyles.buttonSuccessDisabledText];
      btnDisabledIconStyle = Colors.green300;
      break;

    case TextButtonType.Warning:
      btnTextStyle = [globalStyles.textWarning];
      btnIconStyle = Colors.orange700;

      // Disabled
      btnDisabledTextStyle = [buttonStyles.buttonWarningDisabledText];
      btnDisabledIconStyle = Colors.orange300;
      break;

    case TextButtonType.Error:
      // Rest
      btnTextStyle = [globalStyles.textError];
      btnIconStyle = Colors.red700;

      // Disabled
      btnDisabledTextStyle = [buttonStyles.buttonErrorDisabledText];
      btnDisabledIconStyle = Colors.red300;
      break;

    case TextButtonType.Function:
      // Rest
      btnTextStyle = [globalStyles.textFunction];
      btnIconStyle = Colors.turqoise700;

      // Disabled
      btnDisabledTextStyle = [buttonStyles.buttonFunctionDisabledText];
      btnDisabledIconStyle = Colors.turqoise300;
      break;
  }

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                    Variant                                     ||
  // ! ||--------------------------------------------------------------------------------||

  switch (variant) {
    case TextButtonVariant.One:
      btnTextStyle = [...btnTextStyle, globalStyles.buttonTextOne];
      break;

    case TextButtonVariant.Two:
      btnTextStyle = [...btnTextStyle, globalStyles.buttonTextTwo];
      break;

    case TextButtonVariant.Three:
      btnTextStyle = [...btnTextStyle, globalStyles.buttonTextThree];
      break;

    case TextButtonVariant.Four:
      btnTextStyle = [...btnTextStyle, globalStyles.buttonTextFour];
      break;
  }

  return (
    <Pressable onPress={onPress} disabled={disabled} style={styles.pressable}>
      <Text style={disabled ? btnDisabledTextStyle : btnTextStyle}>{text}</Text>
      {displayIcon && (
        <Icon
          height={24}
          width={24}
          fill={disabled ? btnDisabledIconStyle : btnIconStyle}
        />
      )}
    </Pressable>
  );
};

// ! ||--------------------------------------------------------------------------------||
// ! ||                                 Exported Enums                                 ||
// ! ||--------------------------------------------------------------------------------||

export enum TextButtonType {
  Dark,
  Light,
  Success,
  Warning,
  Error,
  Function,
}

export enum TextButtonVariant {
  One,
  Two,
  Three,
  Four,
}

export default TextButton;

// ! ||--------------------------------------------------------------------------------||
// ! ||                                     Styles                                     ||
// ! ||--------------------------------------------------------------------------------||

const styles = StyleSheet.create({
  pressable: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.mdsm,
  },
});
