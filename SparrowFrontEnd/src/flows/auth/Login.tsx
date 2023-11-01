import * as React from 'react';
import { View, Text, TextInput } from 'react-native';
import { StackScreenProps } from '@react-navigation/stack';

import { Colors } from '../../styles/Colors';
import { globalStyles } from '../../styles/Global';

import { AuthStackParamList } from '../../components/atoms/types';
import Button from '../../components/atoms/Button';

import { login } from './accountPigeon';

type LoginProps = StackScreenProps<AuthStackParamList, 'Login'>;

const LoginScreen = ({navigation}: LoginProps) => {
    const [PhoneNumber, setPhoneNumber] = React.useState('');
    const [buttonEnabled, setButtonEnabled] = React.useState(true);
    
    function handleLogin() {
        setButtonEnabled(false);
        
        login({ PhoneNumber })
        .then(navigate)
        .finally(() => setButtonEnabled(true));
    }

    function navigate() {
        navigation.navigate('Verify',
        {
            PhoneNumber,
            Forward: () => { navigation.navigate('Continue',
            {
                Message: 'Welcome back.',
                Forward: () => navigation.replace('Main')
            })}
        });
    }

    return(
        <View>
            <Text>Phone Number</Text>
            <TextInput
                value={PhoneNumber}
                onChangeText={setPhoneNumber}
                keyboardType='phone-pad' />
            <Button
                btnText={'Login'}
                btnIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonPrimary, globalStyles.buttonFull]}
                btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonFull, globalStyles.buttonPrimaryLight]}
                btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                btnActiveIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                onPress={navigate}
                disabled={!buttonEnabled}
                />
            <Button
                btnText={'CAN\'T LOG IN?'}
                btnIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonPrimary, globalStyles.buttonFull]}
                btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonFull, globalStyles.buttonPrimaryLight]}
                btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                btnActiveIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                disabled={!buttonEnabled}
                />
        </View>
    );
};

export default LoginScreen