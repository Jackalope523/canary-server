import { NavigationProp, useNavigation } from '@react-navigation/native';
import { StackNavigationProp, StackScreenProps } from '@react-navigation/stack';
import React from 'react';

import
{
    SafeAreaView,
    ScrollView,
    StatusBar,
    StyleSheet,
    Text,
    useColorScheme,
    View,
    Button
  }
from 'react-native';

import { RootStackParamList } from '../../../../App';
import style from '../../../theme/styles';

type PortalProps = StackScreenProps<RootStackParamList, 'Portal'>;

export default function PortalScreen({navigation}: PortalProps): JSX.Element {
    return (
    <SafeAreaView style={style.sectionContainer}>
      <StatusBar />
      <Button title={"Enter"} onPress={() => navigation.navigate('Authentication') } />
      <View style={style.footer} />
    </SafeAreaView>
  );
}