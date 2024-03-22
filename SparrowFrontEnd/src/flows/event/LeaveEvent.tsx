import { Image, StyleSheet, Text, View } from 'react-native';
import React from 'react';
import { globalStyles } from '../../styles/GlobalStyles';
import Button, {
  ButtonDisplay,
  ButtonSize,
  ButtonType,
} from '../../components/Button';
import { Spacing } from '../../styles/SpacingStyles';

interface LeaveEventScreenProps {}

const LeaveEventScreen = (props: LeaveEventScreenProps) => {
  return (
    <View style={[globalStyles.baseContainer, styles.container]}>
      <View style={styles.info}>
        <Text
          style={[
            globalStyles.headingTextTwo,
            globalStyles.textDark,
            globalStyles.textAlignCenter,
          ]}>
          Are you sure you want to leave the event?
        </Text>
        <Image
          source={require('../../assets/illustrations/temp/illustration-placeholder.png')}
          style={globalStyles.illustrationLarge}
          resizeMode="contain"
        />
      </View>

      {/* TODO hook up onPress */}

      <View style={styles.controls}>
        <Button
          type={ButtonType.Success}
          size={ButtonSize.Medium}
          display={ButtonDisplay.Full}
          text={'Stay in event'}
          onPress={null}
        />
        <Button
          type={ButtonType.Error}
          size={ButtonSize.Medium}
          display={ButtonDisplay.Full}
          text={'Leave event'}
          onPress={null}
        />
      </View>
    </View>
  );
};

export default LeaveEventScreen;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'space-between',
  },

  info: {
    rowGap: Spacing.lg,
  },

  controls: {
    rowGap: Spacing.lg,
  },
});
