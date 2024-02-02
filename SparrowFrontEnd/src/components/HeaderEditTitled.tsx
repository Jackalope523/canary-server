import { Pressable, Text, View } from 'react-native';
import React from 'react';

import { navigationStyles } from '../styles/NavigationStyles';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';
import { globalStyles } from '../styles/GlobalStyles';
const Icon = createIconSetFromFontello(fontelloConfig);

interface HeaderEditTitledProps {
  title: string;
}

const HeaderEditTitled: React.FC<HeaderEditTitledProps> = ({ title }) => {
  return (
    <View style={[navigationStyles.header, navigationStyles.headerEdit]}>
      {/* TODO onPress -> navigate to the previous screen */}
      <View style={navigationStyles.headerEditLeft}>
        <Pressable onPress={null}>
          <Icon
            name="arrow-back-outline"
            size={24}
            height={24}
            width={24}
            style={navigationStyles.headerIcon}
          />
        </Pressable>
        <Text style={[globalStyles.textDark, globalStyles.headingTextFive]}>
          {title}
        </Text>
      </View>

      {/* TODO onPress -> save changes and change to the default header that the screen uses */}
      <Pressable onPress={null}>
        <Text style={[globalStyles.textDark, globalStyles.buttonTextThree]}>
          Save
        </Text>
      </Pressable>
    </View>
  );
};

export default HeaderEditTitled;
