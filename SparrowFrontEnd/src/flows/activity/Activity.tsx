// #region Imports
import React, { Dispatch, SetStateAction, useEffect, useState } from 'react';
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

  const [data, setData] = useState<eventShard[]>([]);

  useEffect(() => {

  }, []);

  const AllId = 0;
  const ByYouId = 1;
  const ByFriendsId = 2;
  const AttendingId = 3;
  const WatchingId = 4;

  const exclusiveButtons = [];
  exclusiveButtons.push(AllId)

  const pressedButtons: [boolean, Dispatch<SetStateAction<boolean>>][] = [];
  pressedButtons.push(useState<boolean>(false));
  pressedButtons.push(useState<boolean>(false));
  pressedButtons.push(useState<boolean>(false));
  pressedButtons.push(useState<boolean>(false));
  pressedButtons.push(useState<boolean>(false));

  const hiddenCards: [boolean, Dispatch<SetStateAction<boolean>>][] = [];


  const setSelected = (id: number, value: boolean) => {
    for (let i = 0; i < exclusiveButtons.length; i++) {
      pressedButtons[i][1](false);
    }
    pressedButtons[id][1](true);
  }

  const setExclusiveSelected = (id: number, value: boolean) => {
    for (let i = 0; i < pressedButtons.length; i++) {
      if (i === id) {
        pressedButtons[i][1](true);
      }
      else {
        pressedButtons[i][1](false);
      } 
    }
  }

  const All = () => {
    
  }

  const ByYou = () => {
    
  }

  const ByFriends = () => {
    
  }

  const Attending = () => {
    
  }

  const Watching = () => {
   
  }

  const buttons: ButtonProps[] = [
    {
      id: AllId,
      type: ButtonType.SecondaryDark,
      size: ButtonSize.Small,
      display: ButtonDisplay.Full,
      text: 'All',
      onPress: All,
      pressed: pressedButtons[AllId][0],
      setPressed: setExclusiveSelected
    },
    {
      id: ByYouId,
      type: ButtonType.SecondaryDark,
      size: ButtonSize.Small,
      display: ButtonDisplay.Full,
      text: 'By you',
      onPress: ByYou,
      pressed: pressedButtons[ByYouId][0],
      setPressed: setSelected
    },
    {
      id: ByFriendsId,
      type: ButtonType.SecondaryDark,
      size: ButtonSize.Small,
      display: ButtonDisplay.Full,
      text: 'By friends',
      onPress: ByFriends,
      pressed: pressedButtons[ByFriendsId][0],
      setPressed: setSelected
    },
    {
      id: AttendingId,
      type: ButtonType.SecondaryDark,
      size: ButtonSize.Small,
      display: ButtonDisplay.Full,
      text: 'Attending',
      onPress: Attending,
      pressed: pressedButtons[AttendingId][0],
      setPressed: setSelected
    },
    {
      id: WatchingId,
      type: ButtonType.SecondaryDark,
      size: ButtonSize.Small,
      display: ButtonDisplay.Full,
      text: 'Watching',
      onPress: Watching,
      pressed: pressedButtons[WatchingId][0],
      setPressed: setSelected
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
              hidden={false}
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
