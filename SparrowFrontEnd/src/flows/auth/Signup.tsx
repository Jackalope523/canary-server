import * as React from 'react';
import { View, Text, TextInput } from 'react-native';
import { StackScreenProps } from '@react-navigation/stack';
import DatePicker from 'react-native-date-picker';

import { Colors } from '../../styles/Colors';
import { globalStyles } from '../../styles/Global';

import { AuthStackParamList } from '../../components/atoms/types';
import Button, {
  ButtonType,
  ButtonSize,
  ButtonDisplay,
} from '../../components/Button';

import { signup } from './accountPigeon';

type SignupProps = StackScreenProps<AuthStackParamList, 'Signup'>;

const SignupScreen = ({ navigation }: SignupProps) => {
  const [PhoneNumber, setPhoneNumber] = React.useState('');
  const [Email, setEmail] = React.useState('');
  const [Name, setName] = React.useState('');
  const [DateOfBirth, setDateOfBirth] = React.useState(new Date());
  const [buttonEnabled, setButtonEnabled] = React.useState(true);

  function handleSignup() {
    setButtonEnabled(false);

    signup({ PhoneNumber, Email, Name, DateOfBirth })
      .then(navigate)
      .finally(() => setButtonEnabled(true));
  }

  function navigate() {
    navigation.navigate('Verify', {
      PhoneNumber,
      Forward: () => {
        navigation.navigate('Continue', {
          Message:
            'Your account has been successfully verified. Welcome to Sparrow!',
          Forward: () => navigation.replace('Main'),
        });
      },
    });
  }

  return (
    <View>
      <Text>Phone Number</Text>
      <TextInput
        value={PhoneNumber}
        onChangeText={setPhoneNumber}
        keyboardType="phone-pad"
      />
      <Text>Email</Text>
      <TextInput value={Email} onChangeText={setEmail} />
      <Text>Name</Text>
      <TextInput value={Name} onChangeText={setName} />
      <Text>Date of Birth</Text>
      <DatePicker
        date={DateOfBirth}
        onDateChange={setDateOfBirth}
        mode="date"
        timeZoneOffsetInMinutes={0}
        maximumDate={new Date()}
      />
      {/* <Button
        btnText={'Sign up'}
        btnIconStyle={[
          globalStyles.buttonIconSmall,
          globalStyles.buttonIconSmall.light,
        ]}
        btnStyle={[
          globalStyles.textButtonExtraSmall,
          globalStyles.buttonPrimary,
          globalStyles.buttonFull,
        ]}
        btnTextStyle={[
          globalStyles.textButtonExtraSmall.text,
          globalStyles.textLight,
        ]}
        btnActiveStyle={[
          globalStyles.textButtonExtraSmall,
          globalStyles.buttonFull,
          globalStyles.buttonPrimaryLight,
        ]}
        btnActiveTextStyle={[
          globalStyles.textButtonExtraSmall.text,
          globalStyles.textLight,
        ]}
        btnActiveIconStyle={[
          globalStyles.buttonIconSmall,
          globalStyles.buttonIconSmall.light,
        ]}
        onPress={navigate}
        disabled={!buttonEnabled}
      /> */}

      <Button
        type={ButtonType.PrimaryDark}
        size={ButtonSize.Large}
        display={ButtonDisplay.Contained}
        btnText={'Sign up'}
        onPress={navigate}
        disabled={!buttonEnabled}
      />
    </View>
  );
};

export default SignupScreen;
