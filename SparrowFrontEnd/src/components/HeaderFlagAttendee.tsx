import {
  Button,
  Dimensions,
  Pressable,
  StyleSheet,
  Text,
  View,
  ViewStyle,
} from 'react-native';
import React from 'react';
import { Colors } from '../styles/ColorStyles';
import Animated, {
  runOnJS,
  useAnimatedStyle,
  useSharedValue,
  withTiming,
} from 'react-native-reanimated';
import MaskedView from '@react-native-masked-view/masked-view';
import { navigationStyles } from '../styles/NavigationStyles';
import { globalStyles } from '../styles/GlobalStyles';

/* --- DEV NOTES --- */
/*

  1. FIX: not animating - add androidRenderingMode="software" to MaskedView

*/

// Icons
import ArrowBack from '../assets/icons/arrow-back-outline.svg';
import FavoriteOutline from '../assets/icons/favorite-outline.svg';
import ShareOutline from '../assets/icons/share-outline.svg';
import KebabFill from '../assets/icons/kebab-fill.svg';

type Props = {
  title: string;

  previousType: APType;
  nextType: ANType;
  previousTypeStyle: ViewStyle[];
  nextTypeStyle: ViewStyle[];
};

const HeaderFlagAttendee: React.FC<Props> = ({
  title,
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
    case APType.StartingLate:
      previousTypeStyle = [styles.flagStartingLate];
      break;

    case APType.StartingSoon:
      previousTypeStyle = [styles.flagStartingSoon];
      break;

    case APType.Live:
      previousTypeStyle = [styles.flagLive];
      break;

    case APType.Terminated:
      previousTypeStyle = [styles.flagTerminated];
      break;
  }

  switch (nextType) {
    case ANType.StartingLate:
      nextTypeStyle = [styles.flagStartingLate];
      break;

    case ANType.StartingSoon:
      nextTypeStyle = [styles.flagStartingSoon];
      break;

    case ANType.Live:
      nextTypeStyle = [styles.flagLive];
      break;

    case ANType.Terminated:
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

        {/* Content */}
        <View style={navigationStyles.flag}>
          <View style={navigationStyles.flagLeft}>
            {/* TODO onPress -> navigate back one screen */}
            <Pressable onPress={null}>
              <ArrowBack
                width={24}
                height={24}
                fill={Colors.sparrowDarkBrown}
              />
            </Pressable>
            <Text style={[globalStyles.textDark, globalStyles.headingTextFive]}>
              {title}
            </Text>
          </View>

          {/* TODO if the event is terminated, HIDE flagRight VIEW */}
          <View style={navigationStyles.flagRight}>
            {/* TODO replace favorite-outline with watch icon */}
            {/* TODO onPress -> watch event */}
            <Pressable onPress={null}>
              <FavoriteOutline
                width={24}
                height={24}
                fill={Colors.sparrowDarkBrown}
              />
            </Pressable>
            {/* TODO onPress -> navigate to event share screen */}
            <Pressable onPress={null}>
              <ShareOutline
                width={24}
                height={24}
                fill={Colors.sparrowDarkBrown}
              />
            </Pressable>
            {/* TODO onPress -> open options dropdown menu */}
            <Pressable onPress={null}>
              <KebabFill
                width={24}
                height={24}
                fill={Colors.sparrowDarkBrown}
              />
            </Pressable>
          </View>
        </View>

        {/* Foreground */}
        <View style={[styles.bg, styles.foreground, previousTypeStyle]} />
      </View>

      {/* TODO remove after logic has been implemented; this is for testing purposes */}
      <View>
        <Button title="Animate" onPress={handlePress} />
      </View>
    </View>
  );
};

export default HeaderFlagAttendee;

// APTYPE - ATTENDEE PREVIOUS TYPE
export enum APType {
  StartingLate,
  StartingSoon,
  Live,
  Terminated,
}

export enum ANType {
  StartingLate,
  StartingSoon,
  Live,
  Terminated,
}

const styles = StyleSheet.create({
  base: {
    gap: 8,
  },

  mask: {
    backgroundColor: 'black',
    alignSelf: 'center',
    height: 20,
    width: 20,
    borderRadius: 20,
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
    borderColor: 'red',
    borderWidth: 2,
    justifyContent: 'center',
  },

  content: {
    paddingHorizontal: 24,
    paddingVertical: 16,
  },

  // Flag types
  flagStartingLate: {
    backgroundColor: Colors.azure400,
  },

  flagStartingSoon: {
    backgroundColor: Colors.orange400,
  },

  flagLive: {
    backgroundColor: Colors.green400,
  },

  flagTerminated: {
    backgroundColor: Colors.red400,
  },
});
