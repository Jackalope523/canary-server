import { StyleSheet, Text, View } from 'react-native';
import React from 'react';
import { Colors } from '../../styles/ColorStyles';
import { Spacing } from '../../styles/SpacingStyles';

type Props = {};

const SliderLabel = (props: Props) => {
  return (
    <View style={styles.container}>
      <Text>SliderLabel</Text>
    </View>
  );
};

export default SliderLabel;

const styles = StyleSheet.create({
  container: {
    backgroundColor: Colors.lavender400,
    marginBottom: Spacing.md,
  },
});
