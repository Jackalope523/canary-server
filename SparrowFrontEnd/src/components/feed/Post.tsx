import { View, Image, Text, StyleSheet, FlatList } from 'react-native';
import * as React from 'react';
import { Gesture, GestureDetector } from 'react-native-gesture-handler';
import { runOnJS } from 'react-native-reanimated';
import Avatar from '../Avatar';
import PaginationIndicator from './PaginationIndicator';
import FlagMedium, { FlagType } from '../FlagMedium';
import DropdownSelectorIcon, { Align, Icon } from '../DropdownSelectorIcon';
import dropdownOptionsPost from '../DropdownOptionsPost';
import { globalStyles } from '../../styles/GlobalStyles';
import { Colors } from '../../styles/ColorStyles';
import { Spacing } from '../../styles/SpacingStyles';
import { CustomDimensions } from '../../styles/CustomDimensionStyles';

// Icons
import PersonIcon from '../../assets/icons/account-fill.svg';
import LocationIcon from '../../assets/icons/location-fill.svg';
import LikeOutlineIcon from '../../assets/icons/favorite-outline.svg';
import LikeFillIcon from '../../assets/icons/favorite-fill.svg';

/*

TODO implement mechanics:

2. Add pinch to zoom functinality
3. Add an animation for the like button icon
4. Make image unswipable if there's only one image

*/

export interface PhotoPostProps {
  name: string;
  media?: string[];
  author: string[];
  location: string;
}

export const PhotoPost: React.FC<PhotoPostProps> = ({
  name = 'Poker Night',
  media,
  author = 'John Marston',
  location = 'St Denis',
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
  1. add an animation to the media container
  
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
      {/* TOP ENDS */}
      {/* CARD */}
      <View style={styles.card}>
        <View style={[styles.cardInfo, styles.cardInfoTop]}>
          <Text
            style={[globalStyles.textDark, globalStyles.headingTextThree]}
            numberOfLines={2}>
            {name}
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
              {author}
            </Text>
          </View>
          <View style={styles.info}>
            <LocationIcon
              height={24}
              width={24}
              fill={Colors.sparrowDarkBrown}
            />
            <Text
              style={[globalStyles.textDark, globalStyles.bodyTextOne]}
              numberOfLines={1}>
              {location}
            </Text>
          </View>
        </View>
      </View>
      {/* CARD ENDS */}
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
