import { ImageBackground, Text, View, Pressable } from 'react-native';
import React, { useState } from 'react';
import { cardStyles } from '../../styles/Cards';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
import { globalStyles } from '../../styles/Global';
import { Colors } from '../../styles/Colors';

// !! THIS COMPONENT IS CURRENTLY NOT IN USE !!
// TODO DELETE  this component

const Icon = createIconSetFromFontello(fontelloConfig);

// TEMP. example imports
const bgImage = {
  uri: 'https://images.unsplash.com/photo-1562519819-016930ada31b?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=687&q=80',
};

// INFO
// ImageBackground source - {item.uri}
// Text onTextLayout - {handleTextLayout}

const EventCardMedium = ({
  eventDate,
  eventTime,
  eventAttendees,
  eventTitle,
  eventLocation,
  eventHeroImage,
  onPress,
}) => {
  // Activity component testing
  // If textWrapper text exceeds 2 lines, align items to flex-start
  // const [isTextOverflowing, setIsTextOverflowing] = useState(false);

  // const handleTextLayout = (event) => {
  //     const { lines } = event.nativeEvent;

  //     setIsTextOverflowing(lines.length > 2);
  // };

  return (
    <Pressable onPress={onPress}>
      <View style={cardStyles.eventCardMedium}>
        <ImageBackground
          source={eventHeroImage}
          resizeMode="cover"
          imageStyle={cardStyles.eventCardMedium.bgImage2}>
          <View style={cardStyles.eventCardMedium.content}>
            <View style={cardStyles.eventCardMedium.content.topWrapper}>
              <View style={cardStyles.eventCardMedium.content.container}>
                <View
                  style={
                    cardStyles.eventCardMedium.content.container.textWrapper
                  }>
                  <Icon
                    name="date-outline"
                    size={24}
                    height={24}
                    width={24}
                    style={
                      cardStyles.eventCardMedium.content.container.textWrapper
                        .icon
                    }
                  />
                  <Text
                    style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
                    {eventDate}
                  </Text>
                </View>
                <View
                  style={
                    cardStyles.eventCardMedium.content.container.textWrapper
                  }>
                  <Icon
                    name="time-outline"
                    size={24}
                    height={24}
                    width={24}
                    style={
                      cardStyles.eventCardMedium.content.container.textWrapper
                        .icon
                    }
                  />
                  <Text
                    style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
                    {eventTime}
                  </Text>
                </View>
              </View>
              <View style={cardStyles.eventCardMedium.content.container}>
                <View
                  style={
                    cardStyles.eventCardMedium.content.container.textWrapper
                  }>
                  <Icon
                    name="account-outline"
                    size={24}
                    height={24}
                    width={24}
                    style={
                      cardStyles.eventCardMedium.content.container.textWrapper
                        .icon
                    }
                  />
                  <Text
                    style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
                    {eventAttendees}
                  </Text>
                </View>
              </View>
            </View>
            <View style={cardStyles.eventCardMedium.content.container}>
              <Text
                numberOfLines={2}
                style={[
                  globalStyles.headingTextThree,
                  globalStyles.textDark,
                  cardStyles.eventCardMedium.content.container.title,
                ]}>
                {eventTitle}
              </Text>
              <View
                style={[
                  cardStyles.eventCardMedium.content.container.textWrapper,
                  cardStyles.eventCardMedium.content.container
                    .textWrapperCenter,
                ]}>
                <Icon
                  name="location-outline"
                  size={24}
                  height={24}
                  width={24}
                  style={
                    cardStyles.eventCardMedium.content.container.textWrapper
                      .icon
                  }
                />
                <Text
                  numberOfLines={1}
                  style={[
                    globalStyles.bodyTextOne,
                    globalStyles.textDark,
                    cardStyles.eventCardMedium.content.container.textWrapper
                      .innerText,
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
