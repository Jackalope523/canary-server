import { StackScreenProps } from '@react-navigation/stack';
import React from 'react';

import
{
    SafeAreaView,
    StatusBar,
    View,
    Button
  }
from 'react-native';

import { RootStackParamList } from '../../../../App';
import style from '../../../theme/styles';

type RegisterProps = StackScreenProps<RootStackParamList, 'Register'>;

export default function RegisterScreen({navigation}: RegisterProps): JSX.Element {
    return (
    <SafeAreaView style={style.sectionContainer}>
        <StatusBar />
        <Button title={"Register"} onPress={() => navigation.navigate('Register')} />
      <View style={style.footer} />
    </SafeAreaView>
  );
}