import { View, Text, Image, StyleSheet, Dimensions } from 'react-native';
import * as React from 'react';
import { globalStyles } from '../../../styles/Global';
import { Spacing } from '../../../styles/Spacing';
import { CustomDimensions } from '../../../styles/CustomDimensions';

// TODO rename this file later when you come up with a better name

// Screen dimensions
const screenHeight = Dimensions.get('screen').height;

// Navigation dimensions
const navHeight = CustomDimensions.navigationHeight;

const NoNotifications = () => {
  return (
    <View style={[styles.container, globalStyles.baseContainer]}>
      <Image
        source={require('../../assets/illustrations/temp/not-found.png')}
        style={globalStyles.illustrationLarge}
      />
      <View style={styles.textWrapper}>
        <Text
          style={[
            globalStyles.headingTextThree,
            globalStyles.textDark,
            styles.textAlign,
          ]}>
          You don't have any notifications yet.
        </Text>
        <Text
          style={[
            globalStyles.bodyTextOne,
            globalStyles.textDark,
            styles.textAlign,
          ]}>
          We'll notify you when you get invited to events and during other
          occurances.
        </Text>
      </View>
    </View>
  );
};

export default NoNotifications;

const styles = StyleSheet.create({
  container: {
    height: screenHeight - navHeight,
    alignItems: 'center',
    justifyContent: 'center',
  },

  textAlign: {
    textAlign: 'center',
  },

  textWrapper: {
    marginTop: Spacing.xl,
    rowGap: Spacing.sm,
  },
});
