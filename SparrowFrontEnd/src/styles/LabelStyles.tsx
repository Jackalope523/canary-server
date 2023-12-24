import { StyleSheet } from 'react-native';
import { Colors } from './ColorStyles';

export const labelStyles = StyleSheet.create({
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Labels                                     ||
  // ! ||--------------------------------------------------------------------------------||
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                  Number label                                  ||
  // ! ||--------------------------------------------------------------------------------||

  // Base
  numberLabel: {
    paddingHorizontal: 12,
    paddingVertical: 4,
    borderRadius: 100,
  },

  // Primary
  numberLabelPrimary: {
    backgroundColor: Colors.sparrowDarkBrown,
  },

  numberLabelPrimaryText: {
    color: Colors.sparrowSand,
  },

  // Secondary
  numberLabelSecondary: {
    borderWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
  },

  numberLabelSecondaryText: {
    color: Colors.sparrowDark,
  },

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                   Text label                                   ||
  // ! ||--------------------------------------------------------------------------------||

  // Large
  textLabelLarge: {
    paddingHorizontal: 16,
    paddingVertical: 8,
    borderRadius: 100,
  },

  textLabelLargeRest: {
    borderWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
  },

  textLabelLargeRestText: {
    color: Colors.sparrowDark,
  },

  textLabelLargeSelected: {
    backgroundColor: Colors.sparrowDarkBrown,
  },

  textLabelLargeSelectedText: {
    color: Colors.sparrowSand,
  },

  // Small
  textLabelSmall: {
    paddingHorizontal: 12,
    paddingVertical: 4,
    borderWidth: 2,
    borderRadius: 100,
    borderColor: Colors.sparrowDarkBrown,
  },

  textLabelSmallText: {
    color: Colors.sparrowDark,
  },

  textLabelSmallYou: {
    backgroundColor: Colors.orange400,
  },

  textLabelSmallFriend: {
    backgroundColor: Colors.yellow400,
  },
});
