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

import { verify } from '../api/loginAPI';

type VerifyProps = StackScreenProps<RootStackParamList, 'Verify'>;

export default function VerifyScreen({route, navigation}: VerifyProps): JSX.Element {
  const [code, setCode] = useState('');
  const [errorText, setErrorText] = useState('');
  const [buttonEnabled, setButtonEnabled] = useState(true);

  function handleVerify() {
    setButtonEnabled(false);
    setErrorText('');
    Promise.resolve(verify(route.params.phoneNumber, code))
    .then(() => navigation.replace('Landing'))
    .catch(() => setErrorText('Incorrect code'))
    .finally(() => setButtonEnabled(true));
  }

  return (
  <SafeAreaView style={style.sectionContainer}>
    <StatusBar />
    <Text>A code was sent to {route.params.phoneNumber}</Text>
    <Text style={style.errorText}>{errorText}</Text>
    <TextInput
    value={code}
    onChangeText={setCode}
    placeholder='XXXXXX'
    keyboardType='number-pad'
    maxLength={6}
    style={style.inputField}/>
    <Button
    title={"Verify"}
    disabled={!buttonEnabled}
    onPress={handleVerify} />
    <View style={style.footer} />
  </SafeAreaView>
  );
}