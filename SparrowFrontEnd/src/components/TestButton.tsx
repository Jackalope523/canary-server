import * as React from 'react'
import { StyleSheet, Text, Pressable, StyleProp, ViewStyle, TextStyle } from 'react-native'
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

  type?: string;
  size?: string;
  display?: string;
  
  disabled?: boolean;

  self?:number;
  status?:number;
  changeState?:(myNumber:number)=>void;

  // Rest styles
  btnStyle?: ViewStyle[];
  btnTextStyle?: TextStyle[];
  btnIconStyle?: TextStyle[];
  
// THIS ALSO WORKS
//   btnStyle?: StyleProp<ViewStyle>[];
//   btnTextStyle?: StyleProp<TextStyle>[];
//   btnIconStyle?: StyleProp<TextStyle>[];

// TODO delete this
//   btnStyle?: StyleProp<ViewStyle>;
//   btnTextStyle?: StyleProp<TextStyle>;
//   btnIconStyle?: StyleProp<TextStyle>;

  // Active styles
  btnActiveStyle?: ViewStyle[];
  btnActiveTextStyle?: TextStyle[];
  btnActiveIconStyle?: TextStyle[];

// THIS ALSO WORKS
//   btnActiveStyle?: StyleProp<ViewStyle>[];
//   btnActiveTextStyle?: StyleProp<TextStyle>[];
//   btnActiveIconStyle?: StyleProp<TextStyle>[];

// TODO delete this
//   btnActiveStyle?: StyleProp<ViewStyle>;
//   btnActiveTextStyle?: StyleProp<TextStyle>;
//   btnActiveIconStyle?: StyleProp<TextStyle>;
}

