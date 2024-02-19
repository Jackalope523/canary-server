import * as React from 'react';
import { View, StyleSheet } from 'react-native';
import { StackNavigationProp } from '@react-navigation/stack';
import { AuthStackParamList } from '../../../components/atoms/types';

import RadioSurveyScreen from '../../../components/survey/RadioSurveyScreen';
import Highlight, { HighlightType } from '../../../components/Highlight';

// Props
interface Q2Props {
  navigation: StackNavigationProp<AuthStackParamList>;
}

const Q2Screen: React.FC<Q2Props> = ({ navigation }) => {
  return (
    <View style={styles.container}>
      <RadioSurveyScreen
        title={
          <>
            Which activity sounds more
            <Highlight type={HighlightType.Fuchsia}>fun to you</Highlight>?
          </>
        }
        options={[
          'Going on a group hike',
          'Playing basketball',
          'Playing a board game with a small group',
          'Partying',
          'Wine tasting',
        ]}
        navigation={navigation}
        navigateTo="Q3"
      />
    </View>
  );
};

export default Q2Screen;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    // justifyContent: 'space-between',
    // alignItems: 'center',
  },
});
