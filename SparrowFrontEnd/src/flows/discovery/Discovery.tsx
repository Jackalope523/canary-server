import * as React from 'react';
import {
  View,
  Text,
  StyleSheet,
  TextInput,
  Pressable,
  ImageBackground,
  FlatList,
} from 'react-native';
import SearchBar from '../../components/discovery/SearchBar';
import { Colors } from '../../styles/ColorStyles';
import { globalStyles } from '../../styles/GlobalStyles';
import { navigationStyles } from '../../styles/NavigationStyles';
import { Spacing } from '../../styles/SpacingStyles';

import Button, {
  ButtonType,
  ButtonSize,
  ButtonDisplay,
} from '../../components/Button';
import EventCardMedium from '../../components/EventCardMedium';
import { SAMPLEEVENTDATA } from '../../data/sampleEventData';
import SearchFilter from '../../components/organisms/SearchFilter';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
import { SafeAreaView } from 'react-native-safe-area-context';
import {
  GestureHandlerRootView,
  ScrollView,
} from 'react-native-gesture-handler';
import { buttonStyles } from '../../styles/ButtonStyles';
import ExclusiveButtonView from '../../components/ExclusiveButtonView';
import ExclusiveButtonScroll from '../../components/ExclusiveButtonScroll';

const Icon = createIconSetFromFontello(fontelloConfig);

// TEMP. map image - replace with actual working map
const tempMapImage = require('../../assets/images/temp/temp-map.png');

