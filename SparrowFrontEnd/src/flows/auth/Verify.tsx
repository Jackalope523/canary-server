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

import { verify } from './accountPigeon';

type VerifyProps = StackScreenProps<AuthStackParamList, 'Verify'>;

const VerifyScreen = ({ route }: VerifyProps) => {
  const [Code, setCode] = React.useState('');
  const [errorText, setErrorText] = React.useState('');
  const [buttonEnabled, setButtonEnabled] = React.useState(true);

  function handleVerify() {
    setButtonEnabled(false);
    setErrorText('');

    verify({ PhoneNumber: route.params.PhoneNumber, Code })
      .then(route.params.Forward)
      .catch(() => setErrorText('Incorrect code'))
      .finally(() => setButtonEnabled(true));
  }

  return (
    <View>
      <Text>
        Enter the 6-digit code we sent to your number ending in
        {route.params.PhoneNumber.substring(
          route.params.PhoneNumber.length - 4,
        )}
        .
      </Text>
      <Text style={{ color: Colors.red400 }}>{errorText}</Text>
      <TextInput
        value={Code}
        onChangeText={setCode}
        placeholder="000000"
        keyboardType="number-pad"
        maxLength={6}
      />
      <Button
        type={ButtonType.PrimaryDark}
        size={ButtonSize.Large}
        display={ButtonDisplay.Contained}
        text={'Verify & Continue'}
        onPress={route.params.Forward}
        disabled={!buttonEnabled}
      />
      <Button
        type={ButtonType.PrimaryDark}
        size={ButtonSize.Large}
        display={ButtonDisplay.Contained}
        text={"I didn't receive a code"}
        disabled={!buttonEnabled}
      />
    </View>
  );
};

export default VerifyScreen;
