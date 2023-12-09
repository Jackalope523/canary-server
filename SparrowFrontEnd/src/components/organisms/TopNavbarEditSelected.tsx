import { View, Text } from 'react-native';
import * as React from 'react';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
import { globalStyles } from '../../styles/Global';
import { Colors } from '../../styles/Colors';
import { navigationStyles } from '../../styles/Navigation';

const Icon = createIconSetFromFontello(fontelloConfig);

const TopNavbarEditSelected = () => {
  return (
    <View style={[navigationStyles.topNavbar, navigationStyles.topNavbarEdit]}>
      <Icon style={navigationStyles.topNavbarIcon} name="arrow-back-outline" />
      <Text
        style={[globalStyles.buttonTextThree, { color: Colors.sparrowDark }]}>
        Save
      </Text>
    </View>
  );
};

export default TopNavbarEditSelected;
