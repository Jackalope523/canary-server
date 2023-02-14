import { BottomTabScreenProps } from '@react-navigation/bottom-tabs';
import React from 'react';

import
{
    SafeAreaView,
    Text,
    View,
    Button,
    TextInput
  }
from 'react-native';

import { RootTabsParamList } from '../../../../App';
import style from '../../../theme/styles';

type ProfileProps = BottomTabScreenProps<RootTabsParamList, 'Profile'>;

export default function ProfileScreen({navigation}: ProfileProps): JSX.Element {
    return (
    <SafeAreaView>
      <View>
        <Text>Profile Screen</Text>
      </View>
    </SafeAreaView>
  );
}

