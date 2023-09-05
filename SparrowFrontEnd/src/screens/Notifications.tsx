import * as React from 'react';
import { View, Text, Platform } from 'react-native';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';
import { globalStyles } from '../styles/Global';
import { Colors } from '../styles/Colors';
import { navigationStyles } from '../styles/Navigation';

const Icon = createIconSetFromFontello(fontelloConfig);

const NotificationsScreen = ({navigation}) => {

  React.useLayoutEffect(() => {
    navigation.setOptions({

      headerLeft: () => <Icon style={navigationStyles.topNavbar.icons} name="arrow-back-outline" onPress={() => navigation.goBack()} />,

      // headerLeftContainerStyle: {
      //   marginLeft: 24,
      // },
    })
  })

    return (
        <View>
            <Text>Notifications screen</Text>
        </View>
    );
};

export default NotificationsScreen