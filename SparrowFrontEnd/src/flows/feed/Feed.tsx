import React, { useEffect } from 'react';
import { View, StyleSheet, FlatList, SectionList } from 'react-native';
import Post, { PhotoPostProps } from '../../components/feed/Post';
import { Spacing } from '../../styles/SpacingStyles';
import { SAMPLE_PAST_EVENT_DATA } from '../../data/samplePastEventData';
import { FlagType } from '../../components/FlagMedium';

import { getUserFeed } from './feedPigeon';
import {getAccount, modifyAccount } from '../auth/accountPigeon';
import { etchingShard } from '../event/eventPigeon';
import { length } from '@turf/turf';


const FeedScreen = () => {

  let posts : JSX.Element[] = [];

  useEffect(() => {
    getUserFeed({ DepthCharge: 30, LastDepth: 0 })
    .then(value => 
      {
        let headers = value.Headers;
        let etchings = value.Etchings;
         for (let i = 0; i < headers.length; i++) {
          let photos = [];
          let authors = [];
          for (let j = 0; j < etchings.length; j++) {
            if (etchings[j].EventId === headers[i].Id) {
              photos.push(etchings[j].ImageURL);
              authors.push(etchings[j].User.Name);
            }
          }
          posts.push(
            <Post
              name = {headers[i].Name}
              media = {photos}
              author = {authors}
              location = {"Valentine"}
            />);   

            console.log("LENGTH: " + posts);
         }
      })
    .catch(() => console.log("ERROR"));
  }, []);


  return (
    <View style={styles.container}>
      <FlatList
        showsVerticalScrollIndicator={false}
        ItemSeparatorComponent={() => <View style={{ height: Spacing.lg }} />}
        contentContainerStyle={styles.contentContainer}
        overScrollMode="never"
        data={posts}
        renderItem={({ item }) => (item)}
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
