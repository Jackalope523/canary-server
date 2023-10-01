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
    // const [searchCloseVisible, setSearchCloseVisible] = React.useState(false);
    const [searchText, setSearchText] = React.useState('');

    // Toggle search container
    const toggleSearch = () => {
        setsearchContentVisible(!searchContentVisible);
        // setSearchCloseVisible(!searchCloseVisible);
        setSearchText('');
    };

    // Clear search input text

    return (
        <View style={styles.mapWrapper}>
            <ImageBackground source={tempMapImage} resizeMode='cover' style={styles.mapImage}>
                <View style={[globalStyles.baseContainer, styles.container]}>

                    {/* Search */}
                    <View style={navigationStyles.search}>

                        {/* Search bar */}
                        {/* style={searchContentVisible ? navigationStyles.search.searchBarWrapper : navigationStyles.search.searchBarWrapper}> */}
                        <View style={navigationStyles.search.searchBarWrapper}>
                            {/* TODO add CANCEL SEARCH (round x) button */}
                            <View style={navigationStyles.search.searchBarWrapper.searchBar}>
                                <Icon name="search-outline" style={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.dark]} />
                                <TextInput
                                    style={navigationStyles.search.searchBarWrapper.searchBar.textInput}
                                    onPressIn={toggleSearch}
                                    placeholder='Search for events'
                                    value={searchText}
                                    onChangeText={(text) => setSearchText(text)}
                                    />
                                <Pressable onPress={() => setSearchText('')}>
                                    <Icon name="close-fill" style={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.dark]} />
                                </Pressable>
                            </View>

                            {searchContentVisible ? (
                            <Pressable onPress={toggleSearch} style={searchContentVisible ? navigationStyles.search.searchBarWrapper.closeButtonWrapper : null}>
                                {/* TODO this icon isn't perfectly vertically aligned - fix that */}
                                <Icon name="close-outline" style={[globalStyles.buttonIconMedium, globalStyles.buttonIconMedium.dark]} />
                            </Pressable>
                            ) : null }
                        </View>

                        {/* Search content */}

                        {searchContentVisible ? (

                        <View style={navigationStyles.search.searchContent}>

                            {/* TODO make FILTER and SORT buttons functional */}
                            <View style={navigationStyles.search.searchContent.searchOptions}>
                                <Pressable style={[globalStyles.textButtonSmall, globalStyles.textPrimary, globalStyles.buttonFull, globalStyles.iconButtonSmall]}>
                                    <Icon name="filter-fill" style={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]} />
                                    <Text style={[globalStyles.textButtonSmall.text, globalStyles.textLight]}>Filter</Text>
                                </Pressable>
                                <Pressable style={[globalStyles.textButtonSmall, globalStyles.textPrimary, globalStyles.buttonFull, globalStyles.iconButtonSmall]}>
                                    <Icon name="sort-outline" style={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]} />
                                    <Text style={[globalStyles.textButtonSmall.text, globalStyles.textLight]}>Sort</Text>
                                </Pressable>
                            </View>


                            {/* TODO fix last 2 events not visible */}
                            {/* Potential not-ideal fix = screen height - (searchBarWrapper height + filterSort height) */}
                            {/* TODO maybe add an opacity 0 to opacity 100 gradient at the top */}

                            <SearchFilter />

                            {/* <FlatList
                            showsVerticalScrollIndicator={false}
                            contentContainerStyle={{paddingVertical: Spacing.lg, paddingBottom: 800}}
                            ItemSeparatorComponent={() => <View style={{height: Spacing.md}} />}
                            overScrollMode='never'
                            keyExtractor={(item) => item.id}
                            data={SAMPLEEVENTDATA}
                            renderItem={({ item }) => (
                                <EventCardMedium
                                    onPress={null}
                                    eventHeroImage={item.uri}
                                    eventDate={item.date}
                                    eventTime={item.time}
                                    eventAttendees={item.attendees}
                                    eventLocation={item.location}
                                    eventTitle={item.title}
                                    />
                                )}
                            /> */}

                        </View>

                        ) : null }

                    </View>

                    {!searchContentVisible && (
                    <View style={styles.buttonWrapper}>
                        {/* TODO replace onPress={null} with go to CREATE EVENT screen */}
                        <Button
                            btnText={'Create Event'}
                            btnIcon={'add-outline'}
                            btnIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                            btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.textPrimary, globalStyles.buttonContained]}
                            btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                            onPress={null}
                        />
                    </View>
                    )}

                </View>

            </ImageBackground>
        </View>

        // <View style={styles.mapContainer}>
        //     <ImageBackground source={tempMapImage} resizeMode='cover' style={styles.mapImage}>
        //         <View style={navigationStyles.searchBar.container}>
        //             <View style={searchContentVisible ? styles.searchBarInnerContainerVisible : null}>
        //                 <TextInput
        //                     onPressIn={() => [setsearchContentVisible(!searchContentVisible), setSearchCloseVisible(!searchCloseVisible)]}
        //                     placeholder='Search for events'
        //                     style={searchContentVisible ? styles.searchBarStylesVisible : null}
        //                 >
        //                 <Icon name="search-outline"/>
        //                 </TextInput>

        //                 {searchCloseVisible ? (
        //                 <Pressable style={searchContentVisible ? styles.searchCloseStylesVisible : null}>
        //                     <Icon name="close-outline" />
        //                 </Pressable>
        //                 ) : null }
        //             </View>

        //             {searchContentVisible ? (

        //             <View style={styles.searchContent}>
        //                 <Pressable style={[globalStyles.filterButton, globalStyles.filterButtonRest]}>
        //                     <Text style={globalStyles.filterButtonText}>Filter</Text>
        //                 </Pressable>
        //                 <Pressable style={[globalStyles.sortButton, globalStyles.sortButtonRest]}>
        //                     <Text style={globalStyles.sortButtonText}>Sort</Text>
        //                 </Pressable>
        //             </View>

        //             ) : null }
        //         </View>

        //         {/* TODO replace onPress={null} with go to CREATE EVENT screen */}
        //         <Button
        //             btnText={'Create Event'}
        //             btnIcon={'add-outline'}
        //             btnIconStyle={[globalStyles.buttonIconSmall]}
        //             btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.textPrimary, globalStyles.buttonContained]}
        //             btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
        //             onPress={null}
        //         />

        //     </ImageBackground>
        // </View>
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