import { View, Text, Image } from 'react-native';
import * as React from 'react';
import { avatarStyles } from '../styles/Avatars';

interface AvatarProps {
  status?: AvatarStatus;
  size?: AvatarSize;
}

export const Avatar: React.FC<AvatarProps> = ({
  status = null,
  size = null,
}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Status                                     ||
  // ! ||--------------------------------------------------------------------------------||

  switch (status) {
    case AvatarStatus.Online:
      avatarStyles.avatarOnline;
      break;

    case AvatarStatus.Offline:
      avatarStyles.avatarOffline;
      break;
  }

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Size                                      ||
  // ! ||--------------------------------------------------------------------------------||

  switch (size) {
    case AvatarSize.Small:
      avatarStyles.avatarSquareSmall;
      break;

    case AvatarSize.Medium:
      avatarStyles.avatarSquareMedium;
      break;

    case AvatarSize.Large:
      avatarStyles.avatarSquareLarge;
      break;
  }

  // TODO finish making this into a component by implementing the following logic:

  /*
  
  If user is a friend and is online return an image styled with avatarStyles.avatarSquareMedium, avtarStyles.avatarOnline
  If user is a friend and is online return an image styles with avatarStyles.avatarSquareMedium, avtarStyles.avatarOffline
  Else return an image styled with avatarStyles.avatarSquareMedium, avtarStyles.avatarAnon
  
  */

  return (
    <View>
      <Text>Avatar</Text>
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

export default Avatar;
