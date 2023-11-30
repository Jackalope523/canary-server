import * as React from 'react'
import { StyleSheet, Text, Pressable, StyleProp, ViewStyle, TextStyle } from 'react-native'
import { globalStyles } from '../styles/Global';
import { buttonStyles } from '../styles/Buttons';
import { Spacing } from '../styles/Spacing';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';

const Icon = createIconSetFromFontello(fontelloConfig);

interface ButtonProps {
  onPress?: () => void;
  btnText?: string;
  btnStyle?: StyleProp<ViewStyle>;

  // Active styles
  btnActiveStyle?: StyleProp<ViewStyle>;
  btnActiveTextStyle?: StyleProp<TextStyle>;
  btnActiveIconStyle?: StyleProp<ViewStyle>;


  btnTextStyle?: StyleProp<TextStyle>;
  btnIcon?: string;
  btnIconStyle?: StyleProp<TextStyle>;

  self?:number;
  status?:number;
  changeState?:(myNumber:number)=>void;

  disabled?: boolean;
}

// TODO conditional button styling
//  1. change styles to Active styles when button is pressed/selected

const Button: React.FC<ButtonProps> = ({
  onPress, 
  btnText, 
  btnStyle, 
  btnTextStyle, 
  btnIcon, 
  btnIconStyle, 
  btnActiveStyle, 
  btnActiveTextStyle, 
  btnActiveIconStyle, 
  self,
  status,
  changeState,
  disabled
  }) => {
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
// ! ||                                     Primary                                    ||
// ! ||--------------------------------------------------------------------------------||

// Large
export const ButtonPrimaryLargeContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle = [buttonStyles.textButtonLarge, buttonStyles.buttonContained, buttonStyles.buttonPrimaryDark, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textLight], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonLarge, buttonStyles.buttonContained, buttonStyles.buttonPrimaryDark, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textLight], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonPrimaryLargeFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonLarge, buttonStyles.buttonFull, buttonStyles.buttonPrimaryDark, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textLight], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonLarge, buttonStyles.buttonFull, buttonStyles.buttonPrimaryDark, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textLight], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Medium
export const ButtonPrimaryMediumContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonMedium, buttonStyles.buttonContained, buttonStyles.buttonPrimaryDark, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textLight], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonMedium, buttonStyles.buttonContained, buttonStyles.buttonPrimaryDark, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textLight], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonPrimaryMediumFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonMedium, buttonStyles.buttonFull, buttonStyles.buttonPrimaryDark, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textLight], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonMedium, buttonStyles.buttonFull, buttonStyles.buttonPrimaryDark, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textLight], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Small
export const ButtonPrimarySmallContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonSmall, buttonStyles.buttonContained, buttonStyles.buttonPrimaryDark, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextTwo, globalStyles.textLight], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonSmall, buttonStyles.buttonContained, buttonStyles.buttonPrimaryDark, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextTwo, globalStyles.textLight], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonPrimarySmallFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonSmall, buttonStyles.buttonFull, buttonStyles.buttonPrimaryDark, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextTwo, globalStyles.textLight], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonSmall, buttonStyles.buttonFull, buttonStyles.buttonPrimaryDark, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextTwo, globalStyles.textLight], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Extra small
export const ButtonPrimaryExtraSmallContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonExtraSmall, buttonStyles.buttonContained, buttonStyles.buttonPrimaryDark, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextThree, globalStyles.textLight], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonExtraSmall, buttonStyles.buttonContained, buttonStyles.buttonPrimaryDark, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextThree, globalStyles.textLight], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonPrimaryExtraSmallFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonExtraSmall, buttonStyles.buttonFull, buttonStyles.buttonPrimaryDark, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextThree, globalStyles.textLight], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonExtraSmall, buttonStyles.buttonFull, buttonStyles.buttonPrimaryDark, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextThree, globalStyles.textLight], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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
// ! ||                                    Secondary                                   ||
// ! ||--------------------------------------------------------------------------------||

