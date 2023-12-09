import {
  StyleSheet,
  Text,
  View,
  Dimensions,
  TouchableOpacity,
} from 'react-native';
import React, { useState } from 'react';
import { Colors } from '../../styles/Colors';
import Slider from '@react-native-community/slider';

type Props = {};

// Dimensions
const SCREENWIDTH = Dimensions.get('window').width;
const THUMBSIZE = 18;
const MAXWIDTH = SCREENWIDTH - THUMBSIZE / 2 + 6;
const SLIDERWIDTH = SCREENWIDTH - 24 * 4;

const SingleValueSelector = (props: Props) => {
  return (
    <View style={styles.container}>
      <Text>SVS1 using react-native-community/slider</Text>
      <Slider
        style={{ width: SLIDERWIDTH }}
        minimumValue={1}
        maximumValue={25}
        step={1}
        minimumTrackTintColor={Colors.orange500}
        maximumTrackTintColor={Colors.orange500}
        thumbTintColor={Colors.sparrowDarkBrown}
      />
    </View>
  );
};

export default SingleValueSelector;

const styles = StyleSheet.create({
  container: {
    marginHorizontal: 24,
    justifyContent: 'center',
    alignSelf: 'center',
  },

  sliderBack: {
    height: 8,
    backgroundColor: Colors.orange700,
    borderRadius: 8,
  },

  sliderFront: {
    height: 8,
    backgroundColor: Colors.orange300,
    borderRadius: 8,
    position: 'absolute',
  },

  thumb: {
    position: 'absolute',
    left: -10,
    width: THUMBSIZE,
    height: THUMBSIZE,
    backgroundColor: Colors.green300,
    borderColor: Colors.sparrowDarkBrown,
    borderWidth: 4,
    borderRadius: THUMBSIZE,
  },

  label: {
    position: 'absolute',
    top: -40,
    bottom: 20,
    backgroundColor: Colors.sparrowDark,
    borderRadius: 4,
    alignSelf: 'center',
    justifyContent: 'center',
    alignItems: 'center',
  },

  labelText: {
    color: Colors.sparrowSand,
    padding: 4,
    fontWeight: 'bold',
    width: '100%',
    fontSize: 16,
  },
});
