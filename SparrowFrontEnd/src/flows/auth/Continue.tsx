import * as React from 'react';
import { View, Text, TextInput } from 'react-native';
import { StackScreenProps } from '@react-navigation/stack';

import { AuthStackParamList } from '../../components/atoms/types';
import Button, {
  ButtonType,
  ButtonSize,
  ButtonDisplay,
} from '../../components/Button';

type ContinueProps = StackScreenProps<AuthStackParamList, 'Continue'>;

const ContinueScreen = ({ route }: ContinueProps) => {
  return (
    <View>
      <Text>{route.params.Message}</Text>
      <Button
        type={ButtonType.PrimaryDark}
        size={ButtonSize.Large}
        display={ButtonDisplay.Contained}
        btnText="Continue"
        onPress={route.params.Forward}
      />
    </View>
  );
};

export default ContinueScreen;
