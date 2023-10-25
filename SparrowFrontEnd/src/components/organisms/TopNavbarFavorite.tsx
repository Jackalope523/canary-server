import { View, Text } from 'react-native'
import * as React from 'react'

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
import { globalStyles } from '../../styles/Global';
import { Colors } from '../../styles/Colors';
import { navigationStyles } from '../../styles/Navigation';

const Icon = createIconSetFromFontello(fontelloConfig);

const TopNavbarFavorite = () => {
  return (
    <View style={[navigationStyles.topNavbar, navigationStyles.topNavbar.favorite]}>
      <Icon style={navigationStyles.topNavbar.icons} name="arrow-back-outline" />
      {/* TODO change favorite-outline to favorite-fill on tap, when user is adding event to favorites */}
      <Icon style={navigationStyles.topNavbar.icons} name="favorite-outline" />
    </View>
  )
}

export default TopNavbarFavorite