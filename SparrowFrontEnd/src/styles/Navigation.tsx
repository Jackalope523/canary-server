import { StyleSheet } from "react-native";
import { Colors } from "./Colors";
import { Spacing } from "./Spacing";


export const navigationStyles = StyleSheet.create({
    // Navigation
    // Top navbar (header)
    topNavbar: {
        flexDirection: 'row',
        alignItems: 'center',
        paddingHorizontal: Spacing.lg,
        paddingVertical: 12,
        backgroundColor: Colors.sparrowSand,
        borderBottomWidth: 2,
        borderColor: Colors.sparrowDarkBrown,

        icons: {
            color: Colors.sparrowDarkBrown,
        },

        // Use as gap for icon on the left (usually the back arrow)
        gapLeft: {
            marginRight: Spacing.md,
        },
        
        // Types
        // Default with title
        defaultTitled: {
            columnGap: Spacing.md,
        },

        // Favorite
        favorite: {
            justifyContent: 'space-between',
        },

        // Options
        options: {
            justifyContent: 'space-between',
        },

        // Edit
        edit: {
            justifyContent: 'space-between',

            wrapper: {
                flexDirection: 'row',
                columnGap: Spacing.md,
            },
        },
    },

    // Search
    search: {        
        searchBarWrapper: {
            flexDirection: 'row',
            // columnGap: Spacing.sm,
        
            // alignItems: 'center',
            // verticalAlign: 'baseline',
            justifyContent: 'center',

            searchBar: {
                backgroundColor: Colors.sparrowSand,
                paddingHorizontal: 16,
                paddingVertical: 8,
                borderWidth: 2,
                borderRadius: 8,
                borderColor: Colors.sparrowDarkBrown,
                flex: 1,

                alignSelf: 'center',
                alignItems: 'center',
                columnGap: Spacing.md,
                flexDirection: 'row',

                textInput: {
                    margin: 0,
                    padding: 0,
                    flex: 1,

                    fontFamily: 'UncutSans-Regular',
                    fontSize: 16,
                    fontWeight: 'regular',
                },
            },
    
            closeButtonWrapper: {
                // TODO remove bgc later
                backgroundColor: Colors.red400,
                
                // OG padding in prototype is 16 but that doesn't work well here so I'm using 10
                // To make it more similar to OG padding, make paddingHorizontal: 16, paddingVertical: 10
                padding: 8,
            },
        },

        searchContent: {
            searchOptions: {
                flexDirection: 'row',
                columnGap: Spacing.md,
                paddingTop: Spacing.md,

                // TODO remove bgc later
                backgroundColor: Colors.fuchsia500,
            },
        },
    },

    // TEST
    test: {
        alignSelf: 'baseline',
        verticalAlign: 'middle',
        textAlign: 'center',
        justifyContent: 'center'
    },

    // OLD 2
    // mapContainer: {
    //     flex: 1,
    // },

    // mapImage: {
    //     flex: 1,
    // },

    // tempTextInput: {
    //     backgroundColor: 'lightgrey',
    //     borderWidth: 2,
    //     marginHorizontal: 22,
    //     marginVertical: 16,
    //     paddingHorizontal: 16,
    // },

    // // Search bar

    // searchBarContainer: {
    //     backgroundColor: Colors.orange200,
    // },

    // searchContainer: {
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

    // // OLD
    // searchBar: {
    //     backgroundColor: Colors.azure400,

    //     container: {
    //         backgroundColor: Colors.azure400,
    //         paddingHorizontal: 16,
    //     },
    // },
});