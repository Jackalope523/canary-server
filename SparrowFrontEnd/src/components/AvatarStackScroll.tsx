import { ScrollView, StyleSheet } from 'react-native';
import React from 'react';
import Avatar, { AvatarSize, AvatarStatus } from './Avatar';

import { Spacing } from '../styles/SpacingStyles';

interface AvatarStackScrollProps {
  avatars: any[];
  size: AvatarSize;
  onPress?: () => void;
  type?: AvatarType;
  viewStyle?: any;
}

export const AvatarStackScroll: React.FC<AvatarStackScrollProps> = ({
  avatars,
  size,
  onPress = null,
  type = null,
  viewStyle,
}) => {
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                      Type                                      ||
  // ! ||--------------------------------------------------------------------------------||

  switch (type) {
    case AvatarType.Beside:
      if (size === AvatarSize.Large) {
        viewStyle = [styles.besideLarge, styles.base];
      } else if (size === AvatarSize.Medium) {
        viewStyle = [styles.besideMedium, styles.base];
      } else if (size === AvatarSize.Small) {
        viewStyle = [styles.besideSmall, styles.base];
      }
      break;

    case AvatarType.Stacked:
      if (size === AvatarSize.Large) {
        viewStyle = [styles.stackedLarge, styles.base];
      } else if (size === AvatarSize.Medium) {
        viewStyle = [styles.stackedMedium, styles.base];
      } else if (size === AvatarSize.Small) {
        viewStyle = [styles.stackedSmall, styles.base];
      }
      break;
  }

  return (
    <ScrollView
      contentContainerStyle={viewStyle}
      overScrollMode="never"
      horizontal
      showsHorizontalScrollIndicator={false}>
      {avatars.map((avatar, index) => (
        <Avatar key={index} size={size} image={avatar} onPress={onPress} showUserType={true} />
      ))}
    </ScrollView>
  );
};

export enum AvatarType {
  Beside,
  Stacked,
}

export default AvatarStackScroll;

const styles = StyleSheet.create({
  // Base
  base: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: Spacing.lg,
  },

  // Beside
  besideLarge: {
    columnGap: Spacing.md,
  },

  besideMedium: {
    columnGap: Spacing.sm,
  },

  besideSmall: {
    columnGap: Spacing.sm,
  },

  // Stacked
  stackedLarge: {
    columnGap: -36,
  },

  stackedMedium: {
    columnGap: -Spacing.lg,
  },

  stackedSmall: {
    columnGap: -Spacing.md,
  },
});
