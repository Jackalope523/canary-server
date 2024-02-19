import * as React from 'react';
import { View, StyleSheet } from 'react-native';
import { StackNavigationProp } from '@react-navigation/stack';
import { AuthStackParamList } from '../../../components/atoms/types';

import RadioSurveyScreen from '../../../components/survey/RadioSurveyScreen';
import Highlight, { HighlightType } from '../../../components/Highlight';

// Props
interface Q4Props {
  navigation: StackNavigationProp<AuthStackParamList>;
}

const Q4Screen: React.FC<Q4Props> = ({ navigation }) => {
  return (
    <View style={styles.container}>
      <RadioSurveyScreen
        title={
          <>
            How often do you feel
            <Highlight type={HighlightType.Rose}>bored</Highlight>
            during the week?
          </>
        }
        options={[
          'Every day',
          '4-6 days a week',
          '2-3 days a week',
          'one day a week',
          'never',
        ]}
        navigation={navigation}
        navigateTo="Q5"
      />
    </View>
  );
};

export default Q4Screen;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    // justifyContent: 'space-between',
    // alignItems: 'center',
  },
});
