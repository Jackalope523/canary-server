import * as React from 'react';
import {
  StyleSheet,
  Text,
  Pressable,
  StyleProp,
  ViewStyle,
  TextStyle,
  View,
} from 'react-native';
import { globalStyles } from '../styles/GlobalStyles';
import { buttonStyles } from '../styles/ButtonStyles';
import { Spacing } from '../styles/SpacingStyles';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';
import { Colors } from '../styles/ColorStyles';

const Icon = createIconSetFromFontello(fontelloConfig);

// Types
interface RadioButtonProps {
  onPress?: () => void;
  text?: string;
}

/*

TODO animate:

1. RADIO - grow, shrink
2. BUTTON - fade in, fade out

*/

export const RadioButton: React.FC<RadioButtonProps> = ({
  onPress = null,
  text,
}) => {
  const [isSelected, setIsSelected] = React.useState(false);

  const TEMPDATA = [
    {
      id: 1,
      name: 'Accounting',
    },
    {
      id: 2,
      name: 'Art',
    },
    {
      id: 3,
      name: 'Biology',
    },
    {
      id: 4,
      name: 'Chemistry',
    },
  ];

  return (
    // <View style={styles.container}>
    <View>
      {TEMPDATA.map(() => (
        <Pressable
          onPress={() => {
            setIsSelected(!isSelected);
          }}
          style={
            isSelected ? [styles.button, styles.buttonSelected] : styles.button
          }>
          <View
            style={
              isSelected
                ? [styles.radioRest, styles.radioSelected]
                : styles.radioRest
            }
          />
          <Text
            style={
              isSelected
                ? [globalStyles.buttonTextTwo, globalStyles.textLight]
                : [globalStyles.buttonTextTwo, globalStyles.textDark]
            }>
            {text}
          </Text>
        </Pressable>
      ))}
    </View>
    // </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,

    alignItems: 'center',
    justifyContent: 'center',
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
    borderRadius: 9, // OR 18
  },

  radioSelected: {
    backgroundColor: Colors.sparrowSand,
  },
});

export default RadioButton;
