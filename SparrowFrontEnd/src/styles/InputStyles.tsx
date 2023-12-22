import { Dimensions, StyleSheet } from 'react-native';
import { Colors } from './ColorStyles';
import { Spacing } from './SpacingStyles';

// In Figma: Design System -> Input fields and selectors -> Text Input (label)
// TODO currently not in use - either delete or use instead of styling in the same file as the tsx code

export const inputStyles = StyleSheet.create({
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                   Text Input                                   ||
  // ! ||--------------------------------------------------------------------------------||
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Small                                     ||
  // ! ||--------------------------------------------------------------------------------||

  textInputSmallWrapper: {
    rowGap: Spacing.xs,
  },

  textInputSmallRest: {
    padding: 16,
    backgroundColor: Colors.orange400,
  },

  textInputSmallSelected: {
    padding: 16,
    backgroundColor: Colors.orange400,
  },

  textInputSmallDisabled: {
    padding: 16,
    backgroundColor: Colors.orange400,
  },
});
