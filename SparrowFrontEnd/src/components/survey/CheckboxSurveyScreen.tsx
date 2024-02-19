import * as React from 'react';
import { View, Text, StyleSheet } from 'react-native';
import { StackNavigationProp } from '@react-navigation/stack';
import { AuthStackParamList } from '../atoms/types';

import Button, { ButtonType, ButtonSize, ButtonDisplay } from '../Button';
import Checkbox from '../Checkbox';

import { globalStyles } from '../../styles/GlobalStyles';
import { Spacing } from '../../styles/SpacingStyles';
import { Colors } from '../../styles/ColorStyles';

// Types
interface CheckboxSurveyScreenProps {
  navigation: StackNavigationProp<AuthStackParamList>;
  navigateTo: any;

  title?: string | React.ReactNode;
  options?: string[];
}

const CheckboxSurveyScreen: React.FC<CheckboxSurveyScreenProps> = ({
  navigation,
  navigateTo,
  options,
  title = 'NULL',
}) => {
  const [buttonEnabled, setButtonEnabled] = React.useState(false);

  function continueButton() {
    navigation.navigate(navigateTo);
  }

  return (
    <View style={[globalStyles.baseContainer, styles.container]}>
      <View style={styles.headerContainer}>
        <Text style={[globalStyles.headingTextTwo, globalStyles.textDark]}>
          {title}
        </Text>
      </View>
      <View style={styles.contentContainer}>
        <View style={styles.CheckboxContainer}>
          {/* <CheckboxButton
            onPress={(item) => console.log(item)}
            buttonText={options}
          /> */}

          <Checkbox text={options} onPress={() => setButtonEnabled(true)} />
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

export default CheckboxSurveyScreen;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'space-between',
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

  CheckboxContainer: {},

  buttonContainer: {
    width: '100%',
    rowGap: Spacing.md,
  },
});
