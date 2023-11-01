import * as React from 'react';
import { View, Text } from 'react-native';
import { StackNavigationProp, StackScreenProps } from '@react-navigation/stack';
import AsyncStorage from '@react-native-async-storage/async-storage';

import { Colors } from '../../styles/Colors';
import { globalStyles } from '../../styles/Global';

import Button from '../../components/atoms/Button';
import { AuthStackParamList } from '../../components/atoms/types';

import { getAccount } from './accountPigeon';
import { initialiseAxiosSession } from '../../lib/axios';

type LandingProps = StackScreenProps<AuthStackParamList, 'Landing'>;

const LandingScreen = ({navigation}: LandingProps) => {
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

    return (
        <View>
            <Text>Sparrow</Text>
            <Button
                btnText={'Sign up'}
                btnIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonPrimary, globalStyles.buttonFull]}
                btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonFull, globalStyles.buttonPrimaryLight]}
                btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                btnActiveIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                onPress={signupButton}
                disabled={!buttonEnabled}
                />
            <Button
                btnText={'OR LOG IN'}
                btnIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonPrimary, globalStyles.buttonFull]}
                btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonFull, globalStyles.buttonPrimaryLight]}
                btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                btnActiveIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                onPress={loginButton}
                disabled={!buttonEnabled}
                />
        </View>
    );
};

export default LandingScreen;