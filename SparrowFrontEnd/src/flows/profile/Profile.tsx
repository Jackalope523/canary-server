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
import BirdIcon from '../../assets/icons/bird-fill-colored.svg';

import { globalStyles } from '../../styles/GlobalStyles';
import Avatar, { AvatarSize } from '../../components/Avatar';
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
import UpcomingEvent from '../../components/otherUserProfile/UpcomingEvent';
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

  // TEMP. for testing purposes
  const user = SAMPLE_USER_DATA.find((user) => user.id === '1');

  const testUserData = React.useEffect(() => {
    console.log('user', user);
  }, [user]);

  // Handle change layout for gallery
  const handleChangeLayout = () => {
    console.log('change layout button pressed');
  };

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
          <BirdIcon width={24} height={24} />
        </View>
      </View>

      {/* EVENTS */}
      <View style={styles.events}>
        <View style={styles.eventsHeadingContainer}>
          <Text style={[globalStyles.textDark, globalStyles.headingTextThree]}>
            Previously attended
          </Text>
          {/* TODO hook up change layout functionality */}
          {/* TODO implement a way to activate the changeLayout function in the Gallery component, through this parent component */}

          <Pressable onPress={handleChangeLayout}>
            <LayoutMediumIcon
              width={24}
              height={24}
              fill={Colors.sparrowDarkBrown}
            />
          </Pressable>
        </View>
        <FlatList
          data={SAMPLE_PAST_EVENT_DATA.filter((item) => item.status === 'passed')}
          renderItem={({ item }) => (
            <PreviouslyAttendedEvent
              eventTitle={item.title}
              eventDate={item.time}
              onPress={() => console.log('Event card image pressed')}
              onPressViewEvent={() => console.log('View event button pressed')}
              images={item.media ? [item] : []}

              // TODO show the avatar of the user who posted the photo; right now it's showing the host's avatar
              posterAvatar={item.avatar}
              // TODO replace item.host name with the name of the user who posted the photo
              // TODO fix poster images loading slowly
              posterName={item.host}
            />
          )}
          keyExtractor={(item) => item.id}
          ItemSeparatorComponent={() => <View style={{ height: Spacing.lg }} />}
        />
      </View>
    </ScrollView>
  );
};

export default ProfileScreen;

const styles = StyleSheet.create({
  topContainer: {
    alignItems: 'center',
    paddingBottom: Spacing.xl,
    paddingTop: Spacing.sm,
  },

  user: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingTop: Spacing.mdsm,
    columnGap: Spacing.xs,
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
