import * as React from 'react';
import {
  View,
  Text,
  TextInput,
  StyleSheet,
  Linking,
  Image,
  ScrollView,
} from 'react-native';

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
import TextInputSmall, { InputType } from '../../components/TextInputSmall';
import { Spacing } from '../../styles/SpacingStyles';
import Checkbox from '../../components/Checkbox';
import Hyperlink from '../../components/Hyperlink';
import KeyboardAvoidingContainer from '../../components/KeyboardAvoidingContainer';
import KeyboardAwareContainer from '../../components/testing/KeyboardAwareContainer';
import DateOfBirthInput from '../../components/auth/DateOfBirthInput';

// TODO insert hyperlinks for terms of service and privacy policy
// TODO setup checkbox onPress events

type SignupProps = StackScreenProps<AuthStackParamList, 'Signup'>;

const SignupScreen = ({ navigation }: SignupProps) => {
  const [PhoneNumber, setPhoneNumber] = React.useState('');
  const [Email, setEmail] = React.useState('');
  const [Name, setName] = React.useState('');
  const [DateOfBirth, setDateOfBirth] = React.useState(new Date());

  const [isButtonEnabled, setButtonEnabled] = React.useState(true);

  // TODO when all fields marked with a asterisk are filled out correctly, button should be enabled
  // TODO add proper validation for all fields
  const [isFormValid, setIsFormValid] = React.useState(false);

  const [agreesToTerms, setAgreesToTerms] = React.useState(false);
  const [agreesToPrivacy, setAgreesToPrivacy] = React.useState(false);

  // const validateForm = () => {
  //   const isNameValid = Name.length > 0;
  //   const areCheckboxesChecked = agreesToTerms && agreesToPrivacy;

  //   setIsFormValid(isNameValid && areCheckboxesChecked);
  // };

  // React.useEffect(() => {
  //   validateForm();
  // }, [Name, agreesToTerms, agreesToPrivacy]);

  function handleSignup() {
    const validateForm = () => {
      const isNameValid = Name.length > 0;
      const areCheckboxesChecked = agreesToTerms && agreesToPrivacy;

      setIsFormValid(isNameValid && areCheckboxesChecked);
    };

    React.useEffect(() => {
      validateForm();
    }, [Name, agreesToTerms, agreesToPrivacy]);

    signup({ PhoneNumber, Email, Name, DateOfBirth })
      .then(navigate)
      .finally(() => setButtonEnabled(true));
  }

  // function handleSignup() {
  //   setButtonEnabled(false);

  //   signup({ PhoneNumber, Email, Name, DateOfBirth })
  //     .then(navigate)
  //     .finally(() => setButtonEnabled(true));
  // }

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
    <KeyboardAvoidingContainer>
      <ScrollView
        keyboardShouldPersistTaps="handled"
        contentContainerStyle={[styles.container, globalStyles.baseContainer]}
        overScrollMode="never"
        showsVerticalScrollIndicator={false}>
        <Image
          source={require('../../assets/illustrations/temp/illustration-placeholder.png')}
          style={[globalStyles.illustrationMedium, styles.image]}
          resizeMode="contain"
        />
        <View style={styles.inputSection}>
          <TextInputSmall
            type={InputType.FirstName}
            label="First name"
            value={Name}
            onChangeText={setName}
            inputMode="text"
            maxLength={256}
            required
            description="Your name will be public and visible to all users."
          />
          <TextInputSmall
            type={InputType.Email}
            label="Email"
            value={Email}
            onChangeText={setEmail}
            autoComplete="email"
            inputMode="email"
            maxLength={256}
            recommended
            description="We recommend binding an email address to your account in case you change your phone number."
          />
          {/* <TextInputSmall
          label="Date of Birth"
          value={DateOfBirth}
          onChangeText={setDateOfBirth}
          placeholder="MM/DD/YYYY"
          inputMode="numeric"
          maxLength={8}
          required
          description="You must be 18 years or older to use Sparrow. Your date of birth will not be visible to other users."
        /> */}
          <DateOfBirthInput />
        </View>
        <View style={styles.checkboxSection}>
          <View style={styles.checkboxInnerSection}>
            <Checkbox
              text="I agree to Sparrow's Terms of Service"
              onPress={() => setAgreesToTerms(!agreesToTerms)}
            />
            <Checkbox
              text="I agree to Sparrow's Privacy Policy"
              onPress={() => setAgreesToPrivacy(!agreesToPrivacy)}
            />
          </View>
          <Text style={globalStyles.textDark}>
            You can go over Sparrow's{' '}
            <Hyperlink
              text="Terms of service"
              onPress={() => Linking.openURL('http://google.com')}
            />{' '}
            and{' '}
            <Hyperlink
              text="Privacy Policy"
              onPress={() => Linking.openURL('http://google.com')}
            />{' '}
            on our official website.
          </Text>
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
          text={'Continue'}
          onPress={navigate}
          disabled={!isFormValid}
        />
      </ScrollView>
    </KeyboardAvoidingContainer>
  );
};

// ! ||--------------------------------------------------------------------------------||
// ! ||                                     Styles                                     ||
// ! ||--------------------------------------------------------------------------------||
const styles = StyleSheet.create({
  container: {
    // alignItems: 'center',

    // TODO Might have to use justifyContent: 'flex-end' instead for KeyboardAvoidingContainer to work
    // justifyContent: 'center',
    justifyContent: 'flex-end',
    // paddingBottom: 48,

    flex: 1,
  },

  image: {
    marginBottom: Spacing.xl,
    alignSelf: 'center',
  },

  inputSection: {
    rowGap: Spacing.sm,
  },

  checkboxSection: {
    paddingVertical: Spacing.lg,
    rowGap: Spacing.sm,
    alignSelf: 'stretch',
  },

  checkboxInnerSection: {
    rowGap: Spacing.md,
  },
});

export default SignupScreen;