// Large
export const ButtonSecondaryLargeContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonLarge, buttonStyles.buttonContained, buttonStyles.buttonSecondary, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonLarge, buttonStyles.buttonContained, buttonStyles.buttonSecondary, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonSecondaryLargeFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonLarge, buttonStyles.buttonFull, buttonStyles.buttonSecondary, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonLarge, buttonStyles.buttonFull, buttonStyles.buttonSecondary, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Medium
export const ButtonSecondaryMediumContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonMedium, buttonStyles.buttonContained, buttonStyles.buttonSecondary, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonMedium, buttonStyles.buttonContained, buttonStyles.buttonSecondary, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonSecondaryMediumFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonMedium, buttonStyles.buttonFull, buttonStyles.buttonSecondary, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonMedium, buttonStyles.buttonFull, buttonStyles.buttonSecondary, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Small
export const ButtonSecondarySmallContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonSmall, buttonStyles.buttonContained, buttonStyles.buttonSecondary, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonSmall, buttonStyles.buttonContained, buttonStyles.buttonSecondary, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonSecondarySmallFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonSmall, buttonStyles.buttonFull, buttonStyles.buttonSecondary, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonSmall, buttonStyles.buttonFull, buttonStyles.buttonSecondary, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Extra small
export const ButtonSecondaryExtraSmallContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonExtraSmall, buttonStyles.buttonContained, buttonStyles.buttonSecondary, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextThree, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonExtraSmall, buttonStyles.buttonContained, buttonStyles.buttonSecondary, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextThree, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonSecondaryExtraSmallFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonExtraSmall, buttonStyles.buttonFull, buttonStyles.buttonSecondary, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextThree, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonExtraSmall, buttonStyles.buttonFull, buttonStyles.buttonSecondary, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextThree, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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
// ! ||                                    Tertiary                                    ||
// ! ||--------------------------------------------------------------------------------||

// Large
export const ButtonTertiaryLargeContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonLarge, buttonStyles.buttonContained, buttonStyles.buttonTertiary, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textLight], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonLarge, buttonStyles.buttonContained, buttonStyles.buttonTertiary, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textLight], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonTertiaryLargeFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonLarge, buttonStyles.buttonFull, buttonStyles.buttonTertiary, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textLight], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonLarge, buttonStyles.buttonFull, buttonStyles.buttonTertiary, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textLight], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Medium
export const ButtonTertiaryMediumContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonMedium, buttonStyles.buttonContained, buttonStyles.buttonTertiary, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textLight], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonMedium, buttonStyles.buttonContained, buttonStyles.buttonTertiary, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textLight], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonTertiaryMediumFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonMedium, buttonStyles.buttonFull, buttonStyles.buttonTertiary, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textLight], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonMedium, buttonStyles.buttonFull, buttonStyles.buttonTertiary, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textLight], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Small
export const ButtonTertiarySmallContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonSmall, buttonStyles.buttonContained, buttonStyles.buttonTertiary, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextTwo, globalStyles.textLight], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonSmall, buttonStyles.buttonContained, buttonStyles.buttonTertiary, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextTwo, globalStyles.textLight], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonTertiarySmallFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonSmall, buttonStyles.buttonFull, buttonStyles.buttonTertiary, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextTwo, globalStyles.textLight], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonSmall, buttonStyles.buttonFull, buttonStyles.buttonTertiary, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextTwo, globalStyles.textLight], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Extra small
export const ButtonTertiaryExtraSmallContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonExtraSmall, buttonStyles.buttonContained, buttonStyles.buttonTertiary, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextThree, globalStyles.textLight], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonExtraSmall, buttonStyles.buttonContained, buttonStyles.buttonTertiary, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextThree, globalStyles.textLight], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonTertiaryExtraSmallFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonExtraSmall, buttonStyles.buttonFull, buttonStyles.buttonTertiary, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextThree, globalStyles.textLight], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonExtraSmall, buttonStyles.buttonFull, buttonStyles.buttonTertiary, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextThree, globalStyles.textLight], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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
// ! ||                                     Success                                    ||
// ! ||--------------------------------------------------------------------------------||

