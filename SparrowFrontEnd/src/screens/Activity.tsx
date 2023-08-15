import * as React from 'react';
import { View } from 'react-native';
import { globalStyles } from '../styles/Global';

import EventCardMedium from '../components/organisms/EventCardMedium';
import NotificationIndicator from '../components/molecules/NotificationIndicator';

const ActivityScreen = () => {
    return (
        <View style={globalStyles.baseContainer}>
            <NotificationIndicator />
            <EventCardMedium />
        </View>
    );
};

export default ActivityScreen