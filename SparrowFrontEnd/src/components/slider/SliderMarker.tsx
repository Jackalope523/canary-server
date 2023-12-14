import { StyleSheet, Text, View } from 'react-native';
import React from 'react';
import { Colors } from '../../styles/Colors';
import { Spacing } from '../../styles/SpacingStyles';
import { globalStyles } from '../../styles/GlobalStyles';

type Props = {};

// TODO Pass selected value from SingleValueSlider (marker) to SliderMarker in a <Text> element
const markerValue = '>1km';
const thumbSize = 18;

const SliderMarker = (props: Props) => {
  return (
    <View style={styles.container}>
      <Text style={[globalStyles.labelTextAsTyped, globalStyles.textLight]}>
        {markerValue}
      </Text>
      <View style={styles.thumb} />
    </View>
  );
};

export default SliderMarker;

const styles = StyleSheet.create({
  container: {
    backgroundColor: Colors.orange300,
    alignItems: 'center',
    bottom: 10,
  },

  thumb: {
    height: thumbSize,
    width: thumbSize,
    borderRadius: thumbSize / 2,
    backgroundColor: Colors.picton500,
  },
});
