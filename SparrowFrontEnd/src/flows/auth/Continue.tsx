import * as React from 'react';
import { View, Text, TextInput } from 'react-native';
import { StackScreenProps } from '@react-navigation/stack';

import { Colors } from '../../styles/Colors';
import { globalStyles } from '../../styles/Global';

import { AuthStackParamList } from '../../components/atoms/types';
import Button from '../../components/atoms/Button';

type ContinueProps = StackScreenProps<AuthStackParamList, 'Continue'>;

const ContinueScreen = ({route, navigation}: ContinueProps) => {
    return(
        <View>
            <Text>{route.params.Message}</Text>
            <Button
                btnText={'Continue'}
                btnIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonPrimary, globalStyles.buttonFull]}
                btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonFull, globalStyles.buttonPrimaryLight]}
                btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                btnActiveIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                onPress={route.params.Forward}
                />
        </View>
    );
};

export default ContinueScreen