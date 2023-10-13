import React, { useState } from 'react';
import { View, Text, StyleSheet, FlatList, ImageBackground} from 'react-native';

// Testing components
import TopNavbarDefaultTitled from '../components/organisms/TopNavbarDefaultTitled';
import TopNavbarEdit from '../components/organisms/TopNavbarEdit';
import TopNavbarEditSelected from '../components/organisms/TopNavbarEditSelected';
import TopNavbarFavorite from '../components/organisms/TopNavbarFavorite';
import TopNavbarOptions from '../components/organisms/TopNavbarOptions';
import Notification from '../components/organisms/Notification';
import NoNotifications from '../components/organisms/NoNotifications';

import EventCardMedium from '../components/organisms/EventCardMedium';
import { Spacing } from '../styles/Spacing';
import { Colors } from '../styles/Colors';
import { globalStyles } from '../styles/Global';
import { cardStyles } from '../styles/Cards';
import Button from '../components/atoms/Button';

import RangeSelector from '../components/atoms/RangeSelector';
import SingleValueSelector from '../components/atoms/SingleValueSelector';
// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';
import RangeSelector2 from '../components/atoms/RangeSelector2';

const Icon = createIconSetFromFontello(fontelloConfig);

// Activity component testing
// Sample event image dataset
const img1 = {uri: 'https://images.unsplash.com/photo-1562519819-016930ada31b?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=987&q=80'}
const img2 = {uri: 'https://images.unsplash.com/photo-1551632811-561732d1e306?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1170&q=80'}
const img3 = {uri: 'https://images.unsplash.com/photo-1589182337358-2cb63099350c?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=987&q=80'}
const img4 = {uri: 'https://images.unsplash.com/photo-1538221566857-f20f826391c6?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1171&q=80'}
const img5 = {uri: 'https://images.unsplash.com/photo-1607822775841-940a09c90117?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1031&q=80'}

// Sample event dataset
const SAMPLEEVENTDATA = [
    {
        id: '1',
        date: 'This Tuesday',
        time: '15:00',
        attendees: 6,
        title: 'Two-on-two basketball at Venice Beach',
        location: 'Venice Beach, Venice',
        uri: img1,
    },

    {
        id: '2',
        date: 'This Thursday',
        time: '11:30',
        attendees: 4,
        title: 'Chill, Chilean hike',
        location: 'Somewhere in Chile',
        uri: img2,
    },

    {
        id: '3',
        date: 'This Friday',
        time: '18:00',
        attendees: 12,
        title: 'Yoga with alpacas',
        location: 'Botanical 124th street, Nowhere Boulevard, Lima',
        uri: img3,
    },

    {
        id: '4',
        date: 'Next Tuesday',
        time: '14:30',
        attendees: 2,
        title: 'Challenge: Beat me in chess and win a slice of pizza',
        location: 'University of Earth and Air, Noexistotown',
        uri: img4,
    },

    {
        id: '5',
        date: 'Next Saturtday',
        time: '12:30',
        attendees: 5,
        title: 'ROLLINGDOWN Downhill longboarding competition 2023',
        location: 'Tiny and chill hill with gnomes and majestic flying butterflies, Newstreet 203F, Sigulda',
        uri: img5,
    },
];

const FeedScreen = () => {
    // Activity component testing
    // If textWrapper text exceeds 2 lines, align items to flex-start
    const [isTextOverflowing, setIsTextOverflowing] = useState(false);

    const handleTextLayout = (event) => {
        const { lines } = event.nativeEvent;

        setIsTextOverflowing(lines.length > 2);
    };

    return (
        // <View>
        //     <Text>Testing sort and filter components</Text>
        //     {/* <Text>Range selector here</Text> */}
        //     <RangeSelector
        //         min={1}
        //         max={4}
        //         steps={1}
        //         onValueChange={(range) => console.log(range)}
        //         style={{ flex: 1 }}
        //     />

        //     {/* <Text>SingleValueSelector here</Text> */}
        //     {/* <SingleValueSelector /> */}
        // </View>
        <View style={styles.container}>
            <View style={styles.contentContainer}>
                <RangeSelector2 />
            </View>
        </View>
    );
};

export default FeedScreen

const styles = StyleSheet.create ({
    // TEMP. styles
    container: {
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center',
    },

    contentContainer: {
        width: '90%',
        height: 300,
    },

    // other styles
    headingText: {
        color: Colors.sparrowDark,
        marginBottom: Spacing.md,
        marginLeft: Spacing.lg,
    },

    mainWrapper: {
        paddingBottom: Spacing.lg,
    },

    topWrapper: {
        marginHorizontal: Spacing.lg,
        marginTop: Spacing.lg,
    },

    notificationWrapper: {
        alignItems: 'flex-end',
    },

    // TODO DELETE THIS
    eventCardContainer: {
        marginHorizontal: Spacing.lg,
        flexDirection: 'row',
        columnGap: Spacing.md,
    },
});