export const TestButton: React.FC<ButtonProps> = ({
    onPress = null, 
    btnText = "NULL", 
    btnStyle = [],
    btnTextStyle= [],
    btnIconStyle = [], 
    btnIcon = null, 
    btnActiveStyle= [], 
    btnActiveTextStyle= [], 
    btnActiveIconStyle = [], 
    self = null,
    status = null,
    changeState = null,
    disabled = null,
    type = null,
    size = null,
    display = null
  }) => {
    // Logging checks
    console.log('Prop checking')
    console.log('type:', type);
    console.log('size:', size);
    console.log('display:', display);

    // Enums
    enum ButtonType {
      PrimaryDark = "PrimaryDark",
      PrimaryLight = "PrimaryLight",
      Secondary = "Secondary",
      Tertiary = "Tertiary",
      Success = "Success",
      Warning = "Warning",
      Error = "Error",
      Function = "Function",
    }

    enum ButtonSize {
      Large = "Large",
      Medium = "Medium",
      Small = "Small",
      ExtraSmall = "ExtraSmall",
    }

    enum ButtonDisplay {
      Contained = "Contained",
      Full = "Full",
    }

    // ! ||--------------------------------------------------------------------------------||
    // ! ||                                      Type                                      ||
    // ! ||--------------------------------------------------------------------------------||

    // Primary
    const PrimaryDark = {
        btnStyle: buttonStyles.buttonPrimaryDark,
        btnTextStyle: globalStyles.textLight,
        btnIconStyle: globalStyles.textLight,
        btnActiveStyle: buttonStyles.buttonPrimaryDark,
        btnActiveTextStyle: globalStyles.textLight,
        btnActiveIconStyle: globalStyles.textLight,
    }

    if (type === ButtonType.PrimaryDark) {
        console.log('Applying Primary Dark styles');

        btnStyle = [...btnStyle, PrimaryDark.btnStyle];
        btnTextStyle = [...btnTextStyle, PrimaryDark.btnTextStyle];
        btnIconStyle = [...btnIconStyle, PrimaryDark.btnIconStyle];
        btnActiveStyle = [...btnActiveStyle, PrimaryDark.btnActiveStyle];
        btnActiveTextStyle = [...btnActiveTextStyle, PrimaryDark.btnActiveTextStyle];
        btnActiveIconStyle = [...btnActiveIconStyle, PrimaryDark.btnActiveIconStyle];
    }

    const PrimaryLight = {
        btnStyle: buttonStyles.buttonPrimaryLight,
        btnTextStyle: globalStyles.textLight,
        btnIconStyle: globalStyles.textLight,
        btnActiveStyle: buttonStyles.buttonPrimaryLight,
        btnActiveTextStyle: globalStyles.textLight,
        btnActiveIconStyle: globalStyles.textLight,
    }

    if (type === ButtonType.PrimaryLight) {
        console.log('Applying Primary Light styles');

        btnStyle = [...btnStyle, PrimaryLight.btnStyle];
        btnTextStyle = [...btnTextStyle, PrimaryLight.btnTextStyle];
        btnIconStyle = [...btnIconStyle, PrimaryLight.btnIconStyle];
        btnActiveStyle = [...btnActiveStyle, PrimaryLight.btnActiveStyle];
        btnActiveTextStyle = [...btnActiveTextStyle, PrimaryLight.btnActiveTextStyle];
        btnActiveIconStyle = [...btnActiveIconStyle, PrimaryLight.btnActiveIconStyle];
    }

    // Secondary
    const Secondary = {
        btnStyle: buttonStyles.buttonSecondary,
        btnTextStyle: globalStyles.textDark,
        btnIconStyle: globalStyles.textDark,
        btnActiveStyle: buttonStyles.buttonSecondary,
        btnActiveTextStyle: globalStyles.textDark,
        btnActiveIconStyle: globalStyles.textDark,
    }

    if (type === ButtonType.Secondary) {
        console.log('Applying Secondary styles');

        btnStyle = [...btnStyle, Secondary.btnStyle];
        btnTextStyle = [...btnTextStyle, Secondary.btnTextStyle];
        btnIconStyle = [...btnIconStyle, Secondary.btnIconStyle];
        btnActiveStyle = [...btnActiveStyle, Secondary.btnActiveStyle];
        btnActiveTextStyle = [...btnActiveTextStyle, Secondary.btnActiveTextStyle];
        btnActiveIconStyle = [...btnActiveIconStyle, Secondary.btnActiveIconStyle];
    }

    // Tertiary
    const Tertiary = {
        btnStyle: buttonStyles.buttonTertiary,
        btnTextStyle: globalStyles.textLight,
        btnIconStyle: globalStyles.textLight,
        btnActiveStyle: buttonStyles.buttonTertiary,
        btnActiveTextStyle: globalStyles.textLight,
        btnActiveIconStyle: globalStyles.textLight,
    }

    if (type === ButtonType.Tertiary) {
        console.log('Applying Tertiary styles');

        btnStyle = [...btnStyle, Tertiary.btnStyle];
        btnTextStyle = [...btnTextStyle, Tertiary.btnTextStyle];
        btnIconStyle = [...btnIconStyle, Tertiary.btnIconStyle];
        btnActiveStyle = [...btnActiveStyle, Tertiary.btnActiveStyle];
        btnActiveTextStyle = [...btnActiveTextStyle, Tertiary.btnActiveTextStyle];
        btnActiveIconStyle = [...btnActiveIconStyle, Tertiary.btnActiveIconStyle];
    }

    // Success
    const Success = {
        btnStyle: buttonStyles.buttonSuccess,
        btnTextStyle: globalStyles.textDark,
        btnIconStyle: globalStyles.textDark,
        btnActiveStyle: buttonStyles.buttonSuccess,
        btnActiveTextStyle: globalStyles.textDark,
        btnActiveIconStyle: globalStyles.textDark,
    }

    if (type === ButtonType.Success) {
        console.log('Applying Success styles');

        btnStyle = [...btnStyle, Success.btnStyle];
        btnTextStyle = [...btnTextStyle, Success.btnTextStyle];
        btnIconStyle = [...btnIconStyle, Success.btnIconStyle];
        btnActiveStyle = [...btnActiveStyle, Success.btnActiveStyle];
        btnActiveTextStyle = [...btnActiveTextStyle, Success.btnActiveTextStyle];
        btnActiveIconStyle = [...btnActiveIconStyle, Success.btnActiveIconStyle];
    }

    // Warning
    const Warning = {
        btnStyle: buttonStyles.buttonWarning,
        btnTextStyle: globalStyles.textDark,
        btnIconStyle: globalStyles.textDark,
        btnActiveStyle: buttonStyles.buttonWarning,
        btnActiveTextStyle: globalStyles.textDark,
        btnActiveIconStyle: globalStyles.textDark,
    }

    if (type === ButtonType.Warning) {
        console.log('Applying Warning styles');

        btnStyle = [...btnStyle, Warning.btnStyle];
        btnTextStyle = [...btnTextStyle, Warning.btnTextStyle];
        btnIconStyle = [...btnIconStyle, Warning.btnIconStyle];
        btnActiveStyle = [...btnActiveStyle, Warning.btnActiveStyle];
        btnActiveTextStyle = [...btnActiveTextStyle, Warning.btnActiveTextStyle];
        btnActiveIconStyle = [...btnActiveIconStyle, Warning.btnActiveIconStyle];
    }

    // Error
    const Error = {
        btnStyle: buttonStyles.buttonError,
        btnTextStyle: globalStyles.textDark,
        btnIconStyle: globalStyles.textDark,
        btnActiveStyle: buttonStyles.buttonError,
        btnActiveTextStyle: globalStyles.textDark,
        btnActiveIconStyle: globalStyles.textDark,
    }

    if (type === ButtonType.Error) {
        console.log('Applying Error styles');

        btnStyle = [...btnStyle, Error.btnStyle];
        btnTextStyle = [...btnTextStyle, Error.btnTextStyle];
        btnIconStyle = [...btnIconStyle, Error.btnIconStyle];
        btnActiveStyle = [...btnActiveStyle, Error.btnActiveStyle];
        btnActiveTextStyle = [...btnActiveTextStyle, Error.btnActiveTextStyle];
        btnActiveIconStyle = [...btnActiveIconStyle, Error.btnActiveIconStyle];
    }

    // Function
    const Function = {
        btnStyle: buttonStyles.buttonFunction,
        btnTextStyle: globalStyles.textDark,
        btnIconStyle: globalStyles.textDark,
        btnActiveStyle: buttonStyles.buttonFunction,
        btnActiveTextStyle: globalStyles.textDark,
        btnActiveIconStyle: globalStyles.textDark,
    }

    if (type === ButtonType.Function) {
        console.log('Applying Function styles');

        btnStyle = [...btnStyle, Function.btnStyle];
        btnTextStyle = [...btnTextStyle, Function.btnTextStyle];
        btnIconStyle = [...btnIconStyle, Function.btnIconStyle];
        btnActiveStyle = [...btnActiveStyle, Function.btnActiveStyle];
        btnActiveTextStyle = [...btnActiveTextStyle, Function.btnActiveTextStyle];
        btnActiveIconStyle = [...btnActiveIconStyle, Function.btnActiveIconStyle];
    }

    // ! ||--------------------------------------------------------------------------------||
    // ! ||                                      Size                                      ||
    // ! ||--------------------------------------------------------------------------------||

    // Large
    const Large = {
        btnStyle: [buttonStyles.textButtonLarge, Gap.gapMedium],
        btnTextStyle: [globalStyles.buttonTextOne],
        btnActiveStyle: [buttonStyles.textButtonLarge, Gap.gapMedium], 
        btnActiveTextStyle: [globalStyles.buttonTextOne],
    }

    if (size === ButtonSize.Large) {
        console.log('Applying Large styles')

        btnStyle = [...btnStyle, ...Large.btnStyle];
        btnTextStyle = [...btnTextStyle, ...Large.btnTextStyle];
        btnActiveStyle = [...btnActiveStyle, ...Large.btnActiveStyle];
        btnActiveTextStyle = [...btnActiveTextStyle, ...Large.btnActiveTextStyle];
    }

    // Medium
    const Medium = {
        btnStyle: [buttonStyles.textButtonMedium, Gap.gapMedium],
        btnTextStyle: [globalStyles.buttonTextOne],
        btnActiveStyle: [buttonStyles.textButtonMedium, Gap.gapMedium],
        btnActiveTextStyle: [globalStyles.buttonTextOne],
    }

    if (size === ButtonSize.Medium) {
        console.log('Applying Medium styles')

        btnStyle = [...btnStyle, ...Medium.btnStyle];
        btnTextStyle = [...btnTextStyle, ...Medium.btnTextStyle];
        btnActiveStyle = [...btnActiveStyle, ...Medium.btnActiveStyle];
        btnActiveTextStyle = [...btnActiveTextStyle, ...Medium.btnActiveTextStyle];
    }

    // Small
    const Small = {
        btnStyle: [buttonStyles.textButtonSmall, Gap.gapMedium],
        btnTextStyle: [globalStyles.buttonTextTwo],
        btnActiveStyle: [buttonStyles.textButtonSmall, Gap.gapMedium],
        btnActiveTextStyle: [globalStyles.buttonTextTwo],
    }

    if (size === ButtonSize.Small) {
        console.log('Applying Small styles')

        btnStyle = [...btnStyle, ...Small.btnStyle];
        btnTextStyle = [...btnTextStyle, ...Small.btnTextStyle];
        btnActiveStyle = [...btnActiveStyle, ...Small.btnActiveStyle];
        btnActiveTextStyle = [...btnActiveTextStyle, ...Small.btnActiveTextStyle];
    }

    // Extra Small
    const ExtraSmall = {
        btnStyle: [buttonStyles.textButtonExtraSmall, Gap.gapMedium],
        btnTextStyle: [globalStyles.buttonTextThree],
        btnActiveStyle: [buttonStyles.textButtonExtraSmall, Gap.gapMedium],
        btnActiveTextStyle: [globalStyles.buttonTextThree],
    }
    
    if (size === ButtonSize.ExtraSmall) {
        console.log('Applying Extra Small styles')

        btnStyle = [...btnStyle, ...ExtraSmall.btnStyle];
        btnTextStyle = [...btnTextStyle, ...ExtraSmall.btnTextStyle];
        btnActiveStyle = [...btnActiveStyle, ...ExtraSmall.btnActiveStyle];
        btnActiveTextStyle = [...btnActiveTextStyle, ...ExtraSmall.btnActiveTextStyle];
    }

    // ! ||--------------------------------------------------------------------------------||
    // ! ||                                     Display                                    ||
    // ! ||--------------------------------------------------------------------------------||

    // Contained
    const Contained = {
        btnStyle: buttonStyles.buttonContained,
        btnActiveStyle: buttonStyles.buttonContained,
    }

    if (display === ButtonDisplay.Contained) {
        console.log('Applying Contained styles')

        btnStyle = [...btnStyle, Contained.btnStyle];
        btnActiveStyle = [...btnActiveStyle, Contained.btnActiveStyle];
    }

    // Full
    const Full = {
        btnStyle: buttonStyles.buttonFull,
        btnActiveStyle: buttonStyles.buttonFull,
    }

    if (display === ButtonDisplay.Full) {
        console.log('Applying Full styles')

        btnStyle = [...btnStyle, Full.btnStyle];
        btnActiveStyle = [...btnActiveStyle, Full.btnActiveStyle];
    }

    // Logging checks
    console.log('btnStyle:', btnStyle);
    console.log('btnTextStyle:', btnTextStyle);
    console.log('btnActiveStyle:', btnActiveStyle);
    console.log('btnActiveTextStyle:', btnActiveTextStyle);

  const [isPressed, setIsPressed] = React.useState(false);
   
  const handlePressIn = () => {
    if (self != null && status != null && changeState != null)
    { 
      if (status == self)
        changeState(-1);
      else
        changeState(self);
    }
    else
    {
      setIsPressed(true);
      if (onPress != null)
      { onPress(); }
    }
  };
  
  return (
    <Pressable
      onPress={(handlePressIn)}
      style={[(self == status) || isPressed ? btnActiveStyle : btnStyle, styles.btnBase]}
      disabled={disabled}>
      {btnIcon && <Icon style={btnIconStyle} name={btnIcon} /> }
        <Text style={(self == status) || isPressed ? btnActiveTextStyle : btnTextStyle}>{btnText}</Text>
    </Pressable>
  )
}

// ! ||--------------------------------------------------------------------------------||
// ! ||                                 Exported ENUMS                                 ||
// ! ||--------------------------------------------------------------------------------||

export enum ButtonType {
  PrimaryDark = "PrimaryDark",
  PrimaryLight = "PrimaryLight",
  Secondary = "Secondary",
  Tertiary = "Tertiary",
  Success = "Success",
  Warning = "Warning",
  Error = "Error",
  Function = "Function",
}

export enum ButtonSize {
  Large = "Large",
  Medium = "Medium",
  Small = "Small",
  ExtraSmall = "ExtraSmall",
}

export enum ButtonDisplay {
  Contained = "Contained",
  Full = "Full",
}

const styles = StyleSheet.create ({
    btnBase: {
      flexDirection: 'row',
      alignItems: 'baseline',
    },
  })
  
export default TestButton;