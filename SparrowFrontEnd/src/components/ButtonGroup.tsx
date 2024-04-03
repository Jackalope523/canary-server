import * as React from 'react';
import {
  Pressable,
  Text,
  StyleSheet,
  GestureResponderEvent,
} from 'react-native';
import { Gap, Spacing } from '../styles/SpacingStyles';
import { buttonStyles } from '../styles/ButtonStyles';
import { globalStyles } from '../styles/GlobalStyles';

import { ScrollView } from 'react-native-gesture-handler';
import { Colors } from '../styles/ColorStyles';

// Types
interface ButtonGroupProps {
  // onSelect: (item: string) => void;
  onSelect: (item: string | GestureResponderEvent) => void;

  buttonText: string[];
  // icon?: string | false;
  displayIcon: boolean;
}

export const ButtonGroup: React.FC<ButtonGroupProps> = ({
  onSelect,
  buttonText,
  displayIcon = false,
}) => {
  const [selectedId, setSelectedId] = React.useState(0);

  const handleTap = (item: string | GestureResponderEvent, id: number) => {
    setSelectedId(id);
    onSelect(item);
  };

  return (
    <ScrollView
      keyboardShouldPersistTaps="handled"
      contentContainerStyle={styles.container}
      overScrollMode="never"
      showsVerticalScrollIndicator={false}
      showsHorizontalScrollIndicator={false}
      horizontal={true}>
      {buttonText.map((buttonLabel, index) => {
        return (
          <Pressable
            onPress={(item) => handleTap(item, index)}
            key={index}
            style={
              index === selectedId
                ? [
                    buttonStyles.buttonSecondaryDarkSelected,
                    buttonStyles.textButtonMedium,
                    Gap.gapMedium,
                  ]
                : [
                    buttonStyles.buttonSecondaryDark,
                    buttonStyles.textButtonMedium,
                    Gap.gapMedium,
                  ]
            }>
            {/* {displayIcon && (
              <Icon
                name={icon}
                size={24}
                height={24}
                width={24}
                style={
                  index === selectedId
                    ? globalStyles.textLight
                    : globalStyles.textDark
                }
              />
            )} */}
            {displayIcon && (
              <Icon
                height={24}
                width={24}
                fill={
                  index === selectedId
                    ? Colors.sparrowSand
                    : Colors.sparrowDarkBrown
                }
              />
            )}

            <Text
              style={
                index === selectedId
                  ? [globalStyles.textLight, globalStyles.buttonTextOne]
                  : [globalStyles.textDark, globalStyles.buttonTextOne]
              }>
              {buttonLabel}
            </Text>
          </Pressable>
        );
      })}
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    gap: Spacing.md,
    // flex: 1,
    // flexDirection: 'row',
  },
});

export default ButtonGroup;
