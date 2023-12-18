import { View } from 'react-native';
import * as React from 'react';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
import { navigationStyles } from '../../styles/NavigationStyles';

const Icon = createIconSetFromFontello(fontelloConfig);

const TopNavbarOptions = () => {
  return (
    <View
      style={[navigationStyles.topNavbar, navigationStyles.topNavbarOptions]}>
      <Icon style={navigationStyles.topNavbarIcon} name="arrow-back-outline" />
      <Icon style={navigationStyles.topNavbarIcon} name="kebab-fill" />
    </View>
  );
};

export default TopNavbarOptions;
