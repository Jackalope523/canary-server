import { StackScreenProps } from '@react-navigation/stack';
import React, { useState } from 'react';

import
{
  SafeAreaView,
  TextInput,
  View,
  Button,
  DatePickerIOSBase
}
from 'react-native';
import DatePicker from 'react-native-date-picker';

import { RootStackParamList } from '../../../../App';
import style from '../../../theme/styles';
import { signup } from '../api/loginAPI';

type RegisterProps = StackScreenProps<RootStackParamList, 'Register'>;

export default function RegisterScreen({route, navigation}: RegisterProps): JSX.Element {
  const [phoneNumber, setPhoneNumber] = useState(route.params.phoneNumber);
  const [email, setEmail] = useState('');
  const [name, setName] = useState('');
  const [dateOfBirth, setDateOfBirth] = useState(new Date);
  const [buttonEnabled, setButtonEnabled] = useState(true);
  
  function handleSignup() {
    setButtonEnabled(false);
    Promise.resolve(signup(phoneNumber, email, name, dateOfBirth))
    .then(() => navigation.navigate('Verify', { phoneNumber: phoneNumber}))
    .finally(() => setButtonEnabled(true));
  }

  return (
  <SafeAreaView style={style.sectionContainer}>
    <View>
      <TextInput
      value={phoneNumber}
      onChangeText={setPhoneNumber}
      placeholder='phone number'
      keyboardType='phone-pad'
      style={style.inputField} />
      <TextInput
      value={email}
      onChangeText={setEmail}
      placeholder='email'
      style={[style.inputField, {marginVertical: 0}]} />
      <TextInput
      value={name}
      onChangeText={setName}
      placeholder='name'
      style={[style.inputField, {marginBottom: 0}]} />
      <DatePicker
      date={dateOfBirth}
      onDateChange={setDateOfBirth}
      mode='date'
      timeZoneOffsetInMinutes={0}
      maximumDate={new Date()}
      style={style.inputField} />
      <Button
      title={"Sign up"}
      disabled={!buttonEnabled}
      onPress={handleSignup} />
    </View>
    <View style={style.footer} />
  </SafeAreaView>
  );
}