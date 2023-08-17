import * as React from 'react';
import { View, Text, StyleSheet, ScrollView } from 'react-native';
import { globalStyles } from '../styles/Global';
import { Colors } from '../styles/Colors';
import { Spacing } from '../styles/Spacing';
import { labelStyles } from '../styles/Labels';

import EventCardMedium from '../components/organisms/EventCardMedium';
import NotificationIndicator from '../components/molecules/NotificationIndicator';

const ActivityScreen = () => {
    return (
        <View>
            <View style={styles.topWrapper}>
                <View style={styles.notificationWrapper}>
                    <NotificationIndicator />
                </View>
                <Text style={[globalStyles.displayTextTwo, {color: Colors.sparrowRed, marginTop: Spacing.lg, marginBottom: Spacing.md}]}>Hey, User!</Text>
                <Text style={[globalStyles.headingTextOne, {color: Colors.sparrowDark, marginBottom: Spacing.md}]}>Upcoming events</Text>
            </View>

            {/* TODO make event card 100% size of regular vw - 24 (margin size) */}
            <ScrollView horizontal={true}>
                <View style={styles.eventCardContainer}>
                    <EventCardMedium />
                    <EventCardMedium />
                    <EventCardMedium />
                </View>
            </ScrollView>
        </View>
    );
};

export default ActivityScreen

const styles = StyleSheet.create ({
    topWrapper: {
        marginHorizontal: Spacing.lg,
        marginTop: Spacing.lg,
    },

    notificationWrapper: {
        alignItems: 'flex-end',
    },

    eventCardContainer: {
        marginLeft: Spacing.lg,
        flexDirection: 'row',
        columnGap: Spacing.md,
    },
});