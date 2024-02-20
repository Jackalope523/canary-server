import * as React from 'react';
import { View, Text, StyleSheet } from 'react-native';
import { StackScreenProps } from '@react-navigation/stack';
import { BottomTabParamList } from '../../components/atoms/types';
import { getAccount, userShard } from '../auth/accountPigeon';
import Button, {
  ButtonDisplay,
  ButtonSize,
  ButtonType,
} from '../../components/Button';

import { globalStyles } from '../../styles/GlobalStyles';
import Avatar, { AvatarSize, AvatarStatus } from '../../components/Avatar';
import { SAMPLE_USER_DATA } from '../../data/sampleUserData';
import { Colors } from '../../styles/ColorStyles';
import { Spacing } from '../../styles/SpacingStyles';
import TextLabel, {
  LabelDisplay,
  LabelSize,
  LabelType,
} from '../../components/TextLabel';
import { labelText } from '../../components/LabelText';
import PreviouslyAttendedEvent from '../../components/otherUserProfile/previouslyAttendedEvent';

import { SAMPLEEVENTDATA } from '../../data/sampleUpcomingEventData';

// Icons
import AddIcon from '../../assets/icons/add-outline.svg';
import { EventStatus } from '../../components/EventCardSmall';
import { SAMPLE_PAST_EVENT_DATA } from '../../data/samplePastEventData';

type OtherUserProfileScreenProps = StackScreenProps<
  BottomTabParamList,
  'Profile'
>;

const OtherUserProfileScreen = ({
  navigation,
}: OtherUserProfileScreenProps) => {
  // Sample event data
  const upcomingEventData = SAMPLEEVENTDATA.find((event) => event.id === '2');
  const pastEventData = SAMPLE_PAST_EVENT_DATA.find(
    (event) => event.id === '3',
  );

  const [debugText, setDebugText] = React.useState('');

  function handleGetAccount() {
    if (debugText == '') return;

    getAccount()
      .then((data) => setDebugText(`Name: ${data.Name}`))
      .catch(() => setDebugText('Could not retrieve account info.'));
  }

  handleGetAccount();

  /*
  
    TODO hook up to real data
    user types: anon, friend
  
  */

  // TEMP. for testing purposes
  let friend = true;
  let status = AvatarStatus.Offline;

  const user = SAMPLE_USER_DATA.find((user) => user.id === '1');

  const testUserData = React.useEffect(() => {
    console.log('user', user);
  }, [user]);

  return (
    <View style={globalStyles.baseContainer}>
      <View style={styles.topContainer}>
        <Avatar size={AvatarSize.Large} status={status} image={user?.avatar} />
        <View style={styles.userInfo}>
          <View style={styles.userInfoInner}>
            <Text style={[globalStyles.headingTextTwo, globalStyles.textDark]}>
              {user?.name}
            </Text>

            {friend ? (
              <TextLabel
                text={labelText.friend}
                type={LabelType.Friend}
                size={LabelSize.Small}
                display={LabelDisplay.Contained}
              />
            ) : null}
          </View>
          <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
            {user?.location}
          </Text>
        </View>
        <View>
          {friend ? (
            <Button
              type={ButtonType.PrimaryDark}
              size={ButtonSize.ExtraSmall}
              display={ButtonDisplay.Contained}
              text={'Invite to event'}
              displayIcon={true}
              Icon={AddIcon}
              onPress={null}
            />
          ) : (
            <Button
              type={ButtonType.PrimaryDark}
              size={ButtonSize.ExtraSmall}
              display={ButtonDisplay.Contained}
              text={'Add friend'}
              displayIcon={true}
              Icon={AddIcon}
              onPress={null}
            />
          )}
        </View>
      </View>

      <Text
        style={[globalStyles.bodyTextOne, globalStyles.textDark, styles.bio]}>
        {user?.bio}
      </Text>

      {/* LABELS HERE */}
      <View style={styles.labelContainer}>
        <TextLabel
          text={labelText.userSince}
          type={LabelType.Primary}
          size={LabelSize.Small}
          display={LabelDisplay.Contained}
        />

        <TextLabel
          text={labelText.lastSeen}
          type={LabelType.Primary}
          size={LabelSize.Small}
          display={LabelDisplay.Contained}
        />
      </View>

      {/* EVENTS */}
      <View style={styles.events}>
        <Text style={[globalStyles.headingTextTwo, globalStyles.textDark]}>
          Events
        </Text>
        <View style={styles.eventsContainer}>
          {/* inner wrapper start */}
          <View style={styles.eventsInnerWrapper}>
            <Text style={[globalStyles.headingTextFour, globalStyles.textDark]}>
              Attended
            </Text>
            <View
              style={[
                styles.eventsInnerContainer,
                styles.eventsContainerAttended,
              ]}>
              <Text
                style={[globalStyles.displayTextTwo, globalStyles.textLight]}>
                {user?.eventsAttended}
              </Text>
            </View>
          </View>

          <View style={styles.eventsInnerWrapper}>
            <Text style={[globalStyles.headingTextFour, globalStyles.textDark]}>
              Hosted
            </Text>
            <View
              style={[
                styles.eventsInnerContainer,
                styles.eventsContainerHosted,
              ]}>
              <Text
                style={[globalStyles.displayTextTwo, globalStyles.textLight]}>
                {user?.eventsHosted}
              </Text>
            </View>
          </View>
          {/* inner wrapper end */}
        </View>

        {friend ? (
          <View>
            {/* Upcoming RSVP'd */}
            <View style={styles.upcomingEvents}>
              <Text
                style={[globalStyles.headingTextTwo, globalStyles.textDark]}>
                Upcoming RSVP'd
              </Text>
              {/* <PreviouslyAttendedEvent /> */}
            </View>
            <View style={styles.pastEvents}>
              <Text
                style={[globalStyles.headingTextTwo, globalStyles.textDark]}>
                Previously attended
              </Text>
              <PreviouslyAttendedEvent
                eventStatus={EventStatus.Past}
                eventHeroImage={pastEventData?.media[0]}
                eventTitle={pastEventData?.title}
                eventDate={pastEventData?.time}
                eventLocation={pastEventData?.location}
                eventAttendees={pastEventData?.attendees}
                onPress={() => console.log('Event card image pressed')}
                /*
                
                TODO fix possible bug when using pastEventData?.media -
                error cannot read property 'slice' of undefined

                current fix below needs testing

                TODO if Gallery has 1 image:
                1. hide view more button
                2. set numColumns to 1
                3. hide change layout button

                TODO if Gallery has 0 images:
                1. hide Gallery component

                */
                images={pastEventData?.media ? [pastEventData] : []}
              />
            </View>
          </View>
        ) : null}
      </View>
    </View>
  );
};

