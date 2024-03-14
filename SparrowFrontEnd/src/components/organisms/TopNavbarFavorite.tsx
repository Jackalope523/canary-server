import { View } from 'react-native';
import * as React from 'react';
import { navigationStyles } from '../../styles/NavigationStyles';

// Icons
import ArrowBack from '../../assets/icons/arrow-back-outline.svg';
import FavoriteOutline from '../../assets/icons/favorite-outline.svg';
import { Colors } from '../../styles/ColorStyles';

const TopNavbarFavorite = () => {
  return (
    <View
      style={[navigationStyles.topNavbar, navigationStyles.topNavbarFavorite]}>
      <ArrowBack width={24} height={24} fill={Colors.sparrowDarkBrown} />
      {/* TODO change favorite-outline to favorite-fill on tap, when user is adding event to favorites */}
      <FavoriteOutline height={24} width={24} fill={Colors.sparrowDarkBrown} />
    </View>
  );
};

export default TopNavbarFavorite;
