import { Button, Dimensions, StyleSheet, Text, View } from 'react-native';
import React, { useEffect } from 'react';
import { Colors } from '../../../styles/ColorStyles';
import Animated, {
  runOnJS,
  useAnimatedStyle,
  useSharedValue,
  withSpring,
  withTiming,
} from 'react-native-reanimated';
import MaskedView from '@react-native-masked-view/masked-view';

/* --- DEV NOTES --- */
/*

  1. FIX: not animating - add androidRenderingMode="software" to MaskedView

*/

type Props = {
  buttonIndex: number;
  mode?: 'late' | 'soon' | 'live' | 'terminated';
};

const Header4: React.FC<Props> = ({ mode = 'soon' }) => {
  const scale = useSharedValue(20);
  const r = Dimensions.get('window').width / 2;
  const R = r * 2;
  const anchorPointX = 0;
  const anchorPointY = 0;

  const scale2 = useSharedValue(0);

  // TODO replace with actual header height
  const headerHeight = 55;
  // TODO modify to feature SQR2
  const endHeight = (R / 20) * 1.2;

  const handlePress = () => {
    runOnJS(() => {
      scale2.value = withTiming(endHeight, { duration: 1000 });
    })();
  };

  const animatedStyle = useAnimatedStyle(() => ({
    transform: [{ translateY: headerHeight }, { scale: scale2.value }],
  }));

  // const animatedStyle = useAnimatedStyle(() => ({
  //   height: scale.value,
  //   width: scale.value,
  //   borderRadius: scale.value,
  // }));

  // const flagColor = () => {
  //   if (mode === 'late') {
  //     return styles.flagLate;
  //   } else if (mode === 'soon') {
  //     return styles.flagSoon;
  //   } else if (mode === 'live') {
  //     return styles.flagLive;
  //   } else if (mode === 'terminated') {
  //     return styles.flagTerminated;
  //   } else {
  //     return null;
  //   }
  // };

  return (
    <View style={styles.base}>
      <View style={styles.container}>
        <MaskedView
          androidRenderingMode="software"
          style={StyleSheet.absoluteFill}
          maskElement={
            <Animated.View style={animatedStyle}>
              <View style={styles.mask} />
            </Animated.View>
          }>
          <View style={[styles.backBg, styles.flagLive]} />
        </MaskedView>

        <View style={styles.content}>
          <Text style={{ color: 'red' }}>Example title</Text>
        </View>

        <View style={[styles.frontBg, styles.flagSoon]} />
      </View>

      {/* <Animated.View style={[animatedStyle, styles.mask]} /> */}

      <View style={styles.buttons}>
        <Button title="Animate" onPress={handlePress} />
      </View>
    </View>
  );
};

export default Header4;

const styles = StyleSheet.create({
  base: {
    gap: 32,
  },

  buttons: {
    gap: 8,
  },

  mask: {
    backgroundColor: 'black',
    alignSelf: 'center',

    // TODO replace this with a calculation based on the height of the header
    // top: 60,

    height: 20,
    width: 20,
    borderRadius: 20,
  },

  // previously wrapped around animated.view mask
  maskContainer: {
    backgroundColor: 'transparent',
    // flex: 1,
  },

  frontBg: {
    width: '100%',
    height: '100%',
    position: 'absolute',
    zIndex: -1,
  },

  backBg: {
    width: '100%',
    height: '100%',
    position: 'absolute',
    zIndex: -2,
  },

  container: {
    // height: 48,
    // width: '100%',

    borderColor: 'red',
    borderWidth: 2,

    justifyContent: 'center',
  },

  content: {
    paddingHorizontal: 24,
    paddingVertical: 16,
  },

  // Flag types
  flagLate: {
    backgroundColor: Colors.azure400,
  },

  flagSoon: {
    backgroundColor: Colors.orange400,
  },

  flagLive: {
    backgroundColor: Colors.green400,
  },

  flagTerminated: {
    backgroundColor: Colors.red400,
  },
});
