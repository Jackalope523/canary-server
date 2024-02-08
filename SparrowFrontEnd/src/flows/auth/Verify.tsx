import * as React from 'react';
import { View, Text, StyleSheet, Image, Pressable, Keyboard, KeyboardAvoidingView } from 'react-native';
import { StackScreenProps } from '@react-navigation/stack';

import { globalStyles } from '../../styles/GlobalStyles';

import { AuthStackParamList } from '../../components/atoms/types';
import Button, {
  ButtonType,
  ButtonSize,
  ButtonDisplay,
} from '../../components/Button';

import { verify } from './accountPigeon';
import { Spacing } from '../../styles/SpacingStyles';
import TextButton, {
  TextButtonType,
  TextButtonVariant,
} from '../../components/TextButton';
import OTPInput from '../../components/OTPInput';

type VerifyProps = StackScreenProps<AuthStackParamList, 'Verify'>;

const VerifyScreen = ({ route }: VerifyProps) => {
  const codeLength = 4;
  const [code, setCode] = React.useState('');
  const [codeReady, setCodeReady] = React.useState(false);
  
  // Verification________________________________________________
  const [errorText, setErrorText] = React.useState('');
  const [buttonEnabled, setButtonEnabled] = React.useState(true);

  function handleVerify() {
    setButtonEnabled(false);
    setErrorText('');

    verify({ PhoneNumber: route.params.PhoneNumber, Code: code })
      .then(route.params.Forward)
      .catch(() => setErrorText('Incorrect code'))
      .finally(() => setButtonEnabled(true));
  }
  //_____________________________________________________________

  return (
    <Pressable style={[styles.container, globalStyles.baseContainer]} onPress={Keyboard.dismiss}>
      <View style={styles.contentContainer}>
        <Image
          source={require('../../assets/illustrations/temp/illustration-placeholder.png')}
          style={[globalStyles.illustrationLarge, styles.illustration]}
          resizeMode="contain"
        />
        <Text
          style={[
            globalStyles.bodyTextOne,
            globalStyles.textDark,
            styles.text,
          ]}>
          Enter the 4-digit code we sent to your number ending in
          {route.params.PhoneNumber.substring(
            route.params.PhoneNumber.length - 4,
          )}. 
        </Text>
        <OTPInput codeLength = {codeLength} code = {code} setCode = {setCode} setCodeReady = {setCodeReady}/>
      </View>

      <View style={styles.buttonContainer}>
        <Button
          type={ButtonType.Success}
          size={ButtonSize.Medium}
          display={ButtonDisplay.Full}
          text={'Verify & Continue'}
          onPress={route.params.Forward}
          disabled={!codeReady}
        />

        <TextButton
          text="I haven't received a code"
          type={TextButtonType.Dark}
          variant={TextButtonVariant.Three}
          disabled={!codeReady}
        />
      </View>
    </Pressable>
  );
};

// ! ||--------------------------------------------------------------------------------||
// ! ||                                     Styles                                     ||
// ! ||--------------------------------------------------------------------------------||
const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'space-between',
  },

  contentContainer: {
    alignItems: 'center',
    rowGap: Spacing.lg,
  },

  illustration: {
    marginBottom: Spacing.sm,
  },

  text: {
    textAlign: 'center',
    // paddingBottom: Spacing.lg,
  },

  buttonContainer: {
    alignItems: 'center',
    rowGap: Spacing.md,
  },
});

export default VerifyScreen;
