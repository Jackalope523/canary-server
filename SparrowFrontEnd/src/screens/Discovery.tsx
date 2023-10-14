import * as React from 'react';
import { View, Text, StyleSheet, TextInput, Pressable, ImageBackground, FlatList } from 'react-native';

import SearchBar from '../components/molecules/SearchBar';
import { Colors } from '../styles/Colors';
import { globalStyles } from '../styles/Global';
import { navigationStyles } from '../styles/Navigation';
import { Spacing } from '../styles/Spacing';

import Button from '../components/atoms/Button';
import EventCardMedium from '../components/organisms/EventCardMedium';
import { SAMPLEEVENTDATA } from '../data/sampleEventData';
import SearchFilter from '../components/organisms/SearchFilter';


// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';
import { SafeAreaView } from 'react-native-safe-area-context';
import { GestureHandlerRootView, ScrollView } from 'react-native-gesture-handler';

const Icon = createIconSetFromFontello(fontelloConfig);

// TEMP. map image - replace with actual working map
const tempMapImage = require('../assets/images/temp/temp-map.png');

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

    const [state, setState] = React.useState(-1);

    // TODO when SEARCH is CLOSED - hide SORT view and/or FILTER view

    return (
        <View style={styles.mapWrapper}>
            <ImageBackground source={tempMapImage} resizeMode='cover' style={styles.mapImage}>
                <View>
                    {/* Search */}
                    {/* Search header */}
                    <View style={navigationStyles.search}>
                        <View style={searchContentVisible ? navigationStyles.search.headerWrapper : null}>
                            <View style={navigationStyles.search.headerWrapper.header}>

                        {/* Search bar */}
                        <View style={navigationStyles.search.searchBarWrapper}>
                            <View style={navigationStyles.search.searchBarWrapper.searchBar}>
                                <Icon name="search-outline" style={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.dark]} />
                                <TextInput
                                    style={navigationStyles.search.searchBarWrapper.searchBar.textInput}
                                    color={Colors.sparrowDark}
                                    onPressIn={toggleSearch}
                                    placeholder='Search for events'
                                    placeholderTextColor={Colors.sparrowDark}
                                    value={searchText}
                                    onChangeText={(text) => setSearchText(text)}
                                    autoCorrect={false}
                                    autoCapitalize='none'
                                    onFocus={() => setIsTextInputFocused(true)}
                                    onBlur={() => setIsTextInputFocused(false)}
                                    />
                                {isTextInputFocused && searchText ? (
                                <Pressable onPress={() => setSearchText('')}>
                                    <Icon name="close-fill" style={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.dark]} />
                                </Pressable>
                                ) : null }
                            </View>

                            {searchContentVisible ? (
                            <Pressable onPress={toggleClose} style={searchContentVisible ? navigationStyles.search.searchBarWrapper.closeButtonWrapper : null}>
                                {/* TODO this icon isn't perfectly vertically aligned - fix that */}
                                <Icon name="close-outline" style={[globalStyles.buttonIconMedium, globalStyles.buttonIconMedium.dark]} />
                            </Pressable>
                            ) : null }
                        </View>

                        {/* Search options */}

                        {searchContentVisible ? (

                        <View style={navigationStyles.search.searchOptionsWrapper}>

                            {/* TODO make FILTER and SORT buttons functional */}
                            <View style={navigationStyles.search.searchOptionsWrapper.searchOptions}>
                                <Button
                                    btnText={'Filter'}
                                    btnIcon={'filter-fill'}
                                    btnIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                                    btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonFull, globalStyles.buttonPrimary]}
                                    btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                    onPress={toggleFilter}
                                />
                                <Button
                                    btnText={'Sort'}
                                    btnIcon={'sort-outline'}
                                    btnIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                                    btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonPrimary, globalStyles.buttonFull]}
                                    btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                    onPress={toggleSort}
                                />
                                {/* <Button
                                    btnText={'Filter'}
                                    btnIcon={'filter-fill'}
                                    btnIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                                    btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonFull, globalStyles.buttonPrimary, activeComponent === 'filter' ? globalStyles.buttonPrimaryLight : null]}
                                    btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                    onPress={toggleFilter}
                                />
                                <Button
                                    btnText={'Sort'}
                                    btnIcon={'sort-outline'}
                                    btnIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                                    btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonPrimary, globalStyles.buttonFull, activeComponent === 'sort' ? globalStyles.buttonPrimaryLight : null]}
                                    btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                    onPress={toggleSort}
                                /> */}
                            </View>
                            {/* TODO fix last 2 events not visible */}
                            {/* Potential not-ideal fix = screen height - (searchBarWrapper height + filterSort height) */}
                            {/* TODO maybe add an opacity 0 to opacity 100 gradient at the top */}
                        </View>

                        ) : null }
                        </View>
                        </View>
                    </View>

                    {/* TODO if sort and filter doesn't overlay SearchFilter - hide SearchFilter when sort or filter is active/visible */}
                    {/* TODO make only one open - sort or filter - both cant be open at once */}
                    {activeComponent === 'filter' && searchContentVisible ? (
                        <View style={navigationStyles.search.searchOptionsInnerContainer}>
                            <View style={navigationStyles.search.searchOptionsInnerContainer.section}>
                                <Text style={[globalStyles.headingTextThree, globalStyles.textLight, styles.titleWrapper]}>Date</Text>
                                <GestureHandlerRootView>
                                    <ScrollView horizontal={true} overScrollMode="never" showsHorizontalScrollIndicator={false} contentContainerStyle={{paddingHorizontal: Spacing.lg}}>
                                        <Button
                                            btnText={'Today'}
                                            btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight.outline, styles.buttonGap]}
                                            btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                            btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight, styles.buttonGap]}
                                            btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textDark]}
                                            onPress={null}
                                            self = {1}
                                            status = {state}
                                            changeState = {setState}

                                        />
                                        <Button
                                            btnText={'Tomorrow'}
                                            btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight.outline, styles.buttonGap]}
                                            btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                            btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight, styles.buttonGap]}
                                            btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textDark]}
                                            onPress={null}
                                            self = {2}
                                            status = {state}
                                            changeState = {setState}
                                        />
                                        <Button
                                            btnText={'This week'}
                                            btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight.outline, styles.buttonGap]}
                                            btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                            btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight, styles.buttonGap]}
                                            btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textDark]}
                                            onPress={null}
                                            self = {3}
                                            status = {state}
                                            changeState = {setState}
                                        />
                                        <Button
                                            btnText={'This weekend'}
                                            btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight.outline, styles.buttonGap]}
                                            btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                            btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight, styles.buttonGap]}
                                            btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textDark]}
                                            onPress={null}
                                            self = {4}
                                            status = {state}
                                            changeState = {setState}
                                        />
                                        <Button
                                            btnText={'Next week'}
                                            btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight.outline, styles.buttonGap]}
                                            btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                            btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight, styles.buttonGap]}
                                            btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textDark]}
                                            onPress={null}
                                            self = {5}
                                            status = {state}
                                            changeState = {setState}
                                        />
                                        <Button
                                            btnText={'Next weekend'}
                                            btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight.outline, styles.buttonGap]}
                                            btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                            btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight, styles.buttonGap]}
                                            btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textDark]}
                                            onPress={null}
                                            self = {6}
                                            status = {state}
                                            changeState = {setState}
                                        />
                                    </ScrollView>
                                </GestureHandlerRootView>
                            </View>

                            {/* TODO buttons need to change style when active (tapped and selected) */}
                            <View style={navigationStyles.search.searchOptionsInnerContainer.section}>
                            <Text style={[globalStyles.headingTextThree, globalStyles.textLight, styles.titleWrapper]}>Size</Text>
                                <GestureHandlerRootView>
                                    <ScrollView horizontal={true} overScrollMode="never" showsHorizontalScrollIndicator={false} contentContainerStyle={{paddingHorizontal: Spacing.lg}}>
                                        <Button
                                            btnText={'Cozy'}
                                            btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight.outline, styles.buttonGap]}
                                            btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                            btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight, styles.buttonGap]}
                                            btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textDark]}
                                            onPress={null}
                                        />
                                        <Button
                                            btnText={'Thriving'}
                                            btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight.outline, styles.buttonGap]}
                                            btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                            btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight, styles.buttonGap]}
                                            btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textDark]}
                                            onPress={null}
                                        />
                                        <Button
                                            btnText={'Bombastic'}
                                            btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight.outline, styles.buttonGap]}
                                            btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                            btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight, styles.buttonGap]}
                                            btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textDark]}
                                            onPress={null}
                                        />
                                    </ScrollView>
                                </GestureHandlerRootView>
                            </View>
                            <View style={navigationStyles.search.searchOptionsInnerContainer.section}>
                                <Text style={[globalStyles.headingTextThree, globalStyles.textLight, styles.titleWrapper]}>Distance</Text>
                            </View>
                            <View style={navigationStyles.search.searchOptionsInnerContainer.section}>
                                {/* TODO confirm selection button is hidden/broken because of flex:1 from buttonFull style */}
                                <Button
                                    btnText={'Confirm selection'}
                                    btnStyle={[globalStyles.textButtonMedium, globalStyles.buttonFull, globalStyles.buttonLight]}
                                    btnTextStyle={[globalStyles.textButtonMedium.text, globalStyles.textDark]}
                                    onPress={null}
                                />
                            </View>
                        </View>
                    ) : null }

                    {activeComponent === 'sort' && searchContentVisible ? (
                        <View style={navigationStyles.search.searchOptionsInnerContainer}>
                            <View style={navigationStyles.search.searchOptionsInnerContainer.section}>
                            <Text style={[globalStyles.headingTextThree, globalStyles.textLight, styles.titleWrapper]}>Sort by</Text>
                                <GestureHandlerRootView>
                                    <View style={styles.sortContentWrapper}>
                                        <Button
                                            btnText={'Most popular'}
                                            btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight.outline]}
                                            btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                            btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight]}
                                            btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                            onPress={null}
                                        />
                                        <Button
                                            btnText={'Closest'}
                                            btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight.outline]}
                                            btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                            btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight]}
                                            btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                            onPress={null}
                                        />
                                        <Button
                                            btnText={'Most recent'}
                                            btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight.outline]}
                                            btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                            btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonContained, globalStyles.buttonLight]}
                                            btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                            onPress={null}
                                        />
                                    </View>
                                </GestureHandlerRootView>
                            </View>
                        </View>
                    ) : null }



                    {/* TODO probably have to enable it and disable filter or sort if there's text input in the textInput component */}
                    {!activeComponent && searchContentVisible ? (
                    <View style={navigationStyles.search.searchContent}>
                        <SearchFilter />
                    </View>
                    ) : null }

                    {!searchContentVisible && (
                    <View style={styles.buttonWrapper}>
                        {/* TODO replace onPress={null} with go to CREATE EVENT screen */}
                        <Button
                            btnText={'Create Event'}
                            btnIcon={'add-outline'}
                            btnIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                            btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonPrimary, globalStyles.buttonContained]}
                            btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textButtonExtraSmall.text.uppercase, globalStyles.textLight]}
                            onPress={null}
                        />
                    </View>
                    )}

                </View>
            </ImageBackground>
        </View>
    );
};

export default DiscoveryScreen

const styles = StyleSheet.create ({
    sortContentWrapper: {
        paddingHorizontal: Spacing.lg,
        gap: Spacing.md,
        flexDirection: 'row',
        flexWrap: 'wrap',
    },

    titleWrapper: {
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
})