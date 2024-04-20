//#region imports
import React, { useEffect } from 'react';
import { Image, Pressable, StyleSheet, Text, View } from 'react-native';
import { useFocusEffect } from '@react-navigation/native';
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

import Animated, {useSharedValue, ReduceMotion, withSpring, SlideInUp, SlideOutDown, useAnimatedStyle } from 'react-native-reanimated';
import { TouchableOpacity, TouchableWithoutFeedback } from 'react-native-gesture-handler';
import { SpringConfig } from 'react-native-reanimated/lib/typescript/reanimated2/animation/springUtils';
//#endregion 

interface EventCardLargeProps {
  onPressIn?: () => void;
  onPressOut?: () => void;

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
  onPressIn = () => {},
  onPressOut = () => {},
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

  const eventAttendeesFriendsLabelText = `${eventAttendeesFriends} FRIENDS`;

  const displacement = 6;
  const shift = useSharedValue(displacement);

  useFocusEffect(
    React.useCallback(() => {
      shift.value = withSpring(2, {
        duration: 1000,
        dampingRatio: 0.3,
        stiffness: 1,
        overshootClamping: false,
        restDisplacementThreshold: 0.01,
        restSpeedThreshold: 2,
        reduceMotion: ReduceMotion.System,
      });

      return () => shift.value = displacement;
    }, [])
  );

  const shadowStyle = useAnimatedStyle(() => {
    return {
      height: CustomDimensions.windowHeight - Spacing.xl * 6,
      width: CustomDimensions.windowWidth - Spacing.lg * 2, 
      backgroundColor: Colors.sparrowDark,
      position:'absolute',
      borderRadius: borderRadius.md,
      top: displacement + shift.value 
    };
  });

  const cardStyle = useAnimatedStyle(() => {
    return {
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
      position: "absolute",
      top: shift.value 
    };
  });

  return (
  <Pressable onPressIn={() => onPressIn()} onPressOut={() => onPressOut()}>
    <View style={{ 
        height: styles.eventCardLarge.height + displacement, 
        width: styles.eventCardLarge.width, 
        borderRadius: borderRadius.md, 
    }}>

    {/* <Shadow height = {height} width={styles.eventCardLarge.width}/> */}

    {/* Shadow */}
    <Animated.View style={shadowStyle}/>

    {/* <View style={styles.eventCardLarge}/> */}

    <Animated.View style={cardStyle}>

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

    </Animated.View>

    </View>
  </Pressable>
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
    position: "absolute",
    top: 10
  },

  shadow: {
    height: CustomDimensions.windowHeight - Spacing.xl * 6 + 6,
    width: CustomDimensions.windowWidth - Spacing.lg * 2, 
    backgroundColor: Colors.sparrowDark,
    position:'absolute',
    borderRadius: borderRadius.md,
    top: 10
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
