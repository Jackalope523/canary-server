import { View, Text } from 'react-native'
import React from 'react'

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
import { globalStyles } from '../../styles/Global';
import { Colors } from '../../styles/Colors';
import { navigationStyles } from '../../styles/Navigation';

const Icon = createIconSetFromFontello(fontelloConfig);

// TODO possibly delete all these topNavbar components, if you're going to code them differently

const TopNavbarDefaultTitled = () => {
  return (
    <View style={[navigationStyles.topNavbar, navigationStyles.topNavbar.defaultTitled]}>
      <Icon style={navigationStyles.topNavbar.icons} name="arrow-back-outline" />
      <Text style={[globalStyles.headingTextFive, globalStyles.textDark]}>Notifications</Text>
    </View>
  )
}

export default TopNavbarDefaultTitled