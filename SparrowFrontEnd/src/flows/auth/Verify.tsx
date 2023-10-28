import * as React from 'react';
import { View, Text, TextInput } from 'react-native';
import { StackScreenProps } from '@react-navigation/stack';

import { Colors } from '../../styles/Colors';
import { globalStyles } from '../../styles/Global';

import { AuthStackParamList } from '../../components/atoms/types';
import Button from '../../components/atoms/Button';

import { verify } from './accountPigeon';

type VerifyProps = StackScreenProps<AuthStackParamList, 'Verify'>;

const VerifyScreen = ({route, navigation}: VerifyProps) => {
    const [Code, setCode] = React.useState('');
    const [errorText, setErrorText] = React.useState('');
    const [buttonEnabled, setButtonEnabled] = React.useState(true);
    
    function handleVerify() {
      setButtonEnabled(false);
      setErrorText('');
      Promise.resolve(verify({ PhoneNumber: route.params.PhoneNumber, Code }))
      .then(route.params.Forward)
      .catch(() => setErrorText('Incorrect code'))
      .finally(() => setButtonEnabled(true));
    }

    return(
        <View>
            <Text>Enter the 6-digit code we sent to your number ending in 
                {route.params.PhoneNumber.substring(route.params.PhoneNumber.length - 4)}.</Text>
            <Text style={{color: Colors.red400}}>{errorText}</Text>
            <TextInput
                value={Code}
                onChangeText={setCode}
                placeholder='000000'
                keyboardType='number-pad'
                maxLength={6}
                />
            <Button
                btnText={'Verify & Continue'}
                btnIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonPrimary, globalStyles.buttonFull]}
                btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonFull, globalStyles.buttonPrimaryLight]}
                btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                btnActiveIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                onPress={handleVerify}
                disabled={!buttonEnabled}
                />
            <Button
                btnText={'I DIDN\'T RECEIVE A CODE'}
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

export default VerifyScreen