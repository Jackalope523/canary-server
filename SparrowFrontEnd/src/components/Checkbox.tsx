import { Pressable, StyleSheet, Text, View } from 'react-native';
import { useState } from 'react';
import * as React from 'react';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';
import { Colors } from '../styles/ColorStyles';
import { globalStyles } from '../styles/GlobalStyles';
import { Spacing } from '../styles/SpacingStyles';

const Icon = createIconSetFromFontello(fontelloConfig);

interface CheckboxProps {
  text: string;
  onPress: () => void;
}

export const Checkbox: React.FC<CheckboxProps> = ({ text, onPress }) => {
  const [isChecked, setIsChecked] = useState(false);

  return (
    <View>
      <Pressable
        onPress={() => {
          setIsChecked(!isChecked);
        }}
        style={
          isChecked
            ? [styles.containerRest, styles.containerSelected]
            : styles.containerRest
        }>
        <View
          style={
            isChecked
              ? [styles.checkboxRest, styles.checkboxSelected]
              : styles.checkboxRest
          }>
          <Icon
            name="check-outline"
            size={20}
            color={Colors.sparrowDarkBrown}
            style={{ display: isChecked ? 'flex' : 'none' }}
          />
        </View>
        <View style={{ flex: 1 }}>
          <Text
            style={
              isChecked
                ? [globalStyles.buttonTextTwo, globalStyles.textLight]
                : [globalStyles.buttonTextTwo, globalStyles.textDark]
            }>
            {text}
          </Text>
        </View>
      </Pressable>
    </View>
  );
};

export default Checkbox;

const styles = StyleSheet.create({
  containerRest: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.md,
    paddingVertical: Spacing.sm,
    paddingHorizontal: Spacing.md,
    borderColor: Colors.sparrowDarkBrown,
    borderWidth: 2,
    borderRadius: 8,
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
