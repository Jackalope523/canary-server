import * as React from 'react';
import {
  View,
  Text,
  TextInput,
  StyleSheet,
  Image,
  ScrollView,
  LayoutChangeEvent,
} from 'react-native';
import { StackScreenProps } from '@react-navigation/stack';
import { Colors } from '../../styles/ColorStyles';
import { globalStyles } from '../../styles/GlobalStyles';
import { EventStackParamList } from '../../components/atoms/types';
import { eventShard, getEvent } from './eventPigeon';
import Avatar, { AvatarSize, AvatarStatus } from '../../components/Avatar';
import TextLabel, {
  LabelDisplay,
  LabelSize,
  LabelType,
} from '../../components/TextLabel';
import { labelText } from '../../components/LabelText';
import { Spacing } from '../../styles/SpacingStyles';
import { borderRadius } from '../../styles/BorderStyles';
import TextButton, {
  TextButtonType,
  TextButtonVariant,
} from '../../components/TextButton';
import Gallery from '../../components/testing/OldGallery';
import { SAMPLE_PAST_EVENT_DATA } from '../../data/samplePastEventData';
import Button, {
  ButtonDisplay,
  ButtonSize,
  ButtonType,
} from '../../components/Button';
import AvatarStackScroll, {
  AvatarType,
} from '../../components/AvatarStackScroll';
import { SAMPLE_USER_DATA } from '../../data/sampleUserData';
import SmallMessage, { SmallMessageType } from '../../components/SmallMessage';

type EventProps = StackScreenProps<EventStackParamList, 'Event'>;

// TEMP. images
import tempAvatar from '../../assets/images/temp/host-img-1.jpg';
import tempBanner from '../../assets/images/temp/event-img-1.jpg';
import tempMap from '../../assets/images/temp/temp-map.png';

// Icons
import DateIcon from '../../assets/icons/date-outline.svg';
import TimeIcon from '../../assets/icons/time-outline.svg';
import MapIcon from '../../assets/icons/discovery-fill.svg';
import LocationIcon from '../../assets/icons/location-outline.svg';
import PersonIcon from '../../assets/icons/account-outline.svg';
import ShareIcon from '../../assets/icons/share-outline.svg';
import BirdIcon from '../../assets/icons/bird-fill-colored.svg';
import FeatherIcon from '../../assets/icons/feather-fill-colored.svg';
import HeaderFlagAttendee from '../../components/HeaderFlagAttendee';
import HeaderFlagHost from '../../components/HeaderFlagHost';

