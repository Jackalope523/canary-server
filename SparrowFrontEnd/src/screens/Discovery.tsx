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
                                    btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.textPrimary, globalStyles.buttonFull]}
                                    btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                    onPress={toggleFilter}
                                />
                                <Button
                                    btnText={'Sort'}
                                    btnIcon={'sort-outline'}
                                    btnIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                                    btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.textPrimary, globalStyles.buttonFull]}
                                    btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                    onPress={toggleSort}
                                />
                            </View>
                            {/* TODO fix last 2 events not visible */}
                            {/* Potential not-ideal fix = screen height - (searchBarWrapper height + filterSort height) */}
                            {/* TODO maybe add an opacity 0 to opacity 100 gradient at the top */}
                        </View>

                        ) : null }
                        </View>
                        </View>
                    </View>

                    {/* TODO insert sort and filter views here */}
                    {/* TODO if sort and filter doesn't overlay SearchFilter - hide SearchFilter when sort or filter is active/visible */}
                    {/* TODO make only one open - sort or filter - both cant be open at once */}
                    {activeComponent === 'filter' ? (
                        <View>
                            <Text style={globalStyles.headingTextOne}>FILTER VIEW</Text>
                        </View>
                    ) : null }

                    {activeComponent === 'sort' ? (
                        <View>
                            <Text style={globalStyles.headingTextTwo}>SORT VIEW</Text>
                        </View>
                    ) : null }


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
                            btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.textPrimary, globalStyles.buttonContained]}
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