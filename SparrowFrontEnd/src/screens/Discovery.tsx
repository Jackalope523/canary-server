import * as React from 'react';
import { View, Text, TouchableOpacity, StyleSheet, TextInput, Pressable } from 'react-native';

import SearchBar from '../components/molecules/SearchBar';
import { cardStyles } from '../styles/Cards';
import { Colors } from '../styles/Colors';
import { globalStyles } from '../styles/Global';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';

const Icon = createIconSetFromFontello(fontelloConfig);

// export default function DiscoveryScreen({ navigation }) {
//     return (
//         <View>
//             <Text onPress={() => navigation.navigate('Activity')}>Discovery Screen</Text>
//             <SearchBar />
//             <TouchableOpacity style={[globalStyles.textButtonExtraSmall, globalStyles.textButtonPrimary]}>
//                 <Text style={globalStyles.textButtonExtraSmallText}>Create Event</Text>
//             </TouchableOpacity>
//         </View>
//     );
// };

const DiscoveryScreen = () => {
    const [searchContainerVisible, setSearchContainerVisible] = React.useState(false);
    const [searchCloseVisible, setSearchCloseVisible] = React.useState(false);

    return (
        <View>
            <Text>Discovery</Text>


            <View style={styles.searchBarContainer}>
                <View style={searchContainerVisible ? styles.searchBarInnerContainerVisible : null}>
                    <TextInput onPressIn={() => [setSearchContainerVisible(!searchContainerVisible), setSearchCloseVisible(!searchCloseVisible)]} placeholder='Search' style={searchContainerVisible ? styles.searchBarStylesVisible : null}>
                    <Icon name="search-outline"/>
                    </TextInput>

                    {searchCloseVisible ? (
                    <Pressable style={searchContainerVisible ? styles.searchCloseStylesVisible : null}>
                        <Icon name="close-outline" />
                    </Pressable>
                    ) : null }
                </View>

                {searchContainerVisible ? (

                <View style={styles.searchContainer}>
                    <Pressable style={[globalStyles.filterButton, globalStyles.filterButtonRest]}>
                        <Text style={globalStyles.filterButtonText}>Filter</Text>
                    </Pressable>
                    <Pressable style={[globalStyles.sortButton, globalStyles.sortButtonRest]}>
                        <Text style={globalStyles.sortButtonText}>Sort</Text>
                    </Pressable>
                    <View style={cardStyles.eventCardMedium}>
                        <Text style={globalStyles.bodyTextOne}>This Tuesday</Text>
                        <Text style={globalStyles.bodyTextOne}>At 1630</Text>
                        <Text style={globalStyles.bodyTextOne}>12 people attending</Text>
                        <Text style={globalStyles.headingTextTwo}>Dog Walk and Play Meetup at Central Park</Text>
                        <Text style={globalStyles.bodyTextOne}>Central Park, Manhattan, New York City</Text>
                    </View>
                </View>

                ) : null }
            </View>


            <TouchableOpacity style={[globalStyles.textButtonExtraSmall, globalStyles.textButtonPrimary]}>
                <Text style={globalStyles.textButtonExtraSmallText}>Create Event</Text>
            </TouchableOpacity>
        </View>
    );
};

export default DiscoveryScreen

const styles = StyleSheet.create ({
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