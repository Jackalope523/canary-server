import { View, Text } from 'react-native';
import * as React from 'react';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
import { navigationStyles } from '../../styles/Navigation';

const Icon = createIconSetFromFontello(fontelloConfig);

const TopNavbarEdit = () => {
  return (
    <View style={[navigationStyles.topNavbar, navigationStyles.topNavbarEdit]}>
      <Icon style={navigationStyles.topNavbarIcon} name="arrow-back-outline" />
      <View style={navigationStyles.topNavbarEditWrapper}>
        <Icon style={navigationStyles.topNavbarIcon} name="edit-outline" />
        <Icon style={navigationStyles.topNavbarIcon} name="settings-fill-alt" />
      </View>
    </View>
  );
};

export default TopNavbarEdit;
