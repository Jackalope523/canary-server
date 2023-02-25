import AsyncStorage from '@react-native-async-storage/async-storage';
import { StackNavigationProp, StackScreenProps } from '@react-navigation/stack';
import React, { useState } from 'react';

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
import { initialiseAxiosSession } from '../../../lib/axios';
import style from '../../../theme/styles';
import { getUserProfile } from '../../profile/api/profileAPI';
import { logout } from '../api/loginAPI';

type PortalProps = StackScreenProps<RootStackParamList, 'Portal'>;

export default function PortalScreen({navigation}: PortalProps): JSX.Element {
  const [buttonEnabled, setButtonEnabled] = useState(true);
  const [miniToken, setMiniToken] = useState('');
  
  function tryTokenLogin() {
    setButtonEnabled(false);
    AsyncStorage.getItem('token')
    .then((token) => {
      if (token != null) {
        initialiseAxiosSession(token);
        Promise.resolve(getUserProfile())
        .then(() => navigation.replace('Landing'))
        .catch(() => console.log('login failed with token'))
        .finally(() => setButtonEnabled(true));
      }
    })
    .catch(() => setButtonEnabled(true));
  }

  function tryLogout() {
    setButtonEnabled(false);
    AsyncStorage.setItem('token', '');
    Promise.resolve(logout())
    .finally(() => setButtonEnabled(true));
  }

  AsyncStorage.getItem('token')
  .then((token) => setMiniToken((token == null || token === '' ? '' : token.substring(0,50) + '...')));

  return (
    (miniToken === '' ?
      <SafeAreaView style={style.sectionContainer}>
        <StatusBar />
          <Text style={{marginBottom: 20}}>No token</Text>
          <Button
          title={"Enter no auth"}
          disabled={!buttonEnabled}
          onPress={() => navigation.navigate('Authentication')} />
        <View style={style.footer} />
      </SafeAreaView>
    :
      <SafeAreaView style={style.sectionContainer}>
        <StatusBar />
        <Text style={{marginBottom: 20}}>Token is {miniToken}</Text>
        <Button
        title={"Enter via token"}
        disabled={!buttonEnabled}
        onPress={tryTokenLogin} />
        <View style={{margin: 5}} />
        <Button
        title={"Clear Token"}
        disabled={!buttonEnabled}
        onPress={tryLogout} />
        <View style={style.footer} />
      </SafeAreaView>
    )
  );
}