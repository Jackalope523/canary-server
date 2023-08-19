import * as React from 'react';
import { View, Text, StyleSheet, ScrollView } from 'react-native';
import { globalStyles } from '../styles/Global';
import { Colors } from '../styles/Colors';
import { Spacing } from '../styles/Spacing';

import EventCardMedium from '../components/organisms/EventCardMedium';
import NotificationIndicator from '../components/molecules/NotificationIndicator';

const ActivityScreen = () => {
    return (
        <ScrollView style={styles.mainWrapper} overScrollMode="never">
            <View style={styles.topWrapper}>
                <View style={styles.notificationWrapper}>
                    <NotificationIndicator />
                </View>
                <Text style={[globalStyles.displayTextTwo, {color: Colors.sparrowRed, marginTop: Spacing.lg, marginBottom: Spacing.md}]}>Hey, User!</Text>
            </View>

            {/* TODO make event card heading such as "upcoming events" like it is in the prototype - another heading on scroll */}
            {/* stickyHeaderIndices is NOT SUPPORTED with horizontal scrolling */}
            <View style={styles.sectionWrapper}>
                <Text style={[globalStyles.headingTextOne, {color: Colors.sparrowDark, marginBottom: Spacing.md, marginLeft: Spacing.lg}]}>Upcoming events</Text>
                <ScrollView horizontal={true} overScrollMode="never">
                    <View style={styles.eventCardContainer}>
                        <EventCardMedium />
                        <EventCardMedium />
                        <EventCardMedium />
                    </View>
                </ScrollView>
            </View>

            <View style={styles.sectionWrapper}>
                <Text style={[globalStyles.headingTextOne, {color: Colors.sparrowDark, marginBottom: Spacing.md, marginLeft: Spacing.lg}]}>Recommended</Text>
                <ScrollView horizontal={true} overScrollMode="never">
                    <View style={styles.eventCardContainer}>
                        <EventCardMedium />
                        <EventCardMedium />
                        <EventCardMedium />
                    </View>
                </ScrollView>
            </View>
        </ScrollView>
    );
};

export default ActivityScreen

const styles = StyleSheet.create ({
    mainWrapper: {
        paddingBottom: Spacing.lg,
    },

    sectionWrapper: {
        marginBottom: Spacing.lg,   
    },

    topWrapper: {
        marginHorizontal: Spacing.lg,
        marginTop: Spacing.lg,
    },

    notificationWrapper: {
        alignItems: 'flex-end',
    },

    eventCardContainer: {
        // flex: 1,
        marginLeft: Spacing.lg,
        flexDirection: 'row',
        columnGap: Spacing.md,
    },
});