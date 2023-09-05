import { View, Text, Pressable, Platform } from 'react-native'
import React from 'react'
import { globalStyles } from '../../styles/Global';
import { Colors } from '../../styles/Colors';
import { notificationStyles } from '../../styles/Notifications';
import { labelStyles } from '../../styles/Labels';

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
        <Icon name="notification-outline-alt" style={notificationStyles.notificationIndicator.icon}/>
        <View style={[labelStyles.numberLabel, labelStyles.numberLabel.dark]}>
          <Text style={[globalStyles.labelTextUppercase, {color: Colors.sparrowSand}]}>12</Text>
        </View>
      </View>
    </Pressable>
  )
}

export default NotificationIndicator