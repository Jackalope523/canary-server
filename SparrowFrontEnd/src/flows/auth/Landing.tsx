import * as React from 'react';
import { View, Text, Pressable, StyleSheet, Image } from 'react-native';
import { StackNavigationProp, StackScreenProps } from '@react-navigation/stack';
import AsyncStorage from '@react-native-async-storage/async-storage';

import Button, {
  ButtonType,
  ButtonSize,
  ButtonDisplay,
} from '../../components/Button';
import { AuthStackParamList } from '../../components/atoms/types';

import { getAccount } from './accountPigeon';
import { initialiseAxiosSession } from '../../lib/axios';
import { globalStyles } from '../../styles/GlobalStyles';
import TextButton, {
  TextButtonType,
  TextButtonVariant,
} from '../../components/TextButton';
import { Spacing } from '../../styles/SpacingStyles';
import TextInputSmall from '../../components/TextInputSmall';

type LandingProps = StackScreenProps<AuthStackParamList, 'Landing'>;

const LandingScreen = ({ navigation }: LandingProps) => {
  const [buttonEnabled, setButtonEnabled] = React.useState(true);

  // This should attempt to log the user in to their account before doing anything else
  function tryTokenLogin() {
    setButtonEnabled(false);
    AsyncStorage.getItem('token')
      .then((token) => {
        if (token != null) {
          initialiseAxiosSession(token);
          getAccount()
            .then(() => navigation.replace('Landing'))
            .catch(() => console.log('login failed with token'))
            .finally(() => setButtonEnabled(true));
        }
      })
      .catch(() => setButtonEnabled(true));
  }

  function signupButton() {
    navigation.navigate('Signup');
  }

  function loginButton() {
    navigation.navigate('Login');
  }

  // TODO <Pressable> text button may need to be made into a component named TextButton or something alike
  return (
    <View style={[globalStyles.baseContainer, styles.container]}>
      <View style={styles.contentWrapper}>
        <Text style={[globalStyles.headingTextTwo, globalStyles.textDark]}>
          Sparrow
        </Text>
        <Image
          source={require('../../assets/illustrations/temp/illustration-placeholder.png')}
          style={globalStyles.illustrationFull}
          resizeMode="contain"
        />
      </View>
      <View style={styles.buttonWrapper}>
        <Button
          type={ButtonType.Success}
          size={ButtonSize.Medium}
          display={ButtonDisplay.Full}
          text={'Sign up'}
          onPress={signupButton}
          disabled={!buttonEnabled}
        />
        <TextButton
          type={TextButtonType.Dark}
          variant={TextButtonVariant.Three}
          text="Or log in"
          onPress={loginButton}
          disabled={!buttonEnabled}
        />
      </View>
    </View>
  );
};

export default LandingScreen;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'space-between',
    alignItems: 'center',
  },

  contentWrapper: {
    alignItems: 'center',
    rowGap: Spacing.lg,
  },

  buttonWrapper: {
    alignItems: 'center',
    width: '100%',
    rowGap: Spacing.md,
  },
});
