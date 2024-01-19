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

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';
import { Colors } from '../styles/ColorStyles';

const Icon = createIconSetFromFontello(fontelloConfig);

// Types
interface RadioButtonProps {
  onPress: (item: string | GestureResponderEvent) => void;

  buttonText: string[] | React.ReactNode;
}

/*

TODO animate:

1. RADIO - grow, shrink
2. BUTTON - fade in, fade out

*/

export const RadioButton: React.FC<RadioButtonProps> = ({
  onPress,
  buttonText,
}) => {
  /*

  TODO disable the CONTINUE button until a selection is made;
  can maybe use this logic - selectedId >= 0 === true
  
  -1 for no default selection which should disable the CONTINUE button
  0 for first item as default selection

  */

  const [selectedId, setSelectedId] = React.useState(-1);

  const handleTap = (item: string | GestureResponderEvent, id: number) => {
    setSelectedId(id);
    onPress(item);

    console.log(`Button pressed: ${item}, ID: ${id}`);

    // setSelectedId(1);
    // onSelect(item);
  };

  // TODO replace view with flatlist

  return (
    <View style={styles.container}>
      {buttonText.map((buttonLabel, index) => (
        <Pressable
          onPress={(item) => handleTap(item, index)}
          key={index}
          style={
            index === selectedId
              ? [styles.button, styles.buttonSelected]
              : [styles.button]
          }>
          <View
            style={
              index === selectedId
                ? [styles.radioRest, styles.radioSelected]
                : [styles.radioRest]
            }
          />
          <Text
            style={
              index === selectedId
                ? [globalStyles.buttonTextTwo, globalStyles.textLight]
                : [globalStyles.buttonTextTwo, globalStyles.textDark]
            }>
            {buttonLabel}
          </Text>
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

  button: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: Spacing.md,
    borderColor: Colors.sparrowDarkBrown,
    borderWidth: 2,
    borderRadius: 8,
    paddingVertical: Spacing.sm,
    paddingHorizontal: Spacing.md,

    // Full-width with stretch; contained with flex-start
    alignSelf: 'stretch',
  },

  buttonSelected: {
    backgroundColor: Colors.sparrowDarkBrown,
  },

  radioRest: {
    height: 18,
    width: 18,
    borderColor: Colors.sparrowDarkBrown,
    borderWidth: 2,
    borderRadius: 18 / 2,
  },

  radioSelected: {
    backgroundColor: Colors.sparrowSand,
  },
});

export default RadioButton;
