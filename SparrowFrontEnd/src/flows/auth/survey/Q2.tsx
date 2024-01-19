import * as React from 'react';
import { View, Text, Pressable, StyleSheet, Image } from 'react-native';
import { StackNavigationProp, StackScreenProps } from '@react-navigation/stack';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { AuthStackParamList } from '../../../components/atoms/types';

import { getAccount } from '.././accountPigeon';
import { initialiseAxiosSession } from '../../../lib/axios';

import RadioSurveyScreen from '../../../components/survey/RadioSurveyScreen';
import Highlight, {
  HighlightSize,
  HighlightType,
} from '../../../components/Highlight';

// Props
interface Q2Props {
  navigation: StackNavigationProp<AuthStackParamList>;

  // onPress: (item: string | GestureResponderEvent) => void;
  // buttonText: string[];
}

const Q2Screen: React.FC<Q2Props> = () => {
  // const [buttonEnabled, setButtonEnabled] = React.useState(true);

  // TODO <Pressable> text button may need to be made into a component named TextButton or something alike
  return (
    <View style={styles.container}>
      <RadioSurveyScreen
        title={
          <>
            Which activity sounds more
            <Highlight
              type={HighlightType.Fuchsia}
              size={HighlightSize.HeadingTextTwo}>
              fun to you
            </Highlight>
            ?
          </>
        }
        options={[
          'Going on a group hike',
          'Playing basketball',
          'Playing a board game with a small group',
          'Partying',
          'Wine tasting',
        ]}
        // TODO replace with Q3
        navigateTo={'Q1'}
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
