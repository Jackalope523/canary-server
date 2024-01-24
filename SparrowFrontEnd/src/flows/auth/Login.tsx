import * as React from 'react';
import {
  View,
  Text,
  TextInput,
  ScrollView,
  StyleSheet,
  Image,
  Dimensions,
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
    <View style={[styles.container, globalStyles.baseContainer]}>
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
          value={PhoneNumber}
          onChangeText={setPhoneNumber}
          inputMode="tel"
          maxLength={15}
          required
        />
        <View style={styles.buttonContainer}>
          <Button
            type={ButtonType.Success}
            size={ButtonSize.Medium}
            display={ButtonDisplay.Full}
            text={'Log in'}
            onPress={navigate}
            disabled={!buttonEnabled}
          />

          <TextButton
            text="Can't log in?"
            type={TextButtonType.Dark}
            variant={TextButtonVariant.Three}
            onPress={null}
          />
        </View>
      </View>
    </View>
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
