import { Pressable, Text, View } from 'react-native';
import React from 'react';

import { navigationStyles } from '../styles/NavigationStyles';
import { globalStyles } from '../styles/GlobalStyles';
import { Colors } from '../styles/ColorStyles';

// Icons
import ArrowBack from '../assets/icons/arrow-back-outline.svg';

interface HeaderDefaultTitledProps {
  title: string;
}

const HeaderDefaultTitled: React.FC<HeaderDefaultTitledProps> = ({ title }) => {
  return (
    <View
      style={[navigationStyles.header, navigationStyles.headerDefaultTitled]}>
      {/* TODO onPress -> navigate to the previous screen */}
      <Pressable onPress={null}>
        <ArrowBack width={24} height={24} fill={Colors.sparrowDarkBrown} />
      </Pressable>
      <Text style={[globalStyles.textDark, globalStyles.headingTextFive]}>
        {title}
      </Text>
    </View>
  );
};

export default HeaderDefaultTitled;