const EventScreen = ({ route, navigation }: EventProps) => {
  // Sample data
  const pastEventData = SAMPLE_PAST_EVENT_DATA.find(
    (event) => event.id === '3',
  );

  const avatarData = SAMPLE_USER_DATA.map((user) => user.avatar);

  const hostRating = 4.5;
  const hostName = 'Jordan';
  const hostAvatar = tempAvatar;

  const eventTitle = 'Dog Walk and Play Meetup at Central Park';
  const eventDate = 'This Tuesday';
  const eventTime = '16:30';
  const eventLocation = 'Central Park, Manhattan, New York';
  const eventDescription = `Join us for a fun dog walk and play at Central Park. We will meet at the entrance and walk around the park. We will have a break at the dog park for some playtime. All dogs are welcome!`;

  // types - Live, Upcoming (subtypes - in >24h, in <24h), Terminated
  const eventStatus = 'Terminated';

  // user who is viewing the event
  // types - host, attendee
  const userType = 'Attendee';
  // user who is hosting
  // types - you, friend, anon
  const hostType = 'You';

  const hasUserJoinedEvent = false;

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Layout                                     ||
  // ! ||--------------------------------------------------------------------------------||
  const [multiline, setMultiline] = React.useState(false);

  const handleTextLayout = (event: LayoutChangeEvent) => {
    const { lines } = event.nativeEvent;
    setMultiline(lines.length > 1);
  };

  /*
  
  TODO get exact height from button component;
  best if I measure this once as the component size wouldn't change and
  create a separate TS file for button height constants and import it here

  BLOCK: the new Button.tsx (animated) needs to be fixed beforehand

  */
  const attendeeControlsButtonHeight = 24;

  // TODO fix TS error in the code below, then uncomment cause it's needed

  // const [errorText, setErrorText] = React.useState('');
  // const [eventText, setEventText] = React.useState('');

  // function handleGetEvent() {
  //   setErrorText('');

  //   getEvent(route.params.EventID)
  //     .then((data) => populateScreen(data))
  //     .catch(() => setErrorText('Could not retrieve data. Incorrect code'));
  // }

  // if (eventText == '' || errorText == '')
  //   // To avoid recursion from component reloading on set state
  //   handleGetEvent();

  // function populateScreen(data: eventShard) {
  //   setEventText(`Event Title: ${data.Name}\n
  //           Host Name: ${data.Host.Name}\n\n
  //           Event Description: ${data.Description}\n
  //           Start Time: ${data.StartTime}`);
  // }

  function manageAttendees() {
    navigation.navigate('ManageAttendees');
  }

  function inviteFriends() {
    navigation.navigate('Share');
  }

  function terminateEvent() {
    navigation.navigate('TerminateEvent');
  }

  function leaveEvent() {
    navigation.navigate('LeaveEvent');
  }

  return (
    <View>
      {/* TODO add a fixed HeaderFlag component here after bugfix */}
      <Text style={{ backgroundColor: Colors.fuchsia400 }}>
        fixed HeaderFlag component here after bugfix
      </Text>

      {/* ATTENDEE CONTROLS */}
      {userType === 'Attendee' ? (
        <View style={styles.attendeeControls}>
          {hasUserJoinedEvent ? (
            <Button
              type={ButtonType.Error}
              size={ButtonSize.Medium}
              display={ButtonDisplay.Full}
              text={'Leave event'}
              onPress={leaveEvent}
            />
          ) : (
            <Button
              type={ButtonType.Success}
              size={ButtonSize.Medium}
              display={ButtonDisplay.Full}
              text={'Join event'}
              onPress={null}
            />
          )}
        </View>
      ) : null}

      {/* EVENT */}
      <ScrollView
        contentContainerStyle={styles.container}
        overScrollMode="never"
        showsVerticalScrollIndicator={false}>
        {/* HOST SECTION  */}
        <View style={[styles.hostSectionWrapper, styles.basePadding]}>
          <View style={styles.hostSection}>
            <Avatar
              image={hostAvatar}
              size={AvatarSize.Medium}
            />
            <View>
              <View style={styles.hostNameType}>
                <Text
                  style={[globalStyles.headingTextFour, globalStyles.textDark]}>
                  {hostName}
                </Text>
                {hostType === 'You' ? (
                  <BirdIcon width={24} height={24} />
                ) : hostType === 'Friend' ? (
                  <FeatherIcon width={24} height={24} />
                ) : null}
              </View>
              <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
                Rating: {hostRating}
              </Text>
            </View>
          </View>
        </View>

        {/* EVENT INFO SECTION */}
        <View>
          <View style={styles.basePadding}>
            <Image
              source={tempBanner}
              style={styles.bannerImage}
              resizeMode="cover"
            />
            <Text
              style={[
                globalStyles.headingTextThree,
                globalStyles.textDark,
                styles.title,
              ]}>
              {eventTitle}
            </Text>
          </View>

          {/* DATE AND TIME SECTION */}
          <View style={[styles.dateTimeSection, styles.basePadding]}>
            <Text style={[globalStyles.headingTextFour, globalStyles.textDark]}>
              Date and time
            </Text>
            {/* inner */}
            <View style={styles.dateTimeInnerSection}>
              {/* date */}
              <View style={styles.dateTime}>
                <DateIcon
                  width={24}
                  height={24}
                  fill={Colors.sparrowDarkBrown}
                />
                <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
                  {eventDate}
                </Text>
              </View>

              {/* time */}
              <View style={styles.dateTime}>
                <TimeIcon
                  width={24}
                  height={24}
                  fill={Colors.sparrowDarkBrown}
                />
                <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
                  {eventTime}
                </Text>
              </View>
            </View>
          </View>

          {/* LOCATION SECTION */}
          {/* TODO add mapbox */}
          <View style={[styles.locationSection, styles.basePadding]}>
            {/* inner */}
            <View style={styles.locationInnerSection}>
              <Text
                style={[globalStyles.headingTextFour, globalStyles.textDark]}>
                Location
              </Text>
              <TextButton
                type={TextButtonType.Dark}
                variant={TextButtonVariant.Four}
                displayIcon
                Icon={MapIcon}
                text="show on maps"
                onPress={null}
              />
            </View>

            <Image
              source={tempMap}
              style={styles.mapImage}
              resizeMode="cover"
            />

            <View
              style={[
                styles.location,
                multiline ? styles.textAlignFlexStart : styles.textAlignCenter,
              ]}>
              <LocationIcon
                width={24}
                height={24}
                fill={Colors.sparrowDarkBrown}
              />
              <Text
                style={[globalStyles.bodyTextOne, globalStyles.textDark]}
                onTextLayout={handleTextLayout}>
                {eventLocation}
              </Text>
            </View>
          </View>

          {/* ABOUT SECTION */}
          <View style={[styles.aboutSection, styles.basePadding]}>
            <Text style={[globalStyles.headingTextFour, globalStyles.textDark]}>
              About
            </Text>
            <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
              {eventDescription}
            </Text>
          </View>

          {/* ATTENDEES SECTION */}
          <View style={styles.attendeesSection}>
            <View style={[styles.attendeesInnerSection, styles.basePadding]}>
              <Text
                style={[globalStyles.headingTextFour, globalStyles.textDark]}>
                Attendees
              </Text>
              {userType === 'Host' ? (
                <TextButton
                  type={TextButtonType.Dark}
                  variant={TextButtonVariant.Four}
                  displayIcon
                  Icon={PersonIcon}
                  text="manage"
                  onPress={manageAttendees}
                />
              ) : (
                <TextButton
                  type={TextButtonType.Dark}
                  variant={TextButtonVariant.Four}
                  displayIcon
                  Icon={ShareIcon}
                  text="invite friends"
                  onPress={inviteFriends}
                />
              )}
            </View>
            <AvatarStackScroll
              avatars={avatarData}
              type={AvatarType.Beside}
              size={AvatarSize.Large}
              onPress={null}
            />
          </View>

          {/* GALLERY SECTION */}
          {eventStatus === 'Terminated' && pastEventData?.media.length > 0 && (
            <View
              style={[
                eventStatus === 'Terminated'
                  ? styles.basePadding
                  : styles.gallerySection,
                styles.basePadding,
              ]}>
              <Gallery images={pastEventData?.media ? [pastEventData] : []} />
            </View>
          )}

          {/* CONTROLS SECTION */}
          {/* HOST */}
          {eventStatus === 'Terminated' ? null : (
            <>
              {userType === 'Host' && (
                <View style={[styles.controlsSection, styles.basePadding]}>
                  <Button
                    type={ButtonType.Error}
                    size={ButtonSize.Medium}
                    display={ButtonDisplay.Full}
                    text={'Terminate event'}
                    onPress={terminateEvent}
                  />

                  <SmallMessage
                    type={SmallMessageType.Info}
                    message={
                      <>
                        Terminating an event restricts access to create a new
                        event for
                        <Text style={globalStyles.bodyTextTwoBold}>
                          {' '}
                          15 minutes
                        </Text>
                        .
                      </>
                    }
                  />
                </View>
              )}
            </>
          )}
        </View>

        {
          userType === 'Attendee' ? (
            <View
              style={{
                paddingBottom: Spacing.lg * 2 + attendeeControlsButtonHeight,
              }}
            />
          ) : (
            <View
              style={{
                paddingBottom: Spacing.lg,
              }}
            />
          )
        }
      </ScrollView >
    </View >
  );
};

