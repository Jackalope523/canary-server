import { StyleSheet } from "react-native";
import { Colors } from "./Colors";
import { Spacing } from "./Spacing";

export const globalStyles = StyleSheet.create({
    // Layout
    // Containers
    // Used for navigation container, tab navigator
    mainContainer: {
        backgroundColor: Colors.sparrowSand,
    },

    // Used for screens
    baseContainer: {
        // flex: 1,
        margin: 24,
    },

    // Typography
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
        fontSize: 16,
        fontFamily: 'UncutSans-Bold',
        textTransform: 'uppercase',
        // lineHeight: 24,
        // letterSpacing: 2.2,
    },

    // Label text
    labelTextAsTyped: {
        fontSize: 16,
        fontFamily: 'UncutSans-Bold',
    },

    labelTextUppercase: {
        fontSize: 16,
        textTransform: 'uppercase',
        fontFamily: 'UncutSans-Bold',
    },

    labelTextTitleCase: {
        fontSize: 16,
        textTransform: 'capitalize',
        fontFamily: 'UncutSans-Bold',
    },

    // Text
    // Color variants
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

    // Buttons
    // Primary
    buttonPrimary: {
        backgroundColor: Colors.sparrowDarkBrown,
        borderColor: Colors.sparrowDarkBrown,
    },

    // Primary Light
    buttonPrimaryLight: {
        backgroundColor: Colors.sparrowBrown,
        borderColor: Colors.sparrowBrown,
    },

    // Secondary
    buttonSecondary: {
        backgroundColor: Colors.sparrowSand,
        borderColor: Colors.sparrowSand,

        outline: {
            borderWidth: 2,
            borderRadius: 8,
            borderColor: Colors.sparrowSand,
        },
    },

    // Button Tertiary
    buttonTertiary: {
        backgroundColor: Colors.sparrowDark,
        borderColor: Colors.sparrowDark,
    },

    // Success
    buttonSuccess: {
        backgroundColor: Colors.green400,
        borderColor: Colors.green400,
    },

    // Warning
    buttonWarning: {
        backgroundColor: Colors.orange400,
        borderColor: Colors.orange400,
    },

    // Error
    buttonError: {
        backgroundColor: Colors.red400,
        borderColor: Colors.red400,
    },

    // Function
    buttonFunction: {
        backgroundColor: Colors.turqoise300,
        borderColor: Colors.turqoise300
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

    // Text buttons
    // Color variants
    // Primary
    // textButtonPrimary: {
    //     backgroundColor: Colors.sparrowDarkBrown,
    // },

    // // Secondary
    // textButtonSecondary: {
    //     backgroundColor: Colors.sparrowSand,
    // },

    // // Success
    // textButtonSuccess: {
    //     backgroundColor: Colors.green400,
    // },

    // // Warning
    // textButtonWarning: {
    //     backgroundColor: Colors.orange400,
    // },

    // // Error
    // textButtonError: {
    //     backgroundColor: Colors.red400,
    // },

    // // Function
    // textButtonFunction: {
    //     backgroundColor: Colors.turqoise300,
    // },

    // Sizes
    // Large
    textButtonLarge: {
        alignItems: 'center',
        justifyContent: 'center',
        paddingVertical: 24,
        paddingHorizontal: 32,
        borderRadius: 8,
        borderWidth: 2,
        // borderColor: Colors.sparrowDarkBrown,

        text: {
            fontFamily: 'UncutSans-Medium',
            fontSize: 18,
        },
    },

    // textButtonLargeText: {
    //     fontFamily: 'UncutSans-Medium',
    //     // color: Colors.sparrowDark,
    //     fontSize: 18,
    //     // lineHeight: 26,
    //     // letterSpacing: 0.25,
    // },

    // Medium
    textButtonMedium: {
        alignItems: 'center',
        justifyContent: 'center',
        paddingVertical: 16,
        paddingHorizontal: 24,
        borderRadius: 8,
        borderWidth: 2,
        // borderColor: Colors.sparrowDarkBrown,

        text: {
            fontFamily: 'UncutSans-Medium',
            fontSize: 18,
        },
    },

    // textButtonMediumText: {
    //     fontFamily: 'UncutSans-Medium',
    //     // color: Colors.sparrowDark,
    //     fontSize: 18,
    //     // lineHeight: 26,
    //     // letterSpacing: 0.25,
    // },

    // Small
    textButtonSmall :{
        alignItems: 'center',
        justifyContent: 'center',
        paddingVertical: 8,
        paddingHorizontal: 16,
        borderWidth: 2,
        // borderColor: Colors.sparrowDarkBrown,
        borderRadius: 8,

        text: {
            fontFamily: 'UncutSans-Bold',
            fontSize: 18,
        },
    },

    // textButtonSmallText :{
    //     fontFamily: 'UncutSans-Bold',
    //     // color: Colors.sparrowDark,
    //     fontSize: 18,
    //     // textTransform: 'uppercase',
    //     // lineHeight: 26,
    //     // letterSpacing: 0.24,
    // },

    // Extra small
    textButtonExtraSmall: {
        alignItems: 'center',
        justifyContent: 'center',
        paddingVertical: 8,
        paddingHorizontal: 16,
        borderRadius: 8,
        borderWidth: 2,
        // borderColor: Colors.sparrowDarkBrown,
        columnGap: Spacing.sm,

        text: {
            fontFamily: 'UncutSans-Bold',
            fontSize: 16,
            
            uppercase: {
                textTransform: 'uppercase',
            },
        },
    },

    // TODO delete
    // textButtonExtraSmallText: {
    //     fontFamily: 'UncutSans-Bold',
    //     // color: Colors.sparrowDark,
    //     fontSize: 16,
    //     textTransform: 'uppercase',
    //     // lineHeight: 24,
    //     // letterSpacing: 2.2,
    // },

    // TODO delete later
    // // Sort buttons
    // // Color variants
    // // Rest
    // sortButtonRest: {
    //     backgroundColor: Colors.sparrowBrown,
    //     borderColor: Colors.sparrowDark,
    // },

    // // Selected
    // sortButtonSelected: {
    //     backgroundColor: Colors.sparrowRed,
    //     borderColor: Colors.sparrowDark,
    // },

    // // Disabled
    // sortButtonDisabled: {
    //     backgroundColor: Colors.sand200,
    //     borderColor: Colors.sand300,
    // },

    // // Sizes
    // sortButton: {
    //     alignItems: 'center',
    //     justifyContent: 'center',
    //     paddingVertical: 8,
    //     paddingHorizontal: 16,
    //     borderWidth: 2,
    //     borderRadius: 8,
    // },

    // sortButtonText: {
    //     fontFamily: 'UncutSans-Bold',
    //     // color: Colors.sparrowSand,
    //     fontSize: 18,
    //     // lineHeight: 26,
    //     // letterSpacing: 2.4,
    // },

    // // Filter buttons
    // // Color variants
    // // Rest
    // filterButtonRest: {
    //     backgroundColor: Colors.sparrowBrown,
    //     borderColor: Colors.sparrowDark,
    // },

    // // Selected
    // filterButtonSelected: {
    //     backgroundColor: Colors.sparrowRed,
    //     borderColor: Colors.sparrowDark,
    // },

    // // Disabled
    // filterButtonDisabled: {
    //     backgroundColor: Colors.sand200,
    //     borderColor: Colors.sand300,
    // },

    // // Sizes
    // filterButton: {
    //     alignItems: 'center',
    //     justifyContent: 'center',
    //     paddingVertical: 8,
    //     paddingHorizontal: 16,
    //     borderWidth: 2,
    //     borderRadius: 8,
    // },

    // filterButtonText: {
    //     fontFamily: 'UncutSans-Bold',
    //     // color: Colors.sparrowSand,
    //     fontSize: 18,
    //     // lineHeight: 26,
    //     // letterSpacing: 2.4,
    // },

    // Button icons
    // Small
    buttonIconSmall: {
        fontSize: 24,

        // Fix for icon being cut off
        height: 24,

        light: {
            color: Colors.sparrowSand,
        },

        dark: {
            color: Colors.sparrowDarkBrown,
        },
    },

    // Medium
    buttonIconMedium: {
        fontSize: 32,

        // Fix for icon being cut off
        height: 32,

        light: {
            color: Colors.sparrowSand,
        },

        dark: {
            color: Colors.sparrowDarkBrown,
        },
    },

    // Illustrations
    illustration: {
        resizeMode: 'center',

        large: {
            height: 280,
        },

        medium: {
            height: 200,
        },
    },
});