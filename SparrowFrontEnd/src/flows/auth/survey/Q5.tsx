import * as React from 'react';
import { View, StyleSheet } from 'react-native';
import { StackNavigationProp } from '@react-navigation/stack';
import { AuthStackParamList } from '../../../components/atoms/types';

import RadioSurveyScreen from '../../../components/survey/RadioSurveyScreen';
import Highlight, { HighlightType } from '../../../components/Highlight';

// Props
interface Q5Props {
  navigation: StackNavigationProp<AuthStackParamList>;
}

const Q5Screen: React.FC<Q5Props> = ({ navigation }) => {
  return (
    <View style={styles.container}>
      <RadioSurveyScreen
        title={
          <>
            How often is your daily schedule
            <Highlight type={HighlightType.Orange}>flexible</Highlight>?
          </>
        }
        options={['Always', 'Very often', 'Sometimes', 'Rarely', 'Never']}
        navigation={navigation}
        navigateTo="Q6"
      />
    </View>
  );
};

export default Q5Screen;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    // justifyContent: 'space-between',
    // alignItems: 'center',
  },
});
