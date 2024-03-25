// #region Imports
import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  StyleSheet,
  TextInput,
  Pressable,
  useWindowDimensions,
  FlexStyle,
  Keyboard
} from 'react-native';
import { Colors } from '../../styles/ColorStyles';
import { globalStyles } from '../../styles/GlobalStyles';
import { navigationStyles } from '../../styles/NavigationStyles';
import { Spacing } from '../../styles/SpacingStyles';

import Button, {
  ButtonType,
  ButtonSize,
  ButtonDisplay,
} from '../../components/Button';
import SearchFilter from './SearchFilter';

import Animated, {
  useAnimatedStyle,
  useSharedValue,
  withTiming,
} from 'react-native-reanimated';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
import {
  GestureHandlerRootView,
} from 'react-native-gesture-handler';
import ExclusiveButtonView from '../../components/ExclusiveButtonView';
import ExclusiveButtonScroll from '../../components/ExclusiveButtonScroll';

import Map from './Map';
import SearchBar from './SearchBar';
import {isToday, isTomorrow, isNextWeek, isNextWeekend, isThisWeek, isThisWeekend} from './chronologicalTools';

import { EventCardMediumProps, EventCardMedium } from '../../components/EventCardMedium';
import { point, Point, distance, Feature, Properties } from '@turf/turf';
import Geolocation from 'react-native-geolocation-service';
import { etchingShard, eventShard } from '../event/eventPigeon';
import { getAllEvents } from './discoverPigeon';
// #endregion

const Icon = createIconSetFromFontello(fontelloConfig);

// TEMP. map image - replace with actual working map
const tempMapImage = require('../../assets/images/temp/temp-map.png');

enum ActiveComponent {
  None,
  Filter,
  Sort
}

