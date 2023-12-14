import { View } from 'react-native';
import * as React from 'react';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
import { navigationStyles } from '../../styles/NavigationStyles';

const Icon = createIconSetFromFontello(fontelloConfig);

const TopNavbarFavorite = () => {
  return (
    <View
      style={[navigationStyles.topNavbar, navigationStyles.topNavbarFavorite]}>
      <Icon style={navigationStyles.topNavbarIcon} name="arrow-back-outline" />
      {/* TODO change favorite-outline to favorite-fill on tap, when user is adding event to favorites */}
      <Icon style={navigationStyles.topNavbarIcon} name="favorite-outline" />
    </View>
  );
};

export default TopNavbarFavorite;
