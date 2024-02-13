import { Pressable, StyleSheet, Text, View, ViewStyle } from 'react-native';
import React from 'react';
import { Colors } from '../styles/ColorStyles';
import { Spacing } from '../styles/SpacingStyles';
import { globalStyles } from '../styles/GlobalStyles';

/*

TODO test in various scenarios and customize as necessary

For ex. it's possible that when the kebab icon is in use,
we would need different layout that places the dropdown next to the kebab icon

*/

// Icons
import MeatballIcon from '../assets/icons/meatball-outline.svg';
import KebabIcon from '../assets/icons/kebab-fill.svg';

interface DropdownSmallProps {
  options: { id: string; text: string; onPress: () => void }[];
  icon: Icon;
  align: Align;

  containerStyle?: ViewStyle[];
  dropdownContainerStyle?: ViewStyle[];
}

const DropdownSmall = ({
  options,
  icon,
  align,
  containerStyle,
  dropdownContainerStyle,
}: DropdownSmallProps) => {
  const [selected, setSelected] = React.useState(false);

  // Alignment
  switch (align) {
    case Align.Left: {
      containerStyle = [styles.containerAlignLeft, styles.container];
      dropdownContainerStyle = [
        styles.dropdownContainerAlignLeft,
        styles.dropdownContainer,
      ];
      break;
    }

    case Align.Right: {
      containerStyle = [styles.containerAlignRight, styles.container];
      dropdownContainerStyle = [
        styles.dropdownContainerAlignRight,
        styles.dropdownContainer,
      ];
    }
  }

  return (
    <View style={containerStyle}>
      <Pressable onPress={() => setSelected(!selected)}>
        {icon === Icon.Meatball ? (
          <MeatballIcon height={24} width={24} fill={Colors.sparrowDarkBrown} />
        ) : (
          <KebabIcon height={24} width={24} fill={Colors.sparrowDarkBrown} />
        )}
      </Pressable>
      {selected ? (
        <View>
          <View style={dropdownContainerStyle}>
            {options.map((item, index) => (
              <Pressable key={item.id} onPress={item.onPress}>
                <Text
                  key={index}
                  style={[
                    globalStyles.buttonTextOne,
                    globalStyles.textLight,
                    styles.text,
                  ]}>
                  {item.text}
                </Text>
              </Pressable>
            ))}
          </View>
        </View>
      ) : null}
    </View>
  );
};

export default DropdownSmall;

// Exported enums
export enum Icon {
  Meatball,
  Kebab,
}

export enum Align {
  Left,
  Right,
}

const styles = StyleSheet.create({
  container: {
    zIndex: 10,
  },

  dropdownContainer: {
    backgroundColor: Colors.sparrowDarkBrown,
    paddingHorizontal: Spacing.lg,
    paddingVertical: Spacing.md,
    rowGap: Spacing.sm,
    borderRadius: 8,
    zIndex: 10,
    position: 'absolute',
    marginTop: Spacing.xs,
  },

  containerAlignRight: {
    alignItems: 'flex-start',
  },

  containerAlignLeft: {
    alignItems: 'flex-end',
  },

  dropdownContainerAlignRight: {
    left: 0,
  },

  dropdownContainerAlignLeft: {
    right: 0,
  },

  text: { textTransform: 'capitalize' },
});
