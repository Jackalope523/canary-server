import { ImageBackground, Text, View, Pressable } from 'react-native';
import React, { useState } from 'react';
import { cardStyles } from '../styles/CardStyles';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';
import { globalStyles } from '../styles/GlobalStyles';
import { Colors } from '../styles/ColorStyles';

// !! THIS COMPONENT IS CURRENTLY NOT IN USE !!

const Icon = createIconSetFromFontello(fontelloConfig);

// TEMP. example imports
const bgImage = {
  uri: 'https://images.unsplash.com/photo-1562519819-016930ada31b?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=687&q=80',
};

// Icons
import DateOutline from '../assets/icons/date-outline.svg';
import TimeOutline from '../assets/icons/time-outline.svg';
import AccountOutline from '../assets/icons/account-outline.svg';
import LocationOutline from '../assets/icons/location-outline.svg';

// INFO
// ImageBackground source - {item.uri}
// Text onTextLayout - {handleTextLayout}

// Types
interface EventCardMediumProps {
  onPress: () => void;

  eventDate: string;
  eventTime: string;
  eventAttendees: number;
  eventTitle: string;
  eventLocation: string;
  eventHeroImage: { uri: string };
}

export const EventCardMedium: React.FC<EventCardMediumProps> = ({
  eventDate = null,
  eventTime = null,
  eventAttendees = null,
  eventTitle = null,
  eventLocation = null,

  // TODO insert an IMAGE NOT FOUND image here
  eventHeroImage = {
    uri: 'https://images.unsplash.com/photo-1541140134513-85a161dc4a00?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
  },
  onPress = null,
}) => {
  // TODO if LOCATION text exceeds 1 line, add ... and hide overflowing text

  return (
    <Pressable onPress={onPress}>
      <View style={cardStyles.eventCardMedium}>
        <ImageBackground
          source={eventHeroImage}
          resizeMode="cover"
          imageStyle={cardStyles.eventCardMediumImage}>
          <View style={cardStyles.eventCardMediumContent}>
            <View style={cardStyles.eventCardMediumTopWrapper}>
              <View style={cardStyles.eventCardMediumContentInner}>
                <View style={cardStyles.eventCardMediumTextWrapper}>
                  <DateOutline
                    height={24}
                    width={24}
                    fill={Colors.sparrowDarkBrown}
                  />
                  <Text
                    style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
                    {eventDate}
                  </Text>
                </View>
                <View style={cardStyles.eventCardMediumTextWrapper}>
                  <TimeOutline
                    height={24}
                    width={24}
                    fill={Colors.sparrowDarkBrown}
                  />
                  <Text
                    style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
                    {eventTime}
                  </Text>
                </View>
              </View>
              <View style={cardStyles.eventCardMediumContentInner}>
                <View style={cardStyles.eventCardMediumTextWrapper}>
                  <AccountOutline
                    height={24}
                    width={24}
                    fill={Colors.sparrowDarkBrown}
                  />
                  <Text
                    style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
                    {eventAttendees}
                  </Text>
                </View>
              </View>
            </View>
            <View style={cardStyles.eventCardMediumContentInner}>
              <Text
                numberOfLines={2}
                style={[
                  globalStyles.headingTextThree,
                  globalStyles.textDark,
                  cardStyles.eventCardMediumTitle,
                ]}>
                {eventTitle}
              </Text>
              <View
                style={[
                  cardStyles.eventCardMediumTextWrapper,
                  cardStyles.eventCardMediumTextWrapperCenter,
                ]}>
                <LocationOutline
                  height={24}
                  width={24}
                  fill={Colors.sparrowDarkBrown}
                />
                <Text
                  numberOfLines={1}
                  style={[
                    globalStyles.bodyTextOne,
                    globalStyles.textDark,
                    cardStyles.eventCardMediumInnerText,
                  ]}>
                  {eventLocation}
                </Text>
              </View>
            </View>
          </View>
        </ImageBackground>
      </View>
    </Pressable>
  );
};

export default EventCardMedium;
