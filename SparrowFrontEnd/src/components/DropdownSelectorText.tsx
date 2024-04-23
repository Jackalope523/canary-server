import { Pressable, StyleSheet, Text, View } from 'react-native';
import React from 'react';
import { globalStyles } from '../styles/GlobalStyles';

// Icons
import ChevronIcon from '../assets/icons/chevron-outline.svg';

import { Colors } from '../styles/ColorStyles';
import { Spacing } from '../styles/SpacingStyles';
import { borderRadius } from '../styles/BorderStyles';

interface DropdownSelectorTextProps {
  options: { id: number; text: string; onPress: () => void }[];
}

const DropdownSelectorText: React.FC<DropdownSelectorTextProps> = ({
  options,
  // other name 2
}) => {
  const [selected, setSelected] = React.useState(false);

  const selectedItem = options[0].text;

  return (
    <View style={styles.dropdownSelectorText}>
      <Pressable
        onPress={() => setSelected(!selected)}
        style={styles.selectedItem}>
        <Text style={[globalStyles.buttonTextOne, globalStyles.textDark]}>
          {selectedItem}
        </Text>
        <ChevronIcon
          height={24}
          width={24}
          fill={Colors.sparrowDarkBrown}
          style={
            selected
              ? { transform: [{ rotate: '180deg' }] }
              : { transform: [{ rotate: '0deg' }] }
          }
        />
      </Pressable>
      {selected ? (
      <View style={styles.dropdown}>
        {options
          .filter((item) => true)
          .map((item, index) => (
            
            <Pressable key={item.id} onPress={item.onPress}>
              <Text
                key={index}
                style={[globalStyles.buttonTextOne, globalStyles.textDark]}>
                {item.text}
              </Text>
            </Pressable>
            
          ))}
      </View>
    ) : null}
    </View>
  );
};

export default DropdownSelectorText;

const styles = StyleSheet.create({
  dropdownSelectorText: {},

  selectedItem: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.sm,
  },

  dropdown: {
    // marginTop: Spacing.sm,
    paddingRight: 10,
    paddingVertical: Spacing.md,
    rowGap: Spacing.sm,
    backgroundColor: Colors.sparrowDarkBrown,
    borderRadius: borderRadius.md,
    zIndex: 10,
    position: 'absolute',
    alignItems: 'center',
    top: '100%',
    alignSelf: 'flex-start',
    width: 110
  },
});