const DiscoveryScreen = () => {

  const { height, width } = useWindowDimensions();

  const [searchContentVisible, setSearchContentVisible] = useState(false);
  const [searchText, setSearchText] = useState('');

  // Search bar text input
  const [isTextInputFocused, setIsTextInputFocused] = useState(false);

  useEffect(() => {
    if (isTextInputFocused) {
      setSearchContentVisible(true);
    }
  }, [isTextInputFocused]);

  // Search options
  const [activeComponent, setActiveComponent] = useState(ActiveComponent.None);

  const toggleActiveComponent = (component:ActiveComponent) => {
    Keyboard.dismiss();
    if (activeComponent === component) {
      setActiveComponent(ActiveComponent.None);
    } else {
      setActiveComponent(component);
    }
  };

  // Filter and sort options button states
  // Date
  const [dateState, setDateState] = useState(-1);

  // Size
  const [sizeState, setSizeState] = useState(-1);

  // Distance
  // TODO make a distance range slider

  // Sort by
  const [sortState, setSortState] = React.useState(-1);

  // TODO split up the page into components - implement the search bar from Discovery/SearchBar and Discovery Search in a different screen

  const [activeButton, setAtiveButton] = React.useState(-1);

  const iconOpacity = useSharedValue(0);

  const animatedIconStyle = useAnimatedStyle(() => {
    return {
      opacity: iconOpacity.value,
    };
  });

  useEffect(() => {
    iconOpacity.value = withTiming(searchContentVisible ? 1 : 0, {
      duration: 200,
    });
  }, [searchContentVisible]);

  const [sortValue, setSortValue] = useState("");
  const [filterDateValue, setFilterDateValue] = useState("");
  const [filterSizeValue, setFilterSizeValue] = useState("");
  const [currentLocation, setCurrentLocation] = useState(point([0, 0]));
  const [events, setEvents] = useState<eventShard[]>([]); 

  const pollCurrentLocation = () => {
    Geolocation.getCurrentPosition(
      position => {
        setCurrentLocation(point([position.coords.longitude, position.coords.latitude]));
      },
      error => {
        console.log(error.code, error.message);
      },
      { enableHighAccuracy: true, timeout: 15000, maximumAge: 10000 }
    );
  }

  useEffect(() => {
    pollCurrentLocation();

    getAllEvents(currentLocation.geometry.coordinates[0], currentLocation.geometry.coordinates[1], 1000000000000000)
    .then(value => { setEvents(value); })
    .catch(() => "SESSION ERROR");
  }, []);

  useEffect(() => {
    if (sortValue === "Closest") {
      pollCurrentLocation();
    }
  }, [sortValue]);

  const generateSortBy = () => {
    switch (sortValue) {
      case "Most Popular":
        return (x: eventShard, y: eventShard) => { return y.NumberOfGuests - x.NumberOfGuests };
      case "Closest":
        return (x: eventShard, y: eventShard) => { return distance(currentLocation, x.Location) - distance(currentLocation, y.Location) };
      case "Most Recent":
        return (x: eventShard, y: eventShard) => { return y.StartTime.getTime() - x.StartTime.getTime() };
    }
  };

  const generateFilterArray = () => {
    let filterArray = new Array<(e: eventShard) => boolean>();
    let today = new Date();

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
    return filterArray;
  }

  return (
    <View style={{ flex: 1 }}>
      <Map events={events}/>

      <View style={searchContentVisible ? { position: 'absolute', width: width,  backgroundColor: "white" } : { position: 'absolute', width: width }}>
        <View style={{ flexDirection: "row"}}>
          {searchContentVisible ? (
          <Pressable onPress={() => {
            setIsTextInputFocused(false);
            setSearchContentVisible(false);
            Keyboard.dismiss()}}>
            <Animated.View style={animatedIconStyle}>
              <Icon
                name="arrow-back-outline"
                size={40}
                style={styles.icon}
                paddingTop={20}
                paddingLeft={20}
              />
            </Animated.View>
          </Pressable>
          ) : null}

          <SearchBar
            maxLength={256}
            text={searchText}
            setText={setSearchText}
            placeholder="Search for events"
            isFocused={isTextInputFocused}
            setIsFocused={setIsTextInputFocused}
          />
        </View>

        {!searchContentVisible ? (
          <View style={{ alignSelf: 'flex-end', paddingEnd: 24 }}>
            <Button
              type={ButtonType.PrimaryDark}
              size={ButtonSize.ExtraSmall}
              display={ButtonDisplay.Contained}
              text="Create Event"
            />
          </View>
        ) : null}


        <View style={[{ alignItems: "center", paddingBottom: 5 }, !searchContentVisible ? { display: 'none' } : {}]}>
          <ExclusiveButtonView
            groupStyle={styles.searchOptions}
            activeButton={activeButton}
            setActiveButton={setAtiveButton}
            buttons=
            {[
              {
                id: 1,
                type: ButtonType.PrimaryDark,
                size: ButtonSize.ExtraSmall,
                display: ButtonDisplay.Full,
                text: "Filter",
                icon: "filter-fill",
                onPress: () => { toggleActiveComponent(ActiveComponent.Filter); }
              },
              {
                id: 2,
                type: ButtonType.PrimaryDark,
                size: ButtonSize.ExtraSmall,
                display: ButtonDisplay.Full,
                text: "Sort",
                icon: "sort-outline",
                onPress: () => { toggleActiveComponent(ActiveComponent.Sort); }
              }
            ]} />
        </View>



        <View style={[{ backgroundColor: Colors.sparrowBrown }, !(activeComponent === ActiveComponent.Sort && searchContentVisible) ? { display: 'none' } : {}]}>
          <View style={{ rowGap: Spacing.md, paddingTop: Spacing.lg, height: '100%' }}>
            <Text style={[globalStyles.headingTextThree, globalStyles.textLight, styles.wrapper]}>
              Sort by
            </Text>
            <GestureHandlerRootView>
              <ExclusiveButtonScroll
                setCurrentValue={setSortValue}
                props=
                {{
                  horizontal: true,
                  overScrollMode: "never",
                  showsHorizontalScrollIndicator: false,
                  contentContainerStyle: styles.sortContentWrapper,
                }}
                buttons=
                {[
                  {
                    id: 1,
                    type: ButtonType.SecondaryLight,
                    size: ButtonSize.ExtraSmall,
                    display: ButtonDisplay.Contained,
                    text: "Most Popular",
                    self: 1,
                    status: sortState,
                    changeState: setSortState,
                  },
                  {
                    id: 2,
                    type: ButtonType.SecondaryLight,
                    size: ButtonSize.ExtraSmall,
                    display: ButtonDisplay.Contained,
                    text: "Closest",
                    self: 2,
                    status: sortState,
                    changeState: setSortState
                  },
                  {
                    id: 3,
                    type: ButtonType.SecondaryLight,
                    size: ButtonSize.ExtraSmall,
                    display: ButtonDisplay.Contained,
                    text: "Most Recent",
                    self: 3,
                    status: sortState,
                    changeState: setSortState,
                  }
                ]}
              />
            </GestureHandlerRootView>
          </View>
        </View>



        <View style={[{ backgroundColor: Colors.sparrowBrown }, !(activeComponent === ActiveComponent.Filter && searchContentVisible) ? { display: 'none' } : {}]}>
          <View style={{ rowGap: Spacing.md, paddingTop: Spacing.lg }}>
            <Text style={[globalStyles.headingTextThree, globalStyles.textLight, styles.wrapper]}>
              Date
            </Text>
            <GestureHandlerRootView>
              <ExclusiveButtonScroll
                setCurrentValue={setFilterDateValue}
                props=
                {{
                  horizontal: true,
                  overScrollMode: "never",
                  showsHorizontalScrollIndicator: false,
                  contentContainerStyle: { paddingHorizontal: Spacing.lg },
                }}
                buttons=
                {[
                  {
                    id: 1,
                    type: ButtonType.SecondaryLight,
                    size: ButtonSize.ExtraSmall,
                    display: ButtonDisplay.Contained,
                    text: "Today",
                    self: 1,
                    status: dateState,
                    changeState: setDateState,
                    onPress: () => { console.log("Today") }
                  },
                  {
                    id: 2,
                    type: ButtonType.SecondaryLight,
                    size: ButtonSize.ExtraSmall,
                    display: ButtonDisplay.Contained,
                    text: "Tomorrow",
                    self: 2,
                    status: dateState,
                    changeState: setDateState,
                    onPress: () => { console.log("Tomorrow") }
                  },
                  {
                    id: 3,
                    type: ButtonType.SecondaryLight,
                    size: ButtonSize.ExtraSmall,
                    display: ButtonDisplay.Contained,
                    text: "This Week",
                    self: 3,
                    status: dateState,
                    changeState: setDateState,
                    onPress: () => { console.log("This week") }
                  },
                  {
                    id: 4,
                    type: ButtonType.SecondaryLight,
                    size: ButtonSize.ExtraSmall,
                    display: ButtonDisplay.Contained,
                    text: "This Weekend",
                    self: 4,
                    status: dateState,
                    changeState: setDateState,
                    onPress: () => { console.log("This weekend") }
                  },
                  {
                    id: 5,
                    type: ButtonType.SecondaryLight,
                    size: ButtonSize.ExtraSmall,
                    display: ButtonDisplay.Contained,
                    text: "Next Week",
                    self: 5,
                    status: dateState,
                    changeState: setDateState,
                    onPress: () => { console.log("Next week") }
                  },
                  {
                    id: 6,
                    type: ButtonType.SecondaryLight,
                    size: ButtonSize.ExtraSmall,
                    display: ButtonDisplay.Contained,
                    text: "Next Weekend",
                    self: 6,
                    status: dateState,
                    changeState: setDateState,
                    onPress: () => { console.log("Next weekend") }
                  }
                ]}
              />
            </GestureHandlerRootView>
          </View>

          {/* TODO buttons need to change style when active (tapped and selected) */}
          <View style={{ rowGap: Spacing.md, paddingTop: Spacing.lg }}>
            <Text style={[globalStyles.headingTextThree, globalStyles.textLight, styles.wrapper]}>
              Size
            </Text>
            <GestureHandlerRootView>
              <ExclusiveButtonScroll
                setCurrentValue={setFilterSizeValue}
                props=
                {{
                  horizontal: true,
                  overScrollMode: "never",
                  showsHorizontalScrollIndicator: false,
                  contentContainerStyle: { paddingHorizontal: Spacing.lg }
                }}
                buttons=
                {[
                  {
                    id: 1,
                    type: ButtonType.SecondaryLight,
                    size: ButtonSize.ExtraSmall,
                    display: ButtonDisplay.Contained,
                    text: "Cozy",
                    self: 1,
                    status: sizeState,
                    changeState: setSizeState,
                    onPress: () => { console.log("Cozy") }
                  },
                  {
                    id: 2,
                    type: ButtonType.SecondaryLight,
                    size: ButtonSize.ExtraSmall,
                    display: ButtonDisplay.Contained,
                    text: "Thriving",
                    self: 2,
                    status: sizeState,
                    changeState: setSizeState,
                    onPress: () => { console.log("Thriving") }
                  },
                  {
                    id: 3,
                    type: ButtonType.SecondaryLight,
                    size: ButtonSize.ExtraSmall,
                    display: ButtonDisplay.Contained,
                    text: "Bombastic",
                    self: 3,
                    status: sizeState,
                    changeState: setSizeState,
                    onPress: () => { console.log("Bombastic") }
                  }
                ]}
              />
            </GestureHandlerRootView>
          </View>

          <View style={{ backgroundColor: Colors.sparrowBrown }}>
            <Text style={[globalStyles.headingTextThree, globalStyles.textLight, styles.wrapper]}>
              Distance
            </Text>
          </View>

          <View style={{ backgroundColor: Colors.sparrowBrown, height: '100%' }}>
            {/* TODO confirm selection button is hidden/broken because of flex:1 from buttonFull style */}
            <View style={styles.wrapper}>
              <Button
                type={ButtonType.Success}
                size={ButtonSize.Medium}
                display={ButtonDisplay.Full}
                text="Confirm selection"
              />
            </View>
          </View>
        </View>
      </View>

      {/* TODO probably have to enable it and disable filter or sort if there's text input in the textInput component */}
      {activeComponent === ActiveComponent.None && searchContentVisible ? (
        <View style={isTextInputFocused ? { paddingTop: 130 } : { paddingTop: 75 }}>
          <SearchFilter
            list={events}
            sortBy={generateSortBy()}
            filterBy={generateFilterArray()}
          />
        </View>
      ) : null}


    </View>
  );
};

const styles = StyleSheet.create({
  icon: {
    color: Colors.sparrowDark,
    flex: 1
  },

  sortContentWrapper: {
    paddingHorizontal: Spacing.lg,
    gap: Spacing.md,
    flexDirection: 'row',
    flexWrap: 'wrap',
  },

  wrapper: {
    paddingHorizontal: Spacing.md,
  },

  buttonGap: {
    marginRight: Spacing.md,
  },

  container: {
    rowGap: Spacing.md,
  },

  map: {
    flex: 1
  },
  page: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "#F5FCFF"
  },
  mapContainer: {
    height: 300,
    width: 300,
    backgroundColor: "tomato"
  },

  searchOptions: {
    flexDirection: 'row',
    columnGap: Spacing.md,
    paddingTop: Spacing.md,
  },
});

export default DiscoveryScreen;
