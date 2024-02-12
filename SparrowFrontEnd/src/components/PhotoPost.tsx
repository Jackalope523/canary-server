import {
  View,
  Image,
  Text,
  StyleSheet,
  FlatList,
  ScrollView,
} from 'react-native';
import * as React from 'react';
import { globalStyles } from '../styles/GlobalStyles';
import { Colors } from '../styles/ColorStyles';
import Avatar from './Avatar';
import { Spacing } from '../styles/SpacingStyles';
import { CustomDimensions } from '../styles/CustomDimensionStyles';
import { SAMPLEEVENTDATA } from '../data/sampleEventData';
import { MEDIA } from '../data/sampleMediaData';
import LocationIndicator from './LocationIndicator';
import { Gesture, GestureDetector } from 'react-native-gesture-handler';

// TEMP. avatar image
import TempAvatarImage from '../assets/images/temp/image-placeholder.png';

// Icons
import PersonIcon from '../assets/icons/account-fill.svg';
import LocationIcon from '../assets/icons/location-fill.svg';
import LikeOutlineIcon from '../assets/icons/favorite-outline.svg';
import LikeFillIcon from '../assets/icons/favorite-fill.svg';
import MeatballIcon from '../assets/icons/meatball-outline.svg';
import { runOnJS } from 'react-native-reanimated';
import FlagMedium, { FlagType } from './FlagMedium';

/*

TODO implement mechanics:

1. when the text is too long add ... at the end (similar to event card medium)
2. DONE: Multiple image support
3. Single image support
4. DONE: Add double press to like functionality
5. Add pinch to zoom functinality
6. Disable location indicator when there is only one image
7. If event is live, use live flag, if it's passed use the time ago text
8. Add an animation for the like button icon

*/

interface PhotoPostProps {
  name: string;
  time: string | FlagType.Live;
  title: string;
  attendees: string[] | string;
  leftoverAttendeeCount: number;
  location: string;
  likeCount: number;

  index?: number;
}

