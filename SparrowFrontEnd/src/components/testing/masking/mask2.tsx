import React, { useEffect } from 'react';
import { Dimensions, Pressable, Text, View } from 'react-native';
import MaskedView from '@react-native-masked-view/masked-view';
import { Colors } from '../../../styles/ColorStyles';
import Animated, {
  useAnimatedStyle,
  useSharedValue,
  withTiming,
} from 'react-native-reanimated';

const Mask2 = () => {
  let headerHeight = 48;

  let circleSize = useSharedValue(20);
  let topSpacing = useSharedValue(headerHeight);

  // let maxSize = Dimensions.get('window').width * 2.5;
  // let maxSize = 1000;

  React.useEffect(() => {
    circleSize.value = withTiming(1000, { duration: 1500 });
    console.log('trigger mask2 animation');
  }, []);

  const animatedStyles = useAnimatedStyle(() => {
    return {
      height: circleSize.value,
      width: circleSize.value,
      borderRadius: circleSize.value,

      // top: topSpacing.value,
    };
  });

  return (
    <View
      style={{
        // flex: 1,
        height: headerHeight,
        width: '100%',
      }}>
      <MaskedView
        style={{
          flex: 1,
          flexDirection: 'row',
          height: '100%',
          position: 'absolute',
          top: 0,
          bottom: 0,
          left: 0,
          right: 0,
        }}
        maskElement={
          <View
            style={{
              // Transparent background because mask is based off alpha channel.
              backgroundColor: 'transparent',
              flex: 1,
            }}>
            <Animated.View
              style={[
                animatedStyles,
                {
                  alignSelf: 'center',
                  backgroundColor: 'black',

                  // TODO FIX THIS
                  // Issue: when this is enabled, the animation isn't starting
                  // top: headerHeight,
                },
              ]}
            />
          </View>
        }>
        {/* Shows behind the mask, you can put anything here, such as an image */}
        <View
          style={{
            flex: 1,
            height: '100%',
            backgroundColor: Colors.green400,
          }}
        />
      </MaskedView>
      <View
        style={{
          flex: 1,
          height: '100%',
          backgroundColor: Colors.orange400,

          position: 'absolute',
          top: 0,
          bottom: 0,
          left: 0,
          right: 0,
          zIndex: -1,
        }}
      />
    </View>
  );
};

export default Mask2;
