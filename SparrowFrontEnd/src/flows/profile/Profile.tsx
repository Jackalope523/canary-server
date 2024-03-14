import * as React from 'react';
import { View, Text } from 'react-native';
import { StackScreenProps } from '@react-navigation/stack';

import { Colors } from '../../styles/ColorStyles';
import { globalStyles } from '../../styles/GlobalStyles';

import {
  AccountStackParamList,
  BottomTabParamList,
} from '../../components/atoms/types';

import Button, {
  ButtonDisplay,
  ButtonType,
  ButtonSize,
} from '../../components/Button';
import { getAccount, userShard } from '../auth/accountPigeon';

/*

// TODO [!!!] IMPORTANT [!!!]

Before beginning work on the Account screen:

1. There's two account screens. Delete one of the screens and pick ONE COMMON
name for an account screen, either Profile or Account

2. Update everything related to the Account screen in MainContainer.tsx
to use the selected name

3. Update everything related to the Account screen in types.tsx

4. Rename types.tsx to something more appropriate and less general

5. In MainContainer.tsx, add the cardStyle prop and styles.cardStyle style
to the Account Stack.Navigator

*/

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
        type={ButtonType.PrimaryDark}
        size={ButtonSize.ExtraSmall}
        display={ButtonDisplay.Contained}
        text="Settings"
        onPress={() => navigation.navigate('Account')}
      />
      <Text>Profile</Text>
    </View>
  );
};

export default ProfileScreen;
