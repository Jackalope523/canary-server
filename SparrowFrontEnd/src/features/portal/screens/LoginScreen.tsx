import { StackScreenProps } from '@react-navigation/stack';
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

import { RootStackParamList } from '../../../../App';
import style from '../../../theme/styles';

type LoginProps = StackScreenProps<RootStackParamList, 'Login'>;

export default function LoginScreen({navigation}: LoginProps): JSX.Element {
    return (
    <SafeAreaView style={style.sectionContainer}>
      <View>
        <TextInput
        placeholder='phone number'
        keyboardType='phone-pad'
        style={style.inputField}/>
        <Button title={"Login"} onPress={() => navigation.navigate('Verify')} />
      </View>
      <View style={style.footer} />
    </SafeAreaView>
  );
}

