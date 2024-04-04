import { View, Image, ImageStyle, Pressable, StyleSheet } from 'react-native';
import * as React from 'react';
import { Colors } from '../styles/ColorStyles';

interface AvatarProps {
  size?: AvatarSize;
  image?: any;
  onPress?: () => void;

  avatarContainer?: ImageStyle[];
}

export const Avatar: React.FC<AvatarProps> = ({
  size = null,
  avatarContainer = [],
  image,
  onPress,
}) => {
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
      break;

    case AvatarSize.Large:
      avatarContainer = [styles.avatarSquareLarge];
      break;
  }

  return (
    <Pressable onPress={onPress}>
      <Image source={image} style={avatarContainer} />
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

  // Medium
  avatarSquareMedium: {
    width: 48,
    height: 48,
    borderRadius: 8,
    borderWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
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
