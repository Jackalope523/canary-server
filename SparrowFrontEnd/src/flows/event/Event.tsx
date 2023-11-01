import * as React from 'react';
import { View, Text, TextInput } from 'react-native';
import { StackScreenProps } from '@react-navigation/stack';

import { Colors } from '../../styles/Colors';
import { globalStyles } from '../../styles/Global';

import { EventStackParamList } from '../../components/atoms/types';
import Button from '../../components/atoms/Button';

import { eventShard, getEvent } from './eventPigeon';


type EventProps = StackScreenProps<EventStackParamList, 'Event'>;

const EventScreen = ({route}: EventProps) => {
    const [errorText, setErrorText] = React.useState('');
    const [eventText, setEventText] = React.useState('');
    
    function handleGetEvent() {
        setErrorText('');
        
        getEvent(route.params.EventID)
        .then(data => populateScreen(data))
        .catch(() => setErrorText('Could not retrieve data. Incorrect code'));
    }

    if (eventText == '' || errorText == '') // To avoid recursion from component reloading on set state
        handleGetEvent();

    function populateScreen(data: eventShard) {
        setEventText(`Event Title: ${data.Name}\n
            Host Name: ${data.Host.Name}\n\n
            Event Description: ${data.Description}\n
            Start Time: ${data.StartTime}`);
    }

    return(
        <View>
            <Text style={{color: Colors.red400}}>{errorText}</Text>
            <Text>{eventText}</Text>
        </View>
    );
};

export default EventScreen