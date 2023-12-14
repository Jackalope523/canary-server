import * as React from 'react';
import { View, Text } from 'react-native';
import { StackScreenProps } from '@react-navigation/stack';

import { Colors } from '../../styles/ColorStyles';
import { globalStyles } from '../../styles/GlobalStyles';

import {
  AccountStackParamList,
  BottomTabParamList,
} from '../../components/atoms/types';

import Button from '../../components/atoms/Button';
import { getAccount, userShard } from '../auth/accountPigeon';

type ProfileProps = StackScreenProps<BottomTabParamList, 'Profile'>;

const ProfileScreen = ({ navigation }: ProfileProps) => {
  const [debugText, setDebugText] = React.useState('');

  function handleGetAccount() {
    if (debugText == '') return;

    getAccount()
      .then((data) => setDebugText(`Name: ${data.Name}`))
      .catch(() => setDebugText('Could not retrieve account info.'));
  }

  handleGetAccount();

  return (
    <View>
      <Button
        btnText={'Settings'}
        btnIconStyle={[
          globalStyles.buttonIconSmall,
          globalStyles.buttonIconSmall.light,
        ]}
        btnStyle={[
          globalStyles.textButtonExtraSmall,
          globalStyles.buttonPrimary,
          globalStyles.buttonFull,
        ]}
        btnTextStyle={[
          globalStyles.textButtonExtraSmall.text,
          globalStyles.textLight,
        ]}
        btnActiveStyle={[
          globalStyles.textButtonExtraSmall,
          globalStyles.buttonFull,
          globalStyles.buttonPrimaryLight,
        ]}
        btnActiveTextStyle={[
          globalStyles.textButtonExtraSmall.text,
          globalStyles.textLight,
        ]}
        btnActiveIconStyle={[
          globalStyles.buttonIconSmall,
          globalStyles.buttonIconSmall.light,
        ]}
        onPress={() => navigation.navigate('Account')}
      />
      <Text>Profile</Text>
    </View>
  );
};

export default ProfileScreen;
