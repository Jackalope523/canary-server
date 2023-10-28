import * as React from 'react';
import { View, Text } from 'react-native';
import { StackNavigationProp, StackScreenProps } from '@react-navigation/stack';
import { AuthStackParamList } from '../../components/atoms/types';

type VerifyProps = StackScreenProps<AuthStackParamList, 'Verify'>;

const VerifyScreen = ({navigation}: VerifyProps) => {
    return (
        <View>
            <Text>Account</Text>
        </View>
    );
};

export default VerifyScreen