import { ScrollView, StyleSheet, Text, View } from 'react-native';
import React from 'react';
import { globalStyles } from '../../styles/GlobalStyles';
import { Spacing } from '../../styles/SpacingStyles';
import Button2, {
  ButtonDisplay,
  ButtonSize,
  ButtonType,
} from '../../components/testing/animations/Button2';
import HeaderDefaultTitled from '../../components/HeaderDefaultTitled';
import HeaderFlagAttendee, {
  ANType,
  APType,
} from '../../components/HeaderFlagAttendee';
import HeaderFlagHost, {
  HNType,
  HPType,
} from '../../components/HeaderFlagHost';
import HeaderEditTitled from '../../components/HeaderEditTitled';
import HeaderOptions from '../../components/HeaderOptions';
import Button from '../../components/Button';

import ImportedIcon from '../../assets/icons/favorite-fill.svg';
import ButtonGroup from '../../components/ButtonGroup';
import FlagMedium, { FlagType } from '../../components/FlagMedium';
import Avatar, { AvatarSize, AvatarStatus } from '../../components/Avatar';

import Post from '../../components/feed/Post';
import DropdownSmall, { Align, Icon } from '../../components/DropdownSmall';
import dropdownOptionsPost from '../../components/DropdownOptionsPost';
import avatarimg from '../../assets/images/temp/host-img-1.jpg';
import EventCardSmall from '../../components/EventCardSmall';

import testImage from '../../assets/images/temp/event-img-11.jpg';

import { SAMPLEEVENTDATA } from '../../data/sampleUpcomingEventData';
import Gallery from '../../components/Gallery';
import OtherUserProfileScreen from '../otherUserProfile/OtherUserProfile';

const TestScreen = () => {
  const upcomingEventData = SAMPLEEVENTDATA.find((event) => event.id === '2');

  return (
    <ScrollView style={styles.container}>
      <View style={styles.header}>
        <Text
          style={[
            globalStyles.displayTextTwo,
            globalStyles.textDark,
            { textAlign: 'center' },
          ]}>
          Testing screen
        </Text>
      </View>

      {/* --- START TESTING CODE BELOW --- */}

      {/* <Gallery />

        <EventCardSmall
          eventHeroImage={upcomingEventData?.uri}
          eventTitle={upcomingEventData?.title}
          eventDate={upcomingEventData?.date}
          eventTime={upcomingEventData?.time}
          eventLocation={upcomingEventData?.location}
          eventAttendees={upcomingEventData?.attendees}
          onPress={() => console.log('Event card image pressed')}
        /> */}

      <OtherUserProfileScreen />

      {/* <HeaderFlagAttendee
        title="Attendee header"
        previousType={APType.StartingSoon}
        nextType={ANType.Live}
      /> */}
      {/* <HeaderFlagHost
        title="Host header"
        previousType={HPType.StartingSoon}
        nextType={HNType.Live}
      /> */}

      {/* <Header4 previousType={PType.StartingSoon} nextType={NType.Live} /> */}
      {/* <Button2
        text="Example button"
        type={ButtonType.Success}
        size={ButtonSize.Medium}
        display={ButtonDisplay.Contained}
      /> */}
    </ScrollView>
  );
};

export default TestScreen;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    gap: 16,
  },

  header: {
    margin: Spacing.lg,
  },
});