const DiscoveryScreen = () => {
  const [searchContentVisible, setSearchContentVisible] = React.useState(false);
  const [searchText, setSearchText] = React.useState('');

  // Search bar text input
  const [isTextInputFocused, setIsTextInputFocused] = React.useState(false);

  // Toggle search bar
  const toggleSearch = () => {
    if (!searchContentVisible) {
      setSearchContentVisible(true);
      // setSearchText('');
    }
  };

  // Toggle search close button
  const toggleClose = () => {
    setSearchContentVisible(false);
    setSearchText('');
  };

  // Search options
  const [activeComponent, setActiveComponent] = React.useState(null);

  // Toggle filter
  const [filterVisible, setFilterVisible] = React.useState(false);

  const toggleFilter = () => {
    // setFilterVisible(!filterVisible);

    if (activeComponent === 'filter') {
      setActiveComponent(null);
    } else {
      setActiveComponent('filter');
    }
  };

  // Toggle sort
  const [sortVisible, setSortVisible] = React.useState(false);

  const toggleSort = () => {
    // setSortVisible(!sortVisible);
    if (activeComponent === 'sort') {
      setActiveComponent(null);
    } else {
      setActiveComponent('sort');
    }
  };

  // Filter and sort options button states
  // Date
  const [dateState, setDateState] = React.useState(-1);

  // Size
  const [sizeState, setSizeState] = React.useState(-1);

  // Distance
  // TODO make a distance range slider

  // Sort by
  const [sortState, setSortState] = React.useState(-1);

  // TODO split up the page into components - implement the search bar from Discovery/SearchBar and Discovery Search in a different screen

  return (
    <View style={styles.mapWrapper}>
      <ImageBackground
        source={tempMapImage}
        resizeMode="cover"
        style={styles.mapImage}>
        <View>
          {/* Search */}
          {/* Search header */}
          <View>
            <View
              style={
                searchContentVisible
                  ? navigationStyles.searchHeaderWrapper
                  : null
              }>
              <View style={navigationStyles.searchHeader}>
                {/* Search bar */}
                <View style={navigationStyles.searchBarWrapper}>
                  <View style={navigationStyles.searchBar}>
                    <Icon
                      name="search-outline"
                      style={buttonStyles.buttonIconSmallDark}
                    />
                    <TextInput
                      style={navigationStyles.searchBarTextInput}
                      color={Colors.sparrowDark}
                      onPressIn={toggleSearch}
                      placeholder="Search for events"
                      placeholderTextColor={Colors.sparrowDark}
                      value={searchText}
                      onChangeText={(text) => setSearchText(text)}
                      autoCorrect={false}
                      autoCapitalize="none"
                      onFocus={() => setIsTextInputFocused(true)}
                      onBlur={() => setIsTextInputFocused(false)}
                    />
                    {isTextInputFocused && searchText ? (
                      <Pressable onPress={() => setSearchText('')}>
                        <Icon
                          name="close-fill"
                          style={buttonStyles.buttonIconSmallDark}
                        />
                      </Pressable>
                    ) : null}
                  </View>

                  {searchContentVisible ? (
                    <Pressable
                      onPress={toggleClose}
                      style={
                        searchContentVisible
                          ? navigationStyles.searchBarWrapperCloseButtonWrapper
                          : null
                      }>
                      {/* TODO this icon isn't perfectly vertically aligned - fix that */}
                      <Icon
                        name="close-outline"
                        style={buttonStyles.buttonIconMediumDark}
                      />
                    </Pressable>
                  ) : null}
                </View>

                {/* Search options */}

                {searchContentVisible ? (
                  <View>
                    {/* TODO make FILTER and SORT buttons functional */}
                    <ExclusiveButtonView 
                    groupStyle = {navigationStyles.searchOptions}  
                    buttons = 
                    {
                      [
                        {
                          id: 1,
                          type: ButtonType.PrimaryDark,
                          size: ButtonSize.ExtraSmall,
                          display: ButtonDisplay.Full,
                          text: "Filter",
                          icon: "filter-fill",
                          onPress: toggleFilter
                        },
                        {
                          id: 2,
                          type: ButtonType.PrimaryDark,
                          size: ButtonSize.ExtraSmall,
                          display: ButtonDisplay.Full,
                          text: "Sort",
                          icon: "sort-outline",
                          onPress: toggleSort
                        }
                      ]
                    }/>                        
                    {/* TODO fix last 2 events not visible */}
                    {/* Potential not-ideal fix = screen height - (searchBarWrapper height + filterSort height) */}
                    {/* TODO maybe add an opacity 0 to opacity 100 gradient at the top */}
                  </View>
                ) : null}
              </View>
            </View>
          </View>

          {/* TODO if sort and filter doesn't overlay SearchFilter - hide SearchFilter when sort or filter is active/visible */}
          {/* TODO make only one open - sort or filter - both cant be open at once */}
          {activeComponent === 'filter' && searchContentVisible ? (
            <View style={navigationStyles.searchOptionsInner}>
              <View style={navigationStyles.searchOptionsInnerSection}>
                <Text
                  style={[
                    globalStyles.headingTextThree,
                    globalStyles.textLight,
                    styles.wrapper,
                  ]}>
                  Date
                </Text>
                <GestureHandlerRootView>
                <ExclusiveButtonScroll 
                    props = 
                    {
                      {
                        horizontal: true,
                        overScrollMode: "never",
                        showsHorizontalScrollIndicator: false,
                        contentContainerStyle: {paddingHorizontal: Spacing.lg}, 
                      }  
                    } 
                    buttons = 
                    {
                      [
                        {
                          id: 1,
                          type: ButtonType.SecondaryLight,
                          size: ButtonSize.ExtraSmall,
                          display: ButtonDisplay.Contained,
                          text: "Today",
                          self: 1,
                          status: dateState,
                          changeState: setDateState,
                        },
                        {
                          id: 2,
                          type: ButtonType.SecondaryLight,
                          size: ButtonSize.ExtraSmall,
                          display: ButtonDisplay.Contained,
                          text: "Tomorrow",
                          self: 2,
                          status: dateState,
                          changeState: setDateState
                        },
                        {
                          id: 3,
                          type: ButtonType.SecondaryLight,
                          size: ButtonSize.ExtraSmall,
                          display: ButtonDisplay.Contained,
                          text: "This week",
                          self: 3,
                          status: dateState,
                          changeState: setDateState
                        },
                        {
                          id: 4,
                          type: ButtonType.SecondaryLight,
                          size: ButtonSize.ExtraSmall,
                          display: ButtonDisplay.Contained,
                          text: "This weekend",
                          self: 4,
                          status: dateState,
                          changeState: setDateState
                        },
                        {
                          id: 5,
                          type: ButtonType.SecondaryLight,
                          size: ButtonSize.ExtraSmall,
                          display: ButtonDisplay.Contained,
                          text: "Next week",
                          self: 5,
                          status: dateState,
                          changeState: setDateState,
                        },
                        {
                          id: 6,
                          type: ButtonType.SecondaryLight,
                          size: ButtonSize.ExtraSmall,
                          display: ButtonDisplay.Contained,
                          text: "Next weekend",
                          self: 6,
                          status: dateState,
                          changeState: setDateState
                        }
                      ]
                    }/>                         
                </GestureHandlerRootView>
              </View>

              {/* TODO buttons need to change style when active (tapped and selected) */}
              <View style={navigationStyles.searchOptionsInnerSection}>
                <Text
                  style={[
                    globalStyles.headingTextThree,
                    globalStyles.textLight,
                    styles.wrapper,
                  ]}>
                  Size
                </Text>
                <GestureHandlerRootView>
                  <ExclusiveButtonScroll
                    props = 
                    {
                      {
                        horizontal: true,
                        overScrollMode: "never",
                        showsHorizontalScrollIndicator: false,
                        contentContainerStyle: {paddingHorizontal: Spacing.lg}
                      }
                    }
                    buttons = 
                    {
                      [
                        {
                          id: 1,
                          type: ButtonType.SecondaryLight,
                          size: ButtonSize.ExtraSmall,
                          display: ButtonDisplay.Contained,
                          text: "Cozy",
                          self: 1,
                          status: sizeState,
                          changeState: setSizeState
                        },
                        {
                          id: 2,
                          type: ButtonType.SecondaryLight,
                          size: ButtonSize.ExtraSmall,
                          display: ButtonDisplay.Contained,
                          text: "Thriving",
                          self: 2,
                          status: sizeState,
                      changeState: setSizeState
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
                        }
                      ]
                    }
                  />
                </GestureHandlerRootView>
              </View>
              <View style={navigationStyles.searchOptionsInnerSection}>
                <Text
                  style={[
                    globalStyles.headingTextThree,
                    globalStyles.textLight,
                    styles.wrapper,
                  ]}>
                  Distance
                </Text>
              </View>
              <View style={navigationStyles.searchOptionsInnerSection}>
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
          ) : null}

          {activeComponent === 'sort' && searchContentVisible ? (
            <View style={navigationStyles.searchOptionsInner}>
              <View style={navigationStyles.searchOptionsInnerSection}>
                <Text
                  style={[
                    globalStyles.headingTextThree,
                    globalStyles.textLight,
                    styles.wrapper,
                  ]}>
                  Sort by
                </Text>
                <GestureHandlerRootView>
                <ExclusiveButtonView 
                    groupStyle = {styles.sortContentWrapper}  
                    buttons = 
                    {
                      [
                        {
                          id: 1,
                          type: ButtonType.SecondaryLight,
                          size: ButtonSize.ExtraSmall,
                          display: ButtonDisplay.Contained,
                          text: "Most popular",
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
                          text: "Most recent",
                          self: 3,
                          status: sortState,
                          changeState: setSortState,
                        }
                      ]
                    }/>                     
                </GestureHandlerRootView>
              </View>
            </View>
          ) : null}

          {/* TODO probably have to enable it and disable filter or sort if there's text input in the textInput component */}
          {!activeComponent && searchContentVisible ? (
            <View style={navigationStyles.searchContent}>
              <SearchFilter />
            </View>
          ) : null}

          {!searchContentVisible && (
            <View style={styles.buttonWrapper}>
              <Button
                type={ButtonType.PrimaryDark}
                size={ButtonSize.ExtraSmall}
                display={ButtonDisplay.Contained}
                text="Create Event"
              />
            </View>
          )}
        </View>
      </ImageBackground>
    </View>
  );
};

export default DiscoveryScreen;

const styles = StyleSheet.create({
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

  // Map
  mapWrapper: {
    flex: 1,
  },

  mapImage: {
    flex: 1,
  },

  // Create event button wrapper
  buttonWrapper: {
    alignSelf: 'flex-end',
    marginHorizontal: 24,
  },
});
