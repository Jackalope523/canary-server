import React, { PropsWithChildren } from 'react';

import
{
  Text,
  View,
  Button,
  TextInput
}
from 'react-native';

import styles from '../theme/styles';

export type EventModel = {
    id: string;
    host: { id: string, name: string };
    name: string;
    description: string;
    type: string;
    startTime: Date;
    position: { latitude: number, longitude: number };
    numberAttendees: number;
};

export default function EventSegment({id, name, description, startTime}: EventModel): JSX.Element {
    return (
        <View key={id}>
            <View style={styles.eventSegment}>
                <Text>{name}</Text>
                <Text>{description}</Text>
                <Text>Starting at {startTime.toLocaleTimeString()}</Text>
            </View>
        </View>
    );
}