import React from 'react';
import { View, StyleSheet, FlatList } from 'react-native';
import PhotoPost from '../../components/PhotoPost';
import { Spacing } from '../../styles/SpacingStyles';
import { SAMPLE_PAST_EVENT_DATA } from '../../data/samplePastEventData';
import { FlagType } from '../../components/FlagMedium';

/*

TODO [!!!] IMPORTANT

Before beginning work on the Feed screen:
1. Add a cardStyle to the Feed Screen in MainContainer.tsx to fix the background color

*/

const FeedScreen = () => {
  return (
    <View style={styles.container}>
      <FlatList
        showsVerticalScrollIndicator={false}
        ItemSeparatorComponent={() => <View style={{ height: Spacing.lg }} />}
        overScrollMode="never"
        keyExtractor={(item) => item.id}
        data={SAMPLE_PAST_EVENT_DATA}
        renderItem={({ item }) => (
          <PhotoPost
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
    // </ScrollView>
  );
};

export default FeedScreen;

const styles = StyleSheet.create({
  // TODO first item in the list should have a top margin and the last item should have a bottom margin
  container: {
    marginHorizontal: Spacing.lg,
  },
});
