import {
  Image,
  ImageSourcePropType,
  Pressable,
  StyleSheet,
  Text,
  View,
} from 'react-native';
import React from 'react';

import { globalStyles } from '../styles/GlobalStyles';
import { Colors } from '../styles/ColorStyles';
import { Spacing } from '../styles/SpacingStyles';
import { CustomDimensions } from '../styles/CustomDimensionStyles';

// Icons
import DateOutline from '../assets/icons/date-outline.svg';
import TimeOutline from '../assets/icons/time-outline.svg';
import AccountOutline from '../assets/icons/account-outline.svg';
import LocationOutline from '../assets/icons/location-outline.svg';

interface EventCardSmallProps {
  eventHeroImage: ImageSourcePropType;
  eventTitle?: string;
  eventDate?: string;
  eventTime?: string;
  eventLocation?: string;
  eventAttendees?: number;

  onPress?: () => void;

  eventStatus?: EventStatus;
}

const EventCardSmall: React.FC<EventCardSmallProps> = ({
  eventHeroImage,
  eventTitle = 'NULL',
  eventDate = 'NULL',
  eventTime = 'NULL',
  eventLocation = 'NULL',
  eventAttendees,
  onPress = null,
  eventStatus = EventStatus.Upcoming,
}) => {
  // Text
  const attendees = `${eventAttendees} attendees`;

  // Image size
  const imageSize = (CustomDimensions.windowWidth - Spacing.lg * 2) / 4.8;

  return (
    <View style={styles.container}>
      <Pressable
        onPress={onPress}
        style={{ height: imageSize, width: imageSize }}>
        <Image source={eventHeroImage} style={styles.image} />
      </Pressable>
      <View style={styles.eventDetailsWrapper}>
        <Text
          style={[globalStyles.headingTextFour, globalStyles.textDark]}
          numberOfLines={2}>
          {eventTitle}
        </Text>
        <View style={styles.dateAndTime}>
          <View style={styles.eventDetailsInnerContainer}>
            <DateOutline
              height={24}
              width={24}
              fill={Colors.sparrowDarkBrown}
            />
            <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
              {eventDate}
            </Text>
          </View>
          {eventStatus === EventStatus.Upcoming && (
            <View style={styles.eventDetailsInnerContainer}>
              <TimeOutline
                height={24}
                width={24}
                fill={Colors.sparrowDarkBrown}
              />
              <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
                {eventTime}
              </Text>
            </View>
          )}
        </View>
        <View style={styles.eventDetailsInnerContainer}>
          <LocationOutline
            height={24}
            width={24}
            fill={Colors.sparrowDarkBrown}
          />
          <Text
            style={[globalStyles.bodyTextOne, globalStyles.textDark]}
            numberOfLines={1}>
            {eventLocation}
          </Text>
        </View>
        {eventStatus === EventStatus.Upcoming && (
          <View style={styles.eventDetailsInnerContainer}>
            <AccountOutline
              height={24}
              width={24}
              fill={Colors.sparrowDarkBrown}
            />
            <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
              {attendees}
            </Text>
          </View>
        )}
      </View>
    </View>
  );
};

// Exported enums
export enum EventStatus {
  Upcoming,
  Past,
}

export default EventCardSmall;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    flexDirection: 'row',
    columnGap: Spacing.md,
  },

  image: {
    width: '100%',
    height: '100%',

    borderRadius: 8,
    borderWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
  },

  eventDetailsWrapper: {
    flex: 1,
    rowGap: Spacing.xxs,
  },

  dateAndTime: {
    flexDirection: 'row',
    columnGap: Spacing.md,
    marginTop: Spacing.sm,
  },

  eventDetailsInnerContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.xs,
  },
});