// Large
export const ButtonSuccessLargeContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonLarge, buttonStyles.buttonContained, buttonStyles.buttonSuccess, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonLarge, buttonStyles.buttonContained, buttonStyles.buttonSuccess, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonSuccessLargeFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonLarge, buttonStyles.buttonFull, buttonStyles.buttonSuccess, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonLarge, buttonStyles.buttonFull, buttonStyles.buttonSuccess, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Medium
export const ButtonSuccessMediumContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonMedium, buttonStyles.buttonContained, buttonStyles.buttonSuccess, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonMedium, buttonStyles.buttonContained, buttonStyles.buttonSuccess, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonSuccessMediumFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonMedium, buttonStyles.buttonFull, buttonStyles.buttonSuccess, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonMedium, buttonStyles.buttonFull, buttonStyles.buttonSuccess, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Small
export const ButtonSuccessSmallContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonSmall, buttonStyles.buttonContained, buttonStyles.buttonSuccess, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonSmall, buttonStyles.buttonContained, buttonStyles.buttonSuccess, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonSuccessSmallFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonSmall, buttonStyles.buttonFull, buttonStyles.buttonSuccess, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonSmall, buttonStyles.buttonFull, buttonStyles.buttonSuccess, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Extra small
export const ButtonSuccessExtraSmallContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonExtraSmall, buttonStyles.buttonContained, buttonStyles.buttonSuccess, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextThree, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonExtraSmall, buttonStyles.buttonContained, buttonStyles.buttonSuccess, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextThree, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonSuccessExtraSmallFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonExtraSmall, buttonStyles.buttonFull, buttonStyles.buttonSuccess, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextThree, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonExtraSmall, buttonStyles.buttonFull, buttonStyles.buttonSuccess, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextThree, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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
// ! ||                                     Warning                                    ||
// ! ||--------------------------------------------------------------------------------||

// Large
export const ButtonWarningLargeContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonLarge, buttonStyles.buttonContained, buttonStyles.buttonWarning, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonLarge, buttonStyles.buttonContained, buttonStyles.buttonWarning, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonWarningLargeFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonLarge, buttonStyles.buttonFull, buttonStyles.buttonWarning, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonLarge, buttonStyles.buttonFull, buttonStyles.buttonWarning, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Medium
export const ButtonWarningMediumContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonMedium, buttonStyles.buttonContained, buttonStyles.buttonWarning, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonMedium, buttonStyles.buttonContained, buttonStyles.buttonWarning, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonWarningMediumFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonMedium, buttonStyles.buttonFull, buttonStyles.buttonWarning, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonMedium, buttonStyles.buttonFull, buttonStyles.buttonWarning, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Small
export const ButtonWarningSmallContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonSmall, buttonStyles.buttonContained, buttonStyles.buttonWarning, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonSmall, buttonStyles.buttonContained, buttonStyles.buttonWarning, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonWarningSmallFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonSmall, buttonStyles.buttonFull, buttonStyles.buttonWarning, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonSmall, buttonStyles.buttonFull, buttonStyles.buttonWarning, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Extra small
export const ButtonWarningExtraSmallContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonExtraSmall, buttonStyles.buttonContained, buttonStyles.buttonWarning, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextThree, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonExtraSmall, buttonStyles.buttonContained, buttonStyles.buttonWarning, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextThree, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonWarningExtraSmallFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonExtraSmall, buttonStyles.buttonFull, buttonStyles.buttonWarning, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextThree, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonExtraSmall, buttonStyles.buttonFull, buttonStyles.buttonWarning, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextThree, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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
// ! ||                                      Error                                     ||
// ! ||--------------------------------------------------------------------------------||

