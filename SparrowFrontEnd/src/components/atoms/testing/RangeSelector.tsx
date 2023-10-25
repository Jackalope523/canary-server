import {StyleSheet, Text, View, Dimensions, TextInput} from 'react-native';
import * as React from 'react';
import Animated, {
  useAnimatedGestureHandler,
  useAnimatedStyle,
  useSharedValue,
  useAnimatedProps,
  runOnJS
} from 'react-native-reanimated';
import {Colors} from '../../../styles/Colors';
import {
  Gesture,
  GestureHandlerRootView,
  PanGestureHandler,
} from 'react-native-gesture-handler';

// Text input
const ATextInput = Animated.createAnimatedComponent(TextInput);

// Dimensions
const SCREENWIDTH = Dimensions.get('window').width;
const THUMBSIZE = 18;
const MAXWIDTH = SCREENWIDTH - THUMBSIZE / 2 + 6;
const SLIDERWIDTH = SCREENWIDTH - 24 * 4;

const RangeSelector = ({min, max, steps, onValueChange}) => {
  const position = useSharedValue(0);
  const position2 = useSharedValue(SLIDERWIDTH);

  const zIndex = useSharedValue(0);
  const zIndex2 = useSharedValue(0);

  const gestureHandler = useAnimatedGestureHandler({
    onStart: (_, ctx) => {
      ctx.startX = position.value;
    },

    // Fixes selector going outside the slider
    // TODO it can still go out from the right side - fix that
    onActive: (event, ctx) => {
      if (ctx.startX + event.translationX < 0) {
        position.value = 0;
      } else if (ctx.startX + event.translationX > position2.value) {
        position.value = position2.value;
        zIndex.value = 1;
        zIndex2.value = 0;
      } else {
        position.value = ctx.startX + event.translationX;
      }
    },

    onEnd: () => {
      runOnJS(onValueChange)({
        min:
          min +
          Math.floor(position.value / (SLIDERWIDTH / ((max - min) / steps))) *
            steps,
        max:
          min +
          Math.floor(position2.value / (SLIDERWIDTH / ((max - min) / steps))) *
            steps,
      });
    }
  });

  const gestureHandler2 = useAnimatedGestureHandler({
    onStart: (_, ctx) => {
      ctx.startX = position2.value;
    },

    // Fixes selector going outside the slider
    // TODO it can still go out from the right side - fix that
    onActive: (event, ctx) => {
      if (ctx.startX + event.translationX > SLIDERWIDTH) {
        position2.value = SLIDERWIDTH;
      } else if (ctx.startX + event.translationX < position2.value) {
        position2.value = position.value;
        zIndex.value = 0;
        zIndex.value = 1;
      } else {
        position2.value = ctx.startX + event.translationX;
      }
    },

    onEnd: () => {
      runOnJS(onValueChange)({
        min:
          min +
          Math.floor(position.value / (SLIDERWIDTH / ((max - min) / steps))) *
            steps,
        max:
          min +
          Math.floor(position2.value / (SLIDERWIDTH / ((max - min) / steps))) *
            steps,
      });
    }
  });

  const animatedStyle = useAnimatedStyle(() => ({
    transform: [{translateX: position.value}],
    zIndex: zIndex.value,
  }));

  const animatedStyle2 = useAnimatedStyle(() => ({
    transform: [{translateX: position2.value}],
    zIndex: zIndex2.value,
  }));

  const sliderStyle = useAnimatedStyle(() => ({
    transform: [{translateX: position.value}],
    width: position2.value - position.value,
  }));

  const AnimatedTextInput = Animated.createAnimatedComponent(TextInput);

  const minLabelText = useAnimatedProps(() => {
    return {
      text: `$${min + Math.floor(position.value / (SLIDERWIDTH / ((max - min) / steps))) * steps}`
    };
  });

  const maxLabelText = useAnimatedProps(() => {
    return {
      text: `$${min + Math.floor(position2.value / (SLIDERWIDTH / ((max - min) / steps))) * steps}`
    };
  });

  return (
    <GestureHandlerRootView style={[styles.container, {width: SLIDERWIDTH}]}>
      <View style={[styles.sliderBack]} />
      <Animated.View style={[styles.sliderFront, sliderStyle]} />
      <PanGestureHandler onGestureEvent={gestureHandler}>
        <Animated.View style={[styles.thumb, animatedStyle]}>
          <View style={styles.label}>
            <AnimatedTextInput
              style={styles.labelText}
              defaultValue='0'
              animatedProps={minLabelText}
              editable={false}
              />
          </View>
        </Animated.View>
      </PanGestureHandler>
      <PanGestureHandler onGestureEvent={gestureHandler2}>
      <Animated.View style={[styles.thumb, animatedStyle2]}>
          <View style={styles.label}>
            <AnimatedTextInput
              style={styles.labelText}
              defaultValue='0'
              animatedProps={maxLabelText}
              editable={false}
              />
          </View>
        </Animated.View>
      </PanGestureHandler>
    </GestureHandlerRootView>
  );
};

export default RangeSelector;

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

  // track: {
  //   height: 8,
  //   backgroundColor: Colors.orange700,
  //   borderRadius: 8,
  // },

  // labelsContainer: {
  //   width: SCREENWIDTH,
  //   flexDirection: 'row',
  //   justifyContent: 'space-between',
  //   marginBottom: 12,
  // },

  // label: {
  //   color: Colors.sparrowDark,
  // },

  // knob: {
  //   // NEW
  //   position: 'absolute',
  //   left: -10,
  //   width: THUMBSIZE,
  //   height: THUMBSIZE,
  //   backgroundColor: 'white',
  //   borderColor: Colors.green500,
  //   borderWidth: 5,
  //   borderRadius: THUMBSIZE,

  //   // OLD

  //   // position: 'absolute',
  //   // top: -THUMBSIZE / 1.3,
  //   // height: THUMBSIZE,
  //   // width: THUMBSIZE,
  //   // borderRadius: THUMBSIZE,
  //   // // borderTopRightRadius: THUMBSIZE / 2,
  //   // // borderBottomRightRadius: THUMBSIZE / 2,
  //   // // borderBottomLeftRadius: THUMBSIZE / 2,
  //   // backgroundColor: Colors.green500,
  //   // // marginTop: -(THUMBSIZE / 2 - 12),
  //   // marginLeft: -8,
  // },

  // currentValue: {
  //   textAlign: 'center',
  //   fontSize: 30,
  //   fontWeight: 'bold',
  //   color: Colors.green600,
  //   marginBottom: 30,
  // },
});
