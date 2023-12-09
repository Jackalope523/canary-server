import React, { useState } from 'react';
import { View, Text, StyleSheet, ScrollView, FlatList } from 'react-native';
import { globalStyles } from '../../styles/Global';
import { Colors } from '../../styles/Colors';
import { Spacing } from '../../styles/Spacing';

import EventCardMedium from '../../components/EventCardMedium';
import NotificationIndicator from '../../components/molecules/NotificationIndicator';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';

const Icon = createIconSetFromFontello(fontelloConfig);

// Sample data
import { SAMPLEEVENTDATA } from '../../data/sampleEventData';

const ActivityScreen = () => {
  // If textWrapper text exceeds 2 lines, align items to flex-start
  // Doesn't work with some text, not the ideal solution - fix later
  // Not ideal but I can just assign a % of space for the icon and the rest for the text

  // TODO Ideally this would not be needed if the location text would only take up 1 line. If it takes up more it starts looking worse.
  // Maybe make it end with ... if it exceeds 1 line
  const [isTextOverflowing, setIsTextOverflowing] = useState(false);

  const handleTextLayout = (event) => {
    const { lines } = event.nativeEvent;

    setIsTextOverflowing(lines.length > 2);
  };

  return (
    <ScrollView
      style={styles.mainWrapper}
      overScrollMode="never"
      showsVerticalScrollIndicator={false}>
      <View style={styles.topWrapper}>
        <View style={styles.notificationWrapper}>
          <NotificationIndicator />
        </View>
        <Text
          style={[
            globalStyles.displayTextTwo,
            {
              color: Colors.sparrowRed,
              marginTop: Spacing.lg,
              marginBottom: Spacing.md,
            },
          ]}>
          Hey, User!
        </Text>
      </View>
      <View style={{ marginBottom: Spacing.md }}>
        <Text style={[globalStyles.headingTextOne, styles.headingText]}>
          Upcoming events
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
      <View style={{ marginBottom: Spacing.lg }}>
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
