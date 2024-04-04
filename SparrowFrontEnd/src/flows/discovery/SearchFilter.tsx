//#region imports
import React, {useState, useEffect} from 'react';
import { StyleSheet, FlatList } from 'react-native';
import {EventCardMedium} from '../../components/EventCardMedium';
import { point, Point, distance} from '@turf/turf';
import { eventShard } from '../event/eventPigeon';
import {
  formatDate, 
  formatTime, 
  isToday, 
  isTomorrow, 
  isNextWeek, 
  isNextWeekend, 
  isThisWeek, 
  isThisWeekend} 
  from './chronologicalTools';
//#endregion imports

interface SearchFilterProps {
  list?:eventShard[];
  location?:Point;
  searchText?:string;
  sortValue:string;
  filterDateValue:string,
  filterSizeValue:string,
}

const SearchFilter: React.FC<SearchFilterProps> = ({
  list = [],
  location = point([0, 0]),
  searchText = "",
  sortValue = "",
  filterDateValue = "",
  filterSizeValue = "",
}) => {

  const generateSortBy = () => {
    switch (sortValue) {
      case "Most Popular":
        return (x: eventShard, y: eventShard) => { return y.NumberOfGuests - x.NumberOfGuests };
      case "Closest":
        return (x: eventShard, y: eventShard) => { return distance(location, x.Location) - distance(location, y.Location) };
      case "Most Recent":
        return (x: eventShard, y: eventShard) => { return y.StartTime.getTime() - x.StartTime.getTime() };
    }
  };

  const generateFilterArray = () => {
    let filterArray = new Array<(e: eventShard) => boolean>();

    switch (filterDateValue) {
      case "Today":
        filterArray.push((e: eventShard) => { return isToday(e.StartTime) });
        break;
      case "Tomorrow":
        filterArray.push((e: eventShard) => { return isTomorrow(e.StartTime) });
        break;
      case "This Week":
        filterArray.push((e: eventShard) => { return isThisWeek(e.StartTime) });
        break;
      case "This Weekend":
        filterArray.push((e: eventShard) => { return isThisWeekend(e.StartTime) });
        break;
      case "Next Week":
        filterArray.push((e: eventShard) => { return isNextWeek(e.StartTime) });
        break;
      case "Next Weekend":
        filterArray.push((e: eventShard) => { return isNextWeekend(e.StartTime) });
        break;
    }

    switch (filterSizeValue) {
      case "Cozy":
        filterArray.push((e: eventShard) => { return e.NumberOfGuests < 5 });
        break;
      case "Thriving":
        filterArray.push((e: eventShard) => { return e.NumberOfGuests > 15 && e.NumberOfGuests < 30 });
        break;
      case "Bombastic":
        filterArray.push((e: eventShard) => { return e.NumberOfGuests > 30 });
        break;
    }

    filterArray.push((e: eventShard) => { return e.Name.includes(searchText)});

    return filterArray;
  }

  const [toDisplay, setToDisplay] = useState<eventShard[]>([]); 

  useEffect(() => {
    let toProcess = list;

    generateFilterArray().forEach((filter) => {
      toProcess = toProcess.filter(filter)
    });
  
    toProcess.sort(generateSortBy());

    setToDisplay(toProcess);
  }, [searchText, sortValue, filterDateValue, filterSizeValue]);

 

  return (
    <FlatList
        contentContainerStyle = {{alignItems: "center"}}
        horizontal = {false}
        showsVerticalScrollIndicator = {false}
        overScrollMode = "always"
        data = {toDisplay}
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
