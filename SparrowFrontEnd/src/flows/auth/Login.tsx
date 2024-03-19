import React, {useState} from 'react';
import {
  View,
  StyleSheet,
  Image,
  Pressable,
  Keyboard
} from 'react-native';
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
import KeyboardAvoidingContainer from '../../components/KeyboardAvoidingContainer';
import { Spacing } from '../../styles/SpacingStyles';
import TextInputSmall, { InputType } from '../../components/TextInputSmall';
import TextButton, {
  TextButtonType,
  TextButtonVariant,
} from '../../components/TextButton';

type LoginProps = StackScreenProps<AuthStackParamList, 'Login'>;

const LoginScreen = ({ navigation }: LoginProps) => {
  const [validPhoneNumber, setValidPhoneNumber] = useState(false);
  const [PhoneNumber, setPhoneNumber] = React.useState('');
  const [buttonEnabled, setButtonEnabled] = React.useState(true);

  function handleLogin() {
    setButtonEnabled(false);

    login({ PhoneNumber })
      .then(navigate)
      .finally(() => setButtonEnabled(true))
      .catch(() => console.log("ERROR"));
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
    <Pressable style={[styles.container, globalStyles.baseContainer]} onPress = {Keyboard.dismiss}>
      <View style={styles.headerContainer}>
        <Image
          source={require('../../assets/illustrations/temp/illustration-placeholder.png')}
          style={globalStyles.illustrationLarge}
          resizeMode="contain"
        />
      </View>

      <View style={styles.contentContainer}>
        <TextInputSmall
          type={InputType.PhoneNumber}
          label="Phone Number"
          valid={validPhoneNumber}
          setValid={setValidPhoneNumber}
          text={PhoneNumber}
          setText={setPhoneNumber}
          inputMode="tel"
          maxLength={17}
          required = {true}
          mask = '+1 ([000]) [000]-[0000]'
        />
        <View style={styles.buttonContainer}>
          <Button
            type={ButtonType.Success}
            size={ButtonSize.Medium}
            display={ButtonDisplay.Full}
            text={'Log in'}
            onPress={handleLogin}
            disabled={!validPhoneNumber || !buttonEnabled}
          />

          <TextButton
            text="Can't log in?"
            type={TextButtonType.Dark}
            variant={TextButtonVariant.Three}
          />
        </View>
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

  headerContainer: {},

  contentContainer: {
    rowGap: Spacing.lg,
  },

  buttonContainer: {
    alignItems: 'center',
    rowGap: Spacing.md,
  },
});

export default LoginScreen;
