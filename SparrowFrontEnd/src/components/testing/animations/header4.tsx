import {
  Button,
  Dimensions,
  StyleSheet,
  Text,
  View,
  ViewStyle,
} from 'react-native';
import React from 'react';
import { Colors } from '../../../styles/ColorStyles';
import Animated, {
  runOnJS,
  useAnimatedStyle,
  useSharedValue,
  withTiming,
} from 'react-native-reanimated';
import MaskedView from '@react-native-masked-view/masked-view';

/* --- DEV NOTES --- */
/*

  1. FIX: not animating - add androidRenderingMode="software" to MaskedView

*/

type Props = {
  // buttonIndex: number;
  // mode?: 'startingLate' | 'startingSoon' | 'live' | 'terminated';

  previousType: PType;
  nextType: NType;
  previousTypeStyle: ViewStyle[];
  nextTypeStyle: ViewStyle[];
};

const Header4: React.FC<Props> = ({
  previousType,
  nextType,
  previousTypeStyle,
  nextTypeStyle,
}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                   Animations                                   ||
  // ! ||--------------------------------------------------------------------------------||
  const scale = useSharedValue(20);
  // const r = Dimensions.get('window').width / 2;
  // const R = r * 2;
  const width = Dimensions.get('window').width;
  const anchorPointX = 0;
  const anchorPointY = 0;

  const scale2 = useSharedValue(0);

  // TODO replace with actual header height
  const headerHeight = 55;
  // TODO possibly modify later
  const endHeight = (width / 20) * 1.2;

  // TODO possibly add curve later
  const handlePress = () => {
    runOnJS(() => {
      scale2.value = withTiming(endHeight, { duration: 1000 });
    })();
  };

  const animatedStyle = useAnimatedStyle(() => ({
    transform: [{ translateY: headerHeight }, { scale: scale2.value }],
  }));

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                   Flag types                                   ||
  // ! ||--------------------------------------------------------------------------------||

  /*

  Setting flag colors:

  1. if event is in >24 hours - set to late
  2. if event is in <24 hours - set to soon
  3. if event is live - set to live
  4. if event is terminated - set to terminated

  The flag frontBg needs to be the previous type, and the backBg needs to be the next type.
  
  */

  switch (previousType) {
    case PType.StartingLate:
      previousTypeStyle = [styles.flagLate];
      break;

    case PType.StartingSoon:
      previousTypeStyle = [styles.flagSoon];
      break;

    case PType.Live:
      previousTypeStyle = [styles.flagLive];
      break;

    case PType.Terminated:
      previousTypeStyle = [styles.flagTerminated];
      break;
  }

  switch (nextType) {
    case NType.StartingLate:
      nextTypeStyle = [styles.flagLate];
      break;

    case NType.StartingSoon:
      nextTypeStyle = [styles.flagSoon];
      break;

    case NType.Live:
      nextTypeStyle = [styles.flagLive];
      break;

    case NType.Terminated:
      nextTypeStyle = [styles.flagTerminated];
      break;
  }

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
          {/* Background */}
          <View style={[styles.bg, styles.background, nextTypeStyle]} />
        </MaskedView>

        <View style={styles.content}>
          <Text style={{ color: 'red' }}>Example title</Text>
        </View>

        {/* Foreground */}
        <View style={[styles.bg, styles.foreground, previousTypeStyle]} />
      </View>

      <View style={styles.buttons}>
        <Button title="Animate" onPress={handlePress} />
      </View>
    </View>
  );
};

export default Header4;

export enum PType {
  StartingLate,
  StartingSoon,
  Live,
  Terminated,
}

export enum NType {
  StartingLate,
  StartingSoon,
  Live,
  Terminated,
}

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

  bg: {
    width: '100%',
    height: '100%',
    position: 'absolute',
  },

  foreground: {
    zIndex: -1,
  },

  background: {
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
