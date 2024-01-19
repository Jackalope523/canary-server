import * as React from 'react';
import { View, Text, Pressable, StyleSheet, Image } from 'react-native';
import { StackNavigationProp, StackScreenProps } from '@react-navigation/stack';
import AsyncStorage from '@react-native-async-storage/async-storage';

import Button, { ButtonType, ButtonSize, ButtonDisplay } from '../Button';
import { AuthStackParamList } from '../atoms/types';

import { getAccount } from '../../flows/auth/accountPigeon';
import { initialiseAxiosSession } from '../../lib/axios';
import { globalStyles } from '../../styles/GlobalStyles';
import TextButton, { TextButtonType, TextButtonVariant } from '../TextButton';
import { Spacing } from '../../styles/SpacingStyles';
import TextInputSmall from '../TextInputSmall';
import RadioButton from '../RadioButton';
import { Colors } from '../../styles/ColorStyles';
import Highlight, { HighlightSize, HighlightType } from '../Highlight';

// interface RadioSurveyScreenProps extends StackScreenProps<AuthStackParamList, 'Q2'> {}

// Types
interface RadioSurveyScreenProps {
  navigation: StackNavigationProp<AuthStackParamList>;
  // navigateTo: [screen: string];

  navigateTo: any;

  title?: string | React.ReactNode;
  options?: string[];

  // onPress: (item: string | GestureResponderEvent) => void;
  // buttonText: string[];;
}

const RadioSurveyScreen: React.FC<RadioSurveyScreenProps> = ({
  navigation,
  navigateTo,
  options,
  title = 'NULL',
}) => {
  const [buttonEnabled, setButtonEnabled] = React.useState(true);

  // TODO set navigate to next QUESTION screen
  // TODO fix TS error
  function continueButton() {
    navigation.navigate(navigateTo);
  }

  // TODO <Pressable> text button may need to be made into a component named TextButton or something alike
  return (
    <View style={[globalStyles.baseContainer, styles.container]}>
      <View style={styles.headerContainer}>
        <Text style={[globalStyles.headingTextTwo, globalStyles.textDark]}>
          {title}
        </Text>
      </View>
      <View style={styles.contentContainer}>
        <View style={styles.radioContainer}>
          <RadioButton
            onPress={(item) => console.log(item)}
            buttonText={options}
          />
        </View>
        <View style={styles.buttonContainer}>
          <Button
            type={ButtonType.Success}
            size={ButtonSize.Medium}
            display={ButtonDisplay.Full}
            text={'Continue'}
            onPress={continueButton}
            disabled={!buttonEnabled}
          />
        </View>
      </View>
    </View>
  );
};

export default RadioSurveyScreen;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'space-between',

    // alignItems: 'center',
  },

  contentContainer: {
    gap: Spacing.lg,
  },

  headerContainer: {
    marginTop: Spacing.lg,
    rowGap: Spacing.lg,
  },

  textHighlight: {
    color: Colors.orange500,
  },

  radioContainer: {},

  buttonContainer: {
    width: '100%',
    rowGap: Spacing.md,
  },
});
