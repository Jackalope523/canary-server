import React, {useState, useEffect} from 'react';
import {ScrollView, ScrollViewProps, FlatList, View} from 'react-native';
import Button, {ButtonProps} from './Button';
import {Spacing} from '../styles/SpacingStyles';
import Animated, {useSharedValue, withTiming, Easing, ReduceMotion } from 'react-native-reanimated';

interface ExclusiveButtonScrollProps {
    props?: ScrollViewProps;
    buttons?: ButtonProps[];
    setCurrentValue?: React.Dispatch<React.SetStateAction<number|string|undefined>>;
  }

const ExclusiveButtonGroup: React.FC<ExclusiveButtonScrollProps> = 
(
    {
        props = null,
        buttons = [],
        setCurrentValue = () => {},
    }
) => 
{
    const [current, setCurrent] = useState(-1);

    function mapIdToValue(id: number): string | null 
    {
        let map: Map<number, string> = new Map();
        for (let i = 0; i < buttons.length; i++) {
            map.set(buttons[i].id ?? -1, buttons[i].text ?? "undefined");
        }
        return map.get(id) || null;
    }

    useEffect(() => {
        return setCurrentValue(mapIdToValue(current));
    }, [current]);

    const paddingTop = useSharedValue(10);

    useEffect(() => {
      paddingTop.value = withTiming(0, {
        duration: 2000,
        easing: Easing.elastic(2),
        reduceMotion: ReduceMotion.System,
      });
    });

   return  (
    <View>
      <FlatList
        horizontal={true}
        overScrollMode="never"
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
    </View>
  );
};

export default ExclusiveButtonGroup;
