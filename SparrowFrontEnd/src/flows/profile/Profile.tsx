import * as React from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  FlatList,
  Pressable,
} from 'react-native';
import { StackScreenProps } from '@react-navigation/stack';
import { BottomTabParamList } from '../../components/atoms/types';
import { getAccount, userShard } from '../auth/accountPigeon';
import Button, {
  ButtonDisplay,
  ButtonSize,
  ButtonType,
} from '../../components/Button';

// Icons
import AddIcon from '../../assets/icons/add-outline.svg';
import Chevron from '../../assets/icons/chevron-outline.svg';
import LayoutMediumIcon from '../../assets/icons/layout-size-medium-fill-alt.svg';

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

import { SAMPLEEVENTDATA } from '../../data/sampleUpcomingEventData';
import { SAMPLE_PAST_EVENT_DATA } from '../../data/samplePastEventData';
import UpcomingEvent from '../../components/otherUserProfile/upcomingEvent';
import { EventStatus } from '../../components/EventCardSmall';
import PreviouslyAttendedEvent from '../../components/otherUserProfile/PreviouslyAttendedEvent';

type ProfileProps = StackScreenProps<BottomTabParamList, 'Profile'>;

const ProfileScreen = ({ navigation }: ProfileProps) => {
  // Sample event data
  const upcomingEventData = SAMPLEEVENTDATA.find((event) => event.id === '2');
  const pastEventData = SAMPLE_PAST_EVENT_DATA.find(
    (event) => event.id === '3',
  );

  const [showAllItems, setShowAllItems] = React.useState(false);

  // View more
  const onViewMore = () => {
    console.log('View more button pressed');
    setShowAllItems(!showAllItems);
  };

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
  let friend = false;

  const user = SAMPLE_USER_DATA.find((user) => user.id === '1');

  const testUserData = React.useEffect(() => {
    console.log('user', user);
  }, [user]);

  return (
    <ScrollView
      contentContainerStyle={globalStyles.baseContainer}
      overScrollMode="never"
      showsVerticalScrollIndicator={false}>
      <View style={styles.topContainer}>
        <Avatar size={AvatarSize.Large} image={user?.avatar} />
        <View style={styles.user}>
          <Text style={[globalStyles.headingTextTwo, globalStyles.textDark]}>
            {user?.name}
          </Text>
          {/* TODO replace TextLabel YOU with an bird icon */}
          <TextLabel
            text={labelText.you}
            type={LabelType.You}
            size={LabelSize.Small}
            display={LabelDisplay.Contained}
          />
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

      {/* EVENTS */}
      {/* NEW EVENTS DESIGN */}
      <View style={styles.events}>
        <View style={styles.eventsHeadingContainer}>
          <Text style={[globalStyles.textDark, globalStyles.headingTextThree]}>
            Previously attended
          </Text>
          {/* TODO hook up change layout functionality */}
          <LayoutMediumIcon
            width={24}
            height={24}
            fill={Colors.sparrowDarkBrown}
          />
        </View>
        <FlatList
          data={SAMPLE_PAST_EVENT_DATA.filter(
            (item) => !item.time.includes('live'),
          )}
          renderItem={({ item }) => (
            <PreviouslyAttendedEvent
              eventStatus={EventStatus.Past}
              eventHeroImage={item.media[0]}
              eventTitle={item.title}
              eventDate={item.time}
              eventLocation={item.location}
              onPress={() => console.log('Event card image pressed')}
              images={item.media ? [item] : []}
            />
          )}
          keyExtractor={(item) => item.id}
          ItemSeparatorComponent={() => <View style={{ height: Spacing.lg }} />}
        />
      </View>

      {/* OLD EVENTS DESIGN */}
      {/* <View style={styles.events}>
        <Text style={[globalStyles.headingTextTwo, globalStyles.textDark]}>
          Events
        </Text>
        <View>
          <View style={styles.pastEvents}>
            <Text style={[globalStyles.headingTextTwo, globalStyles.textDark]}>
              Previously attended
            </Text>
            <FlatList
              data={SAMPLE_PAST_EVENT_DATA.filter(
                (item) => !item.time.includes('live'),
              )}
              renderItem={({ item }) => (
                <PreviouslyAttendedEvent
                  eventStatus={EventStatus.Past}
                  eventHeroImage={item.media[0]}
                  eventTitle={item.title}
                  eventDate={item.time}
                  eventLocation={item.location}
                  onPress={() => console.log('Event card image pressed')}
                  images={item.media ? [item] : []}
                />
              )}
              keyExtractor={(item) => item.id}
              ItemSeparatorComponent={() => (
                <View style={{ height: Spacing.lg }} />
              )}
            />
          </View>
        </View>
      </View> */}
    </ScrollView>
  );
};

export default ProfileScreen;

const styles = StyleSheet.create({
  // TODO delete viewMore styles after the ViewMoreButton component has been integrated
  viewMore: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingTop: Spacing.md,
  },

  topContainer: {
    alignItems: 'center',
    paddingVertical: Spacing.lg,
  },

  user: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingTop: Spacing.md,
    paddingBottom: Spacing.lg,
    columnGap: Spacing.sm,
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
    rowGap: Spacing.lg,
  },

  eventsHeadingContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
});
