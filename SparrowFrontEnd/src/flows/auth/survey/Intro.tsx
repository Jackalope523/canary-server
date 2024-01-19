import * as React from 'react';
import { View, Text, Pressable, StyleSheet, Image } from 'react-native';
import { StackNavigationProp, StackScreenProps } from '@react-navigation/stack';
import AsyncStorage from '@react-native-async-storage/async-storage';

import Button, {
  ButtonType,
  ButtonSize,
  ButtonDisplay,
} from '../../../components/Button';
import { AuthStackParamList } from '../../../components/atoms/types';

import { getAccount } from '.././accountPigeon';
import { initialiseAxiosSession } from '../../../lib/axios';
import { globalStyles } from '../../../styles/GlobalStyles';
import TextButton, {
  TextButtonType,
  TextButtonVariant,
} from '../../../components/TextButton';
import { Spacing } from '../../../styles/SpacingStyles';
import TextInputSmall from '../../../components/TextInputSmall';

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
      <View style={styles.contentWrapper}>
        <View style={styles.headerWrapper}>
          <Text style={[globalStyles.headingTextOne, globalStyles.textDark]}>
            Let's get to know
            <>
              <Highlight type={HighlightType.Fuchsia}>{name}</Highlight>
            </>
          </Text>
        </View>

        {/* <ExampleScreen /> */}

        {/* <Image
          source={require('../../../assets/illustrations/temp/illustration-placeholder.png')}
          style={globalStyles.illustrationFull}
          resizeMode="contain"
        /> */}
        <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
          To personalize Sparrow to match your interests, we need to learn more
          about you. It won't take more than
          <Highlight type={HighlightType.Dark}>2 minutes</Highlight>.
        </Text>
      </View>
      <View style={styles.buttonWrapper}>
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

  contentWrapper: {
    alignItems: 'center',
    rowGap: Spacing.lg,
  },

  headerWrapper: {
    alignItems: 'center',
  },

  buttonWrapper: {
    alignItems: 'center',
    width: '100%',
    rowGap: Spacing.md,
  },
});
