import { StackScreenProps } from '@react-navigation/stack';
import React from 'react';

import
{
    SafeAreaView,
    StatusBar,
    View,
    Button,
    TextInput
  }
from 'react-native';

import { RootStackParamList } from '../../../../App';
import style from '../../../theme/styles';

type VerifyProps = StackScreenProps<RootStackParamList, 'Verify'>;

export default function VerifyScreen({navigation}: VerifyProps): JSX.Element {
    return (
    <SafeAreaView style={style.sectionContainer}>
      <StatusBar />
        <TextInput
        placeholder='XXXXXX'
        keyboardType='number-pad'
        maxLength={6}
        style={style.inputField}/>
      <Button title={"Verify"} onPress={() => navigation.navigate('Register')} />
      <View style={style.footer} />
    </SafeAreaView>
  );
}