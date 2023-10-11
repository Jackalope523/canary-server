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
    const [searchContentVisible, setsearchContentVisible] = React.useState(false);
    const [searchText, setSearchText] = React.useState('');

    // Search bar text input
    const [isTextInputFocused, setIsTextInputFocused] = React.useState(false);

    // Toggle search bar
    const toggleSearch = () => {
        if (!searchContentVisible) {
            setsearchContentVisible(true);
            // setSearchText('');
        }
    };

    // Toggle search close button
    const toggleClose = () => {
        setsearchContentVisible(false);
        setSearchText('');
    };


    return (
        <View style={styles.mapWrapper}>
            <ImageBackground source={tempMapImage} resizeMode='cover' style={styles.mapImage}>
                <View>
                    {/* Search */}
                    {/* Search header */}
                    <View style={navigationStyles.search}>

                        {/* NEW CODE HERE ----------------------------->>>>>>> */}
                        <View style={searchContentVisible ? navigationStyles.search.headerWrapper : null}>
                            <View style={navigationStyles.search.headerWrapper.header}>

                        {/* Search bar */}
                        {/* style={searchContentVisible ? navigationStyles.search.searchBarWrapper : navigationStyles.search.searchBarWrapper}> */}
                        <View style={navigationStyles.search.searchBarWrapper}>
                            {/* TODO add CANCEL SEARCH (round x) button */}
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
                                    onPress={null}
                                />
                                <Button
                                    btnText={'Sort'}
                                    btnIcon={'sort-outline'}
                                    btnIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                                    btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.textPrimary, globalStyles.buttonFull]}
                                    btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                                    onPress={null}
                                />
                            </View>


                            {/* TODO fix last 2 events not visible */}
                            {/* Potential not-ideal fix = screen height - (searchBarWrapper height + filterSort height) */}
                            {/* TODO maybe add an opacity 0 to opacity 100 gradient at the top */}

                            {/* NEW CODE */}

                            {/* PREVIOUS CODE */}
                            {/* <SearchFilter /> */}

                        </View>

                        ) : null }
                        </View>
                        </View>
                    </View>

                    {searchContentVisible ? (
                    <View style={navigationStyles.search.searchContent}>
                        <SearchFilter />
                    </View>
                    ) : null}

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
    // Wraps all search elements
    searchWrapper: {
        // TODO enable bgC when the screen is finished
        // backgroundColor: Colors.sparrowSand,
    },

    container: {
        rowGap: 16,
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

    // TODO delete styles below

    // TEMP. Search bar
    // searchBarContainer: {
    //     backgroundColor: Colors.orange200,
    // },

    // searchContent: {
    //     // temp. background color for testing purposes - replace with sparrow sand later
    //     backgroundColor: Colors.azure200,
    // },

    // searchBarStylesVisible: {
    //     backgroundColor: 'red',
    //     flex: 3,

    //     height: 50,
    // },

    // searchCloseStylesVisible: {
    //     backgroundColor: 'blue',
    //     flex: 1,

    //     height: 50,
    // },

    // searchBarInnerContainerVisible: {
    //     flexDirection: 'row',
    // },
})