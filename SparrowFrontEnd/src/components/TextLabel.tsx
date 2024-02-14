import * as React from 'react';
import { StyleSheet, Text, View, ViewStyle } from 'react-native';
import { globalStyles } from '../styles/GlobalStyles';
import { Spacing } from '../styles/SpacingStyles';
import { Colors } from '../styles/ColorStyles';
// Types
export interface TextLabelProps {
  text: string;
  type: LabelType;
  size: LabelSize;

  labelStyle?: ViewStyle[];
}

export const TextLabel: React.FC<TextLabelProps> = ({
  text = 'NULL',
  type,
  size,
  labelStyle = [],
}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Type                                      ||
  // ! ||--------------------------------------------------------------------------------||

  switch (type) {
    case LabelType.Primary:
      labelStyle = [styles.primary];
      break;

    case LabelType.Friend:
      labelStyle = [styles.friend];
      break;

    case LabelType.You:
      labelStyle = [styles.you];
      break;
  }

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Size                                      ||
  // ! ||--------------------------------------------------------------------------------||

  switch (size) {
    case LabelSize.Large:
      labelStyle = [...labelStyle, styles.large];
      break;

    case LabelSize.Small:
      labelStyle = [...labelStyle, styles.small];
      break;
  }

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                    Display                                     ||
  // ! ||--------------------------------------------------------------------------------||
  // TODO add display; similar to Button.tsx

  return (
    <View style={labelStyle}>
      <Text style={[globalStyles.labelTextOneAsTyped, globalStyles.textDark]}>
        {text}
      </Text>
    </View>
  );
};

// ! ||--------------------------------------------------------------------------------||
// ! ||                                 Exported enums                                 ||
// ! ||--------------------------------------------------------------------------------||

export enum LabelType {
  Primary,
  Friend,
  You,
}

export enum LabelSize {
  Large,
  Small,
}

// ! ||--------------------------------------------------------------------------------||
// ! ||                                     Styles                                     ||
// ! ||--------------------------------------------------------------------------------||

const styles = StyleSheet.create({
  // Type
  primary: {
    backgroundColor: Colors.sparrowSand,
    borderColor: Colors.sparrowDarkBrown,
    borderWidth: 2,
  },

  friend: {
    backgroundColor: Colors.yellow400,
    // borderColor: Colors.sparrowDarkBrown,
    // TODO check out this experimental border color
    borderColor: Colors.yellow700,
    borderWidth: 2,
  },

  you: {
    backgroundColor: Colors.orange400,
    borderColor: Colors.orange700,
    borderWidth: 2,
  },

  // Size
  large: {
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.sm,
    borderRadius: 100,
  },

  small: {
    paddingHorizontal: 12,
    paddingVertical: Spacing.xs,
    borderRadius: 100,
  },
});

export default TextLabel;
