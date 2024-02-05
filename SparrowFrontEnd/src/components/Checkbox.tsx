import * as React from 'react';
import {
  StyleSheet,
  Text,
  Pressable,
  View,
  GestureResponderEvent,
} from 'react-native';
import { globalStyles } from '../styles/GlobalStyles';
import { Spacing } from '../styles/SpacingStyles';
import { Colors } from '../styles/ColorStyles';

// Icons
import CheckOutline from '../assets/icons/check-outline.svg';

// Types
interface CheckboxGroupProps {
  onPress: (item: string | GestureResponderEvent) => void;

  text: string[] | React.ReactNode;
}

/*

TODO animate:

1. CHECK - path

*/

export const CheckboxGroup: React.FC<CheckboxGroupProps> = ({
  onPress,
  text,
}) => {
  const [selectedIds, setSelectedIds] = React.useState<number[]>([]);

  const handleTap = (item: string | GestureResponderEvent, id: number) => {
    if (selectedIds.includes(id)) {
      // Deselect the item
      setSelectedIds(selectedIds.filter((selectedId) => selectedId !== id));
    } else {
      // Select the item
      setSelectedIds([...selectedIds, id]);
    }

    onPress(item);

    console.log(`Button pressed: ${item}, ID: ${id}`);
  };

  return (
    <View style={styles.container}>
      {text?.map((label: string, index: any) => (
        <Pressable
          onPress={(item) => handleTap(item, index)}
          key={index}
          style={
            selectedIds.includes(index)
              ? [styles.containerRest, styles.containerSelected]
              : styles.containerRest
          }>
          <View
            style={
              selectedIds.includes(index)
                ? [styles.checkboxRest, styles.checkboxSelected]
                : styles.checkboxRest
            }>
            <CheckOutline
              height={24}
              width={24}
              fill={Colors.sparrowDarkBrown}
              style={{ display: selectedIds.includes(index) ? 'flex' : 'none' }}
            />
          </View>
          <View style={{ flex: 1 }}>
            <Text
              style={
                selectedIds.includes(index)
                  ? [globalStyles.buttonTextTwo, globalStyles.textLight]
                  : [globalStyles.buttonTextTwo, globalStyles.textDark]
              }>
              {label}
            </Text>
          </View>
        </Pressable>
      ))}
    </View>
  );
};
const styles = StyleSheet.create({
  container: {
    gap: Spacing.md,

    // flex: 1,
  },

  containerRest: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.md,
    paddingVertical: Spacing.sm,
    paddingHorizontal: Spacing.md,
    borderColor: Colors.sparrowDarkBrown,
    borderWidth: 2,
    borderRadius: 8,
    alignSelf: 'stretch',
  },

  containerSelected: {
    backgroundColor: Colors.sparrowDarkBrown,
  },

  checkboxRest: {
    borderColor: Colors.sparrowDarkBrown,
    borderWidth: 2,
    borderRadius: 4,
    alignItems: 'center',
    justifyContent: 'center',
    height: 24,
    width: 24,
  },

  checkboxSelected: {
    backgroundColor: Colors.sparrowSand,
  },
});

export default CheckboxGroup;
