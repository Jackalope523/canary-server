// #region Imports
import React, { useState } from 'react';
import { View, Text, StyleSheet, ScrollView, FlatList, ViewToken, Pressable } from 'react-native';
import { PreventRemoveContext, useFocusEffect } from '@react-navigation/native';
import { globalStyles } from '../../styles/GlobalStyles';
import { Colors } from '../../styles/ColorStyles';
import { Spacing } from '../../styles/SpacingStyles';
import EventCardMedium from '../../components/EventCardMedium';
import NotificationIndicator from '../../components/activity/NotificationIndicator';
import { ButtonDisplay, ButtonSize, ButtonType } from '../../components/Button';
import ExclusiveButtonView from '../../components/ExclusiveButtonView';
import { SAMPLEEVENTDATA } from '../../data/sampleUpcomingEventData';
import EventCardLarge from '../../components/EventCardLarge';
import DropdownSelectorText from '../../components/DropdownSelectorText';
import dropdownOptionsActivity from './DropdownOptionsActivity';
import { seedDatabase } from '../testing/testPigeon';
import { accountStatus, character, userShard } from '../auth/accountPigeon';
import { eventShard } from '../event/eventPigeon';
import { 
  user1, 
  user2, 
  user3, 
  user4, 
  user5, 
  user6, 
  user7, 
  event1, 
  event2, 
  event3, 
  attendance,
  follows, 
  blocks } from '../testing/gardenShed';

import Animated, {useSharedValue, ReduceMotion, withSpring, SlideInUp, SlideOutDown } from 'react-native-reanimated';
import ExclusiveButtonScroll from '../../components/ExclusiveButtonScroll';
// #endregion

const ActivityScreen = () => {
  // seedDatabase(
  //   [user1, user2, user3, user4, user5, user6, user7], 
  //   [event1, event2, event3], 
  //   attendance, 
  //   follows, 
  //   blocks)
  //   .then(() => console.log("DATABASE SEEDED"));

  const [activeButton, setActiveButton] = useState(-1);

  const [sampledata, setSampleData] = useState(SAMPLEEVENTDATA);

  const ByYou = () => {
    for (let i = 0; i < sampledata.length; i++) {
      if (sampledata[i].host === "Lily") {
        setSampleData(sampledata.splice(i));
      }
    }
  }


  return (
    <View>
      <View style={styles.topContainer}>
        {/* TODO hook up onPress in dropdownOptionsActivity to here */}

        {/* <DropdownSelectorText options={dropdownOptionsActivity} /> */}

        <Text style={[globalStyles.headingTextFour, globalStyles.textDark]}>Activity</Text>
        <View style={styles.notificationContainer}>
          <NotificationIndicator />
        </View>
      </View>


      <ExclusiveButtonScroll
          groupStyle={styles.filter}
          activeButton={activeButton}
          setActiveButton={setActiveButton}
          buttons={[
            {
              id: 1,
              type: ButtonType.SecondaryDark,
              size: ButtonSize.Small,
              display: ButtonDisplay.Full,
              text: 'All',
              onPress: null,
            },
            {
              id: 2,
              type: ButtonType.SecondaryDark,
              size: ButtonSize.Small,
              display: ButtonDisplay.Full,
              text: 'By you',
              onPress: ByYou,
            },
            {
              id: 3,
              type: ButtonType.SecondaryDark,
              size: ButtonSize.Small,
              display: ButtonDisplay.Full,
              text: 'By friends',
              onPress: null,
            },
            {
              id: 4,
              type: ButtonType.SecondaryDark,
              size: ButtonSize.Small,
              display: ButtonDisplay.Full,
              text: 'Upcoming',
              onPress: null,
            },
            {
              id: 5,
              type: ButtonType.SecondaryDark,
              size: ButtonSize.Small,
              display: ButtonDisplay.Full,
              text: 'Watching',
              onPress: null,
            },
          ]}
      />

      <View style={{ marginBottom: Spacing.lg, paddingTop: 8 }}>
        {/* TODO first filter button ("All") has to be set as selected/active on default */}
        {/* TODO we keeping the filter  or deleting it?; in app design it's deleted, so delete if we don't need it */}
        <Animated.FlatList
          showsHorizontalScrollIndicator={false}
          contentContainerStyle={{
            paddingHorizontal: Spacing.lg
          }}
          ItemSeparatorComponent={() => <View style={{ width: Spacing.md }} />}
          overScrollMode="never"
          horizontal={true}
          keyExtractor={(item) => item.id}
          data={sampledata}
          renderItem={({ item }) => (
            <Animated.View entering={SlideInUp} exiting={SlideOutDown}>
              <EventCardLarge
            eventHeroImage={item.uri}
            eventHostName={item.host}
            eventTitle={item.title}
            eventDate={item.date}
            eventTime={item.time}
            eventLocation={item.location}
            eventAttendees={item.attendees}
            eventAttendeesFriends={item.attendeesFriends}
          />
            </Animated.View>
            
            
          )}
        />
      </View>

    </View>
  );
};

export default ActivityScreen;

const styles = StyleSheet.create({
  filter: {
    flexDirection: 'row',
    columnGap: Spacing.md,
    marginLeft: Spacing.lg,
    marginBottom: Spacing.md,
    marginTop: 0,
  },

  topContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingHorizontal: Spacing.lg,
    paddingTop: Spacing.lg,
    paddingBottom: Spacing.md,
  },

  notificationContainer: {
    alignItems: 'flex-end',
  },

  events: {
    marginBottom: Spacing.lg,
    paddingTop: 0,
  }
});
