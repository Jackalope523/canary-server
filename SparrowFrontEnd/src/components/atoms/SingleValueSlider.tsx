import { StyleSheet, Text, View } from 'react-native'
import React from 'react'

import MultiSlider from '@ptomasroos/react-native-multi-slider'
import SliderLabel from './SliderLabel';
import SliderMarker from './SliderMarker';

type Props = {}

const measurementImperial = 'mi';
const measurementMetric = 'km';

const SingleValueSlider = (props: Props) => {
  return (
    <View>
      <Text>SingleValueSlider powered by rnmss</Text>
      <MultiSlider
        min={1}
        max={50}
        sliderLength={300}
        enableLabel={true}
        valueSuffix='km'
        customLabel={SliderLabel}
        customMarker={SliderMarker}
      />
    </View>
  )
}

export default SingleValueSlider

const styles = StyleSheet.create({})