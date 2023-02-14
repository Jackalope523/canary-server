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

type FeedProps = BottomTabScreenProps<RootTabsParamList, 'Feed'>;

export default function FeedScreen({navigation}: FeedProps): JSX.Element {
    return (
    <SafeAreaView>
      <View>
        <Text>Feed Screen</Text>
      </View>
    </SafeAreaView>
  );
}

