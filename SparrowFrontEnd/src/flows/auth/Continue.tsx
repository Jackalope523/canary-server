import * as React from 'react';
import { View, Text, Image, StyleSheet } from 'react-native';
import { StackScreenProps } from '@react-navigation/stack';
import { AuthStackParamList } from '../../components/atoms/types';
import Button, {
  ButtonType,
  ButtonSize,
  ButtonDisplay,
} from '../../components/Button';

import { globalStyles } from '../../styles/GlobalStyles';
import { Spacing } from '../../styles/SpacingStyles';

type ContinueProps = StackScreenProps<AuthStackParamList, 'Continue'>;

const ContinueScreen = ({ route }: ContinueProps) => {
  return (
    <View style={[styles.container, globalStyles.baseContainer]}>
      <View style={styles.contentContainer}>
        <Image
          source={require('../../assets/illustrations/temp/illustration-placeholder.png')}
          style={globalStyles.illustrationLarge}
          resizeMode="contain"
        />
        <Text style={[globalStyles.bodyTextOne, globalStyles.textDark]}>
          {route.params.Message}
        </Text>
      </View>

      <Button
        type={ButtonType.Success}
        size={ButtonSize.Medium}
        display={ButtonDisplay.Full}
        text={'Continue'}
        onPress={route.params.Forward}
      />
    </View>
  );
};

// ! ||--------------------------------------------------------------------------------||
// ! ||                                     Styles                                     ||
// ! ||--------------------------------------------------------------------------------||
const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'space-between',
  },

  contentContainer: {
    alignItems: 'center',
    rowGap: Spacing.xl,
  },

  text: {
    textAlign: 'center',
  },
});

export default ContinueScreen;
