import { StyleSheet } from 'react-native';
import { Colors } from './Colors';
import { Spacing } from './Spacing';

export const buttonStyles = StyleSheet.create({
  // Buttons
  // Primary
  buttonPrimaryDark: {
    backgroundColor: Colors.sparrowDarkBrown,
    borderColor: Colors.sparrowDarkBrown,
  },

  buttonPrimaryDarkSelected: {
    backgroundColor: Colors.sparrowBrown,
    borderColor: Colors.sparrowBrown,
  },

  // Secondary
  buttonSecondaryDark: {
    borderColor: Colors.sparrowDarkBrown,
  },

  // TODO keep selected or rename to active?
  buttonSecondaryDarkSelected: {
    backgroundColor: Colors.sparrowDarkBrown,
    borderColor: Colors.sparrowDarkBrown,
  },

  buttonSecondaryLight: {
    borderColor: Colors.sparrowSand,
  },

  buttonSecondaryLightSelected: {
    backgroundColor: Colors.sparrowSand,
    borderColor: Colors.sparrowSand,
  },

  // Button Tertiary
  buttonTertiary: {
    backgroundColor: Colors.sparrowBrown,
    borderColor: Colors.sparrowBrown,
  },

  // Success
  buttonSuccess: {
    backgroundColor: Colors.green400,
    borderColor: Colors.sparrowDarkBrown,
  },

  // Warning
  buttonWarning: {
    backgroundColor: Colors.orange400,
    borderColor: Colors.sparrowDarkBrown,
  },

  // Error
  buttonError: {
    backgroundColor: Colors.red400,
    borderColor: Colors.sparrowDarkBrown,
  },

  // Function
  buttonFunction: {
    backgroundColor: Colors.turqoise300,
    borderColor: Colors.sparrowDarkBrown,
  },

  // Layout
  // Makes the button width the size of the contents
  buttonContained: {
    alignSelf: 'flex-start',
  },

  // Makes the button full-width
  buttonFull: {
    flex: 1,
  },

  // Icon buttons - buttons with icons
  // These styles currently work in conjuction
  iconButtonSmall: {
    flexDirection: 'row',
    // TODO going to use spacing.sm instead of md for now cause I think it looks better, update Figma later if this is the final setting
    columnGap: Spacing.sm,
  },

  // TODO check and delete borderColor property if it's unused

  // Sizes
  // Large
  // Pair with buttonTextOne
  textButtonLarge: {
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 24,
    paddingHorizontal: 32,
    borderRadius: 8,
    borderWidth: 2,
    // borderColor: Colors.sparrowDarkBrown,
  },

  // Medium
  // Pair with buttonTextOne
  textButtonMedium: {
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 16,
    paddingHorizontal: 24,
    borderRadius: 8,
    borderWidth: 2,
    // borderColor: Colors.sparrowDarkBrown,
  },

  // Small
  // Pair with buttonTextTwo
  textButtonSmall: {
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 8,
    paddingHorizontal: 16,
    borderWidth: 2,
    // borderColor: Colors.sparrowDarkBrown,
    borderRadius: 8,
  },

  // Extra small
  // Pair with buttonTextThree
  textButtonExtraSmall: {
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 8,
    paddingHorizontal: 16,
    borderRadius: 8,
    borderWidth: 2,
    // borderColor: Colors.sparrowDarkBrown,
    columnGap: Spacing.sm,
  },

  // Button icons
  // Small
  buttonIconSmallLight: {
    fontSize: 24,
    // Fix for icon being cut off
    height: 24,
    color: Colors.sparrowSand,
  },

  buttonIconSmallDark: {
    fontSize: 24,
    height: 24,
    color: Colors.sparrowDarkBrown,
  },

  // Medium
  buttonIconMedium: {
    fontSize: 32,
    // Fix for icon being cut off
    height: 32,
    color: Colors.sparrowSand,
  },

  buttonIconMediumDark: {
    fontSize: 32,
    height: 32,
    color: Colors.sparrowDarkBrown,
  },
});
