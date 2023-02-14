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

type DiscoverProps = BottomTabScreenProps<RootTabsParamList, 'Discover'>;

export default function DiscoverScreen({navigation}: DiscoverProps): JSX.Element {
    return (
    <SafeAreaView>
      <View>
        <Text>Map Screen</Text>
      </View>
    </SafeAreaView>
  );
}

