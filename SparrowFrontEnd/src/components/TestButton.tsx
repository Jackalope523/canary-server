import * as React from 'react'
import { StyleSheet, Text, Pressable, StyleProp, ViewStyle, TextStyle } from 'react-native'
import { globalStyles } from '../styles/Global';
import { buttonStyles } from '../styles/Buttons';
import { Gap, Spacing } from '../styles/Spacing';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';
import { Colors } from '../styles/Colors';

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

//   btnStyle?: StyleProp<ViewStyle>;
//   btnTextStyle?: StyleProp<TextStyle>;
//   btnIconStyle?: StyleProp<TextStyle>;

  // Active styles
  btnActiveStyle?: ViewStyle[];
  btnActiveTextStyle?: TextStyle[];
  btnActiveIconStyle?: TextStyle[];

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

    // Type
    // Primary
    const PrimaryDark = {
        btnStyle: buttonStyles.buttonPrimaryDark,
        btnTextStyle: globalStyles.textLight,
        btnIconStyle: globalStyles.textLight,
        btnActiveStyle: buttonStyles.buttonPrimaryDark,
        btnActiveTextStyle: globalStyles.textLight,
        btnActiveIconStyle: globalStyles.textLight,
    }

    if (type === "PrimaryDark") {
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

    if (type === "PrimaryLight") {
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

    if (type === "Secondary") {
        console.log('Applying Secondary styles');

        btnStyle = [...btnStyle, Secondary.btnStyle];
        btnTextStyle = [...btnTextStyle, Secondary.btnTextStyle];
        btnIconStyle = [...btnIconStyle, Secondary.btnIconStyle];
        btnActiveStyle = [...btnActiveStyle, Secondary.btnActiveStyle];
        btnActiveTextStyle = [...btnActiveTextStyle, Secondary.btnActiveTextStyle];
        btnActiveIconStyle = [...btnActiveIconStyle, Secondary.btnActiveIconStyle];
    }

    // Size
    const Large = {
        btnStyle: [buttonStyles.textButtonLarge, Gap.gapMedium],
        btnTextStyle: [globalStyles.buttonTextOne],
        btnActiveStyle: [buttonStyles.textButtonLarge, Gap.gapMedium], 
        btnActiveTextStyle: [globalStyles.buttonTextOne],
    }

    if (size === "Large") {
        console.log('Applying Large styles')

        btnStyle = [...btnStyle, ...Large.btnStyle];
        btnTextStyle = [...btnTextStyle, ...Large.btnTextStyle];
        btnActiveStyle = [...btnActiveStyle, ...Large.btnActiveStyle];
        btnActiveTextStyle = [...btnActiveTextStyle, ...Large.btnActiveTextStyle];
    }

    // Display
    const Contained = {
        btnStyle: buttonStyles.buttonContained,
        btnActiveStyle: buttonStyles.buttonContained,
    }

    if (display === "Contained") {
        console.log('Applying Contained styles')

        btnStyle = [...btnStyle, Contained.btnStyle];
        btnActiveStyle = [...btnActiveStyle, Contained.btnActiveStyle];
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

const styles = StyleSheet.create ({
    btnBase: {
      flexDirection: 'row',
      alignItems: 'baseline',
    },
  })
  
export default TestButton;