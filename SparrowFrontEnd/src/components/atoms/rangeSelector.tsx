import { StyleSheet, Text, View, Dimensions } from 'react-native'
import React from 'react'
import { Colors } from '../../styles/Colors'
import Animated, {useAnimatedGestureHandler, useAnimatedStyle, useSharedValue} from 'react-native-reanimated';
import { GestureHandlerRootView, PanGestureHandler } from 'react-native-gesture-handler';


// TODO replace -40 with real padding size or make the responsiveness better
const screenWidth = Dimensions.get('screen').width - 40;
const knobSize = 20;

const RangeSelector = ({min, max, steps, onValueChange}) => {
  
  const xKnob1 = useSharedValue(0);
  
  const gestureHandler1 = useAnimatedGestureHandler ({
    onStart: (_, ctx) => {},
    onActive: (event, ctx) => {
      xKnob1.value = event.translationX;
    },
    onEnd: () => {},
  });

  const styleLine = useAnimatedStyle(() => {
    return {
      backgroundColor: Colors.orange500,
      height: 3,
      marginTop: -3,
      borderRadius: 3,
      width: 100,
      transform: [{ translateX: 0 }],
    }
  });

  const styleKnob1 = useAnimatedStyle(() => {
    return {
      transform: [
        {
          translateX: xKnob1.value,
        }
      ]
    }
  });

  return (
    <View>
      <View style={styles.rangeWrapper}>
        <View style={styles.labelsWrapper}>
          <Text style={styles.label}>{min}</Text>
          <Text style={styles.label}>{max}</Text>
        </View>
        <View style={styles.track} />
        <Animated.View style={styleLine} />
        <View>
          <GestureHandlerRootView>
            <PanGestureHandler onGestureEvent={gestureHandler1}>
              <Animated.View style={[styles.knob, styleKnob1]} />
            </PanGestureHandler>
          </GestureHandlerRootView>
        </View>
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

  knob: {
    position: 'absolute',
    height: knobSize,
    width: knobSize,
    borderColor: Colors.turqoise800,
    borderWidth: 2,
    borderRadius: knobSize / 2,
    backgroundColor: Colors.turqoise500,
    marginTop: -knobSize + 8,
    marginLeft: -8,
  },
});