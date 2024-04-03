import React, { useState } from 'react';
import { View, StyleProp, ViewStyle } from 'react-native';
import Button, { ButtonProps } from './Button';

interface ExclusiveButtonViewProps {
  groupStyle?: StyleProp<ViewStyle>;
  buttons?: ButtonProps[];
}

export const ExclusiveButtonView: React.FC<ExclusiveButtonViewProps> = ({
  groupStyle = null,
  buttons = [],
}) => {
  const [current, setCurrent] = useState(-1);

  return (
    <View style={groupStyle}>
      {buttons.map((button) => (
        <Button
          id={button.id}
          current={current}
          setCurrent={setCurrent}
          type={button.type}
          size={button.size}
          display={button.display}
          text={button.text}
          displayIcon={button.displayIcon}
          Icon={button.Icon}
          onPress={button.onPress}
        />
      ))}
    </View>
  );
};

export default ExclusiveButtonView;
