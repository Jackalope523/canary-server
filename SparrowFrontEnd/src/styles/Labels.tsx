import { StyleSheet } from "react-native";
import { Colors } from "./Colors";

export const labelStyles = StyleSheet.create ({
    // Labels
    // Number label
    numberLabel: {
        paddingHorizontal: 12,
        paddingVertical: 4,
        borderRadius: 100,

        dark: {
            backgroundColor: Colors.sparrowDarkBrown,

            text: {
                color: Colors.sparrowSand,
            },
        },

        outline: {
            borderWidth: 2,
            borderColor: Colors.sparrowDarkBrown,

            text: {
                color: Colors.sparrowDark,
            },
        },
    },

    // Text label
    // Large
    textLabelLarge: {
        paddingHorizontal: 16,
        paddingVertical: 8,
        borderRadius: 100,

        rest: {
            borderWidth: 2,
            borderColor: Colors.sparrowDarkBrown,

            text: {
                color: Colors.sparrowDark,
            },
        },

        selected: {
            backgroundColor: Colors.sparrowDarkBrown,

            text: {
                color: Colors.sparrowSand,
            },
        },
    },

    // Small
    textLabelSmall: {
        paddingHorizontal: 12,
        paddingVertical: 4,
        borderWidth: 2,
        borderRadius: 100,
        borderColor: Colors.sparrowDarkBrown,

        // Label text
        text: {
            color: Colors.sparrowDark,
        },

        you: {
            backgroundColor: Colors.orange400,
        },

        friend: {
            backgroundColor: Colors.yellow400,
        },
    },

});