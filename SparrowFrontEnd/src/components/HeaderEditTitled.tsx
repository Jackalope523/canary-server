import { Pressable, Text, View } from 'react-native';
import React from 'react';

import { navigationStyles } from '../styles/NavigationStyles';
import { globalStyles } from '../styles/GlobalStyles';

// Icons
import ArrowBack from '../assets/icons/arrow-back-outline.svg';

interface HeaderEditTitledProps {
  title: string;
}

const HeaderEditTitled: React.FC<HeaderEditTitledProps> = ({ title }) => {
  return (
    <View style={[navigationStyles.header, navigationStyles.headerEdit]}>
      {/* TODO onPress -> navigate to the previous screen */}
      <View style={navigationStyles.headerEditLeft}>
        <Pressable onPress={null}>
          <ArrowBack width={24} height={24} fill={Colors.sparrowDarkBrown} />
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
