import { View, Image, Text, StyleSheet, FlatList } from 'react-native';
import * as React from 'react';
import { Gesture, GestureDetector } from 'react-native-gesture-handler';
import { runOnJS } from 'react-native-reanimated';
import Avatar from './Avatar';
import PaginationIndicator from './PaginationIndicator';
import FlagMedium, { FlagType } from './FlagMedium';
import DropdownSmall, { Align, Icon } from './DropdownSmall';
import dropdownOptionsPost from './DropdownOptionsPost';
import { globalStyles } from '../styles/GlobalStyles';
import { Colors } from '../styles/ColorStyles';
import { Spacing } from '../styles/SpacingStyles';
import { CustomDimensions } from '../styles/CustomDimensionStyles';

// Icons
import PersonIcon from '../assets/icons/account-fill.svg';
import LocationIcon from '../assets/icons/location-fill.svg';
import LikeOutlineIcon from '../assets/icons/favorite-outline.svg';
import LikeFillIcon from '../assets/icons/favorite-fill.svg';

/*

TODO implement mechanics:

1. when the text is too long add ... at the end (similar to event card medium)
2. Add pinch to zoom functinality
3. Add an animation for the like button icon
4. Make image unswipable if there's only one image

*/

interface PhotoPostProps {
  name: string;
  avatar: string;
  time: string | FlagType.Live;
  title: string;
  media?: any;

  attendees: string[] | string;
  leftoverAttendeeCount: number;
  location: string;
  likeCount: number;

  index?: number;
}

export const PhotoPost: React.FC<PhotoPostProps> = ({
  name = 'NULL',
  avatar,
  time = 'NULL',
  title = 'NULL',
  media,
  attendees,
  leftoverAttendeeCount,
  location = 'NULL',
  likeCount,
}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                               Location indicator                               ||
  // ! ||--------------------------------------------------------------------------------||
  const [selectedIndex, setSelectedIndex] = React.useState(0);

  const viewabilityConfig = {
    itemVisiblePercentThreshold: 50,
  };

  const onViewableItemsChanged = () => {
    // TODO make the pagination work
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
            image={avatar}
          />
          <Text style={[globalStyles.textDark, globalStyles.headingTextThree]}>
            {name}
          </Text>
        </View>
        {/* TODO replace FlagType.Live in the {time === FlagType.Live ...} NOT in <FlagMedium /> component, with an event status (live) that's passed down from back-end */}
        {time === FlagType.Live ? (
          <FlagMedium type={FlagType.Live} />
        ) : (
          <Text
            style={[globalStyles.textDark, globalStyles.labelTextOneAsTyped]}>
            {time} ago
          </Text>
        )}
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
            data={media}
            pagingEnabled={true}
            keyExtractor={(item) => item.id}
            renderItem={({ item }) => (
              <GestureDetector gesture={doubleTap}>
                <View style={styles.imageContainer}>
                  <Image source={item.uri} style={styles.image} />
                </View>
              </GestureDetector>
            )}
            viewabilityConfigCallbackPairs={
              viewabilityConfigCallbackPairs.current
            }
            snapToAlignment="center"
            decelerationRate={'normal'}
          />
          {media.length > 1 && (
            <PaginationIndicator data={media} selectedIndex={selectedIndex} />
          )}
          {media.length === 1 && null}
        </View>

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
        <DropdownSmall
          icon={Icon.Meatball}
          options={dropdownOptionsPost}
          align={Align.BottomLeft}
        />
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

  imageContainer: {
    height: CustomDimensions.windowWidth - Spacing.lg * 2,
    width: CustomDimensions.windowWidth - Spacing.lg * 2,
  },

  image: {
    alignSelf: 'center',
    resizeMode: 'cover',

    /*
    
    Setting width and height to a little over 100% such as 102% fixes blank bottom pixel spacing,
    but I don't see the problem atm so will leave it as it is
    
    */

    width: '100%',
    height: '100%',
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
