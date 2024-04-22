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

import { globalStyles } from '../styles/GlobalStyles';
import { Spacing } from '../styles/SpacingStyles';
import { Colors } from '../styles/ColorStyles';
import { borderRadius } from '../styles/BorderStyles';
import TextLabel, { LabelDisplay, LabelSize, LabelType } from './TextLabel';
import { CustomDimensions } from '../styles/CustomDimensionStyles';

import Animated, {
  useSharedValue, 
  ReduceMotion, 
  withSpring, 
  useAnimatedStyle, 
  withDelay, 
  withTiming } from 'react-native-reanimated';
//#endregion 

interface EventCardLargeProps {
  onPressIn?: () => void;
  onPressOut?: () => void;

  id: number;
  eventHeroImage: any;
  eventHostName: string;
  eventTitle: string;
  eventDate: string;
  eventTime: string;
  eventLocation: string;
  eventAttendees: number;
  eventAttendeesFriends: number;
  hiddenCards: number[];

  wakeShift?: number;
  shadowWidth?: number;
}

const EventCardLarge: React.FC<EventCardLargeProps> = ({
  onPressIn = () => {},
  onPressOut = () => {},
  id = -1,
  eventHeroImage,
  eventHostName,
  eventTitle,
  eventDate,
  eventTime,
  eventLocation,
  eventAttendees,
  eventAttendeesFriends,
  hiddenCards = [],

  wakeShift = 4,
  shadowWidth = 1,
}) => {
  var friend = true;

  const eventAttendeesFriendsLabelText = `${eventAttendeesFriends} FRIENDS`;


  //||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
  //||                                Animation                                       ||
  //||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
  //#region Animation                                                                 ||
  const cardHeight = CustomDimensions.windowHeight - Spacing.xl * 6;
  const cardWidth = CustomDimensions.windowWidth - Spacing.lg * 2;
  
  const pathHeight = cardHeight + wakeShift + 20;
  const pathWidth = useSharedValue(cardWidth);

  const wakeShiftSV = useSharedValue(wakeShift);
  const shadowWidthSV = useSharedValue(0);

  const hideTop = useSharedValue(0);

  const hoverConfig = {
    duration: 200,
    dampingRatio: 0.3,
    stiffness: 1,
    overshootClamping: false,
    restDisplacementThreshold: 0.01,
    restSpeedThreshold: 2,
    reduceMotion: ReduceMotion.System,
  };

  const slideConfig = {
    duration: 200,
    dampingRatio: 0.3,
    stiffness: 1,
    overshootClamping: false,
    restDisplacementThreshold: 0.01,
    restSpeedThreshold: 2,
    reduceMotion: ReduceMotion.System,
  };

  useFocusEffect(
    React.useCallback(() => {
      wakeShiftSV.value = withTiming(0, hoverConfig);
      shadowWidthSV.value = withTiming(shadowWidth, hoverConfig);

      return () => {
        wakeShiftSV.value = wakeShift;
        shadowWidthSV.value = 0;
      }
    }, [])
  );

  const animatePressIn = () => {
    wakeShiftSV.value = withTiming(wakeShift, hoverConfig);
    shadowWidthSV.value = withTiming(0, hoverConfig);
  }

  const animatePressOut = () => {
    wakeShiftSV.value = withTiming(0, hoverConfig);
    shadowWidthSV.value = withTiming(shadowWidth, hoverConfig);
  }

  const hide = () => {  
    hideTop.value = withTiming(cardHeight + 20, slideConfig);
    pathWidth.value = withDelay(slideConfig.duration, withTiming(0, slideConfig));
  }

  const show = () => {
    pathWidth.value = withTiming(cardWidth);
    hideTop.value = withDelay(slideConfig.duration, withTiming(0, slideConfig));
  }

  const shadowStyle = useAnimatedStyle(() => {
    return {
      height: cardHeight,
      width: cardWidth, 
      backgroundColor: Colors.sparrowDark,
      
      borderRadius: borderRadius.md,

      position:"absolute",
      top: wakeShift + shadowWidthSV.value + hideTop.value
    };
  });

  const cardStyle = useAnimatedStyle(() => {
    return {
      // TODO add dynamic height based on screen size + configure image size
      // REMEMBER: card height needs to stay the same when viewed from the same device; the image size should change on other devices/screen sizes
      // TEMP. config; can't figure out how to make it fill the whole leftover height area (use flex, just where?)
      height: cardHeight,
      width: cardWidth,
      backgroundColor: Colors.sparrowSand,
      padding: Spacing.md,

      borderWidth: 2,
      borderColor: Colors.sparrowDarkBrown,
      borderRadius: borderRadius.md,
    
      position: "absolute",
      top: wakeShiftSV.value + hideTop.value
    };
  });

  const animationPath = useAnimatedStyle(() => {
    return {
      height: pathHeight, 
      width: pathWidth.value, 
      borderRadius: borderRadius.md
    };
  });

  //#endregion                                                                        ||
  //||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||


  //||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
  //||                                  Logic                                         ||
  //||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
  //#region Logic                                                                     ||

  useEffect(() => {
    hiddenCards.includes(id) ? hide() : show()
  }, [hiddenCards]);

  //#endregion                                                                        ||
  //||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||

  return (
  <Pressable onPressIn={animatePressIn} onPressOut={animatePressOut}>
    <Animated.View  style={animationPath}>

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
              <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
                {eventDate} at {' '}
              </Text>
              <Text style={[ globalStyles.bodyTextOneBold, globalStyles.textDark ]}>
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
              numberOfLines={1}
            >
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

    </Animated.View>
  </Pressable>
  );
};

export default EventCardLarge;

const styles = StyleSheet.create({
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
