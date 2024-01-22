import * as React from 'react';
import { View, StyleSheet } from 'react-native';
import { StackNavigationProp } from '@react-navigation/stack';
import { AuthStackParamList } from '../../../components/atoms/types';

import RadioSurveyScreen from '../../../components/survey/RadioSurveyScreen';
import Highlight, { HighlightType } from '../../../components/Highlight';

// Props
interface Q6Props {
  navigation: StackNavigationProp<AuthStackParamList>;
}

const Q6Screen: React.FC<Q6Props> = ({ navigation }) => {
  return (
    <View style={styles.container}>
      <RadioSurveyScreen
        title={
          <>
            How likely are you to try
            <Highlight type={HighlightType.Green}>new experiences</Highlight>?
          </>
        }
        options={[
          'Extremely likely',
          'Somewhat likely',
          'Neutral',
          'Somewhat unlikely',
          'Extremely unlikely',
        ]}
        navigation={navigation}
        navigateTo="Activity"
      />
    </View>
  );
};

export default Q6Screen;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    // justifyContent: 'space-between',
    // alignItems: 'center',
  },
});
