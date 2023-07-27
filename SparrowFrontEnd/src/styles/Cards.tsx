import { StyleSheet } from "react-native";
import { Colors } from "./Colors";

export const cardStyles = StyleSheet.create({
    eventCardMedium: {
        display: 'flex',
        flexWrap: 'wrap',
        alignItems: 'flex-start',

        // temp. background color - needs to be an image of the event
        backgroundColor: 'lightgrey',
        
        borderWidth: 2,
        borderRadius: 8,
        borderColor: Colors.sparrowDarkBrown,
    }
});