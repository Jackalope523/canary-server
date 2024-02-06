import React, { useState } from 'react';
import { ScrollView, ScrollViewProps, FlatList, View } from 'react-native';
import Button, { ButtonProps } from './Button';
import { Spacing } from '../styles/SpacingStyles';

interface ExclusiveButtonScrollProps {
  props?: ScrollViewProps;
  buttons?: ButtonProps[];
}

export const ExclusiveButtonGroup: React.FC<ExclusiveButtonScrollProps> = ({
  props = null,
  buttons = [],
}) => {
  const [current, setCurrent] = useState(-1);

  return (
    <FlatList
      horizontal={true}
      showsHorizontalScrollIndicator={false}
      ItemSeparatorComponent={() => (
        <View style={{ paddingRight: Spacing.md }} />
      )}
      data={buttons}
      renderItem={({ item }) => (
        <Button
          id={item.id}
          current={current}
          setCurrent={setCurrent}
          type={item.type}
          size={item.size}
          display={item.display}
          text={item.text}
          displayIcon={item.displayIcon}
          Icon={item.Icon}
          onPress={item.onPress}
        />
      )}
    />
  );
};

export default ExclusiveButtonGroup;
