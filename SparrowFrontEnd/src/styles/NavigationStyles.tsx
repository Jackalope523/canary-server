import { StyleSheet } from 'react-native';
import { Colors } from './ColorStyles';
import { Spacing } from './SpacingStyles';

export const navigationStyles = StyleSheet.create({
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                                   Navigation                                   ||
  // ! ||--------------------------------------------------------------------------------||
  // ! ||--------------------------------------------------------------------------------||
  // ! ||                               Header (top navbar)                              ||
  // ! ||--------------------------------------------------------------------------------||

  // Base
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: Spacing.lg,
    paddingVertical: 12,
    backgroundColor: Colors.sparrowSand,
    borderBottomWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
  },

  headerIcon: {
    color: Colors.sparrowDarkBrown,
  },

  headerGapLeft: {
    marginRight: Spacing.md,
  },

  // Types
  // Default with title
  headerDefaultTitled: {
    columnGap: Spacing.md,
  },

  // Favorite
  headerFavorite: {
    justifyContent: 'space-between',
  },

  // Options
  headerOptions: {
    justifyContent: 'space-between',
  },

  // Edit
  headerEdit: {
    justifyContent: 'space-between',
  },

  headerEditLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.md,
  },

  // Flags
  flag: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingHorizontal: Spacing.lg,
    paddingVertical: 12,
    borderBottomWidth: 2,
    borderColor: Colors.sparrowDarkBrown,
  },

  flagLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.md,
  },

  flagRight: {
    flexDirection: 'row',
    alignItems: 'center',
    columnGap: Spacing.sm,
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

  searchOptionsInnerSectionContent: {
    columnGap: Spacing.md,
  },

  // TODO DELETE test later when it's not used anymore
  test: {
    alignSelf: 'baseline',
    verticalAlign: 'middle',
    textAlign: 'center',
    justifyContent: 'center',
  },
});
