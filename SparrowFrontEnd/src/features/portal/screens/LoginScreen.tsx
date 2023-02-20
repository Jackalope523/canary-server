import { StackScreenProps } from '@react-navigation/stack';
import React, { useState } from 'react';

import
{
  SafeAreaView,
  Text,
  View,
  Button,
  TextInput
}
from 'react-native';

import { RootStackParamList } from '../../../../App';
import style from '../../../theme/styles';

import { login } from '../api/loginAPI';

type LoginProps = StackScreenProps<RootStackParamList, 'Login'>;

export default function LoginScreen({navigation}: LoginProps): JSX.Element {
  const [phoneNumber, setPhoneNumber] = useState('');
  
  function handleLogin() {
    Promise.resolve(login(phoneNumber))
    .then(() => navigation.navigate('Verify', { phoneNumber: phoneNumber}));
  }

  return (
  <SafeAreaView style={style.sectionContainer}>
    <View>
      <TextInput
      value={phoneNumber}
      onChangeText={setPhoneNumber}
      placeholder='phone number'
      keyboardType='phone-pad'
      style={style.inputField}/>
      <Button
      title={"Login"}
      onPress={handleLogin} />
    </View>
    <View style={style.footer} />
  </SafeAreaView>
  );
}