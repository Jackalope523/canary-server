import {
  Dimensions,
  Pressable,
  StyleSheet,
  Text,
  View,
  ViewStyle,
} from 'react-native';
import React, { useEffect } from 'react';

import { navigationStyles } from '../../styles/NavigationStyles';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
const Icon = createIconSetFromFontello(fontelloConfig);

import { globalStyles } from '../../styles/GlobalStyles';
import { Spacing } from '../../styles/SpacingStyles';
import { Colors } from '../../styles/ColorStyles';
import Animated, {
  useAnimatedStyle,
  useSharedValue,
  withTiming,
} from 'react-native-reanimated';
import MaskedView from '@react-native-masked-view/masked-view';

interface HeaderFlagAttendeeProps {
  title?: string;
  // type?: HeaderType;
  // headerStyle?: ViewStyle[];
}

const { width, height } = Dimensions.get('window');

const Content: React.FC = () => {
  return (
    <View style={styles.topNavbarCopy}>
      <View style={styles.left}>
        <Pressable onPress={null}>
          <Icon
            name="arrow-back-outline"
            size={24}
            height={24}
            width={24}
            style={navigationStyles.topNavbarIcon}
          />
        </Pressable>
        <Text style={[globalStyles.textDark, globalStyles.headingTextFive]}>
          Title text
        </Text>
      </View>
      <View style={styles.right}>
        {/* TODO replace favorite icon with watch icon */}
        <Icon
          name="favorite-outline"
          size={24}
          height={24}
          width={24}
          style={navigationStyles.topNavbarIcon}
        />
        <Icon
          name="share-outline"
          size={24}
          height={24}
          width={24}
          style={navigationStyles.topNavbarIcon}
        />
        <Icon
          name="kebab-fill"
          size={24}
          height={24}
          width={24}
          style={navigationStyles.topNavbarIcon}
        />
      </View>
    </View>
  );
};

const HeaderFlagAttendee: React.FC<HeaderFlagAttendeeProps> = ({
  title,
  // type,
  // headerStyle = [],
}) => {
  // switch (type) {
  //   case HeaderType.StartingLate:
  //     headerStyle = [styles.startingLate];
  //     break;

  //   case HeaderType.StartingSoon:
  //     headerStyle = [styles.startingSoon];
  //     break;

  //   case HeaderType.Live:
  //     headerStyle = [styles.live];
  //     break;

  //   case HeaderType.Terminated:
  //     headerStyle = [styles.terminated];
  //     break;
  // }

  /* --- FROM TUTORIAL ---  */
  // Masking
  // let width = 50;

  // let offset = useSharedValue(0);
  // useEffect(() => {
  //   offset.value = withTiming(4, { duration: 1500 });
  // }, []);

  // const r = width / 2;

  // const animatedStyles3 = useAnimatedStyle(() => ({
  //   transform: [{ scale: offset.value }],
  // }));

  /* --- MASKING, MY TAKE --- */
  // let offset = useSharedValue(0);

  // useEffect(() => {
  //   offset.value = withTiming(4, { duration: 1500 });
  // }, []);

  // const animatedTransition = useAnimatedStyle(() => ({
  //   transform: [{ scale: offset.value }],
  // }));

  // V2
  // let offset = useSharedValue(0);

  // useEffect(() => {
  //   offset.value = withTiming(4, { duration: 1500 });
  // }, []);

  // const r = width / 2;

  // const animatedTransition = useAnimatedStyle(() => ({
  //   transform: [{ scale: offset.value }],
  // }));

  // V3
  let offset = useSharedValue(0);

  useEffect(() => {
    offset.value = withTiming(4, { duration: 1500 });
    console.log('trigger HeaderFlagAttendee animation');
  }, []);

  const animatedTransition = useAnimatedStyle(() => ({
    transform: [
      {
        scaleY: offset.value,
      },
    ],
  }));

  return (
    <View
      style={{
        // flex: 1,
        // height: '100%',
        position: 'relative',
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

          // backgroundColor: Colors.orange400,
        }}
        maskElement={
          // Mask animation..
          <Animated.View
            style={[
              {
                // backgroundColor: 'transparent',
                // justifyContent: 'center',
                // alignItems: 'center',
                // flex: 1,

                // width: '100%',
                // height: '100%',

                width: 20,
                height: 20,
                borderRadius: 100,

                alignSelf: 'center',
              },
              animatedTransition,
            ]}>
            {/* Masked content.. */}
            <View
              style={{
                // v3
                width: 20,
                height: 20,
                borderRadius: 100,
                backgroundColor: 'black',

                // v1
                // width: '100%',
                // height: 48,
                // backgroundColor: 'black',
                // v2
                // flex: 1,
                // height: '100%',
                // backgroundColor: 'black',
              }}
            />
          </Animated.View>
        }>
        {/* --- MaskedView opening tag ends here --- */}
        {/* For masking.. */}
        {/* ChangeTo flag type (next) */}
        <View
          style={[
            styles.live,
            {
              // v2
              flex: 1,
              height: 20,
              width: 20,
              borderRadius: 100,

              // v1
              // flex: 1,
              // height: '100%',
              // width: '100%',
            },
          ]}
        />
      </MaskedView>
      <Content />

      {/* ChangeFrom flag type (current) */}
      <View
        style={[
          styles.startingSoon,
          {
            flex: 1,
            flexDirection: 'row',
            height: '100%',

            position: 'absolute',
            top: 0,
            bottom: 0,
            left: 0,
            right: 0,

            zIndex: -1,
          },
        ]}
      />
    </View>
  );
};

export default HeaderFlagAttendee;

// export enum HeaderType {
//   StartingLate,
//   StartingSoon,
//   Live,
//   Terminated,
// }

const styles = StyleSheet.create({
  topNavbarCopy: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: Spacing.lg,
    paddingVertical: 12,
    borderBottomWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
    justifyContent: 'space-between',
  },

  // >24h
  startingLate: {
    backgroundColor: Colors.picton400,
  },

  // <24h
  startingSoon: {
    backgroundColor: Colors.orange400,
  },

  live: {
    backgroundColor: Colors.green400,
  },

  terminated: {
    backgroundColor: Colors.red400,
  },

  container: {
    justifyContent: 'space-between',
    flex: 1,
  },

  left: {
    flexDirection: 'row',
    columnGap: Spacing.md,
  },

  right: {
    flexDirection: 'row',
    columnGap: Spacing.sm,
  },
});
