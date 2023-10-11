import { StyleSheet, Text, View, Dimensions } from 'react-native'
import React from 'react'
import { Colors } from '../../styles/Colors'
import Animated, {useAnimatedStyle} from 'react-native-reanimated';


// TODO replace -40 with real padding size or make the responsiveness better
const screenWidth = Dimensions.get('screen').width - 40;

const RangeSelector = ({min, max, steps, onValueChange}) => {
  
  const styleLine = useAnimatedStyle(() => {
    return {
      backgroundColor: Colors.orange500,
      height: 3,
      marginTop: -3,
      borderRadius: 3,
      width: 100,
      transform: [{ translateX: 0 }],
    }
  })

  return (
    <View>
      <View style={styles.rangeWrapper}>
        <View style={styles.labelsWrapper}>
          <Text style={styles.label}>{min}</Text>
          <Text style={styles.label}>{max}</Text>
        </View>
        <View style={styles.track} />
        <Animated.View style={styleLine} />
      </View>
    </View>
  )
}

export default RangeSelector

const styles = StyleSheet.create({
  // From tutorial - change when functional
  rangeWrapper: {
    backgroundColor: Colors.fuchsia200,

    padding: 20,
  },

  labelsWrapper: {
    width: screenWidth,
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: 18,
  },

  label: {
    color: Colors.sparrowDark,
    fontSize: 16,
  },
  
  track: {
    height: 3,
    backgroundColor: Colors.fuchsia700,
    borderRadius: 3,
  },
});