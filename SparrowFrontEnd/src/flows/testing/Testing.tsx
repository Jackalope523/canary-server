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

// Icon
import ImportedIcon from '../../assets/icons/favorite-fill.svg';
import ButtonGroup from '../../components/ButtonGroup';
import FlagMedium, { FlagType } from '../../components/FlagMedium';
import Avatar, { AvatarSize, AvatarStatus } from '../../components/Avatar';

import TempAvatarImage from '../../assets/images/temp/image-placeholder.png';
import PhotoPost from '../../components/PhotoPost';
import DropdownSmall, { Align, Icon } from '../../components/DropdownSmall';
import dropdownOptionsPost from '../../components/DropdownOptionsPost';

const TestScreen = () => {
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

      <FlagMedium type={FlagType.StartingSoon} time={'02:23:12'} />

      {/* <Avatar
        size={AvatarSize.Large}
        status={AvatarStatus.Offline}
        image={TempAvatarImage}
      /> */}

      <View style={globalStyles.baseContainer}>
        <PhotoPost
          name="Gale"
          time="14h"
          title="Downhill MTB competition at Grey Feather Mountain"
          attendees={['Beatrice, ', 'John']}
          leftoverAttendeeCount={4}
          location="Venice Beach, Venice, CA"
          likeCount={3}
        />
      </View>

      {/* <Button
        type={ButtonType.Warning}
        size={ButtonSize.Medium}
        display={ButtonDisplay.Full}
        text="Example"
        Icon={ImportedIcon}
        displayIcon={true}
      /> */}

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
