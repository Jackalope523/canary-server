import { StyleSheet, Text, View, Dimensions, TextInput } from 'react-native'
import React from 'react'
import { Colors } from '../../styles/Colors'
import Animated, {useAnimatedGestureHandler, useAnimatedStyle, useSharedValue, useAnimatedProps, runOnJS} from 'react-native-reanimated';
import { GestureHandlerRootView, PanGestureHandler } from 'react-native-gesture-handler';


const AnimatedTextInput = Animated.createAnimatedComponent(TextInput);

// TODO replace -40 with real padding size or make the responsiveness better
const screenWidth = Dimensions.get('screen').width - 40;
const knobSize = 20;
const maxWidth = screenWidth - knobSize / 2 + 6

const RangeSelector = ({min, max, steps, onValueChange}) => {
  
  const xKnob1 = useSharedValue(0);
  const scaleKnob1 = useSharedValue(1);

  const xKnob2 = useSharedValue(maxWidth);
  const scaleKnob2 = useSharedValue(1);
  
  const gestureHandler1 = useAnimatedGestureHandler ({
    onStart: (_, ctx) => {
      ctx.startX = xKnob1.value;
    },
    onActive: (event, ctx) => {
      scaleKnob1.value = 1.3;
      xKnob1.value = ctx.startX + event.translationX < 0 ? 0 : ctx.startX + event.translationX > maxWidth ? maxWidth : ctx.startX + event.translationX;
    },
    onEnd: () => {
      scaleKnob1.value = 1;
      runOnJS(onValueChange)({
        min: `${Math.round((min + (xKnob1.value / maxWidth) * (max - min)) / steps ) * steps }`,
        max: `${Math.round((min + (xKnob2.value / maxWidth) * (max - min)) / steps ) * steps }`,
      });
    },
  });

  const gestureHandler2 = useAnimatedGestureHandler ({
    onStart: (_, ctx) => {
      ctx.startX = xKnob2.value;
    },
    onActive: (event, ctx) => {
      scaleKnob2.value = 1.3;
      xKnob2.value = ctx.startX + event.translationX < xKnob1.value ? xKnob1.value : ctx.startX + event.translationX > maxWidth ? maxWidth : ctx.startX + event.translationX;
    },
    onEnd: () => {
      scaleKnob2.value = 1;
      runOnJS(onValueChange)({
        min: `${Math.round((min + (xKnob1.value / maxWidth) * (max - min)) / steps ) * steps }`,
        max: `${Math.round((min + (xKnob2.value / maxWidth) * (max - min)) / steps ) * steps }`,
      });
    },
  });

  const styleLine = useAnimatedStyle(() => {
    return {
      backgroundColor: Colors.orange500,
      height: 3,
      marginTop: -3,
      borderRadius: 3,
      width: xKnob2.value - xKnob1.value,
      transform: [{ translateX: xKnob1.value }],
    }
  });

  const styleKnob1 = useAnimatedStyle(() => {
    return {
      transform: [
        {
          translateX: xKnob1.value,
        },
        {
          scale: scaleKnob1.value,
        },
      ]
    }
  });

  const styleKnob2 = useAnimatedStyle(() => {
    return {
      transform: [
        {
          translateX: xKnob2.value,
        },
        {
          scale: scaleKnob2.value,
        },
      ]
    }
  });

  const propsLabel1 = useAnimatedProps(() => {
    return {
      text: `${Math.round((min + (xKnob1.value / maxWidth) * (max - min)) / steps ) * steps }`,
    }
  });

  const propsLabel2 = useAnimatedProps(() => {
    return {
      text: `${Math.round((min + (xKnob2.value / maxWidth) * (max - min)) / steps ) * steps }`,
    }
  });

  return (
    <View>
      <View style={styles.rangeWrapper}>
        <View style={styles.labelsWrapper}>
          <AnimatedTextInput defaultValue={'0'} editable={false} style={styles.label} animatedProps={propsLabel1} />
          <AnimatedTextInput defaultValue={'0'} editable={false} style={styles.label} animatedProps={propsLabel2} />
        </View>
        <View style={styles.track} />
        <Animated.View style={styleLine} />
        <View>

          {/* TODO make flex: 1 style work for gesturehandlerrootview */}
          <GestureHandlerRootView>
            <PanGestureHandler onGestureEvent={gestureHandler1}>
              <Animated.View style={[styles.knob, styleKnob1]} />
            </PanGestureHandler>
            <PanGestureHandler onGestureEvent={gestureHandler2}>
              <Animated.View style={[styles.knob, styleKnob2]} />
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