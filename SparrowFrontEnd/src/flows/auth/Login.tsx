import * as React from 'react';
import { View, Text } from 'react-native';
import { StackNavigationProp, StackScreenProps } from '@react-navigation/stack';
import { AuthStackParamList } from '../../components/atoms/types';

type LoginProps = StackScreenProps<AuthStackParamList, 'Login'>;

const LoginScreen = ({navigation}: LoginProps) => {
    return (
        <View>
            <Text>Account</Text>
        </View>
    );
};

export default LoginScreen