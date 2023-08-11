import { StyleSheet } from "react-native";
import { Colors } from "./Colors";

export const globalStyles = StyleSheet.create({
    // Layout
    // Containers
    baseContainer: {
        flex: 1,
        backgroundColor: Colors.sparrowSand,
    },

    // Typography
    // Base text
    baseText: {
        fontFamily: 'UncutSans-Regular',
    },

    // Display text
    displayTextOne: {
        fontSize: 72,
        // lineHeight: 80,
        // letterSpacing: -1.8,
    },

    displayTextTwo: {
        fontSize: 44,
        // lineHeight: 52,
        // letterSpacing: -1,
    },

    // Heading text
    headingTextOne: {
        fontSize: 36,
        // lineHeight: 44,
        // letterSpacing: -0.8,
    },
    
    headingTextTwo: {
        fontSize: 28,
        // lineHeight: 36,
        // letterSpacing: -0.4,
    },

    headingTextThree: {
        fontSize: 22,
        // lineHeight: 30,
        // letterSpacing: -0.25,
    },

    headingTextFour: {
        fontSize: 18,
        // lineHeight: 26,
        // letterSpacing: -0.25,
    },

    headingTextFive: {
        fontSize: 16,
        // lineHeight: 24,
        // letterSpacing: 0,
    },

    // Body text
    bodyTextOne: {
        fontSize: 16,
        // lineHeight: 24,
        // letterSpacing: 0,
    },

    bodyTextTwo: {
        fontSize: 14,
        // lineHeight: 22,
        // letterSpacing: 0,
    },

    // Small text
    smallText: {
        fontSize: 16,
        // lineHeight: 24,
        // letterSpacing: 2.4,
    },

    // Button text
    buttonTextOne: {
        fontSize: 18,
        // lineHeight: 26,
        // letterSpacing: 0.25,
    },

    buttonTextTwo: {
        fontSize: 18,
        // lineHeight: 26,
        // letterSpacing: 2.4,
    },

    buttonTextThree: {
        fontSize: 16,
        // lineHeight: 24,
        // letterSpacing: 2.2,
    },

    // Buttons
    // Text buttons
    // Color variants
    // Primary
    textButtonPrimary: {
        backgroundColor: Colors.sparrowDarkBrown,
    },

    // Secondary
    textButtonSecondary: {
        backgroundColor: Colors.sparrowSand,
    },

    // Success
    textButtonSuccess: {
        backgroundColor: Colors.green400,
    },

    // Warning
    textButtonWarning: {
        backgroundColor: Colors.orange400,
    },

    // Error
    textButtonError: {
        backgroundColor: Colors.red400,
    },

    // Function
    textButtonFunction: {
        backgroundColor: Colors.turqoise300,
    },

    // Sizes
    // Large
    textButtonLarge: {
        alignItems: 'center',
        justifyContent: 'center',
        paddingVertical: 24,
        paddingHorizontal: 32,
        borderWidth: 2,
        borderRadius: 8,
        borderColor: Colors.sparrowDarkBrown,
    },

    textButtonLargeText: {
        fontFamily: 'UncutSans-Medium',
        color: Colors.sparrowDark,
        fontSize: 18,
        // lineHeight: 26,
        // letterSpacing: 0.25,
    },

    // Medium
    textButtonMedium: {
        alignItems: 'center',
        justifyContent: 'center',
        paddingVertical: 16,
        paddingHorizontal: 24,
        borderWidth: 2,
        borderRadius: 8,
        borderColor: Colors.sparrowDarkBrown,
    },

    textButtonMediumText: {
        fontFamily: 'UncutSans-Medium',
        color: Colors.sparrowDark,
        fontSize: 18,
        // lineHeight: 26,
        // letterSpacing: 0.25,
    },

    // Small
    textButtonSmall :{
        alignItems: 'center',
        justifyContent: 'center',
        paddingVertical: 8,
        paddingHorizontal: 16,
        borderWidth: 2,
        borderRadius: 8,
    },

    textButtonSmallText :{
        fontFamily: 'UncutSans-Bold',
        color: Colors.sparrowDark,
        fontSize: 18,
        textTransform: 'uppercase',
        // lineHeight: 26,
        // letterSpacing: 0.24,
    },

    // Extra small
    textButtonExtraSmall: {
        alignItems: 'center',
        justifyContent: 'center',
        paddingVertical: 8,
        paddingHorizontal: 16,
        borderWidth: 2,
        borderRadius: 8,
        borderColor: Colors.sparrowDarkBrown,
    },

    textButtonExtraSmallText: {
        fontFamily: 'UncutSans-Bold',
        color: Colors.sparrowDark,
        fontSize: 16,
        textTransform: 'uppercase',
        // lineHeight: 24,
        // letterSpacing: 2.2,
    },

    // Sort buttons
    // Color variants
    // Rest
    sortButtonRest: {
        backgroundColor: Colors.sparrowBrown,
        borderColor: Colors.sparrowDark,
    },

    // Selected
    sortButtonSelected: {
        backgroundColor: Colors.sparrowRed,
        borderColor: Colors.sparrowDark,
    },

    // Disabled
    sortButtonDisabled: {
        backgroundColor: Colors.sand200,
        borderColor: Colors.sand300,
    },

    // Sizes
    sortButton: {
        alignItems: 'center',
        justifyContent: 'center',
        paddingVertical: 8,
        paddingHorizontal: 16,
        borderWidth: 2,
        borderRadius: 8,
    },

    sortButtonText: {
        fontFamily: 'UncutSans-Bold',
        color: Colors.sparrowSand,
        fontSize: 18,
        // lineHeight: 26,
        // letterSpacing: 2.4,
    },

    // Filter buttons
    // Color variants
    // Rest
    filterButtonRest: {
        backgroundColor: Colors.sparrowBrown,
        borderColor: Colors.sparrowDark,
    },

    // Selected
    filterButtonSelected: {
        backgroundColor: Colors.sparrowRed,
        borderColor: Colors.sparrowDark,
    },

    // Disabled
    filterButtonDisabled: {
        backgroundColor: Colors.sand200,
        borderColor: Colors.sand300,
    },

    // Sizes
    filterButton: {
        alignItems: 'center',
        justifyContent: 'center',
        paddingVertical: 8,
        paddingHorizontal: 16,
        borderWidth: 2,
        borderRadius: 8,
    },

    filterButtonText: {
        fontFamily: 'UncutSans-Bold',
        color: Colors.sparrowSand,
        fontSize: 18,
        // lineHeight: 26,
        // letterSpacing: 2.4,
    },
});