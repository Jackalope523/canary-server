import { View, Text, Pressable, Platform } from 'react-native';
import * as React from 'react';
import { globalStyles } from '../../styles/GlobalStyles';
import { Colors } from '../../styles/ColorStyles';
import { notificationStyles } from '../../styles/NotificationStyles';
import { labelStyles } from '../../styles/LabelStyles';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../../config.json';
const Icon = createIconSetFromFontello(fontelloConfig);

import { useNavigation } from '@react-navigation/native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';

const NotificationIndicator = () => {
  const navigation = useNavigation<NativeStackNavigationProp<StackParamList>>();

  return (
    <Pressable onPress={() => navigation.navigate('Notifications')}>
      <View style={notificationStyles.notificationIndicator}>
        <Icon
          name="notification-outline-alt"
          size={24}
          height={24}
          width={24}
          style={notificationStyles.notificationIndicator.icon}
        />
        <View style={[labelStyles.numberLabel, labelStyles.numberLabelPrimary]}>
          <Text
            style={[
              globalStyles.labelTextUppercase,
              { color: Colors.sparrowSand },
            ]}>
            12
          </Text>
        </View>
      </View>
    </Pressable>
  );
};

export default NotificationIndicator;
