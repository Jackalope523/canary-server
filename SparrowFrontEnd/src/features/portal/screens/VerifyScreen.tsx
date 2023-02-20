import React, { useState } from 'react';
import { StackScreenProps } from '@react-navigation/stack';

import
{
  Text,
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

export default function VerifyScreen({route, navigation}: VerifyProps): JSX.Element {
  const [code, setCode] = useState('');

  return (
  <SafeAreaView style={style.sectionContainer}>
    <StatusBar />
    <Text>A code was sent to {route.params.phoneNumber}</Text>
    <TextInput
    value={code}
    onChangeText={setCode}
    placeholder='XXXXXX'
    keyboardType='number-pad'
    maxLength={6}
    style={style.inputField}/>
    <Button title={"Verify"} onPress={() => navigation.replace('Landing')} />
    <View style={style.footer} />
  </SafeAreaView>
  );
}