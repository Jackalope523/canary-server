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
import OTPInputV2 from '../../components/testing/OTPInputV2';

type VerifyProps = StackScreenProps<AuthStackParamList, 'Verify'>;

const VerifyScreen = ({ route }: VerifyProps) => {
  const codeLength = 4;
  const [code, setCode] = React.useState('');
  const [codeReady, setCodeReady] = React.useState(false);
  
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

  return (
    <View style={[styles.container, globalStyles.baseContainer]}>
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
          )}
          .
        </Text>

        <OTPInputV2 length={4} onChange={() => null} />
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
          onPress={null}
          disabled={!codeReady}
        />
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
