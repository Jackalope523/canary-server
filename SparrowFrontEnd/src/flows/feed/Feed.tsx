import React from 'react';
import { View, StyleSheet, FlatList } from 'react-native';
import Post from '../../components/Post';
import { Spacing } from '../../styles/SpacingStyles';
import { SAMPLE_PAST_EVENT_DATA } from '../../data/samplePastEventData';
import { FlagType } from '../../components/FlagMedium';

const FeedScreen = () => {
  return (
    <View style={styles.container}>
      <FlatList
        showsVerticalScrollIndicator={false}
        ItemSeparatorComponent={() => <View style={{ height: Spacing.lg }} />}
        contentContainerStyle={styles.contentContainer}
        overScrollMode="never"
        keyExtractor={(item) => item.id}
        data={SAMPLE_PAST_EVENT_DATA}
        renderItem={({ item }) => (
          <Post
            key={item.id}
            name={item.host}
            avatar={item.avatar}
            time={item.status === 'live' ? FlagType.Live : item.time}
            title={item.title}
            media={item.media.map((uri) => ({ uri }))}
            attendees={item.attendees}
            leftoverAttendeeCount={item.leftoverAttendeeCount}
            location={item.location}
            likeCount={item.likeCount}
          />
        )}
      />
    </View>
  );
};

export default FeedScreen;

const styles = StyleSheet.create({
  container: {
    paddingHorizontal: Spacing.lg,
  },

  contentContainer: {
    paddingVertical: Spacing.lg,
  },
});