// Large
export const ButtonErrorLargeContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonLarge, buttonStyles.buttonContained, buttonStyles.buttonError, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonLarge, buttonStyles.buttonContained, buttonStyles.buttonError, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonErrorLargeFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonLarge, buttonStyles.buttonFull, buttonStyles.buttonError, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonLarge, buttonStyles.buttonFull, buttonStyles.buttonError, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Medium
export const ButtonErrorMediumContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonMedium, buttonStyles.buttonContained, buttonStyles.buttonError, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonMedium, buttonStyles.buttonContained, buttonStyles.buttonError, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonErrorMediumFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonMedium, buttonStyles.buttonFull, buttonStyles.buttonError, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonMedium, buttonStyles.buttonFull, buttonStyles.buttonError, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Small
export const ButtonErrorSmallContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonSmall, buttonStyles.buttonContained, buttonStyles.buttonError, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonSmall, buttonStyles.buttonContained, buttonStyles.buttonError, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonErrorSmallFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonSmall, buttonStyles.buttonFull, buttonStyles.buttonError, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonSmall, buttonStyles.buttonFull, buttonStyles.buttonError, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Extra small
export const ButtonErrorExtraSmallContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonExtraSmall, buttonStyles.buttonContained, buttonStyles.buttonError, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextThree, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonExtraSmall, buttonStyles.buttonContained, buttonStyles.buttonError, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextThree, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonErrorExtraSmallFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonExtraSmall, buttonStyles.buttonFull, buttonStyles.buttonError, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextThree, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonExtraSmall, buttonStyles.buttonFull, buttonStyles.buttonError, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextThree, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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
// ! ||                                    Function                                    ||
// ! ||--------------------------------------------------------------------------------||

// Large
export const ButtonFunctionLargeContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonLarge, buttonStyles.buttonContained, buttonStyles.buttonFunction, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonLarge, buttonStyles.buttonContained, buttonStyles.buttonFunction, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonFunctionLargeFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonLarge, buttonStyles.buttonFull, buttonStyles.buttonFunction, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonLarge, buttonStyles.buttonFull, buttonStyles.buttonFunction, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Medium
export const ButtonFunctionMediumContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonMedium, buttonStyles.buttonContained, buttonStyles.buttonFunction, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonMedium, buttonStyles.buttonContained, buttonStyles.buttonFunction, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonFunctionMediumFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonMedium, buttonStyles.buttonFull, buttonStyles.buttonFunction, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextOne, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonMedium, buttonStyles.buttonFull, buttonStyles.buttonFunction, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextOne, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Small
export const ButtonFunctionSmallContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonSmall, buttonStyles.buttonContained, buttonStyles.buttonFunction, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonSmall, buttonStyles.buttonContained, buttonStyles.buttonFunction, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonFunctionSmallFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonSmall, buttonStyles.buttonFull, buttonStyles.buttonFunction, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonSmall, buttonStyles.buttonFull, buttonStyles.buttonFunction, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextTwo, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// Extra small
export const ButtonFunctionExtraSmallContained: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonExtraSmall, buttonStyles.buttonContained, buttonStyles.buttonFunction, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextThree, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonExtraSmall, buttonStyles.buttonContained, buttonStyles.buttonFunction, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextThree, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

export const ButtonFunctionExtraSmallFull: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonExtraSmall, buttonStyles.buttonFull, buttonStyles.buttonFunction, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextThree, globalStyles.textDark], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonExtraSmall, buttonStyles.buttonFull, buttonStyles.buttonFunction, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextThree, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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

// TODO delete the code below and update existing buttons
// ETHAN'S example
export const SmallDarkButton: React.FC<ButtonProps> = ({
  onPress = null, 
  btnText = "NULL", 
  btnStyle= [buttonStyles.textButtonExtraSmall, buttonStyles.buttonContained, buttonStyles.buttonSecondaryOutline, {margin: Spacing.md}],
  btnTextStyle= [globalStyles.buttonTextThree, globalStyles.textLight], 
  btnIcon = null, 
  btnIconStyle = null, 
  btnActiveStyle=[buttonStyles.textButtonExtraSmall, buttonStyles.buttonContained, buttonStyles.buttonSecondaryFill, {margin: Spacing.md}], 
  btnActiveTextStyle=[globalStyles.buttonTextThree, globalStyles.textDark], 
  btnActiveIconStyle = null, 
  self = null,
  status = null,
  changeState = null,
  disabled = null
  }) => {
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


const styles = StyleSheet.create ({
  btnBase: {
    flexDirection: 'row',
    alignItems: 'baseline',
  },
})

export default Button;