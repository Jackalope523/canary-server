import * as React from 'react';
import { View, Text, TextInput, ScrollView, StyleSheet, Image, Dimensions } from 'react-native';
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
import TextButton, { TextButtonType, TextButtonVariant } from '../../components/TextButton';

// TODO fix layout - issue is possibly because of flexbox
/*

Fix 1: Remove flex: 1 from textInputSmall component but then it messes up the whole component;
imo the issue lies somewhere within a component that's not using flexbox in the same manner as the textInputSmall component,
possibly there's no flex: 1. Investigate Button.tsx and TextButton.tsx

Fix 2: Wrap textInputSmall again somehow?

*/

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
            <View style={styles.contentWrapper}>
              <Text style={[globalStyles.headingTextTwo, globalStyles.textDark]}>
                Sparrow
              </Text>
              <Image
              source={require('../../assets/illustrations/temp/illustration-placeholder.png')}
              style={[globalStyles.illustrationMedium, styles.image]}
              resizeMode="contain"
              />
            </View>
            <View style={styles.buttonWrapper}>
              <TextInputSmall
                type={InputType.PhoneNumber}
                label="Phone Number"
                value={PhoneNumber}
                onChangeText={setPhoneNumber}
                inputMode="tel"
                maxLength={15}
                required
              />
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

            {/* <Image
              source={require('../../assets/illustrations/temp/illustration-placeholder.png')}
              style={[globalStyles.illustrationMedium, styles.image]}
              resizeMode="contain"
            />

          <View style={styles.contentWrapper}>
            <View style={{flex: 1}}>
            <TextInputSmall
              type={InputType.PhoneNumber}
              label="Phone Number"
              value={PhoneNumber}
              onChangeText={setPhoneNumber}
              inputMode="tel"
              maxLength={15}
              required
            />
            </View>

            <View>
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
          </View> */}

      {/* <TextInput
        value={PhoneNumber}
        onChangeText={setPhoneNumber}
        keyboardType="phone-pad"
      /> */}
      {/* <Button
        type={ButtonType.PrimaryDark}
        size={ButtonSize.Large}
        display={ButtonDisplay.Contained}
        text={'Login'}
        onPress={navigate}
        disabled={!buttonEnabled}
      />
      <Button
        type={ButtonType.PrimaryDark}
        size={ButtonSize.Large}
        display={ButtonDisplay.Contained}
        text={"Can't log in?"}
        disabled={!buttonEnabled}
      /> */}
        </View>
  );
};

// ! ||--------------------------------------------------------------------------------||
// ! ||                                     Styles                                     ||
// ! ||--------------------------------------------------------------------------------||
const styles = StyleSheet.create({
  container: {
    // flex: 1,
    // justifyContent: 'space-between',
    // alignItems: 'center',

    // justifyContent: 'flex-end',
    flex: 1,

    flexDirection: 'column',
    alignItems: 'center',
  },

  contentWrapper: {
    flex: 1,
    alignItems: 'center',
    rowGap: Spacing.lg,
  },

  buttonWrapper: {
    alignItems: 'center',
    // width: '100%',
    rowGap: Spacing.md,

    flex: 1,
  },
});

export default LoginScreen;