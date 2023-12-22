import * as React from 'react';
import { View, Text, TextInput, StyleSheet } from 'react-native';
import { StackScreenProps } from '@react-navigation/stack';
import DatePicker from 'react-native-date-picker';

import { Colors } from '../../styles/ColorStyles';
import { globalStyles } from '../../styles/GlobalStyles';

import { AuthStackParamList } from '../../components/atoms/types';
import Button, {
  ButtonType,
  ButtonSize,
  ButtonDisplay,
} from '../../components/Button';

import { signup } from './accountPigeon';
import TextInputSmall from '../../components/TextInputSmall';
import { Spacing } from '../../styles/SpacingStyles';

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
    <View style={globalStyles.baseContainer}>
      <View style={styles.inputSection}>
        <TextInputSmall
          label="Name"
          value={Name}
          onChangeText={setName}
          required
          description="Your name will be public and visible to all users."
        />
        <TextInputSmall
          label="Email"
          value={Email}
          onChangeText={setEmail}
          autoComplete="email"
          recommended
          description="We recommend binding an email address to your account in case you change your phone number."
        />
        <TextInputSmall
          label="Date of Birth"
          value={DateOfBirth}
          onChangeText={setDateOfBirth}
          placeholder="MM/DD/YYYY"
          required
          description="You must be 18 years or older to use Sparrow. Your date of birth will not be visible to other users."
        />
        <TextInputSmall
          label="Disabled example"
          placeholder="MM/DD/YYYY"
          recommended
          disabled
        />
      </View>
      {/* <Text>Date of Birth</Text>
      <DatePicker
        date={DateOfBirth}
        onDateChange={setDateOfBirth}
        mode="date"
        timeZoneOffsetInMinutes={0}
        maximumDate={new Date()}
      /> */}
      <Button
        type={ButtonType.Success}
        size={ButtonSize.Medium}
        display={ButtonDisplay.Full}
        btnText={'Continue'}
        onPress={navigate}
        disabled={!buttonEnabled}
      />
    </View>
  );
};

// ! ||--------------------------------------------------------------------------------||
// ! ||                                     Styles                                     ||
// ! ||--------------------------------------------------------------------------------||
const styles = StyleSheet.create({
  inputSection: {
    rowGap: Spacing.sm,
  },
});

export default SignupScreen;
