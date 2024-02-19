import * as React from 'react';
import { View, StyleSheet, Text } from 'react-native';
import { StackNavigationProp } from '@react-navigation/stack';
import { AuthStackParamList } from '../../../components/atoms/types';

import RadioSurveyScreen from '../../../components/survey/RadioSurveyScreen';
import Highlight, { HighlightType } from '../../../components/Highlight';
import CheckboxSurveyScreen from '../../../components/survey/CheckboxSurveyScreen';
import ExampleScreen from '../../../components/testing/ExampleScreen';

// Props
interface Q3Props {
  navigation: StackNavigationProp<AuthStackParamList>;
}

const Q3Screen: React.FC<Q3Props> = ({ navigation }) => {
  return (
    <View style={styles.container}>
      <CheckboxSurveyScreen
        title={
          <>
            My ideal
            <Highlight type={HighlightType.Fuchsia}>event</Highlight>
            is
          </>
        }
        options={['Action-packed', 'Relaxing', 'Competitive', 'Cooperative']}
        navigation={navigation}
        navigateTo="Q4"
      />
    </View>
  );
};

export default Q3Screen;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    // justifyContent: 'space-between',
    // alignItems: 'center',
  },
});
