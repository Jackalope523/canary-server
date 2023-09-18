import React, { useState } from 'react';
import { View, Text, StyleSheet, ScrollView, SectionList, FlatList, ImageBackground } from 'react-native';
import { globalStyles } from '../styles/Global';
import { Colors } from '../styles/Colors';
import { Spacing } from '../styles/Spacing';

import EventCardMedium from '../components/organisms/EventCardMedium';
import NotificationIndicator from '../components/molecules/NotificationIndicator';

import { cardStyles } from '../styles/Cards';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';

const Icon = createIconSetFromFontello(fontelloConfig);

// TODO might need to use SectionList or FlatList instead of ScrollView

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
        location: 'Tiny and chill hill, Newstreet 203F, Sigulda',
        uri: img5,
    },
];

const ActivityScreen = () => {
    // TODO FIX THIS, IT DOESN'T WORK - if textWrapper text exceeds 2 lines, align items to flex-start
    const [isTextOverflowing, setIsTextOverflowing] = useState({});

    const handleTextLayout = (event) => {
        const { lines } = event.nativeEvent;

        setIsTextOverflowing(lines.length > 2);
    };
    
    return (
        <ScrollView style={styles.mainWrapper} overScrollMode="never">
            <View style={styles.topWrapper}>
                <View style={styles.notificationWrapper}>
                    <NotificationIndicator />
                </View>
                <Text style={[globalStyles.displayTextTwo, {color: Colors.sparrowRed, marginTop: Spacing.lg, marginBottom: Spacing.md}]}>Hey, User!</Text>
            </View>
            <View style={{marginBottom: Spacing.md}}>
                <Text style={[globalStyles.headingTextOne, styles.headingText]}>Upcoming events</Text>
                <FlatList
                    contentContainerStyle={{paddingHorizontal: Spacing.lg}}
                    ItemSeparatorComponent={() => <View style={{width: Spacing.md}} />}
                    overScrollMode='never'
                    horizontal={true}
                    keyExtractor={(item) => item.id}
                    data={SAMPLEEVENTDATA}
                    renderItem={({ item }) => (
                        <View style={cardStyles.eventCardMedium}>
                            <ImageBackground source={item.uri} resizeMode="cover" imageStyle={cardStyles.eventCardMedium.bgImage2}>
                                <View style={cardStyles.eventCardMedium.content}>
                                    <View style={cardStyles.eventCardMedium.content.topWrapper}>
                                        <View style={cardStyles.eventCardMedium.content.container}>
                                            <View style={cardStyles.eventCardMedium.content.container.textWrapper}>
                                                <Icon name="date-outline" size={24} height={24} width={24} style={cardStyles.eventCardMedium.content.container.textWrapper.icon}/>
                                                <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>{item.date}</Text>
                                            </View>
                                            <View style={cardStyles.eventCardMedium.content.container.textWrapper}>
                                                <Icon name="time-outline" size={24} height={24} width={24} style={cardStyles.eventCardMedium.content.container.textWrapper.icon}/>
                                                <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>{item.time}</Text>
                                            </View>
                                        </View>
                                        <View style={cardStyles.eventCardMedium.content.container}>
                                            <View style={cardStyles.eventCardMedium.content.container.textWrapper}>
                                                <Icon name="account-outline" size={24} height={24} width={24} style={cardStyles.eventCardMedium.content.container.textWrapper.icon}/>
                                                <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>{item.attendees}</Text>
                                            </View>
                                        </View>
                                    </View>
                                    <View style={cardStyles.eventCardMedium.content.container}>
                                        <Text style={[globalStyles.headingTextThree, globalStyles.textDark, cardStyles.eventCardMedium.content.container.title]}>{item.title}</Text>
                                        <View style={[cardStyles.eventCardMedium.content.container.textWrapper, isTextOverflowing ? cardStyles.eventCardMedium.content.container.textWrapperOverflow : cardStyles.eventCardMedium.content.container.textWrapperCenter]}>
                                            <Icon name="location-outline" size={24} height={24} width={24} style={cardStyles.eventCardMedium.content.container.textWrapper.icon} />
                                            <Text onTextLayout={handleTextLayout} style={[globalStyles.bodyTextOne, globalStyles.textDark, cardStyles.eventCardMedium.content.container.textWrapper.innerText]}>{item.location}</Text>
                                        </View>
                                    </View>
                                </View>
                            </ImageBackground>
                        </View>
                    )}
                />
            </View>
            <View style={{marginBottom: Spacing.lg}}>
                <Text style={[globalStyles.headingTextOne, styles.headingText]}>Recommended</Text>
                <FlatList
                    contentContainerStyle={{paddingHorizontal: Spacing.lg}}
                    ItemSeparatorComponent={() => <View style={{width: Spacing.md}} />}
                    overScrollMode='never'
                    horizontal={true}
                    keyExtractor={(item) => item.id}
                    data={SAMPLEEVENTDATA}
                    renderItem={({ item }) => (
                        <View style={cardStyles.eventCardMedium}>
                            <ImageBackground source={item.uri} resizeMode="cover" imageStyle={cardStyles.eventCardMedium.bgImage2}>
                                <View style={cardStyles.eventCardMedium.content}>
                                    <View style={cardStyles.eventCardMedium.content.topWrapper}>
                                        <View style={cardStyles.eventCardMedium.content.container}>
                                            <View style={cardStyles.eventCardMedium.content.container.textWrapper}>
                                                <Icon name="date-outline" size={24} height={24} width={24} style={cardStyles.eventCardMedium.content.container.textWrapper.icon}/>
                                                <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>{item.date}</Text>
                                            </View>
                                            <View style={cardStyles.eventCardMedium.content.container.textWrapper}>
                                                <Icon name="time-outline" size={24} height={24} width={24} style={cardStyles.eventCardMedium.content.container.textWrapper.icon}/>
                                                <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>{item.time}</Text>
                                            </View>
                                        </View>
                                        <View style={cardStyles.eventCardMedium.content.container}>
                                            <View style={cardStyles.eventCardMedium.content.container.textWrapper}>
                                                <Icon name="account-outline" size={24} height={24} width={24} style={cardStyles.eventCardMedium.content.container.textWrapper.icon}/>
                                                <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>{item.attendees}</Text>
                                            </View>
                                        </View>
                                    </View>
                                    <View style={cardStyles.eventCardMedium.content.container}>
                                        <Text style={[globalStyles.headingTextThree, globalStyles.textDark, cardStyles.eventCardMedium.content.container.title]}>{item.title}</Text>
                                        <View style={[cardStyles.eventCardMedium.content.container.textWrapper, isTextOverflowing ? cardStyles.eventCardMedium.content.container.textWrapperOverflow : cardStyles.eventCardMedium.content.container.textWrapperCenter]}>
                                            <Icon name="location-outline" size={24} height={24} width={24} style={cardStyles.eventCardMedium.content.container.textWrapper.icon} />
                                            <Text onTextLayout={handleTextLayout} style={[globalStyles.bodyTextOne, globalStyles.textDark, cardStyles.eventCardMedium.content.container.textWrapper.innerText]}>{item.location}</Text>
                                        </View>
                                    </View>
                                </View>
                            </ImageBackground>
                        </View>
                    )}
                />
            </View>
        </ScrollView>
    );
};

export default ActivityScreen

const styles = StyleSheet.create ({
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