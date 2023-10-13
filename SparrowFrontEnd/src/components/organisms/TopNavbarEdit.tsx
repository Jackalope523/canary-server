import { View, Text } from 'react-native'
import React from 'react'

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
import { globalStyles } from '../../styles/Global';
import { Colors } from '../../styles/Colors';
import { navigationStyles } from '../../styles/Navigation';

const Icon = createIconSetFromFontello(fontelloConfig);

const TopNavbarEdit = () => {
  return (
    <View style={[navigationStyles.topNavbar, navigationStyles.topNavbar.edit]}>
      <Icon style={navigationStyles.topNavbar.icons} name="arrow-back-outline" />
      <View style={navigationStyles.topNavbar.edit.wrapper}>
        <Icon style={navigationStyles.topNavbar.icons} name="edit-outline" />
        <Icon style={navigationStyles.topNavbar.icons} name="settings-fill-alt" />
      </View>
    </View>
  )
}

export default TopNavbarEdit