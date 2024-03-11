import React, {useEffect} from 'react';
import { StyleSheet, Text, View, FlatList } from 'react-native';

import {EventCardMediumProps, EventCardMedium} from '../EventCardMedium';
import { SAMPLEEVENTDATA } from '../../data/sampleEventData';

import { Spacing } from '../../styles/SpacingStyles';
import { point, Point, Feature, Properties} from '@turf/helpers';

//  TODO make search filter, search for events based on TEXT input from Discovery -> searchBar -> TextInput component.
// TODO make FILTER and SORT buttons functional

interface SearchFilterProps {
  list?:EventCardMediumProps[];
  sortBy?:(x:EventCardMediumProps, y:EventCardMediumProps) => number;
  filterBy?:((x:EventCardMediumProps) => boolean)[];
}

const SearchFilter: React.FC<SearchFilterProps> = ({
  list = [],
  sortBy = (x:EventCardMediumProps, y:EventCardMediumProps) => {return 0},
  filterBy = [(x:EventCardMediumProps) => {return true}]
}) => {

  let toDisplay: EventCardMediumProps[] = SAMPLEEVENTDATA.map((data) => 
  {return {
      onPress: () => {},
      eventDate: data.date,
      eventTime: data.time,
      eventAttendees: data.attendees,
      eventTitle: data.title,
      eventLocation: data.location,
      eventHeroImage: data.uri,
      eventCoordinate: point([data.longitude, data.latitude]),
      eventDateTest: data.dateTest
    }
  });

  filterBy.forEach((filter) => {toDisplay = toDisplay.filter(filter)});
  console.log("SORTING");
  toDisplay.sort(sortBy);

  return (
    <FlatList
        contentContainerStyle = {{alignItems: "center"}}
        horizontal = {false}
        showsVerticalScrollIndicator = {false}
        overScrollMode = "never"
        data = {toDisplay}
        renderItem = {({ item }) => (
          <EventCardMedium 
            onPress = {item.onPress}
            eventDate= {item.eventDate}
            eventTime= {item.eventTime}
            eventAttendees= {item.eventAttendees}
            eventTitle= {item.eventTitle}
            eventLocation= {item.eventLocation}
            eventHeroImage= {item.eventHeroImage}
            eventCoordinate={item.eventCoordinate}
          />
        )}
      />
  );
};

export default SearchFilter;

const styles = StyleSheet.create({});
