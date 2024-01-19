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

type Q1Props = StackScreenProps<AuthStackParamList, 'Q1'>;

const Q1Screen = ({ navigation }: Q1Props) => {
  const [buttonEnabled, setButtonEnabled] = React.useState(true);

  function signupButton() {
    navigation.navigate('Q2');
  }

  // TODO <Pressable> text button may need to be made into a component named TextButton or something alike
  return (
    <View style={[globalStyles.baseContainer, styles.container]}>
      <View style={styles.contentWrapper}>
        <Text style={[globalStyles.headingTextTwo, globalStyles.textDark]}>
          Q1 SCREEN
        </Text>
        {/* <Image
          source={require('../../assets/illustrations/temp/illustration-placeholder.png')}
          style={globalStyles.illustrationFull}
          resizeMode="contain"
        /> */}
      </View>
      <View style={styles.buttonWrapper}>
        <Button
          type={ButtonType.Success}
          size={ButtonSize.Medium}
          display={ButtonDisplay.Full}
          text={'TEXT HERE'}
          onPress={signupButton}
          disabled={!buttonEnabled}
        />
      </View>
    </View>
  );
};

export default Q1Screen;

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

  buttonWrapper: {
    alignItems: 'center',
    width: '100%',
    rowGap: Spacing.md,
  },
});
