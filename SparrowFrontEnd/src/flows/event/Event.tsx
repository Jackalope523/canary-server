import * as React from 'react';
import { View, Text, TextInput, StyleSheet, Image } from 'react-native';
import { StackScreenProps } from '@react-navigation/stack';
import { Colors } from '../../styles/ColorStyles';
import { globalStyles } from '../../styles/GlobalStyles';
import { EventStackParamList } from '../../components/atoms/types';
import Button from '../../components/atoms/Button';
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

type EventProps = StackScreenProps<EventStackParamList, 'Event'>;

// TEMP. images
import tempAvatar from '../../assets/images/temp/host-img-1.jpg';
import tempBanner from '../../assets/images/temp/event-img-1.jpg';

// Icons
import DateIcon from '../../assets/icons/date-outline.svg';
import TimeIcon from '../../assets/icons/time-outline.svg';

const EventScreen = ({ route }: EventProps) => {
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
    <View style={globalStyles.baseContainer}>
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
      </View>
    </View>
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

  // TODO column-based layout here (grid)
  dateTimeInnerSection: {
    flexDirection: 'row',
    alignItems: 'center',
  },

  dateTime: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.sm,
  },
});
