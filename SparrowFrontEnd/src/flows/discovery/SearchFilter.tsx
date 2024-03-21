import React, {useState, useEffect} from 'react';
import { StyleSheet, Text, View, FlatList } from 'react-native';

import {EventCardMediumProps, EventCardMedium} from '../../components/EventCardMedium';
import { SAMPLEEVENTDATA } from '../../data/sampleEventData';

import { Spacing } from '../../styles/SpacingStyles';
import { point, Point, Feature, Properties} from '@turf/helpers';
import { eventShard } from '../event/eventPigeon';
import { getAllEvents } from './discoverPigeon';
import {formatDate, formatTime} from './chronologicalTools'

//  TODO make search filter, search for events based on TEXT input from Discovery -> searchBar -> TextInput component.
// TODO make FILTER and SORT buttons functional

interface SearchFilterProps {
  list?:eventShard[];
  sortBy?:(x:eventShard, y:eventShard) => number;
  filterBy?:((x:eventShard) => boolean)[];
}

const SearchFilter: React.FC<SearchFilterProps> = ({
  list = [],
  sortBy = (x:eventShard, y:eventShard) => {return 0},
  filterBy = [(x:eventShard) => {return true}]
}) => {

  const [events, setEvents] = useState<eventShard[]>([]);

  useEffect(() => {
    getAllEvents(53.483, 7.544, 10000)
    .then(value => { setEvents(value); })
    .catch(() => "SESSION ERROR");
  }, []);

  filterBy.forEach((filter) => {setEvents(events.filter(filter))});
  events.sort(sortBy);

  return (
    <FlatList
        contentContainerStyle = {{alignItems: "center"}}
        horizontal = {false}
        showsVerticalScrollIndicator = {false}
        overScrollMode = "never"
        data = {events}
        renderItem = {({ item }) =>
        <EventCardMedium 
          onPress={() => {}}
          eventDate= {formatDate(item.StartTime)}
          eventTime= {formatTime(item.StartTime)}
          eventAttendees= {item.NumberOfGuests}
          eventTitle= {item.Name}
          eventLocation= {"Skogr"}
          eventHeroImage= {{
            uri: 'https://images.unsplash.com/photo-1541140134513-85a161dc4a00?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
          }}
        />}
      />
  );
};

export default SearchFilter;

const styles = StyleSheet.create({});
