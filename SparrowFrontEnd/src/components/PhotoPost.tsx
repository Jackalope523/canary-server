import { View, Image, Text, StyleSheet } from 'react-native';
import * as React from 'react';
import { globalStyles } from '../styles/GlobalStyles';
import { Colors } from '../styles/ColorStyles';
import Avatar from './Avatar';
import { Spacing } from '../styles/SpacingStyles';

// TEMP. avatar image
import TempAvatarImage from '../assets/images/temp/image-placeholder.png';

// Icons
import PersonIcon from '../assets/icons/account-fill.svg';
import LocationIcon from '../assets/icons/location-fill.svg';
import LikeIcon from '../assets/icons/favorite-outline.svg';
import MeatballIcon from '../assets/icons/meatball-outline.svg';

/*

TODO implement similar mechanics as in EventCardMedium:
1. when the text is too long add ... at the end

2. Implement sharing all kinds of media:
- image gallery (multiple images)
- video/s

*/

interface PhotoPostProps {
  name: string;
  time: string;
  title: string;
  attendees: string[] | string;
  leftoverAttendeeCount: number;
  location: string;
  likeCount: number;
}

export const PhotoPost: React.FC<PhotoPostProps> = ({
  /*

variables here
naming ideas:

size,

name,
time,
title,
image, images or media,
attendees,
location,
likesCount

*/

  name = 'NULL',
  time = 'NULL',
  title = 'NULL',
  attendees,
  leftoverAttendeeCount,
  location = 'NULL',
  likeCount,
}) => {
  // 1. switch for size
  //   switch (status) {
  //     case AvatarStatus.Online:
  //       avatarBorder = [avatarStyles.avatarOnline];
  //       break;

  //     case AvatarStatus.Offline:
  //       avatarBorder = [avatarStyles.avatarOffline];
  //       break;
  //   }

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
        <Text style={[globalStyles.textDark, globalStyles.labelTextOneAsTyped]}>
          {time} ago
        </Text>
      </View>
      {/* TOP ENDS */}
      {/* CARD */}
      <View style={styles.card}>
        <View style={styles.cardInfo}>
          <Text style={[globalStyles.textDark, globalStyles.headingTextThree]}>
            {title}
          </Text>
        </View>

        <View style={styles.imageContainer}>
          <Image
            source={require('../assets/images/temp/image-placeholder.png')}
            style={styles.image}
            // style={globalStyles.illustrationFull}
          />
        </View>

        <View style={styles.cardInfo}>
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
          <LikeIcon height={24} width={24} fill={Colors.sparrowDarkBrown} />
          <Text style={[globalStyles.textDark, globalStyles.bodyTextOne]}>
            {likeCount}
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

  /*

    TODO fix the imageContainer and image styles;
    implement the image properly and fix the blurry bottom of the image
  
  */

  imageContainer: {
    // TODO make height responsive and dynamic
    height: 312,
    width: '100%',

    borderTopWidth: 2,
    borderBottomWidth: 2,
    borderTopColor: Colors.sparrowDarkBrown,
    borderBottomColor: Colors.sparrowDarkBrown,
  },

  image: {
    flex: 1,
    alignSelf: 'center',
    resizeMode: 'cover',
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
