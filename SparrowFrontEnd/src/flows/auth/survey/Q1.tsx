import * as React from 'react';
import { View, StyleSheet } from 'react-native';
import { StackNavigationProp } from '@react-navigation/stack';
import { AuthStackParamList } from '../../../components/atoms/types';

import RadioSurveyScreen from '../../../components/survey/RadioSurveyScreen';
import Highlight, { HighlightType } from '../../../components/Highlight';

// Props
interface Q1Props {
  navigation: StackNavigationProp<AuthStackParamList>;
}

const Q1Screen: React.FC<Q1Props> = ({ navigation }) => {
  return (
    <View style={styles.container}>
      <RadioSurveyScreen
        title={
          <>
            My ideal
            <Highlight type={HighlightType.Orange}>hangout group</Highlight>
            consists of...
          </>
        }
        options={['1-3 people', '3-6 people', '6-9 people', '9+ people']}
        navigation={navigation}
        navigateTo="Q2"
      />
    </View>
  );
};

export default Q1Screen;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    // justifyContent: 'space-between',
    // alignItems: 'center',
  },
});
