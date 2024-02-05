import { View, Text } from 'react-native';
import * as React from 'react';
import { navigationStyles } from '../../styles/NavigationStyles';

// Icons
import ArrowBack from '../../assets/icons/arrow-back-outline.svg';
import EditOutline from '../../assets/icons/edit-outline.svg';
import SettingsFill from '../../assets/icons/settings-fill-alt.svg';

const TopNavbarEdit = () => {
  return (
    <View style={[navigationStyles.topNavbar, navigationStyles.topNavbarEdit]}>
      <ArrowBack width={24} height={24} fill={Colors.sparrowDarkBrown} />
      <View style={navigationStyles.topNavbarEditWrapper}>
        <EditOutline width={24} height={24} fill={Colors.sparrowDarkBrown} />
        <SettingsFill width={24} height={24} fill={Colors.sparrowDarkBrown} />
      </View>
    </View>
  );
};

export default TopNavbarEdit;
