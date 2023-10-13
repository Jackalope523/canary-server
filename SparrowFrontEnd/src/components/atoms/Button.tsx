import { StyleSheet, Text, Pressable, StyleProp, ViewStyle, TextStyle } from 'react-native'
import React from 'react'

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';

const Icon = createIconSetFromFontello(fontelloConfig);

interface ButtonProps {
  onPress: () => void;
  btnText: string;
  btnStyle: StyleProp<ViewStyle>;

  btnActiveStyle: StyleProp<ViewStyle>;
  btnActiveTextStyle?: StyleProp<TextStyle>;
  btnActiveIconStyle?: StyleProp<ViewStyle>;


  btnTextStyle?: StyleProp<TextStyle>;
  btnIcon?: string;
  btnIconStyle?: StyleProp<ViewStyle>;
}

const Button: React.FC<ButtonProps> = ({onPress, btnText, btnStyle, btnTextStyle, btnIcon, btnIconStyle, btnActiveStyle, btnActiveTextStyle, btnActiveIconStyle}) => {

  const [isPressed, setIsPressed] = React.useState(false);

  const handlePress = () => {
    setIsPressed(!isPressed);
  };

  return (
    <Pressable onPress={handlePress} style={({ pressed }) => [
      btnStyle,
      styles.btnBase,
      isPressed ? btnActiveStyle : pressed && btnActiveStyle,
    ]}>
      {btnIcon && (
        <Icon style={({ pressed }) => [
          btnIconStyle,
          isPressed ? btnActiveIconStyle : pressed && btnActiveIconStyle,
        ]} name={btnIcon} />
      )}
        <Text style={({ pressed }) => [
          btnTextStyle,
          isPressed ? btnActiveTextStyle : pressed && btnActiveTextStyle,
        ]}>{btnText}</Text>
    </Pressable>
  )
}

// const Button: React.FC<ButtonProps> = ({onPress, btnText, btnStyle, btnTextStyle, btnIcon, btnIconStyle, btnActiveStyle, btnActiveTextStyle, btnActiveIconStyle}) => {
//   return (
//     <Pressable onPress={onPress} style={[btnStyle, styles.btnBase]}>
//       {btnIcon && (
//         <Icon style={btnIconStyle} name={btnIcon} />
//       )}
//         <Text style={btnTextStyle}>{btnText}</Text>
//     </Pressable>
//   )
// }

export default Button

const styles = StyleSheet.create ({
  btnBase: {
    flexDirection: 'row',
    alignItems: 'baseline',
  },
})