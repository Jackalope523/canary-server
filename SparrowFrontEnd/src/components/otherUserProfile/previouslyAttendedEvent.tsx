import { ImageSourcePropType, StyleSheet, Text, View } from 'react-native';
import React, { FC } from 'react';
import EventCardSmall, { EventStatus } from '../EventCardSmall';
import { Spacing } from '../../styles/SpacingStyles';
import { SAMPLE_PAST_EVENT_DATA } from '../../data/samplePastEventData';
import EventCardSmallAccordion from '../EventCardSmallAccordion';
import Gallery from '../Gallery';

type PreviouslyAttendedEventProps = {
  eventTitle: string;
  eventDate: string;
  onPress: () => void;
  images: { media: ImageSourcePropType[] }[];
  posterAvatar: ImageSourcePropType;
  posterName: string;
  onPressViewEvent: () => void;
};

const PreviouslyAttendedEvent: FC<PreviouslyAttendedEventProps> = ({
  eventTitle,
  eventDate,
  onPress,
  images,
  posterAvatar,
  posterName,
  onPressViewEvent,
}) => {
  const [isExpanded, setIsExpanded] = React.useState(false);

  const onPressCard = () => {
    setIsExpanded(!isExpanded);
  }

  return (
    <View style={styles.container}>
      {/* TODO on handlePress in eventCardSmllAccordion, show the gallery, else hide */}
      <EventCardSmallAccordion
        eventTitle={eventTitle}
        eventDate={eventDate}
        onPressViewEvent={onPressViewEvent}
        onPressCard={onPressCard}
      />

      {
        isExpanded && images.length > 0 &&
        <Gallery images={images} posterAvatar={posterAvatar} posterName={posterName} />
      }
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