export const PhotoPost: React.FC<PhotoPostProps> = ({
  name = 'NULL',
  time = 'NULL',
  title = 'NULL',
  attendees,
  leftoverAttendeeCount,
  location = 'NULL',
  likeCount,
}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                               Location indicator                               ||
  // ! ||--------------------------------------------------------------------------------||
  const [index, setIndex] = React.useState(0);

  const viewabilityConfig = {
    itemVisiblePercentThreshold: 50,
  };

  const onViewableItemsChanged = ({
    viewableItems,
    changed,
  }: {
    viewableItems: any[];
    changed: any[];
  }) => {
    console.log('Visible items are', viewableItems);
    console.log('Changed in this iteration', changed);

    // TODO FIX: getting a typescript error when browsing through images - cannot read property "index" of undefined
    setIndex(viewableItems[0].index);
  };

  const viewabilityConfigCallbackPairs = React.useRef([
    { viewabilityConfig, onViewableItemsChanged },
  ]);

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                               Like functionality                               ||
  // ! ||--------------------------------------------------------------------------------||
  /*
 
  TODO if like === true:
  1. change the icon to filled heart
  2. change the color of the icon to orange400
  3. set the likeCount to +1
  4. add an animation to the media container
  
  */

  const [like, setLike] = React.useState(false);

  const doubleTap = Gesture.Tap()
    .numberOfTaps(2)
    .onEnd((_, success) => {
      if (success) {
        console.log('Double tap');
        runOnJS(setLike)(!like);
      }
    });

  return (
    <View style={styles.container}>
      <View style={styles.top}>
        <View style={styles.user}>
          <Avatar
            size={AvatarSize.Medium}
            status={AvatarStatus.Offline}
            image={TempAvatarImage}
          />
          <Text style={[globalStyles.textDark, globalStyles.headingTextThree]}>
            {name}
          </Text>
        </View>
        {time === FlagType.Live ? (
          <FlagMedium type={FlagType.Live} />
        ) : (
          <Text
            style={[globalStyles.textDark, globalStyles.labelTextOneAsTyped]}>
            {time} ago
          </Text>
        )}

        {/* <FlagMedium type={FlagType.Live} />
        <Text style={[globalStyles.textDark, globalStyles.labelTextOneAsTyped]}>
          {time} ago
        </Text> */}
      </View>
      {/* TOP ENDS */}
      {/* CARD */}
      <View style={styles.card}>
        <View style={[styles.cardInfo, styles.cardInfoTop]}>
          <Text style={[globalStyles.textDark, globalStyles.headingTextThree]}>
            {title}
          </Text>
        </View>

        {/* MEDIA CONTAINER */}

        <View>
          <FlatList
            horizontal={true}
            showsHorizontalScrollIndicator={false}
            overScrollMode="never"
            data={MEDIA}
            pagingEnabled={true}
            keyExtractor={(item) => item.id}
            renderItem={({ item }) => (
              <GestureDetector gesture={doubleTap}>
                <View style={styles.imageContainer}>
                  <Image source={item.uri} style={styles.image} />
                </View>
              </GestureDetector>
            )}
            // onScroll={handleScroll}

            viewabilityConfigCallbackPairs={
              viewabilityConfigCallbackPairs.current
            }
            viewabilityConfig={viewabilityConfig}
          />
          <LocationIndicator data={MEDIA} selected={index} />
        </View>

        {/* <View style={styles.imageContainer}>
          <Image
            source={require('../assets/images/temp/image-placeholder.png')}
            style={styles.image}
            // style={globalStyles.illustrationFull}
          />
        </View> */}
        {/* MEDIA CONTAINER ENDS */}

        <View style={[styles.cardInfo, styles.cardInfoBottom]}>
          <View style={styles.info}>
            <PersonIcon height={24} width={24} fill={Colors.sparrowDarkBrown} />
            <Text style={[globalStyles.textDark, globalStyles.bodyTextOne]}>
              {attendees} and {leftoverAttendeeCount} others
            </Text>
          </View>
          <View style={styles.info}>
            <LocationIcon
              height={24}
              width={24}
              fill={Colors.sparrowDarkBrown}
            />
            <Text style={[globalStyles.textDark, globalStyles.bodyTextOne]}>
              {location}
            </Text>
          </View>
        </View>
      </View>
      {/* CARD ENDS */}
      {/* BOTTOM */}
      <View style={styles.bottom}>
        <View style={styles.info}>
          {like ? (
            <LikeFillIcon
              height={24}
              width={24}
              fill={Colors.sparrowDarkBrown}
            />
          ) : (
            <LikeOutlineIcon
              height={24}
              width={24}
              fill={Colors.sparrowDarkBrown}
            />
          )}
          <Text style={[globalStyles.textDark, globalStyles.bodyTextOne]}>
            {like ? likeCount + 1 : likeCount}
          </Text>
        </View>
        <MeatballIcon height={24} width={24} fill={Colors.sparrowDarkBrown} />
      </View>
      {/* BOTTOM ENDS */}
    </View>
  );
};

// ! ||--------------------------------------------------------------------------------||
// ! ||                                 Exported Enums                                 ||
// ! ||--------------------------------------------------------------------------------||

export enum AvatarStatus {
  Online,
  Offline,
}

export enum AvatarSize {
  Small,
  Medium,
  Large,
}

export default PhotoPost;

const styles = StyleSheet.create({
  container: {
    // flex: 1,
    rowGap: Spacing.md,
  },

  top: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.md,
  },

  user: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: 12,
  },

  card: {
    borderWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
    borderRadius: 8,

    backgroundColor: Colors.sparrowSand,
  },

  cardInfo: {
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.sm,
    rowGap: Spacing.sm,
  },

  cardInfoTop: {
    borderBottomWidth: 2,
    borderBottomColor: Colors.sparrowDarkBrown,
  },

  cardInfoBottom: {
    borderTopWidth: 2,
    borderTopColor: Colors.sparrowDarkBrown,
  },

  /*

    TODO fix the imageContainer and image styles;
    implement the image properly and fix the blurry bottom of the image
  
  */

  imageContainer: {
    height: CustomDimensions.windowWidth - Spacing.lg * 2,
    width: CustomDimensions.windowWidth - Spacing.lg * 2,
  },

  image: {
    alignSelf: 'center',
    resizeMode: 'cover',
    // Fixes blank bottom pixel spacing
    width: '102%',
    height: '102%',
  },

  bottom: {
    flexDirection: 'row',
    justifyContent: 'space-between',
  },

  info: {
    flexDirection: 'row',
    alignItems: 'center',

    // TODO change spacing to xs (?)
    columnGap: Spacing.sm,
  },
});
