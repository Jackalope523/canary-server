import { Pressable, Text, View } from 'react-native';
import React from 'react';

import { navigationStyles } from '../../styles/NavigationStyles';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
import { globalStyles } from '../../styles/GlobalStyles';
const Icon = createIconSetFromFontello(fontelloConfig);

interface HeaderDefaultTitledProps {
  title: string;
}

const HeaderDefaultTitled: React.FC<HeaderDefaultTitledProps> = ({ title }) => {
  return (
    <View
      style={[
        navigationStyles.topNavbar,
        navigationStyles.topNavbarDefaultTitled,
      ]}>
      <Pressable onPress={null}>
        <Icon
          name="arrow-back-outline"
          size={24}
          height={24}
          width={24}
          style={navigationStyles.topNavbarIcon}
        />
      </Pressable>
      <Text style={[globalStyles.textDark, globalStyles.headingTextFive]}>
        {title}
      </Text>
    </View>
  );
};

export default HeaderDefaultTitled;
