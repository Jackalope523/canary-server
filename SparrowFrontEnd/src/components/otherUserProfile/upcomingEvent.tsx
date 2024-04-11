import { ImageSourcePropType, StyleSheet, Text, View } from 'react-native';
import React, { FC } from 'react';
import EventCardSmall, { EventStatus } from '../EventCardSmall';
import Gallery from '../testing/OldGallery';
import { Spacing } from '../../styles/SpacingStyles';

import { SAMPLE_PAST_EVENT_DATA } from '../../data/samplePastEventData';
import { SAMPLEEVENTDATA } from '../../data/sampleUpcomingEventData';

type UpcomingEventProps = {
  eventStatus: EventStatus;
  eventHeroImage: ImageSourcePropType;
  eventTitle: string;
  eventDate: string;
  eventTime: string;
  onPress: () => void;
};

const UpcomingEvent: FC<UpcomingEventProps> = ({
  eventStatus,
  eventHeroImage,
  eventTitle,
  eventDate,
  eventTime,
  onPress,
}) => {
  return (
    <View style={styles.container}>
      <EventCardSmall
        eventStatus={eventStatus}
        eventHeroImage={eventHeroImage}
        eventTitle={eventTitle}
        eventDate={eventDate}
        eventTime={eventTime}
        onPress={onPress}
      />
    </View>
  );
};

export default UpcomingEvent;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    rowGap: Spacing.md,
  },
});
