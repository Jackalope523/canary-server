import { BottomTabScreenProps } from '@react-navigation/bottom-tabs';
import { useIsFocused } from '@react-navigation/native';
import React, { useState, useEffect } from 'react';

import
{
    SafeAreaView,
    Text,
    View,
    Button,
    ScrollView
  }
from 'react-native';

import { RootTabsParamList } from '../../../../App';
import EventSegment from '../../../components/EventSegment';
import style from '../../../theme/styles';

type FeedProps = BottomTabScreenProps<RootTabsParamList, 'Feed'>;

let testEvents = [EventSegment({id: '0', host: { id: '', name: 'no one' }, name: 'Skateboarding', description: 'lorem ad ipsum', type: '', startTime: new Date(), position: { latitude: 0, longitude: 0 }, numberAttendees: 1}),
EventSegment({id: '1', host: { id: '', name: 'no one' }, name: 'Group Hike', description: 'lorem ad ipsum', type: '', startTime: new Date(), position: { latitude: 0, longitude: 0 }, numberAttendees: 1}),
EventSegment({id: '2', host: { id: '', name: 'no one' }, name: 'Social', description: 'lorem ad ipsum', type: '', startTime: new Date(), position: { latitude: 0, longitude: 0 }, numberAttendees: 1}),
EventSegment({id: '3', host: { id: '', name: 'no one' }, name: 'Beerio Kart', description: 'lorem ad ipsum', type: '', startTime: new Date(), position: { latitude: 0, longitude: 0 }, numberAttendees: 1}),
EventSegment({id: '4', host: { id: '', name: 'no one' }, name: 'Front Ends open for work please', description: 'lorem ad ipsum', type: '', startTime: new Date(), position: { latitude: 0, longitude: 0 }, numberAttendees: 1}),
EventSegment({id: '5', host: { id: '', name: 'no one' }, name: 'Frisbeeeee', description: 'lorem ad ipsum', type: '', startTime: new Date(), position: { latitude: 0, longitude: 0 }, numberAttendees: 1}),
EventSegment({id: '6', host: { id: '', name: 'no one' }, name: 'Something', description: 'lorem ad ipsum', type: '', startTime: new Date(), position: { latitude: 0, longitude: 0 }, numberAttendees: 1}),
EventSegment({id: '7', host: { id: '', name: 'no one' }, name: 'Go Karting then getting smashed', description: 'lorem ad ipsum', type: '', startTime: new Date(), position: { latitude: 0, longitude: 0 }, numberAttendees: 1}),
EventSegment({id: '8', host: { id: '', name: 'no one' }, name: 'Terrorising McGillians', description: 'lorem ad ipsum', type: '', startTime: new Date(), position: { latitude: 0, longitude: 0 }, numberAttendees: 1}),
EventSegment({id: '9', host: { id: '', name: 'no one' }, name: 'Rally', description: 'lorem ad ipsum', type: '', startTime: new Date(), position: { latitude: 0, longitude: 0 }, numberAttendees: 1})];

export default function FeedScreen({navigation}: FeedProps): JSX.Element {
  const isFocused = useIsFocused();
  const [events, setEvents] = useState(testEvents);
  
  useEffect(() => {
    setEvents(testEvents);
  }, [isFocused]);
  

  return (
    <SafeAreaView style={style.sectionContainer}>
      <ScrollView>
        {events}
      </ScrollView>
    </SafeAreaView>
  );
}

