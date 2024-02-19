import * as React from 'react';
import { View, Text, StyleSheet, Image } from 'react-native';
import { StackNavigationProp, StackScreenProps } from '@react-navigation/stack';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { AuthStackParamList } from '../../../components/atoms/types';

import { getAccount } from '.././accountPigeon';
import { initialiseAxiosSession } from '../../../lib/axios';

import Button, {
  ButtonType,
  ButtonSize,
  ButtonDisplay,
} from '../../../components/Button';

import { globalStyles } from '../../../styles/GlobalStyles';
import { Spacing } from '../../../styles/SpacingStyles';

// EXAMPLE SCREEN
import ExampleScreen from '../../../components/testing/ExampleScreen';
import Highlight, { HighlightType } from '../../../components/Highlight';

type IntroProps = StackScreenProps<AuthStackParamList, 'Intro'>;

const IntroScreen = ({ navigation }: IntroProps) => {
  const [buttonEnabled, setButtonEnabled] = React.useState(true);

  // TODO replace with user's name
  let name = 'User';

  function continueButton() {
    navigation.navigate('Q1');
  }

  // TODO <Pressable> text button may need to be made into a component named TextButton or something alike
  return (
    <View style={[globalStyles.baseContainer, styles.container]}>
      <View style={styles.contentContainer}>
        <View style={styles.headerContainer}>
          <Text
            style={[
              globalStyles.headingTextOne,
              globalStyles.textDark,
              styles.text,
            ]}>
            Let's get to know
            <>
              <Highlight type={HighlightType.Fuchsia}>{name}</Highlight>
            </>
          </Text>
        </View>

        {/* <ExampleScreen /> */}

        <Image
          source={require('../../../assets/illustrations/temp/illustration-placeholder.png')}
          style={globalStyles.illustrationMedium}
          resizeMode="contain"
        />
      </View>

      <View style={styles.bottomContainer}>
        <Text
          style={[
            globalStyles.bodyTextOne,
            globalStyles.textDark,
            styles.text,
          ]}>
          To personalize Sparrow to match your interests, we need to learn more
          about you. It won't take more than
          <Highlight type={HighlightType.Dark}>2 minutes</Highlight>.
        </Text>
        <Button
          type={ButtonType.Success}
          size={ButtonSize.Medium}
          display={ButtonDisplay.Full}
          text={'Begin quiz'}
          onPress={continueButton}
          disabled={!buttonEnabled}
        />
      </View>
    </View>
  );
};

export default IntroScreen;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'space-between',
    alignItems: 'center',
  },

  text: {
    textAlign: 'center',
  },

  contentContainer: {
    alignItems: 'center',
    rowGap: Spacing.lg,
  },

  headerContainer: {},

  bottomContainer: {
    // alignItems: 'center',
    width: '100%',
    rowGap: Spacing.lg,
  },

  // buttonContainer: {
  //   alignItems: 'center',
  //   width: '100%',
  //   rowGap: Spacing.md,
  // },
});
