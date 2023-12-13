import { StyleSheet } from 'react-native';
import { Colors } from './Colors';
import { Spacing } from './Spacing';

export const notificationStyles = StyleSheet.create({
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                  Notifications                                 ||
  // ! ||--------------------------------------------------------------------------------||

  // Notification indicator
  notificationIndicator: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.sm,

    icon: {
      color: Colors.sparrowDarkBrown,
    },
  },

  // Notification
  notification: {
    flexDirection: 'row',
    columnGap: Spacing.md,
  },

  notificationTextWrapper: {
    flexDirection: 'column',
    rowGap: Spacing.sm,
  },
});
