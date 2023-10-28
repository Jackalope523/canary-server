import * as React from 'react';
import { View, Text } from 'react-native';
import { StackNavigationProp, StackScreenProps } from '@react-navigation/stack';
import { AuthStackParamList } from '../../components/atoms/types';

type SignupProps = StackScreenProps<AuthStackParamList, 'Signup'>;

const SignupScreen = ({navigation}: SignupProps) => {
    return (
        <View>
            <Text>Account</Text>
        </View>
    );
};

export default SignupScreen