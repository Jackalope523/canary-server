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
        <Text style={[globalStyles.displayTextTwo, styles.displayText]}>
          Hey, User!
        </Text>
      </View>
      {/* --- FILTER --- */}
      {/* TODO first filter button ("All") has to be set as selected/active on default */}

      <ExclusiveButtonView
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
      />

      {/* --- FILTER END --- */}
      <View style={styles.sectionContainer}>
        <Text style={[globalStyles.headingTextOne, styles.headingText]}>
          Upcoming
        </Text>
        <FlatList
          showsHorizontalScrollIndicator={false}
          contentContainerStyle={{ paddingHorizontal: Spacing.lg }}
          ItemSeparatorComponent={() => <View style={{ width: Spacing.md }} />}
          overScrollMode="never"
          horizontal={true}
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
        />
      </View>

      <View style={styles.sectionContainer}>
        <Text style={[globalStyles.headingTextOne, styles.headingText]}>
          Watched
        </Text>
        <FlatList
          showsHorizontalScrollIndicator={false}
          contentContainerStyle={{ paddingHorizontal: Spacing.lg }}
          ItemSeparatorComponent={() => <View style={{ width: Spacing.md }} />}
          overScrollMode="never"
          horizontal={true}
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
        />
      </View>

      <View style={styles.sectionContainerBottom}>
        <Text style={[globalStyles.headingTextOne, styles.headingText]}>
          Recommended
        </Text>
        <FlatList
          showsHorizontalScrollIndicator={false}
          contentContainerStyle={{ paddingHorizontal: Spacing.lg }}
          ItemSeparatorComponent={() => <View style={{ width: Spacing.md }} />}
          overScrollMode="never"
          horizontal={true}
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
        />
      </View>
    </ScrollView>
  );
};

export default ActivityScreen;

const styles = StyleSheet.create({
  displayText: {
    color: Colors.sparrowRed,
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

  sectionContainer: {
    marginBottom: Spacing.lg,
  },

  sectionContainerBottom: {
    marginBottom: Spacing.xl,
  },
});
