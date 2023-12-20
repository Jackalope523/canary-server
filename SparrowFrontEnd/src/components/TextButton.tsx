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
import { Gap } from '../styles/SpacingStyles';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';

const Icon = createIconSetFromFontello(fontelloConfig);

// Types
interface TextButtonProps {
  onPress?: () => void;
  btnText?: string;

  type?: TextButtonType;
  variant?: TextButtonVariant;

  disabled?: boolean;

  btnTextStyle?: TextStyle[];
}

export const TextButton: React.FC<TextButtonProps> = ({
  onPress = null,
  btnText = 'NULL',
  btnTextStyle = [],
  disabled = null,
  type = null,
  variant = null,
}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Type                                      ||
  // ! ||--------------------------------------------------------------------------------||

  switch (type) {
    case TextButtonType.Dark:
      btnTextStyle = [globalStyles.textDark];
      break;

    case TextButtonType.Light:
      btnTextStyle = [globalStyles.textLight];
      break;

    case TextButtonType.Success:
      btnTextStyle = [globalStyles.textSuccess];
      break;

    case TextButtonType.Warning:
      btnTextStyle = [globalStyles.textWarning];
      break;

    case TextButtonType.Error:
      btnTextStyle = [globalStyles.textError];
      break;

    case TextButtonType.Function:
      btnTextStyle = [globalStyles.textFunction];
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
    <Pressable onPress={onPress} disabled={disabled}>
      <Text style={btnTextStyle}>{btnText}</Text>
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
