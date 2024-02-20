import { ImageSourcePropType, StyleSheet, Text, View } from 'react-native';
import React, { FC } from 'react';
import EventCardSmall, { EventStatus } from '../EventCardSmall';
import Gallery from '../Gallery';
import { Spacing } from '../../styles/SpacingStyles';
import { SAMPLE_PAST_EVENT_DATA } from '../../data/samplePastEventData';

type PreviouslyAttendedEventProps = {
  eventStatus: EventStatus;
  eventHeroImage: ImageSourcePropType;
  eventTitle: string;
  eventDate: string;
  eventTime: string;
  eventLocation: string;
  eventAttendees: number;
  onPress: () => void;
  images: { media: ImageSourcePropType[] }[];
};

const PreviouslyAttendedEvent: FC<PreviouslyAttendedEventProps> = ({
  eventStatus,
  eventHeroImage,
  eventTitle,
  eventDate,
  eventTime,
  eventLocation,
  eventAttendees,
  onPress,
  images,
}) => {
  return (
    <View style={styles.container}>
      <EventCardSmall
        eventStatus={eventStatus}
        eventHeroImage={eventHeroImage}
        eventTitle={eventTitle}
        eventDate={eventDate + ' ago'}
        eventTime={eventTime}
        eventLocation={eventLocation}
        eventAttendees={eventAttendees}
        onPress={onPress}
      />
      {images.length > 0 && <Gallery images={images} />}
    </View>
  );
};

export default PreviouslyAttendedEvent;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    rowGap: Spacing.md,
  },
});
