import * as React from 'react';
import { View, Text, TextInput, StyleSheet, Image } from 'react-native';
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
import { Spacing } from '../../styles/SpacingStyles';
import TextInputSmall, { InputType } from '../../components/TextInputSmall';
import TextButton, {
  TextButtonType,
  TextButtonVariant,
} from '../../components/TextButton';
import ExampleScreen from '../../components/testing/ExampleScreen';

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
    <View style={[styles.container, globalStyles.baseContainer]}>
      {/* <Text>
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
      /> */}

      <ExampleScreen />

      {/* TODO uncomment starting from what's below */}

      {/* <View style={styles.headerContainer}>
        <Image
          source={require('../../assets/illustrations/temp/illustration-placeholder.png')}
          style={globalStyles.illustrationLarge}
          resizeMode="contain"
        />
      </View>

      <View style={styles.contentContainer}>
        <Text
          style={[
            globalStyles.bodyTextOne,
            globalStyles.textDark,
            { textAlign: 'center' },
          ]}>
          Enter the 4-digit code we sent to your number ending with 404.
        </Text>
        <View style={styles.buttonContainer}>
          <Button
            type={ButtonType.Success}
            size={ButtonSize.Medium}
            display={ButtonDisplay.Full}
            text={'Verify & Continue'}
            onPress={null}
            disabled={!buttonEnabled}
          />

          <TextButton
            text="I haven't received a code"
            type={TextButtonType.Dark}
            variant={TextButtonVariant.Three}
            onPress={null}
          /> */}
      {/* </View>
      </View> */}
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

export default VerifyScreen;
