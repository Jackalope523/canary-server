import React from 'react';
import { View, Text, StyleSheet, ScrollView, FlatList } from 'react-native';
import { globalStyles } from '../../styles/GlobalStyles';
import { Colors } from '../../styles/ColorStyles';
import { Spacing } from '../../styles/SpacingStyles';

import EventCardMedium from '../../components/EventCardMedium';
import NotificationIndicator from '../../components/activity/NotificationIndicator';
import { ButtonDisplay, ButtonSize, ButtonType } from '../../components/Button';
import ExclusiveButtonView from '../../components/ExclusiveButtonView';

// Sample data
import { SAMPLEEVENTDATA } from '../../data/sampleUpcomingEventData';
import EventCardLarge from '../../components/EventCardLarge';
import DropdownSelectorIcon from '../../components/DropdownSelectorIcon';

const ActivityScreen = () => {
  return (
    <ScrollView
      style={styles.mainContainer}
      overScrollMode="never"
      showsVerticalScrollIndicator={false}>
      <View style={styles.topContainer}>
        <View style={styles.notificationContainer}>
          <NotificationIndicator />
        </View>
      </View>

      <View style={styles.events}>
        {/* TODO first filter button ("All") has to be set as selected/active on default */}
        {/* TODO we keeping the filter  or deleting it?; in app design it's deleted */}

        {/* <ExclusiveButtonView
          groupStyle={styles.filter}
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
              onPress: null,
            },
            {
              id: 3,
              type: ButtonType.SecondaryDark,
              size: ButtonSize.Small,
              display: ButtonDisplay.Full,
              text: 'By friends',
              onPress: null,
            },
          ]}
        /> */}

        <FlatList
          showsHorizontalScrollIndicator={false}
          contentContainerStyle={{ paddingHorizontal: Spacing.lg }}
          ItemSeparatorComponent={() => <View style={{ width: Spacing.md }} />}
          overScrollMode="never"
          horizontal={true}
          keyExtractor={(item) => item.id}
          data={SAMPLEEVENTDATA}
          renderItem={({ item }) => (
            <EventCardLarge
              onPress={null}
              eventHeroImage={item.uri}
              eventHostName={item.host}
              eventTitle={item.title}
              eventDate={item.date}
              eventTime={item.time}
              eventLocation={item.location}
              eventAttendees={item.attendees}
              eventAttendeesFriends={item.attendeesFriends}
            />
          )}
        />
      </View>
    </ScrollView>
  );
};

export default ActivityScreen;

const styles = StyleSheet.create({
  displayText: {
    color: Colors.picton500,
    marginVertical: Spacing.lg,
  },

  headingText: {
    color: Colors.sparrowDark,
    marginBottom: Spacing.md,
    marginLeft: Spacing.lg,
  },

  mainContainer: {
    paddingBottom: Spacing.lg,
  },

  filter: {
    flexDirection: 'row',
    columnGap: Spacing.md,
    marginLeft: Spacing.lg,
    marginBottom: Spacing.md,
    marginTop: Spacing.lg,
  },

  topContainer: {
    marginHorizontal: Spacing.lg,
    marginTop: Spacing.lg,
  },

  notificationContainer: {
    alignItems: 'flex-end',
  },

  events: {
    marginBottom: Spacing.lg,
  },
});
