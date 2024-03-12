import * as React from 'react';
import {
  View,
  Text,
  TextInput,
  StyleSheet,
  Image,
  ScrollView,
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
import Gallery from '../../components/Gallery';
import { SAMPLE_PAST_EVENT_DATA } from '../../data/samplePastEventData';
import Button, {
  ButtonDisplay,
  ButtonSize,
  ButtonType,
} from '../../components/Button';

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

const EventScreen = ({ route }: EventProps) => {
  const pastEventData = SAMPLE_PAST_EVENT_DATA.find(
    (event) => event.id === '3',
  );

  // TODO fix TS error in the code below

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

  return (
    <ScrollView
      contentContainerStyle={globalStyles.baseContainer}
      overScrollMode="never"
      showsVerticalScrollIndicator={false}>
      {/* <Text style={{ color: Colors.red400 }}>{errorText}</Text>
      <Text>{eventText}</Text> */}

      {/* TODO add FlagLarge component here */}
      <Text>FlagLarge component here after bugfix</Text>

      {/* HOST SECTION  */}
      <View style={styles.hostSectionWrapper}>
        <View style={styles.hostSection}>
          <Avatar
            image={tempAvatar}
            size={AvatarSize.Medium}
            status={AvatarStatus.Online}
          />
          <View>
            <Text style={[globalStyles.headingTextFour, globalStyles.textDark]}>
              Host's name
            </Text>
            <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
              Rating: 0.0
            </Text>
          </View>
        </View>

        <TextLabel
          text={labelText.you}
          type={LabelType.You}
          size={LabelSize.Small}
          display={LabelDisplay.Contained}
        />
      </View>

      {/* EVENT INFO SECTION */}
      <View>
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
          Event title
        </Text>

        {/* DATE AND TIME SECTION */}
        <View style={styles.dateTimeSection}>
          <Text style={[globalStyles.headingTextFour, globalStyles.textDark]}>
            Date and time
          </Text>
          {/* inner */}
          <View style={styles.dateTimeInnerSection}>
            {/* date */}
            <View style={styles.dateTime}>
              <DateIcon width={24} height={24} fill={Colors.sparrowDarkBrown} />
              <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
                Date
              </Text>
            </View>

            {/* time */}
            <View style={styles.dateTime}>
              <TimeIcon width={24} height={24} fill={Colors.sparrowDarkBrown} />
              <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
                Time
              </Text>
            </View>
          </View>
        </View>

        {/* LOCATION SECTION */}
        <View style={styles.locationSection}>
          {/* inner */}
          <View style={styles.locationInnerSection}>
            <Text style={[globalStyles.headingTextFour, globalStyles.textDark]}>
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

          <Image source={tempMap} style={styles.mapImage} resizeMode="cover" />

          {/* TODO check how it looks with more than 1 line, might need to adjust stlye; when more than 2 lines don't use alignItems center */}
          <View style={styles.location}>
            <LocationIcon
              width={24}
              height={24}
              fill={Colors.sparrowDarkBrown}
            />
            <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
              Location
            </Text>
          </View>
        </View>

        {/* ABOUT SECTION */}
        <View style={styles.aboutSection}>
          <Text style={[globalStyles.headingTextFour, globalStyles.textDark]}>
            About
          </Text>
          <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
            Description
          </Text>
        </View>

        {/* ATTENDEES SECTION */}
        {/* TODO add conditional logic - different TextButtons */}
        <View style={styles.attendeesSection}>
          <View style={styles.attendeesInnerSection}>
            <Text style={[globalStyles.headingTextFour, globalStyles.textDark]}>
              Attendees
            </Text>
            <TextButton
              type={TextButtonType.Dark}
              variant={TextButtonVariant.Four}
              displayIcon
              Icon={PersonIcon}
              text="manage"
              onPress={null}
            />
          </View>
          <Text>AvatarStack component here</Text>
        </View>

        {/* GALLERY SECTION */}
        {/* TODO add conditional logic - gallery */}
        <View style={styles.gallerySection}>
          {pastEventData?.media.length > 0 && (
            <Gallery images={pastEventData?.media ? [pastEventData] : []} />
          )}
        </View>

        {/* CONTROLS SECTION */}
        <View style={styles.controlsSection}>
          <Button
            type={ButtonType.Error}
            size={ButtonSize.Medium}
            display={ButtonDisplay.Full}
            text={'Continue'}
            onPress={null}
          />

          <Text>SmallMessage component here</Text>
        </View>
      </View>
    </ScrollView>
  );
};

export default EventScreen;

const styles = StyleSheet.create({
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

  eventSection: {},

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
    alignItems: 'center',
    columnGap: Spacing.sm,
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
});
