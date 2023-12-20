import { StyleSheet } from 'react-native';
import { Colors } from './ColorStyles';
import { Spacing } from './SpacingStyles';

export const buttonStyles = StyleSheet.create({
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Buttons                                    ||
  // ! ||--------------------------------------------------------------------------------||
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Types                                     ||
  // ! ||--------------------------------------------------------------------------------||

  // Primary
  buttonPrimaryDark: {
    backgroundColor: Colors.sparrowDarkBrown,
    borderColor: Colors.sparrowDarkBrown,
  },

  buttonPrimaryDarkSelected: {
    backgroundColor: Colors.sparrowBrown,
    borderColor: Colors.sparrowDarkBrown,
  },

  buttonPrimaryDarkDisasbled: {
    backgroundColor: Colors.sand200,
    borderColor: Colors.sand300,
  },

  // Secondary
  buttonSecondaryDark: {
    borderColor: Colors.sparrowDarkBrown,
  },

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

  /*
  
  Used for both Dark and Light Secondary buttons

  */

  buttonSecondaryDisabled: {
    backgroundColor: Colors.sand100,
    borderColor: Colors.sand300,
  },

  // Button Tertiary
  buttonTertiary: {
    backgroundColor: Colors.sparrowBrown,
    borderColor: Colors.sparrowBrown,
  },

  buttonTertiarySelected: {
    backgroundColor: Colors.sparrowDarkBrown,
    borderColor: Colors.sparrowDarkBrown,
  },

  buttonTertiaryDisabled: {
    backgroundColor: Colors.sand200,
    borderColor: Colors.sand300,
  },

  // Success
  buttonSuccess: {
    backgroundColor: Colors.green400,
    borderColor: Colors.sparrowDarkBrown,
  },

  buttonSuccessDisabled: {
    backgroundColor: Colors.green100,
    borderColor: Colors.green300,
  },

  // Warning
  buttonWarning: {
    backgroundColor: Colors.orange400,
    borderColor: Colors.sparrowDarkBrown,
  },

  buttonWarningDisabled: {
    backgroundColor: Colors.orange100,
    borderColor: Colors.orange300,
  },

  // Error
  buttonError: {
    backgroundColor: Colors.red400,
    borderColor: Colors.sparrowDarkBrown,
  },

  buttonErrorDisabled: {
    backgroundColor: Colors.red100,
    borderColor: Colors.red300,
  },

  // Function
  buttonFunction: {
    backgroundColor: Colors.turqoise300,
    borderColor: Colors.sparrowDarkBrown,
  },

  buttonFunctionDisabled: {
    backgroundColor: Colors.turqoise100,
    borderColor: Colors.turqoise300,
  },

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Display                                    ||
  // ! ||--------------------------------------------------------------------------------||

  // Makes the button width the size of the contents
  buttonContained: {
    alignSelf: 'flex-start',
  },

  // Makes the button full-width
  buttonFull: {
    // flex: 1,
    alignSelf: 'stretch',
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
  },

  // TODO move to ICON STYLES file?
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
  buttonIconMediumLight: {
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
