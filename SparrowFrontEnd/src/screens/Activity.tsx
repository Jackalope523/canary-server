import * as React from 'react';
import { View, Text } from 'react-native';
import { globalStyles } from '../styles/Global';

import EventCardMedium from '../components/organisms/EventCardMedium';
import NotificationIndicator from '../components/molecules/NotificationIndicator';

import { StyleSheet } from 'react-native';

const ActivityScreen = () => {
    return (
        <View>
            <NotificationIndicator />
            <EventCardMedium />
        </View>
    );
};

export default ActivityScreen

const styles = StyleSheet.create({
    containerTest: {
        padding: 20,
        flexDirection: 'row',
        justifyContent: 'space-between',
        flexWrap: 'wrap',
    }
})