export default EventScreen;

const styles = StyleSheet.create({
  container: {
    paddingVertical: Spacing.lg,
  },

  basePadding: {
    paddingHorizontal: Spacing.lg,
  },

  hostSectionWrapper: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingBottom: Spacing.md,
  },

  hostSection: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.mdsm,
  },

  hostNameType: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.xs,
  },

  bannerImage: {
    width: '100%',

    // TODO height could be bigger, calculated based on the screen size
    height: 160,

    borderRadius: borderRadius.md,
    borderWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
  },

  title: {
    paddingTop: Spacing.md,
    paddingBottom: Spacing.lg,
  },

  dateTimeSection: {
    rowGap: Spacing.sm,
  },

  dateTimeInnerSection: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.md,
    paddingBottom: Spacing.md,
  },

  dateTime: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.sm,
    flex: 1,
  },

  locationSection: {
    rowGap: Spacing.sm,
    paddingBottom: Spacing.lg,
  },

  locationInnerSection: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
  },

  // TODO adjust for mapbox
  mapImage: {
    width: '100%',
    height: 160,
    borderRadius: borderRadius.md,
    borderWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
  },

  location: {
    flexDirection: 'row',
    columnGap: Spacing.sm,
  },

  textAlignCenter: {
    alignItems: 'center',
  },

  textAlignFlexStart: {
    alignItems: 'flex-start',
  },

  aboutSection: {
    rowGap: Spacing.sm,
    paddingBottom: Spacing.lg,
  },

  attendeesSection: {
    rowGap: Spacing.sm,
    paddingBottom: Spacing.lg,
  },

  attendeesInnerSection: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
  },

  gallerySection: {
    paddingBottom: Spacing.xl,
  },

  controlsSection: {
    rowGap: Spacing.md,
  },

  attendeeControls: {
    backgroundColor: Colors.sparrowSand,
    padding: Spacing.lg,
    borderTopColor: Colors.sparrowDarkBrown,
    borderTopWidth: 2,

    position: 'absolute',
    bottom: 0,
    flex: 1,
    zIndex: 2,
    width: '100%',
  },
});
