import { StyleSheet } from "react-native";
import { Colors } from "./Colors";
import { Spacing } from "./Spacing";

export const cardStyles = StyleSheet.create({
    // Event cards
    // Medium size card
    eventCardMedium: {
        // TEMP. MARGIN
        margin: 24,

        // Background image
        bgImage: {
            // flex: 1,
            justifyContent: 'center',
        },

        bgImage2: {
            borderWidth: 2,
            borderRadius: 8,
            borderColor: Colors.sparrowDarkBrown,
        },

        content: {
            height: 254,
            margin: 8,
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
                        fontSize: 16,
                    },
                },
            },

            // Wraps date and time container and attendees container
            topWrapper: {
                flexDirection: 'row',
                justifyContent: 'space-between',
            },
        },
    },
});