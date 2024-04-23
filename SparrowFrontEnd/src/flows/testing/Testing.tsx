import { FlatList, Pressable, ScrollView, StyleSheet, Text, View } from 'react-native';
import React from 'react';
import { globalStyles } from '../../styles/GlobalStyles';
import { Spacing } from '../../styles/SpacingStyles';
import { SAMPLEEVENTDATA } from '../../data/sampleUpcomingEventData';
import ActionButton, {
  ActionButtonDisplay,
  ActionButtonSize,
  ActionButtonType,
} from '../../components/ActionButton';
import ArrowIcon from '../../assets/icons/arrow-up-outline-alt.svg';
import EventCardSmallAccordion from '../../components/EventCardSmallAccordion';
import ChevronButton from '../../components/ChevronButton';
import PreviouslyAttendedEvent from '../../components/otherUserProfile/PreviouslyAttendedEvent';
import { SAMPLE_PAST_EVENT_DATA } from '../../data/samplePastEventData';
import Gallery from '../../components/Gallery';
import ViewMoreButton from '../../components/ViewMoreButton';

const TestScreen = () => {
  const upcomingEventData = SAMPLEEVENTDATA.find((event) => event.id === '2');

  // Testing layout change for gallery
  const handleChangeLayout = () => {
    console.log('pressable pressed');
  };

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
      {/* <EventCardSmallAccordion /> */}

      {/* TODO implement a way to activate the changeLayout function in the Gallery component, through this parent component */}
      <Pressable onPress={handleChangeLayout}><Text style={[globalStyles.textDark, globalStyles.headingTextFive]}>Change layout</Text></Pressable>
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

      {/* <Gallery /> */}

      {/* <ActionButton
        Icon={ArrowIcon}
        display={ActionButtonDisplay.Contained}
        type={ActionButtonType.SecondaryLight}
        size={ActionButtonSize.Large}
        disabled={false}
      /> */}
    </ScrollView >
  );
};

export default TestScreen;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    gap: 16,

    padding: Spacing.lg,
  },

  header: {
    marginBottom: Spacing.lg,
  },
});
