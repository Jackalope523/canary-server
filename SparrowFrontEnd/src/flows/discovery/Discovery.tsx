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
import { Colors } from '../../styles/Colors';
import { globalStyles } from '../../styles/Global';
import { navigationStyles } from '../../styles/Navigation';
import { Spacing } from '../../styles/Spacing';

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
          <View style={navigationStyles.search}>
            <View
              style={
                searchContentVisible
                  ? navigationStyles.search.headerWrapper
                  : null
              }>
              <View style={navigationStyles.search.headerWrapper.header}>
                {/* Search bar */}
                <View style={navigationStyles.search.searchBarWrapper}>
                  <View
                    style={navigationStyles.search.searchBarWrapper.searchBar}>
                    <Icon
                      name="search-outline"
                      style={[
                        globalStyles.buttonIconSmall,
                        globalStyles.buttonIconSmall.dark,
                      ]}
                    />
                    <TextInput
                      style={
                        navigationStyles.search.searchBarWrapper.searchBar
                          .textInput
                      }
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
                          style={[
                            globalStyles.buttonIconSmall,
                            globalStyles.buttonIconSmall.dark,
                          ]}
                        />
                      </Pressable>
                    ) : null}
                  </View>

                  {searchContentVisible ? (
                    <Pressable
                      onPress={toggleClose}
                      style={
                        searchContentVisible
                          ? navigationStyles.search.searchBarWrapper
                              .closeButtonWrapper
                          : null
                      }>
                      {/* TODO this icon isn't perfectly vertically aligned - fix that */}
                      <Icon
                        name="close-outline"
                        style={[
                          globalStyles.buttonIconMedium,
                          globalStyles.buttonIconMedium.dark,
                        ]}
                      />
                    </Pressable>
                  ) : null}
                </View>

                {/* Search options */}

                {searchContentVisible ? (
                  <View style={navigationStyles.search.searchOptionsWrapper}>
                    {/* TODO make FILTER and SORT buttons functional */}
                    <View
                      style={
                        navigationStyles.search.searchOptionsWrapper
                          .searchOptions
                      }>
                      <Button
                        type={ButtonType.PrimaryDark}
                        size={ButtonSize.ExtraSmall}
                        display={ButtonDisplay.Full}
                        btnText="Filter"
                        btnIcon="filter-fill"
                        onPress={toggleFilter}
                      />

                      <Button
                        type={ButtonType.PrimaryDark}
                        size={ButtonSize.ExtraSmall}
                        display={ButtonDisplay.Full}
                        btnText="Sort"
                        btnIcon="sort-outline"
                        onPress={toggleSort}
                      />
                    </View>
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
            <View style={navigationStyles.search.searchOptionsInnerContainer}>
              <View
                style={
                  navigationStyles.search.searchOptionsInnerContainer.section
                }>
                <Text
                  style={[
                    globalStyles.headingTextThree,
                    globalStyles.textLight,
                    styles.wrapper,
                  ]}>
                  Date
                </Text>
                <GestureHandlerRootView>
                  <ScrollView
                    horizontal={true}
                    overScrollMode="never"
                    showsHorizontalScrollIndicator={false}
                    contentContainerStyle={{ paddingHorizontal: Spacing.lg }}>
                    <Button
                      type={ButtonType.SecondaryLight}
                      size={ButtonSize.ExtraSmall}
                      display={ButtonDisplay.Contained}
                      btnText="Today"
                      onPress={null}
                      self={1}
                      status={dateState}
                      changeState={setDateState}
                    />

                    <Button
                      type={ButtonType.SecondaryLight}
                      size={ButtonSize.ExtraSmall}
                      display={ButtonDisplay.Contained}
                      btnText="Tomorrow"
                      onPress={null}
                      self={2}
                      status={dateState}
                      changeState={setDateState}
                    />

                    <Button
                      type={ButtonType.SecondaryLight}
                      size={ButtonSize.ExtraSmall}
                      display={ButtonDisplay.Contained}
                      btnText="This week"
                      onPress={null}
                      self={3}
                      status={dateState}
                      changeState={setDateState}
                    />

                    <Button
                      type={ButtonType.SecondaryLight}
                      size={ButtonSize.ExtraSmall}
                      display={ButtonDisplay.Contained}
                      btnText="This weekend"
                      onPress={null}
                      self={4}
                      status={dateState}
                      changeState={setDateState}
                    />

                    <Button
                      type={ButtonType.SecondaryLight}
                      size={ButtonSize.ExtraSmall}
                      display={ButtonDisplay.Contained}
                      btnText="Next week"
                      onPress={null}
                      self={5}
                      status={dateState}
                      changeState={setDateState}
                    />

                    <Button
                      type={ButtonType.SecondaryLight}
                      size={ButtonSize.ExtraSmall}
                      display={ButtonDisplay.Contained}
                      btnText="Next weekend"
                      onPress={null}
                      self={6}
                      status={dateState}
                      changeState={setDateState}
                    />
                  </ScrollView>
                </GestureHandlerRootView>
              </View>

              {/* TODO buttons need to change style when active (tapped and selected) */}
              <View
                style={
                  navigationStyles.search.searchOptionsInnerContainer.section
                }>
                <Text
                  style={[
                    globalStyles.headingTextThree,
                    globalStyles.textLight,
                    styles.wrapper,
                  ]}>
                  Size
                </Text>
                <GestureHandlerRootView>
                  <ScrollView
                    horizontal={true}
                    overScrollMode="never"
                    showsHorizontalScrollIndicator={false}
                    contentContainerStyle={{ paddingHorizontal: Spacing.lg }}>
                    <Button
                      type={ButtonType.SecondaryLight}
                      size={ButtonSize.ExtraSmall}
                      display={ButtonDisplay.Contained}
                      btnText="Cozy"
                      onPress={null}
                      self={1}
                      status={sizeState}
                      changeState={setSizeState}
                    />

                    <Button
                      type={ButtonType.SecondaryLight}
                      size={ButtonSize.ExtraSmall}
                      display={ButtonDisplay.Contained}
                      btnText="Thriving"
                      onPress={null}
                      self={2}
                      status={sizeState}
                      changeState={setSizeState}
                    />

                    <Button
                      type={ButtonType.SecondaryLight}
                      size={ButtonSize.ExtraSmall}
                      display={ButtonDisplay.Contained}
                      btnText="Bombastic"
                      onPress={null}
                      self={3}
                      status={sizeState}
                      changeState={setSizeState}
                    />
                  </ScrollView>
                </GestureHandlerRootView>
              </View>
              <View
                style={
                  navigationStyles.search.searchOptionsInnerContainer.section
                }>
                <Text
                  style={[
                    globalStyles.headingTextThree,
                    globalStyles.textLight,
                    styles.wrapper,
                  ]}>
                  Distance
                </Text>
              </View>
              <View
                style={
                  navigationStyles.search.searchOptionsInnerContainer.section
                }>
                {/* TODO confirm selection button is hidden/broken because of flex:1 from buttonFull style */}
                <View style={styles.wrapper}>
                  <Button
                    type={ButtonType.PrimaryLight}
                    size={ButtonSize.Medium}
                    display={ButtonDisplay.Full}
                    btnText="Confirm selection"
                    onPress={null}
                  />
                </View>
              </View>
            </View>
          ) : null}

          {activeComponent === 'sort' && searchContentVisible ? (
            <View style={navigationStyles.search.searchOptionsInnerContainer}>
              <View
                style={
                  navigationStyles.search.searchOptionsInnerContainer.section
                }>
                <Text
                  style={[
                    globalStyles.headingTextThree,
                    globalStyles.textLight,
                    styles.wrapper,
                  ]}>
                  Sort by
                </Text>
                <GestureHandlerRootView>
                  <View style={styles.sortContentWrapper}>
                    <Button
                      type={ButtonType.SecondaryLight}
                      size={ButtonSize.ExtraSmall}
                      display={ButtonDisplay.Contained}
                      btnText="Most popular"
                      onPress={null}
                      self={1}
                      status={sortState}
                      changeState={setSortState}
                    />

                    <Button
                      type={ButtonType.SecondaryLight}
                      size={ButtonSize.ExtraSmall}
                      display={ButtonDisplay.Contained}
                      btnText="Closest"
                      onPress={null}
                      self={2}
                      status={sortState}
                      changeState={setSortState}
                    />

                    <Button
                      type={ButtonType.SecondaryLight}
                      size={ButtonSize.ExtraSmall}
                      display={ButtonDisplay.Contained}
                      btnText="Most recent"
                      onPress={null}
                      self={3}
                      status={sortState}
                      changeState={setSortState}
                    />
                  </View>
                </GestureHandlerRootView>
              </View>
            </View>
          ) : null}

          {/* TODO probably have to enable it and disable filter or sort if there's text input in the textInput component */}
          {!activeComponent && searchContentVisible ? (
            <View style={navigationStyles.search.searchContent}>
              <SearchFilter />
            </View>
          ) : null}

          {!searchContentVisible && (
            <View style={styles.buttonWrapper}>
              <Button
                type={ButtonType.PrimaryDark}
                size={ButtonSize.ExtraSmall}
                display={ButtonDisplay.Contained}
                btnText="Create Event"
                onPress={null}
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
