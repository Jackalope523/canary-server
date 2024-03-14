import { View, Text } from 'react-native';
import * as React from 'react';
import { globalStyles } from '../../styles/GlobalStyles';
import { Colors } from '../../styles/ColorStyles';
import { navigationStyles } from '../../styles/NavigationStyles';

// Icons
import ArrowBack from '../../assets/icons/arrow-back-outline.svg';

const TopNavbarEditSelected = () => {
  return (
    <View style={[navigationStyles.topNavbar, navigationStyles.topNavbarEdit]}>
      <ArrowBack width={24} height={24} fill={Colors.sparrowDarkBrown} />
      <Text
        style={[globalStyles.buttonTextThree, { color: Colors.sparrowDark }]}>
        Save
      </Text>
    </View>
  );
};

export default TopNavbarEditSelected;
