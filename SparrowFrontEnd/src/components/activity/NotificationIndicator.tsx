import { View, Text, Pressable, Platform } from 'react-native';
import * as React from 'react';
import { useNavigation } from '@react-navigation/native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { StackParamList } from '../atoms/types';

import { globalStyles } from '../../styles/GlobalStyles';
import { Colors } from '../../styles/ColorStyles';
import { notificationStyles } from '../../styles/NotificationStyles';
import { labelStyles } from '../../styles/LabelStyles';

// Icons
import NotificationOutline from '../../assets/icons/notification-outline-v2.svg';

const NotificationIndicator = () => {
  const navigation = useNavigation<NativeStackNavigationProp<StackParamList>>();

  return (
    <Pressable onPress={() => navigation.navigate('Notifications')}>
      <View style={notificationStyles.notificationIndicator}>
        <NotificationOutline
          height={24}
          width={24}
          fill={Colors.sparrowDarkBrown}
        />
        <View style={[labelStyles.numberLabel, labelStyles.numberLabelPrimary]}>
          <Text
            style={[
              globalStyles.labelTextOneUppercase,
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
