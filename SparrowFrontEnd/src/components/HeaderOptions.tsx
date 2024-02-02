import { Pressable, Text, View } from 'react-native';
import React from 'react';

import { navigationStyles } from '../styles/NavigationStyles';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';
import { globalStyles } from '../styles/GlobalStyles';
const Icon = createIconSetFromFontello(fontelloConfig);

interface HeaderOptionsProps {
  title: string;
}

const HeaderOptions: React.FC<HeaderOptionsProps> = ({ title }) => {
  return (
    <View style={[navigationStyles.header, navigationStyles.headerOptions]}>
      {/* TODO onPress -> navigate to the previous screen */}
      <Pressable onPress={null}>
        <Icon
          name="arrow-back-outline"
          size={24}
          height={24}
          width={24}
          style={navigationStyles.headerIcon}
        />
      </Pressable>

      {/* TODO onPress -> open options dropdown */}
      <Pressable onPress={null}>
        <Icon
          name="kebab-fill"
          size={24}
          height={24}
          width={24}
          style={navigationStyles.headerIcon}
        />
      </Pressable>
    </View>
  );
};

export default HeaderOptions;
