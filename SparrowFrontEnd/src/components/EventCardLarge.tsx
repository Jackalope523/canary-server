import { StyleSheet, Text, View } from 'react-native';
import React from 'react';
import Avatar, { AvatarSize } from './Avatar';

interface EventCardLargeProps {}

const EventCardLarge: React.FC<EventCardLargeProps> = () => {
  return (
    <View style={styles.eventCardLarge}>
      <View style={styles.host}>
        <Avatar size={AvatarSize.Small} />
        <View style={styles.hostNameContainer}>
          <Text>Name</Text>
          <Text>Friend icon</Text>
        </View>
      </View>
    </View>
  );
};

export default EventCardLarge;

const styles = StyleSheet.create({
  eventCardLarge: {},
  host: {},
  hostNameContainer: {},
});
