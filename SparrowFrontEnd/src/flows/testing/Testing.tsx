import { StyleSheet, Text, View } from 'react-native';
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

const TestScreen = () => {
  return (
    <View style={styles.container}>
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

      <HeaderOptions title="Options" />
      <HeaderEditTitled title="Selected titled" />
      <HeaderDefaultTitled title="Testing" />
      <HeaderFlagAttendee
        title="Attendee header"
        previousType={APType.StartingSoon}
        nextType={ANType.Live}
      />
      <HeaderFlagHost
        title="Host header"
        previousType={HPType.StartingSoon}
        nextType={HNType.Live}
      />

      {/* <Header4 previousType={PType.StartingSoon} nextType={NType.Live} /> */}
      <Button2
        text="Example button"
        type={ButtonType.Success}
        size={ButtonSize.Medium}
        display={ButtonDisplay.Contained}
      />
    </View>
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
