import { StyleSheet } from "react-native";
import { Colors } from "./Colors";
import { Spacing } from "./Spacing";


export const navigationStyles = StyleSheet.create({
    // Navigation
    // Top navbar
    topNavbar: {
        flexDirection: 'row',
        alignItems: 'center',
        paddingHorizontal: Spacing.lg,
        paddingVertical: 12,
        backgroundColor: Colors.sparrowSand,
        borderBottomWidth: 2,
        borderColor: Colors.sparrowDarkBrown,

        icons: {
            color: Colors.sparrowDarkBrown,
            fontSize: 16,
        },

        // Use as gap for icon on the left (usually the back arrow)
        gapLeft: {
            marginRight: Spacing.md,
        },
        
        // Types
        // Default with title
        defaultTitled: {
            columnGap: Spacing.md,
        },

        // Favorite
        favorite: {
            justifyContent: 'space-between',
        },

        // Options
        options: {
            justifyContent: 'space-between',
        },

        // Edit
        edit: {
            justifyContent: 'space-between',

            wrapper: {
                flexDirection: 'row',
                columnGap: Spacing.md,
            },
        },
    },
});