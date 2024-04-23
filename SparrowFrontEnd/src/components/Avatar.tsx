import { View, Image, ImageStyle, Pressable, StyleSheet, ViewStyle } from 'react-native';
import * as React from 'react';
import { Colors } from '../styles/ColorStyles';

// Icons
import FeatherIcon from '../assets/icons/feather-fill-colored.svg'

interface AvatarProps {
  size?: AvatarSize;
  image?: any;
  onPress?: () => void;
  showUserType: boolean;

  avatarContainer?: ImageStyle[];
  iconStyle?: ViewStyle[];
  iconSize: number;
}

export const Avatar: React.FC<AvatarProps> = ({
  size = null,
  avatarContainer = [],
  iconSize,
  iconStyle = [],
  image,
  onPress,
  showUserType,
}) => {
  // TODO hook up to backend data
  var friend = true;

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Size                                      ||
  // ! ||--------------------------------------------------------------------------------||

  switch (size) {
    case AvatarSize.ExtraSmall:
      avatarContainer = [styles.avatarSquareExtraSmall];
      break;

    case AvatarSize.Small:
      avatarContainer = [styles.avatarSquareSmall];
      break;

    case AvatarSize.Medium:
      avatarContainer = [styles.avatarSquareMedium];
      iconSize = 24;
      iconStyle = [styles.iconMedium];
      break;

    case AvatarSize.Large:
      avatarContainer = [styles.avatarSquareLarge];
      iconSize = 24;
      iconStyle = [styles.iconLarge];
      break;
  }

  return (
    <Pressable onPress={onPress}>
      <Image source={image} style={avatarContainer} />
      {showUserType && friend ?
        <FeatherIcon width={iconSize} height={iconSize} style={iconStyle} />
        : null}
    </Pressable>
  );
};

// ! ||--------------------------------------------------------------------------------||
// ! ||                                 Exported Enums                                 ||
// ! ||--------------------------------------------------------------------------------||

export enum AvatarSize {
  ExtraSmall,
  Small,
  Medium,
  Large,
}

export default Avatar;

const styles = StyleSheet.create({
  // Square avatars
  // Large
  avatarSquareLarge: {
    width: 72,
    height: 72,
    borderRadius: 8,
    borderWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
  },

  iconLarge: {
    // Alternate design
    // alignSelf: 'flex-end',
    // position: 'absolute',
    // bottom: 0,

    alignSelf: 'center',
    bottom: 14,
    marginBottom: -14,
  },

  // Medium
  avatarSquareMedium: {
    width: 48,
    height: 48,
    borderRadius: 8,
    borderWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
  },

  iconMedium: {
    alignSelf: 'center',
    bottom: 14,
    marginBottom: -14,
  },

  // Small
  avatarSquareSmall: {
    width: 32,
    height: 32,
    borderRadius: 4,
    borderWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
  },

  // Extra small
  avatarSquareExtraSmall: {
    width: 24,
    height: 24,
    borderRadius: 4,
    borderWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
  },
});
