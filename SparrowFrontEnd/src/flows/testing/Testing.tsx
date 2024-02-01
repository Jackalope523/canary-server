import { StyleSheet, Text, View } from 'react-native';
import React from 'react';
import { globalStyles } from '../../styles/GlobalStyles';
import { Spacing } from '../../styles/SpacingStyles';

import HeaderDefaultTitled from '../../components/headers/HeaderDefaultTitled';
import HeaderFlagAttendee from '../../components/headers/HeaderFlagAttendee';
import Mask1 from '../../components/testing/animations/mask1';
import { Colors } from '../../styles/ColorStyles';
import Mask2 from '../../components/testing/animations/mask2';
import Mask3 from '../../components/testing/animations/mask3';
import CircleGrowth from '../../components/testing/animations/circleGrowth';
import Mask4 from '../../components/testing/animations/mask4';
import Header4 from '../../components/testing/animations/header4';
import Header5 from '../../components/testing/animations/header5';
import Button2, {
  ButtonDisplay,
  ButtonSize,
  ButtonType,
} from '../../components/testing/animations/Button2';

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
      {/* <HeaderFlagAttendee /> */}
      <Header4 />
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
