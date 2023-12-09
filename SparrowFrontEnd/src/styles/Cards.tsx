import { Dimensions, StyleSheet } from 'react-native';
import { Colors } from './Colors';
import { Spacing } from './Spacing';

// Screen dimensions
const screenWidth = Dimensions.get('screen').width;

export const cardStyles = StyleSheet.create({
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                   Event cards                                  ||
  // ! ||--------------------------------------------------------------------------------||
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Medium                                     ||
  // ! ||--------------------------------------------------------------------------------||

  eventCardMedium: {
    width: screenWidth - Spacing.lg * 2,
  },

  eventCardMediumImage: {
    borderWidth: 2,
    borderRadius: 8,
    borderColor: Colors.sparrowDarkBrown,
  },

  eventCardMediumContent: {
    height: 254,
    margin: Spacing.sm,
    justifyContent: 'space-between',
  },

  eventCardMediumContentInner: {
    paddingHorizontal: 16,
    paddingVertical: 8,
    backgroundColor: Colors.sparrowSand,
    borderWidth: 2,
    borderRadius: 8,
    borderColor: Colors.sparrowDarkBrown,
  },

  eventCardMediumTitle: {
    marginBottom: Spacing.sm,
  },

  eventCardMediumTextWrapper: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.sm,
  },

  eventCardMediumInnerText: {
    flexShrink: 1,
  },

  // TODO check if works, when implementing SVG's instead of icon fonts
  eventCardMediumIcon: {
    color: Colors.sparrowDarkBrown,
  },

  // If textWrapper text exceeds 2 lines, align items to flex-start
  eventCardMediumTextWrapperCenter: {
    alignItems: 'center',
  },

  eventCardMediumTextWrapperOverflow: {
    alignItems: 'flex-start',
  },

  eventCardMediumTopWrapper: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
  },
});
