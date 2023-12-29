import * as React from 'react';
import {
  View,
  Text,
  TextInput,
  StyleSheet,
  Linking,
  Image,
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
import TextInputSmall from '../../components/TextInputSmall';
import { Spacing } from '../../styles/SpacingStyles';
import Checkbox from '../../components/Checkbox';
import Hyperlink from '../../components/Hyperlink';
import { ScrollView } from 'react-native-gesture-handler';

// TODO insert hyperlinks for terms of service and privacy policy
// TODO setup checkbox onPress events

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
    <ScrollView
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
          label="First name"
          value={Name}
          onChangeText={setName}
          inputMode="text"
          required
          description="Your name will be public and visible to all users."
        />
        <TextInputSmall
          label="Email"
          value={Email}
          onChangeText={setEmail}
          autoComplete="email"
          inputMode="email"
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
      </View>
      <View style={styles.checkboxSection}>
        <Checkbox text="I am 18 years or older" onPress={null} />
        <Checkbox text="I agree to Sparrow's Terms of Service" onPress={null} />
        <View style={styles.checkboxInnerSection}>
          <Checkbox text="I agree to Sparrow's Privacy Policy" onPress={null} />
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
    </ScrollView>
  );
};

// ! ||--------------------------------------------------------------------------------||
// ! ||                                     Styles                                     ||
// ! ||--------------------------------------------------------------------------------||
const styles = StyleSheet.create({
  container: {
    alignItems: 'center',
    justifyContent: 'center',
    paddingBottom: 48,
  },

  image: {
    marginBottom: Spacing.xl,
  },

  inputSection: {
    rowGap: Spacing.sm,
  },

  checkboxSection: {
    paddingVertical: Spacing.lg,
    rowGap: Spacing.md,
  },

  checkboxInnerSection: {
    rowGap: Spacing.sm,
  },
});

export default SignupScreen;
