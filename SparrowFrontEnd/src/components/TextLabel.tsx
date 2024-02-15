import * as React from 'react';
import { StyleSheet, Text, TextStyle, View, ViewStyle } from 'react-native';
import { globalStyles } from '../styles/GlobalStyles';
import { Spacing } from '../styles/SpacingStyles';
import { Colors } from '../styles/ColorStyles';
// Types
export interface TextLabelProps {
  text: string;
  type: LabelType;
  size: LabelSize;
  display: LabelDisplay;

  labelStyle?: ViewStyle[];
  textStyle?: TextStyle[];
}

export const TextLabel: React.FC<TextLabelProps> = ({
  text = 'NULL',
  type,
  size,
  display,
  labelStyle = [],
  textStyle = [],
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
      textStyle = [globalStyles.labelTextOneAsTyped, globalStyles.textDark];
      break;

    case LabelSize.Small:
      labelStyle = [...labelStyle, styles.small];
      textStyle = [globalStyles.labelTextTwoAsTyped, globalStyles.textDark];
      break;
  }

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                    Display                                     ||
  // ! ||--------------------------------------------------------------------------------||

  switch (display) {
    case LabelDisplay.Contained:
      labelStyle = [...labelStyle, styles.contained];
      break;

    case LabelDisplay.Full:
      labelStyle = [...labelStyle, styles.full];
      break;
  }

  return (
    <View>
      <View style={labelStyle}>
        <Text style={textStyle}>{text}</Text>
      </View>
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

export enum LabelDisplay {
  Contained,
  Full,
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
    borderColor: Colors.sparrowDarkBrown,
    borderWidth: 2,
  },

  you: {
    backgroundColor: Colors.orange400,
    borderColor: Colors.sparrowDarkBrown,
    borderWidth: 2,
  },

  // Size
  large: {
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.sm,
    borderRadius: 100,
  },

  small: {
    paddingHorizontal: Spacing.mdsm,
    paddingVertical: Spacing.xs,
    borderRadius: 100,
  },

  // Display
  contained: {
    alignSelf: 'flex-start',
  },

  full: {
    alignSelf: 'stretch',
  },
});

export default TextLabel;
