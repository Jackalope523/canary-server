import { View, Image, ImageStyle, Pressable } from 'react-native';
import * as React from 'react';
import { avatarStyles } from '../styles/AvatarStyles';
import { globalStyles } from '../styles/GlobalStyles';

interface AvatarProps {
  status?: AvatarStatus;
  size?: AvatarSize;
  image?: any;
  onPress?: () => void;

  avatarBorder?: ImageStyle[];
  avatarContainer?: ImageStyle[];
}

export const Avatar: React.FC<AvatarProps> = ({
  status = null,
  size = null,
  avatarBorder = [],
  avatarContainer = [],
  image,
  onPress,
}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Status                                     ||
  // ! ||--------------------------------------------------------------------------------||

  switch (status) {
    case AvatarStatus.Online:
      avatarBorder = [avatarStyles.avatarOnline];
      break;

    case AvatarStatus.Offline:
      avatarBorder = [avatarStyles.avatarOffline];
      break;
  }

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Size                                      ||
  // ! ||--------------------------------------------------------------------------------||

  switch (size) {
    case AvatarSize.Small:
      avatarContainer = [avatarStyles.avatarSquareSmall];
      break;

    case AvatarSize.Medium:
      avatarContainer = [avatarStyles.avatarSquareMedium];
      break;

    case AvatarSize.Large:
      avatarContainer = [avatarStyles.avatarSquareLarge];
      break;
  }

  // TODO finish making this into a component by implementing the following logic:

  /*
  
  If user is a friend and is online return an image styled with avatarStyles.avatarSquareMedium, avtarStyles.avatarOnline
  If user is a friend and is online return an image styles with avatarStyles.avatarSquareMedium, avtarStyles.avatarOffline
  Else return an image styled with avatarStyles.avatarSquareMedium, avtarStyles.avatarAnon
  
  */

  return (
    <Pressable onPress={onPress}>
      <Image source={image} style={[avatarContainer, avatarBorder]} />
    </Pressable>
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

export default Avatar;
