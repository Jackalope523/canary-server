import { StyleSheet, Text, View } from 'react-native';
import React from 'react';
import { globalStyles } from '../../styles/GlobalStyles';
import { Spacing } from '../../styles/SpacingStyles';

import HeaderDefaultTitled from '../../components/headers/HeaderDefaultTitled';
import HeaderFlagAttendee from '../../components/headers/HeaderFlagAttendee';
import Mask1 from '../../components/testing/masking/mask1';
import { Colors } from '../../styles/ColorStyles';
import Mask2 from '../../components/testing/masking/mask2';

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
      {/* <HeaderDefaultTitled title="Titled" /> */}
      <HeaderFlagAttendee />
      {/* <Mask1 /> */}
      <Mask2 />
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
