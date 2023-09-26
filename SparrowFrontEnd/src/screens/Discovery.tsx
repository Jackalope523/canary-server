import * as React from 'react';
import { View, Text, StyleSheet, TextInput, Pressable, ImageBackground } from 'react-native';

import SearchBar from '../components/molecules/SearchBar';
import { Colors } from '../styles/Colors';
import { globalStyles } from '../styles/Global';
import { navigationStyles } from '../styles/Navigation';

import Button from '../components/atoms/Button';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';

const Icon = createIconSetFromFontello(fontelloConfig);

// TEMP. map image - replace with actual working map
const tempMapImage = require('../assets/images/temp/temp-map.png');

const DiscoveryScreen = () => {
    const [searchContainerVisible, setSearchContainerVisible] = React.useState(false);
    const [searchCloseVisible, setSearchCloseVisible] = React.useState(false);
    const [searchText, setSearchText] = React.useState('');

    // Toggle search container
    const toggleSearch = () => {
        setSearchContainerVisible(!searchContainerVisible);
        setSearchCloseVisible(!searchCloseVisible);
        setSearchText('');
    };

    return (
        <View style={styles.mapContainer}>
            <ImageBackground source={tempMapImage} resizeMode='cover' style={styles.mapImage}>

                {/* Search */}
                <View style={navigationStyles.search}>

                    {/* Search bar */}
                    <View style={searchContainerVisible ? styles.searchBarInnerContainerVisible : null}>
                        <TextInput
                            onPressIn={toggleSearch}
                            placeholder='Search for events'
                            style={searchContainerVisible ? styles.searchBarStylesVisible : null}
                        >
                        <Icon name="search-outline"/>
                        </TextInput>

                        {searchCloseVisible ? (
                        <Pressable style={searchContainerVisible ? styles.searchCloseStylesVisible : null}>
                            <Icon name="close-outline" />
                        </Pressable>
                        ) : null }
                    </View>

                    {/* Search content */}

                    {searchContainerVisible ? (

                    <View style={styles.searchContainer}>
                        <Pressable style={[globalStyles.filterButton, globalStyles.filterButtonRest]}>
                            <Text style={globalStyles.filterButtonText}>Filter</Text>
                        </Pressable>
                        <Pressable style={[globalStyles.sortButton, globalStyles.sortButtonRest]}>
                            <Text style={globalStyles.sortButtonText}>Sort</Text>
                        </Pressable>
                    </View>

                    ) : null }
                </View>

                {/* TODO replace onPress={null} with go to CREATE EVENT screen */}
                <Button
                    btnText={'Create Event'}
                    btnIcon={'add-outline'}
                    btnIconStyle={globalStyles.buttonIconSmall}
                    btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.textButtonPrimary, globalStyles.buttonContained]}
                    btnTextStyle={[globalStyles.textButtonExtraSmallText, globalStyles.textLight]}
                    onPress={null}
                />

            </ImageBackground>
        </View>

        // <View style={styles.mapContainer}>
        //     <ImageBackground source={tempMapImage} resizeMode='cover' style={styles.mapImage}>
        //         <View style={navigationStyles.searchBar.container}>
        //             <View style={searchContainerVisible ? styles.searchBarInnerContainerVisible : null}>
        //                 <TextInput
        //                     onPressIn={() => [setSearchContainerVisible(!searchContainerVisible), setSearchCloseVisible(!searchCloseVisible)]}
        //                     placeholder='Search for events'
        //                     style={searchContainerVisible ? styles.searchBarStylesVisible : null}
        //                 >
        //                 <Icon name="search-outline"/>
        //                 </TextInput>

        //                 {searchCloseVisible ? (
        //                 <Pressable style={searchContainerVisible ? styles.searchCloseStylesVisible : null}>
        //                     <Icon name="close-outline" />
        //                 </Pressable>
        //                 ) : null }
        //             </View>

        //             {searchContainerVisible ? (

        //             <View style={styles.searchContainer}>
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
        //             btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.textButtonPrimary, globalStyles.buttonContained]}
        //             btnTextStyle={[globalStyles.textButtonExtraSmallText, globalStyles.textLight]}
        //             onPress={null}
        //         />

        //     </ImageBackground>
        // </View>
    );
};

export default DiscoveryScreen

const styles = StyleSheet.create ({
    mapContainer: {
        flex: 1,
    },

    mapImage: {
        flex: 1,
    },

    searchBarContainer: {
        backgroundColor: Colors.orange200,
    },

    searchContainer: {
        // temp. background color for testing purposes - replace with sparrow sand later
        backgroundColor: Colors.azure200,
    },

    tempTextInput: {
        backgroundColor: 'lightgrey',
        borderWidth: 2,
        marginHorizontal: 22,
        marginVertical: 16,
        paddingHorizontal: 16,
    },

    searchBarStylesVisible: {
        backgroundColor: 'red',
        flex: 3,

        height: 50,
    },

    searchCloseStylesVisible: {
        backgroundColor: 'blue',
        flex: 1,

        height: 50,
    },

    searchBarInnerContainerVisible: {
        flexDirection: 'row',
    },
})