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
  options: { id: number; text: string; onPress: () => void }[];
  icon: Icon;
  align: Align;

  containerStyle?: ViewStyle[];
  buttonStyle?: ViewStyle[];
  dropdownContainerStyle?: ViewStyle[];
}

const DropdownSmall = ({
  options,
  icon,
  align,
  containerStyle,
  buttonStyle,
  dropdownContainerStyle,
}: DropdownSmallProps) => {
  const [selected, setSelected] = React.useState(false);

  // Alignment
  switch (align) {
    case Align.BottomLeft: {
      containerStyle = [styles.containerAlignBottomLeft, styles.container];
      dropdownContainerStyle = [
        styles.dropdownContainerAlignBottomLeft,
        styles.dropdownContainer,
      ];
      buttonStyle = [styles.buttonAlignBottomLeft];
      break;
    }

    case Align.BottomRight: {
      containerStyle = [styles.containerAlignBottomRight, styles.container];
      dropdownContainerStyle = [
        styles.dropdownContainerAlignBottomRight,
        styles.dropdownContainer,
      ];
      buttonStyle = [styles.buttonAlignBottomRight];
      break;
    }
  }

  return (
    <View style={containerStyle}>
      <Pressable onPress={() => setSelected(!selected)} style={buttonStyle}>
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
                  style={[globalStyles.buttonTextOne, globalStyles.textLight]}>
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
  BottomLeft,
  BottomRight,
}

const styles = StyleSheet.create({
  container: {
    zIndex: 10,
    flexDirection: 'row',
    columnGap: Spacing.sm,
  },

  dropdownContainer: {
    backgroundColor: Colors.sparrowDarkBrown,
    paddingHorizontal: Spacing.lg,
    paddingVertical: Spacing.md,
    rowGap: Spacing.sm,
    borderRadius: 8,
    zIndex: 10,
    position: 'absolute',
    alignItems: 'center',
  },

  containerAlignBottomRight: {},

  buttonAlignBottomRight: {
    position: 'absolute',
    left: 0,
  },

  dropdownContainerAlignBottomRight: {},

  containerAlignBottomLeft: {},

  buttonAlignBottomLeft: {
    position: 'absolute',
    right: 0,
  },

  dropdownContainerAlignBottomLeft: {
    right: 0,
    bottom: Spacing.xl,
  },
});
