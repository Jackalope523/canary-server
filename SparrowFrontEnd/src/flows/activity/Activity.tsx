// #region Imports
import React, { useEffect, useState } from 'react';
import { View, Text, StyleSheet, ScrollView, FlatList, ViewToken, Pressable, ButtonProps } from 'react-native';
import { PreventRemoveContext, useFocusEffect } from '@react-navigation/native';
import { globalStyles } from '../../styles/GlobalStyles';
import { Colors } from '../../styles/ColorStyles';
import { Spacing } from '../../styles/SpacingStyles';
import EventCardMedium from '../../components/EventCardMedium';
import NotificationIndicator from '../../components/activity/NotificationIndicator';
import Button, { ButtonDisplay, ButtonSize, ButtonType } from '../../components/Button';
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

import Animated, {useSharedValue, ReduceMotion, withSpring, SlideInUp, SlideOutDown, SlideInDown, Easing, SequencedTransition } from 'react-native-reanimated';
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

  //||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
  //||                                  Logic                                         ||
  //||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
  //#region Logic                                                                     ||

  const AllId = 1;
  const ByYouId = 2;
  const ByFriendsId = 3;
  const AttendingId = 4;
  const WatchingId = 5;


  const [allSelected, parentSetAllSelected] = useState<boolean>(false);
  const [byYouSelected, parentSetByYouSelected] = useState<boolean>(false);
  const [byFriendsSelected, parentSetByFriendsSelected] = useState<boolean>(false);
  const [attendingSelected, parentSetAttendingSelected] = useState<boolean>(false);
  const [watchingSelected, parentSetWatchingSelected] = useState<boolean>(false);

  const pressedButtons = [];

  const [sampledata, setSampleData] = useState(SAMPLEEVENTDATA);

  const [hiddenCards, setHiddenCards] = useState<number[]>([]);

  const childSetAllSelected = (x: boolean) => {
    parentSetAllSelected(true);
    parentSetByYouSelected(false);
    parentSetByFriendsSelected(false);
    parentSetAttendingSelected(false);
    parentSetWatchingSelected(false);
  }

  const childSetByYouSelected = (x: boolean) => {
    parentSetAllSelected(false);
    parentSetByYouSelected(true);
  }

  const childSetByFriendsSelected = (x: boolean) => {
    parentSetAllSelected(false);
    parentSetByFriendsSelected(true);
 
  }

  const childSetAttendingSelected = (x: boolean) => {
    parentSetAllSelected(false);
    parentSetAttendingSelected(true);
  }

  const childSetWatchingSelected = (x: boolean) => {
    parentSetAllSelected(false);
    parentSetWatchingSelected(true);
  }


  const All = () => {
    setHiddenCards([]);
  }

  const ByYou = () => {
    let ids: number[] = [...hiddenCards];
    for (let i = 0; i < SAMPLEEVENTDATA.length; i++) {
      if (true) {
        ids.push(SAMPLEEVENTDATA[i].id);
      }
    }
    setHiddenCards(ids);
  }

  const ByFriends = () => {
    let ids: number[] = [...hiddenCards];
    for (let i = 0; i < SAMPLEEVENTDATA.length; i++) {
      if (SAMPLEEVENTDATA[i].host === "David") {
        ids.push(SAMPLEEVENTDATA[i].id);
      }
    }
    setHiddenCards(ids);
  }

  const Attending = () => {
    let ids: number[] = [...hiddenCards];
    for (let i = 0; i < SAMPLEEVENTDATA.length; i++) {
      if (SAMPLEEVENTDATA[i].host === "Alice") {
        ids.push(SAMPLEEVENTDATA[i].id);
      }
    }
    setHiddenCards(ids);
  }

  const Watching = () => {
    let ids: number[] = [...hiddenCards];
    for (let i = 0; i < SAMPLEEVENTDATA.length; i++) {
      if (SAMPLEEVENTDATA[i].host === "Robert") {
        ids.push(SAMPLEEVENTDATA[i].id);
      }
    }
    setHiddenCards(ids);
  }

  const buttons: ButtonProps[] = [
    {
      id: AllId,
      type: ButtonType.SecondaryDark,
      size: ButtonSize.Small,
      display: ButtonDisplay.Full,
      text: 'All',
      onPress: All,
      pressed: allSelected,
      setPressed: childSetAllSelected
    },
    {
      id: ByYouId,
      type: ButtonType.SecondaryDark,
      size: ButtonSize.Small,
      display: ButtonDisplay.Full,
      text: 'By you',
      onPress: ByYou,
      pressed: byYouSelected,
      setPressed: childSetByYouSelected
    },
    {
      id: ByFriendsId,
      type: ButtonType.SecondaryDark,
      size: ButtonSize.Small,
      display: ButtonDisplay.Full,
      text: 'By friends',
      onPress: ByFriends,
      pressed: byFriendsSelected,
      setPressed: childSetByFriendsSelected
    },
    {
      id: AttendingId,
      type: ButtonType.SecondaryDark,
      size: ButtonSize.Small,
      display: ButtonDisplay.Full,
      text: 'Attending',
      onPress: Attending,
      pressed: attendingSelected,
      setPressed: childSetAttendingSelected
    },
    {
      id: WatchingId,
      type: ButtonType.SecondaryDark,
      size: ButtonSize.Small,
      display: ButtonDisplay.Full,
      text: 'Watching',
      onPress: Watching,
      pressed: watchingSelected,
      setPressed: childSetWatchingSelected
    },
  ];

  //#endregion                                                                        ||
  //||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||

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
      
      <Animated.FlatList
        showsHorizontalScrollIndicator={false}
        contentContainerStyle={{
          paddingHorizontal: Spacing.lg
        }}
        ItemSeparatorComponent={() => <View style={{ width: Spacing.md }} />}
        overScrollMode="never"
        horizontal={true}
        keyExtractor={(item) => item.id}
        data={buttons}
        renderItem={({ item }) => (
          <Button
            id={item.id}
            type={item.type}
            size={item.size}
            display={item.display}
            text={item.text}
            onPress={item.onPress}
            pressed={item.pressed}
            setPressed={item.setPressed}
          />
        )}
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
          data={SAMPLEEVENTDATA}
          renderItem={({ item }) => (
            <EventCardLarge
              id={item.id}
              eventHeroImage={item.uri}
              eventHostName={item.host}
              eventTitle={item.title}
              eventDate={item.date}
              eventTime={item.time}
              eventLocation={item.location}
              eventAttendees={item.attendees}
              eventAttendeesFriends={item.attendeesFriends}
              hiddenCards={hiddenCards}
            /> 
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
    marginTop: 0
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
