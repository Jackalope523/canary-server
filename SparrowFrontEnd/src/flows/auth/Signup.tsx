import * as React from 'react';
import { View, Text, TextInput } from 'react-native';
import { StackScreenProps } from '@react-navigation/stack';
import DatePicker from 'react-native-date-picker';

import { Colors } from '../../styles/Colors';
import { globalStyles } from '../../styles/Global';

import { AuthStackParamList } from '../../components/atoms/types';
import Button from '../../components/atoms/Button';

import { signup } from './accountPigeon';

type SignupProps = StackScreenProps<AuthStackParamList, 'Signup'>;

const SignupScreen = ({navigation}: SignupProps) => {
    const [PhoneNumber, setPhoneNumber] = React.useState('');
    const [Email, setEmail] = React.useState('');
    const [Name, setName] = React.useState('');
    const [DateOfBirth, setDateOfBirth] = React.useState(new Date);
    const [buttonEnabled, setButtonEnabled] = React.useState(true);
    
    function handleSignup() {
        setButtonEnabled(false);
        Promise.resolve(signup({ PhoneNumber, Email, Name, DateOfBirth }))
        .then(() => navigation.navigate('Verify',
        {
            PhoneNumber,
            Forward: () => { navigation.navigate('Continue',
            {
                Message: 'Your account has been successfully verified. Welcome to Sparrow!',
                Forward: () => navigation.replace('Landing')
            })},
        }))
        .finally(() => setButtonEnabled(true));
    }

    return(
        <View>
            <TextInput
                value={PhoneNumber}
                onChangeText={setPhoneNumber}
                keyboardType='phone-pad' />
            <TextInput
                value={Email}
                onChangeText={setEmail} />
            <TextInput
                value={Name}
                onChangeText={setName} />
            <DatePicker
                date={DateOfBirth}
                onDateChange={setDateOfBirth}
                mode='date'
                timeZoneOffsetInMinutes={0}
                maximumDate={new Date()} />
            <Button
                btnText={'Sign up'}
                btnIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonPrimary, globalStyles.buttonFull]}
                btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonFull, globalStyles.buttonPrimaryLight]}
                btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                btnActiveIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                onPress={handleSignup}
                disabled={!buttonEnabled}
                />
        </View>
    );
};

export default SignupScreen