import { Dimensions, StyleSheet } from 'react-native';
import { Colors } from './Colors';
import { Spacing } from './Spacing';

import { useState } from 'react';

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

  // eventCardMedium: {
  //     width: screenWidth - Spacing.lg * 2,

  //     bgImage2: {
  //         borderWidth: 2,
  //         borderRadius: 8,
  //         borderColor: Colors.sparrowDarkBrown,
  //     },

  //     content: {
  //         height: 254,
  //         margin: Spacing.sm,
  //         justifyContent: 'space-between',

  //         // Event detail containers
  //         container: {
  //             paddingHorizontal: 16,
  //             paddingVertical: 8,
  //             backgroundColor: Colors.sparrowSand,
  //             borderWidth: 2,
  //             borderRadius: 8,
  //             borderColor: Colors.sparrowDarkBrown,

  //             title: {
  //                 marginBottom: Spacing.sm,
  //             },

  //             // Wraps text and icon
  //             textWrapper: {
  //                 flexDirection: 'row',
  //                 alignItems: 'center',
  //                 columnGap: Spacing.sm,

  //                 icon: {
  //                     color: Colors.sparrowDarkBrown,
  //                 },

  //                 // Inner text
  //                 innerText: {
  //                     flexShrink: 1,
  //                 },
  //             },

  //             // If textWrapper text exceeds 2 lines, align items to flex-start
  //             textWrapperCenter: {
  //                 alignItems: 'center',
  //             },

  //             textWrapperOverflow: {
  //                 alignItems: 'flex-start',
  //             },
  //         },

  //         // Wraps date and time container and attendees container
  //         topWrapper: {
  //             flexDirection: 'row',
  //             justifyContent: 'space-between',
  //             alignItems: 'flex-start',
  //         },
  //     },
  // },
});
