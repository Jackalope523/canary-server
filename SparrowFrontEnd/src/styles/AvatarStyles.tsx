import { StyleSheet } from 'react-native';
import { Colors } from './ColorStyles';

// TODO move these styles to components -> Avatar.tsx after implementing Avatar component in Activity -> Notifications

export const avatarStyles = StyleSheet.create({
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Avatars                                    ||
  // ! ||--------------------------------------------------------------------------------||
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Borders                                    ||
  // ! ||--------------------------------------------------------------------------------||

  avatarOnline: {
    borderWidth: 2,
    borderColor: Colors.green400,
  },

  avatarOffline: {
    borderWidth: 2,
    borderColor: Colors.orange400,
  },

  avatarInvisible: {
    borderWidth: 2,
    borderColor: Colors.orange400,
  },

  avatarAnon: {
    borderWidth: 2,
    borderColor: Colors.sand300,
  },

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                  Single avatar                                 ||
  // ! ||--------------------------------------------------------------------------------||

  // Square

  // Large
  avatarSquareLarge: {
    width: 72,
    height: 72,
    borderRadius: 8,
  },

  // Medium
  avatarSquareMedium: {
    width: 48,
    height: 48,
    borderRadius: 8,
  },

  // Small
  avatarSquareSmall: {
    width: 24,
    height: 24,
    borderRadius: 4,
  },
});
