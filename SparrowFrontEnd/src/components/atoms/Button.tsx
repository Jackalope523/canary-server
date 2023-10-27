import { StyleSheet, Text, Pressable, StyleProp, ViewStyle, TextStyle } from 'react-native'
import * as React from 'react'

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';

const Icon = createIconSetFromFontello(fontelloConfig);

interface ButtonProps {
  onPress: () => void;
  btnText: string;
  btnStyle: StyleProp<ViewStyle>;

  // Active styles
  btnActiveStyle: StyleProp<ViewStyle>;
  btnActiveTextStyle?: StyleProp<TextStyle>;
  btnActiveIconStyle?: StyleProp<ViewStyle>;


  btnTextStyle?: StyleProp<TextStyle>;
  btnIcon?: string;
  btnIconStyle?: StyleProp<ViewStyle>;

  self?:number;
  status?:number;
  changeState?:(myNumber:number)=>void

}

// TODO conditional button styling
//  1. change styles to Active styles when button is pressed/selected

const Button: React.FC<ButtonProps> = ({onPress, btnText, btnStyle, btnTextStyle, btnIcon, btnIconStyle, btnActiveStyle, btnActiveTextStyle, btnActiveIconStyle, self,status,changeState}) => {
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
      style={[(self == status) || isPressed ? btnActiveStyle : btnStyle, styles.btnBase]}>
      {btnIcon && <Icon style={btnIconStyle} name={btnIcon} /> }
        <Text style={(self == status) || isPressed ? btnActiveTextStyle : btnTextStyle}>{btnText}</Text>
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