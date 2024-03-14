import { Pressable, Text, View } from 'react-native';
import React from 'react';

import { navigationStyles } from '../styles/NavigationStyles';
import { globalStyles } from '../styles/GlobalStyles';
import { Colors } from '../styles/ColorStyles';

// Icons
import ArrowBack from '../assets/icons/arrow-back-outline.svg';
import KebabFill from '../assets/icons/kebab-fill.svg';

interface HeaderOptionsProps {
  title: string;
}

const HeaderOptions: React.FC<HeaderOptionsProps> = ({ title }) => {
  return (
    <View style={[navigationStyles.header, navigationStyles.headerOptions]}>
      {/* TODO onPress -> navigate to the previous screen */}
      <Pressable onPress={null}>
        <ArrowBack width={24} height={24} fill={Colors.sparrowDarkBrown} />
      </Pressable>

      {/* TODO onPress -> open options dropdown */}
      <Pressable onPress={null}>
        <KebabFill width={24} height={24} fill={Colors.sparrowDarkBrown} />
      </Pressable>
    </View>
  );
};

export default HeaderOptions;
