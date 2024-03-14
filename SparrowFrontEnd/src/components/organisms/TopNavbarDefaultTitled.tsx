import { View, Text } from 'react-native';
import * as React from 'react';
import { globalStyles } from '../../styles/GlobalStyles';
import { navigationStyles } from '../../styles/NavigationStyles';
import { buttonStyles } from '../../styles/ButtonStyles';

// Icons
import ArrowBack from '../../assets/icons/arrow-back-outline.svg';
import { Colors } from '../../styles/ColorStyles';

// TODO possibly delete all these topNavbar components, if you're going to code them differently

const TopNavbarDefaultTitled = () => {
  return (
    <View
      style={[
        navigationStyles.topNavbar,
        navigationStyles.topNavbarDefaultTitled,
      ]}>
      <ArrowBack width={24} height={24} fill={Colors.sparrowDarkBrown} />
      <Text style={[globalStyles.headingTextFive, buttonStyles.buttonTertiary]}>
        Notifications
      </Text>
    </View>
  );
};

export default TopNavbarDefaultTitled;
