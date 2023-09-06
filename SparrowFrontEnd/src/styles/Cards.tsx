import { Dimensions, StyleSheet } from "react-native";
import { Colors } from "./Colors";
import { Spacing } from "./Spacing";

// Screen dimensions
const screenWidth = Dimensions.get('screen').width;

export const cardStyles = StyleSheet.create({
    // Event cards
    // Medium size card
    eventCardMedium: {
        // Width is 100% of screen width - 24 (margin) x 2 (LR)
        width: screenWidth - Spacing.lg * 2,

        bgImage2: {
            borderWidth: 2,
            borderRadius: 8,
            borderColor: Colors.sparrowDarkBrown,
        },

        content: {
            height: 254,
            margin: Spacing.sm,
            justifyContent: 'space-between',

            // Event detail containers
            container: {
                paddingHorizontal: 16,
                paddingVertical: 8,
                backgroundColor: Colors.sparrowSand,
                borderWidth: 2,
                borderRadius: 8,
                borderColor: Colors.sparrowDarkBrown,

                title: {
                    marginBottom: Spacing.sm,
                },

                // Wraps text and icon
                textWrapper: {
                    flexDirection: 'row',
                    alignItems: 'center',
                    columnGap: Spacing.sm,

                    icon: {
                        color: Colors.sparrowDarkBrown,

                        // TODO change fontSize back to 16 - 24 if necessary and other icon sizes have been changed to 24
                        // fontSize is 42 purely for inspection purposes
                        // fontSize: 42,
                    },
                },
            },

            // Wraps date and time container and attendees container
            topWrapper: {
                flexDirection: 'row',
                justifyContent: 'space-between',
                alignItems: 'flex-start',
            },
        },
    },
});