export default OtherUserProfileScreen;

const styles = StyleSheet.create({
  topContainer: {
    alignItems: 'center',
    paddingVertical: Spacing.lg,
  },

  userInfo: {
    alignItems: 'center',
    paddingTop: Spacing.md,
    paddingBottom: Spacing.lg,
  },

  userInfoInner: {
    flexDirection: 'row',
    columnGap: Spacing.mdsm,
    justifyContent: 'center',
    alignItems: 'center',
  },

  bio: {
    marginBottom: Spacing.md,
  },

  // Labels
  labelContainer: {
    flexDirection: 'row',
    columnGap: Spacing.mdsm,
  },

  // Events
  events: {
    marginTop: Spacing.xl,
  },

  eventsContainer: {
    flexDirection: 'row',
    columnGap: Spacing.md,
  },

  eventsInnerWrapper: {
    flex: 1,
  },

  eventsInnerContainer: {
    paddingVertical: Spacing.lg,
    paddingHorizontal: Spacing.lg,
    borderRadius: 8,
    borderWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
    alignItems: 'center',
  },

  eventsContainerAttended: {
    backgroundColor: Colors.green400,
  },

  eventsContainerHosted: {
    backgroundColor: Colors.picton400,
  },

  upcomingEvents: {
    paddingTop: Spacing.xl,
    rowGap: Spacing.md,
  },

  pastEvents: {
    paddingTop: Spacing.xl,
    rowGap: Spacing.md,
  },
});
