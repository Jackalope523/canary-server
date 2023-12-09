import { StyleSheet } from 'react-native';
import { Colors } from './Colors';
import { Spacing } from './Spacing';

export const navigationStyles = StyleSheet.create({
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                   Navigation                                   ||
  // ! ||--------------------------------------------------------------------------------||
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                               Top navbar (header)                              ||
  // ! ||--------------------------------------------------------------------------------||

  // Base
  topNavbar: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: Spacing.lg,
    paddingVertical: 12,
    backgroundColor: Colors.sparrowSand,
    borderBottomWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
  },

  topNavbarIcon: {
    color: Colors.sparrowDarkBrown,
  },

  topNavbarGapLeft: {
    marginRight: Spacing.md,
  },

  // Types
  // Default with title
  topNavbarDefaultTitled: {
    columnGap: Spacing.md,
  },

  // Favorite
  topNavbarFavorite: {
    justifyContent: 'space-between',
  },

  // Options
  topNavbarOptions: {
    justifyContent: 'space-between',
  },

  // Edit
  topNavbarEdit: {
    justifyContent: 'space-between',
  },

  topNavbarEditWrapper: {
    flexDirection: 'row',
    columnGap: Spacing.md,
  },

  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                     Search                                     ||
  // ! ||--------------------------------------------------------------------------------||

  searchHeaderWrapper: {
    backgroundColor: Colors.sparrowSand,
    borderBottomWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
  },

  searchHeader: {
    padding: 24,
  },

  searchBarWrapper: {
    flexDirection: 'row',
    // columnGap: Spacing.sm,

    // alignItems: 'center',
    // verticalAlign: 'baseline',
    justifyContent: 'center',
  },

  searchBarWrapperCloseButtonWrapper: {
    // TODO remove bgc later
    backgroundColor: Colors.red400,

    // OG padding in prototype is 16 but that doesn't work well here so I'm using 10
    // To make it more similar to OG padding, make paddingHorizontal: 16, paddingVertical: 10
    padding: 8,
  },

  searchBar: {
    backgroundColor: Colors.sparrowSand,
    paddingHorizontal: 16,
    paddingVertical: 8,
    borderWidth: 2,
    borderRadius: 8,
    borderColor: Colors.sparrowDarkBrown,
    flex: 1,

    alignSelf: 'center',
    alignItems: 'center',
    columnGap: Spacing.md,
    flexDirection: 'row',
  },

  searchBarTextInput: {
    margin: 0,
    padding: 0,
    flex: 1,

    fontFamily: 'UncutSans-Regular',
    fontSize: 16,
    fontWeight: 'regular',
  },

  searchOptions: {
    flexDirection: 'row',
    columnGap: Spacing.md,
    paddingTop: Spacing.md,

    // TODO remove bgc later
    backgroundColor: Colors.fuchsia500,
  },

  searchOptionsInner: {
    backgroundColor: Colors.sparrowBrown,
    height: '100%',
  },

  searchOptionsInnerSection: {
    rowGap: Spacing.md,
    paddingTop: Spacing.lg,
  },

  searchOptionsInnerSectionContent: {
    columnGap: Spacing.md,
  },

  searchContent: {
    marginHorizontal: 24,
  },

  // TODO DELETE test later when it's not used anymore
  test: {
    alignSelf: 'baseline',
    verticalAlign: 'middle',
    textAlign: 'center',
    justifyContent: 'center',
  },
});
