//#region imports
import React, { useState } from 'react';
import { View, StyleProp, ViewStyle } from 'react-native';
import Button, { ButtonProps } from './Button';
import Animated, {useSharedValue, ReduceMotion, withSpring } from 'react-native-reanimated';
// #endregion

interface ExclusiveButtonViewProps {
    groupStyle?: StyleProp<ViewStyle>
    buttons?: ButtonProps[];
    activeButton?: number;
    setActiveButton?: React.Dispatch<React.SetStateAction<number>>;
  }

export const ExclusiveButtonView: React.FC<ExclusiveButtonViewProps> = 
(
    {
        groupStyle = null,
        buttons = [],
        activeButton = -1,
        setActiveButton
    }
) => 
{
   return  (
    <View style = {groupStyle}>
        {
        buttons.map(
            (button) =>  
                <Button
                    id ={button.id}
                    current = {activeButton}
                    setCurrent = {setActiveButton}
                    type={button.type}
                    size={button.size}
                    display={button.display}
                    text={button.text}
                    icon={button.icon}
                    onPress={button.onPress}
                />
        )
        }
    </View> 
    );
};

export default ExclusiveButtonView;
