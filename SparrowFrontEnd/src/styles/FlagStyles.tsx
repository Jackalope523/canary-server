import { StyleSheet } from 'react-native';
import { Colors } from './ColorStyles';

// Flag colors
export const FlagColors = {
  startingLate: Colors.picton400,
  startingSoon: Colors.orange400,
  live: Colors.green400,
  terminated: Colors.red400,
};

// Flag styles (backgroundColor)
export const FlagStyles = StyleSheet.create({
  startingSoon: {
    backgroundColor: FlagColors.startingSoon,
  },

  startingLate: {
    backgroundColor: FlagColors.startingLate,
  },

  live: {
    backgroundColor: FlagColors.live,
  },

  terminated: {
    backgroundColor: FlagColors.terminated,
  },
});
