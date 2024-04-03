import { View } from 'react-native';
import * as React from 'react';
import { navigationStyles } from '../../styles/NavigationStyles';

// Icons
import ArrowBack from '../../assets/icons/arrow-back-outline.svg';
import KebabFill from '../../assets/icons/kebab-fill.svg';

const TopNavbarOptions = () => {
  return (
    <View
      style={[navigationStyles.topNavbar, navigationStyles.topNavbarOptions]}>
      <ArrowBack width={24} height={24} fill={Colors.sparrowDarkBrown} />
      <KebabFill width={24} height={24} fill={Colors.sparrowDarkBrown} />
    </View>
  );
};

export default TopNavbarOptions;
