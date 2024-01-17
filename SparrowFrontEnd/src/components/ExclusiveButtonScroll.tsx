import React, {useState} from 'react';
import {ScrollView, ScrollViewProps} from 'react-native';
import Button, {ButtonProps} from './Button';

interface ExclusiveButtonScrollProps {
    props?: ScrollViewProps;
    buttons?: ButtonProps[];
  }

export const ExclusiveButtonGroup: React.FC<ExclusiveButtonScrollProps> = 
(
    {
        props = null,
        buttons = []
    }
) => 
{
    const [current, setCurrent] = useState(-1);

   return  (
    <ScrollView 
        horizontal={props?.horizontal}
        overScrollMode={props?.overScrollMode}
        showsHorizontalScrollIndicator={props?.showsHorizontalScrollIndicator}
        contentContainerStyle={props?.contentContainerStyle}
    >
        {
            buttons.map(
                (button) =>  
                    <Button
                      id ={button.id}
                      current = {current}
                      setCurrent = {setCurrent}
                      type={button.type}
                      size={button.size}
                      display={button.display}
                      text={button.text}
                      icon={button.icon}
                      onPress={button.onPress}
                    />
                    )
        }
    </ScrollView> 
    );
};

export default ExclusiveButtonGroup;