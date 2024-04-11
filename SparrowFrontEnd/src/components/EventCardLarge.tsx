//#region imports
import { Image, StyleSheet, Text, View } from 'react-native';
import React from 'react';
import Avatar, { AvatarSize } from './Avatar';

// Icons
import FeatherIcon from '../assets/icons/feather-fill-colored.svg';
import DateIcon from '../assets/icons/date-outline.svg';
import LocationIcon from '../assets/icons/location-outline.svg';
import PersonIcon from '../assets/icons/account-outline.svg';

// TODO delete later after updating; TEMP. imports
import TempAvatarImage from '../assets/images/temp/host-img-1.jpg';
import tempHeroImage from '../assets/images/temp/event-img-1.2.jpg';

import { globalStyles } from '../styles/GlobalStyles';
import { Spacing } from '../styles/SpacingStyles';
import { Colors } from '../styles/ColorStyles';
import { borderRadius } from '../styles/BorderStyles';
import TextLabel, { LabelDisplay, LabelSize, LabelType } from './TextLabel';
import { CustomDimensions } from '../styles/CustomDimensionStyles';
import { VectorSource } from '@rnmapbox/maps';
import symbolicateStackTrace from 'react-native/Libraries/Core/Devtools/symbolicateStackTrace';
import Shadow from '../flows/Shadow';
//#endregion 

interface EventCardLargeProps {
  onPress: () => void;

  eventHeroImage: any;
  eventHostName: string;
  eventTitle: string;
  eventDate: string;
  eventTime: string;
  eventLocation: string;
  eventAttendees: number;
  eventAttendeesFriends: number;
}

const EventCardLarge: React.FC<EventCardLargeProps> = ({
  onPress,
  eventHeroImage,
  eventHostName,
  eventTitle,
  eventDate,
  eventTime,
  eventLocation,
  eventAttendees,
  eventAttendeesFriends,
}) => {
  // TODO hook up to back-end data
  var friend = true;

  // TODO delete the constants below after hooking up data; TEMP. data

  // const eventHostName = 'Robert';
  // const eventTitle = 'Hike and Sunrise Breakfast Adventure at Pine Ridge Trail';
  // const eventDate = 'This Saturtday';
  // const eventTime = '12:30';
  // const eventLocation = 'Pine Ridge Trail, Trailhead Parking Lot Number 2';
  // const eventAttendees = 9;
  // const eventAttendeesFriends = 2;

  const eventAttendeesFriendsLabelText = `${eventAttendeesFriends} FRIENDS`;

  return (
  <View style={styles.eventCardLargeContainer}>

    <Shadow height = {styles.eventCardLarge.height} width={styles.eventCardLarge.width}/>

    <View style={styles.eventCardLarge}>

      {/* Host Info */}
      <View style={styles.host}>
        <Avatar size={AvatarSize.Small} image={TempAvatarImage} />
        <View style={styles.hostNameContainer}>
          <Text style={[globalStyles.headingTextFour, globalStyles.textDark]}>
            {eventHostName}
          </Text>
          {friend && <FeatherIcon height={24} width={24} />}
        </View>
      </View>

      {/* Image Title Adaptive Combo */}
      <View style={styles.eventTop}>
        <Image
          source={eventHeroImage}
          resizeMode="cover"
          style={styles.eventHeroImage}
        />
        <Text
          style={[globalStyles.headingTextThree, globalStyles.textDark]}
          numberOfLines={2}>
          {eventTitle}
        </Text>
      </View>
      {/* aligned to bottom */}

      {/* Event Info */}
      <View style={styles.eventInfo}>
        <View style={styles.eventInfoItem}>
          <DateIcon
            height={24}
            width={24}
            fill={Colors.sparrowDarkBrown}
          />             
          <Text style={globalStyles.textDark}>
            <Text
              style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
              {eventDate} at{' '}
            </Text>
            <Text
              style={[
                globalStyles.bodyTextOneBold,
                globalStyles.textDark,
              ]}>
              {eventTime}
            </Text>
          </Text>
        </View>
        <View style={styles.eventInfoItem}>
          <LocationIcon
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
      </View>

      {/* Attendee Info*/}
      <View style={styles.eventAttendees}>
        <View style={styles.eventInfoItem}>
          <PersonIcon
            height={24}
            width={24}
            fill={Colors.sparrowDarkBrown}
          />
          <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
            {eventAttendees}
          </Text>
        </View>
        {eventAttendeesFriends > 0 && (
        <TextLabel
          text={eventAttendeesFriendsLabelText}
          type={LabelType.Friend}
          size={LabelSize.Small}
          display={LabelDisplay.Contained}
        />
        )}
      </View>

    </View>
    {/* TODO make shadow effects a re-usable component; it should use the height and width of the chosen component */}
    
  </View>
  );
};

export default EventCardLarge;

const styles = StyleSheet.create({
  eventCardLargeContainer: {
    height: CustomDimensions.windowHeight - Spacing.xl * 6 + 6,
    width: CustomDimensions.windowWidth - Spacing.lg * 2,
  },

  eventCardLarge: {
    backgroundColor: Colors.sparrowSand,
    padding: Spacing.md,
    borderWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
    borderRadius: borderRadius.md,

    // TODO add dynamic height based on screen size + configure image size
    // REMEMBER: card height needs to stay the same when viewed from the same device; the image size should change on other devices/screen sizes
    // TEMP. config; can't figure out how to make it fill the whole leftover height area (use flex, just where?)
    height: CustomDimensions.windowHeight - Spacing.xl * 6,
    width: CustomDimensions.windowWidth - Spacing.lg * 2,
    flex: 1,
    position: "absolute"
  },

  shadow: {
    height: CustomDimensions.windowHeight - Spacing.xl * 6 + 6,
    width: CustomDimensions.windowWidth - Spacing.lg * 2, 
    backgroundColor: Colors.sparrowDark,
    position:'absolute',
    borderRadius: borderRadius.md
  },

  // Host
  host: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.sm,
    paddingBottom: Spacing.md,
    flex: 1,
  },

  hostNameContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.xs,
  },

  eventTop: {
    rowGap: Spacing.md,
    flex: 9,

    // TODO titls should always be on the same line; space-between config between top and bottom
    paddingBottom: Spacing.lg,
  },

  eventHeroImage: {
    width: '100%',
    flex: 1,
    borderRadius: borderRadius.md,
    borderWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
  },

  eventInfo: {
    rowGap: Spacing.sm,
    paddingBottom: 30,
    flex: 1,
  },

  // TODO fix eventInfoItem overlapping the card
  eventInfoItem: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.sm,
  },

  eventAttendees: {
    paddingTop: 5,
    borderTopWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    flex: 1,
  },
});
