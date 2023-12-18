import * as React from 'react';
import { View, Text, TextInput } from 'react-native';
import { StackScreenProps } from '@react-navigation/stack';

import { Colors } from '../../styles/ColorStyles';
import { globalStyles } from '../../styles/GlobalStyles';

import { AuthStackParamList } from '../../components/atoms/types';
import Button, {
  ButtonType,
  ButtonSize,
  ButtonDisplay,
} from '../../components/Button';

import { login } from './accountPigeon';

type LoginProps = StackScreenProps<AuthStackParamList, 'Login'>;

const LoginScreen = ({ navigation }: LoginProps) => {
  const [PhoneNumber, setPhoneNumber] = React.useState('');
  const [buttonEnabled, setButtonEnabled] = React.useState(true);

  function handleLogin() {
    setButtonEnabled(false);

    login({ PhoneNumber })
      .then(navigate)
      .finally(() => setButtonEnabled(true));
  }

  function navigate() {
    navigation.navigate('Verify', {
      PhoneNumber,
      Forward: () => {
        navigation.navigate('Continue', {
          Message: 'Welcome back.',
          Forward: () => navigation.replace('Main'),
        });
      },
    });
  }

  return (
    <View>
      <Text>Phone Number</Text>
      <TextInput
        value={PhoneNumber}
        onChangeText={setPhoneNumber}
        keyboardType="phone-pad"
      />
      <Button
        type={ButtonType.PrimaryDark}
        size={ButtonSize.Large}
        display={ButtonDisplay.Contained}
        btnText={'Login'}
        onPress={navigate}
        disabled={!buttonEnabled}
      />
      <Button
        type={ButtonType.PrimaryDark}
        size={ButtonSize.Large}
        display={ButtonDisplay.Contained}
        btnText={"Can't log in?"}
        disabled={!buttonEnabled}
      />
    </View>
  );
};

export default LoginScreen;
