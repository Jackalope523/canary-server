import { Dimensions, StyleSheet } from 'react-native';
import { Colors } from './ColorStyles';
import { Spacing } from './SpacingStyles';

const windowWidth = Dimensions.get('window').width;
const windowHeight = Dimensions.get('window').height;

export const globalStyles = StyleSheet.create({
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Layout                                     ||
  // ! ||--------------------------------------------------------------------------------||
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                   Containers                                   ||
  // ! ||--------------------------------------------------------------------------------||

  // Used for navigation container, tab navigator
  mainContainer: {
    backgroundColor: Colors.sparrowSand,
  },

  // Used for screens
  baseContainer: {
    // flex: 1,
    margin: 24,
  },

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                   Typography                                   ||
  // ! ||--------------------------------------------------------------------------------||

  // Base text
  baseText: {
    fontFamily: 'UncutSans-Regular',
  },

  // Display text
  displayTextOne: {
    fontSize: 72,
    fontFamily: 'UncutSans-Bold',
    // lineHeight: 80,
    // letterSpacing: -1.8,
  },

  displayTextTwo: {
    fontSize: 44,
    fontFamily: 'UncutSans-Bold',
    // lineHeight: 52,
    // letterSpacing: -1,
  },

  // Heading text
  headingTextOne: {
    fontSize: 36,
    fontFamily: 'UncutSans-Bold',
    // lineHeight: 44,
    // letterSpacing: -0.8,
  },

  headingTextTwo: {
    fontSize: 28,
    fontFamily: 'UncutSans-Bold',
    // lineHeight: 36,
    // letterSpacing: -0.4,
  },

  headingTextThree: {
    fontSize: 22,
    fontFamily: 'UncutSans-Bold',
    // lineHeight: 30,
    // letterSpacing: -0.25,
  },

  headingTextFour: {
    fontSize: 18,
    fontFamily: 'UncutSans-Bold',
    // lineHeight: 26,
    // letterSpacing: -0.25,
  },

  headingTextFive: {
    fontSize: 16,
    fontFamily: 'UncutSans-Bold',
    // lineHeight: 24,
    // letterSpacing: 0,
  },

  // Body text
  bodyTextOne: {
    fontSize: 16,
    fontFamily: 'UncutSans-Regular',
    // lineHeight: 24,
    // letterSpacing: 0,
  },

  bodyTextTwo: {
    fontSize: 14,
    fontFamily: 'UncutSans-Regular',
    // lineHeight: 22,
    // letterSpacing: 0,
  },

  // Small text
  smallText: {
    fontSize: 16,
    fontFamily: 'UncutSans-Regular',
    // lineHeight: 24,
    // letterSpacing: 2.4,
  },

  // Button text
  buttonTextOne: {
    fontSize: 18,
    fontFamily: 'UncutSans-Medium',
    // lineHeight: 26,
    // letterSpacing: 0.25,
  },

  buttonTextTwo: {
    fontSize: 18,
    fontFamily: 'UncutSans-Bold',
    // lineHeight: 26,
    // letterSpacing: 2.4,
  },

  buttonTextThree: {
    fontSize: 14,
    fontFamily: 'UncutSans-Bold',
    // lineHeight: 24,
    // letterSpacing: 2.2,
  },

  buttonTextFour: {
    fontSize: 14,
    fontFamily: 'UncutSans-Bold',
    textTransform: 'uppercase',
    // lineHeight: 24,
    // letterSpacing: 2.2,
  },

  // Label text
  // One
  labelTextOneAsTyped: {
    fontSize: 16,
    fontFamily: 'UncutSans-Bold',
  },

  labelTextOneUppercase: {
    fontSize: 16,
    textTransform: 'uppercase',
    fontFamily: 'UncutSans-Bold',
  },

  labelTextOneTitlecase: {
    fontSize: 16,
    textTransform: 'capitalize',
    fontFamily: 'UncutSans-Bold',
  },

  labelTextOneItalic: {
    fontSize: 16,
    fontFamily: 'UncutSans-BoldItalic',
  },

  // Two
  labelTextTwoAsTyped: {
    fontSize: 14,
    fontFamily: 'UncutSans-Semibold',
  },

  labelTextTwoUppercase: {
    fontSize: 14,
    textTransform: 'uppercase',
    fontFamily: 'UncutSans-Semibold',
  },

  labelTextTwoItalic: {
    fontSize: 14,
    fontFamily: 'UncutSans-SemiboldItalic',
  },

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Text                                      ||
  // ! ||--------------------------------------------------------------------------------||

  // Hyperlink
  hyperlink: {
    color: Colors.red500,
    textDecorationLine: 'underline',
  },

  // Alignment
  textAlignCenter: {
    textAlign: 'center',
  },

  textAlignRight: {
    textAlign: 'right',
  },

  textAlignLeft: {
    textAlign: 'left',
  },

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                 Color variants                                 ||
  // ! ||--------------------------------------------------------------------------------||

  // Dark
  textDark: {
    color: Colors.sparrowDark,
  },

  // Light
  textLight: {
    color: Colors.sparrowSand,
  },

  // Primary
  textPrimary: {
    color: Colors.sparrowDarkBrown,
  },

  // Secondary
  textSecondary: {
    color: Colors.sparrowSand,
  },

  // Success
  textSuccess: {
    color: Colors.green400,
  },

  // Warning
  textWarning: {
    color: Colors.orange400,
  },

  // Error
  textError: {
    color: Colors.red400,
  },

  // Function
  textFunction: {
    color: Colors.turqoise300,
  },

  // Disabled
  // TODO test usability, this was previously sand400
  textDisabled: {
    color: Colors.sand300,
  },

  // Placeholder
  textPlaceholder: {
    color: Colors.sand400,
  },

  // ! ||--------------------------------------------------------------------------------||

  // Highlights

  // Dark
  highlightDark: {
    color: Colors.sparrowDark,
    fontFamily: 'UncutSans-Bold',
  },

  // Light
  highlightLight: {
    color: Colors.sparrowSand,
    fontFamily: 'UncutSans-Bold',
  },

  // Yellow
  highlightYellow: {
    color: Colors.yellow500,
  },

  // Orange
  highlightOrange: {
    color: Colors.orange500,
  },

  // Red
  highlightRed: {
    color: Colors.red500,
  },

  // Rose
  highlightRose: {
    color: Colors.rose500,
  },

  // Fuchsia
  highlightFuchsia: {
    color: Colors.fuchsia500,
  },

  // Lavender
  highlightLavender: {
    color: Colors.lavender500,
  },

  // Green
  highlightGreen: {
    color: Colors.green500,
  },

  // Turqoise
  highlightTurqoise: {
    color: Colors.turqoise500,
  },

  // Picton
  highlightPicton: {
    color: Colors.picton500,
  },

  // Azure
  highlightAzure: {
    color: Colors.azure500,
  },

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                  Illustrations                                 ||
  // ! ||--------------------------------------------------------------------------------||

  // Large
  illustrationLarge: {
    alignSelf: 'center',
    resizeMode: 'center',
    height: 280,
  },

  // Medium
  illustrationMedium: {
    alignSelf: 'center',
    resizeMode: 'center',
    height: 200,
  },

  // Full
  illustrationFull: {
    maxWidth: windowWidth - Spacing.lg * 2,
    maxHeight: windowWidth,
    aspectRatio: 1,
  },
});
