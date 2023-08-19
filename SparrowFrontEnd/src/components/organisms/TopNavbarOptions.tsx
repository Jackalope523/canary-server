import { View, Text } from 'react-native'
import React from 'react'

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
import { globalStyles } from '../../styles/Global';
import { Colors } from '../../styles/Colors';
import { navigationStyles } from '../../styles/Navigation';

const Icon = createIconSetFromFontello(fontelloConfig);

const TopNavbarOptions = () => {
  return (
    <View style={[navigationStyles.topNavbar, navigationStyles.topNavbar.options]}>
      <Icon style={navigationStyles.topNavbar.icons} name="arrow-back-outline" />
      <Icon style={navigationStyles.topNavbar.icons} name="kebab-fill" />
    </View>
  )
}

export default TopNavbarOptions