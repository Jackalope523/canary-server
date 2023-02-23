import { BottomTabScreenProps } from '@react-navigation/bottom-tabs';
import React, { useState, useEffect } from 'react';

import
{
  SafeAreaView,
  Text,
  View,
  Button,
  TextInput
}
from 'react-native';

import { RootTabsParamList } from '../../../../App';
import style from '../../../theme/styles';

import { getUserProfile } from '../api/profileAPI';

type ProfileProps = BottomTabScreenProps<RootTabsParamList, 'Profile'>;

export type UserProfile = {
    name: string;
    numberOfFollowers: number;
    reputation: number;
}

export default function ProfileScreen({navigation}: ProfileProps): JSX.Element {
    const [name, setName] = useState('loading');
    const [numberOfFollowers, setNumberOfFollowers] = useState(0);
    const [reputation, setReputation] = useState(0);

    function retrieveUserData() {
        Promise.resolve(getUserProfile())
        .then((profile) => {
            setName(profile.name);
            setNumberOfFollowers(profile.numberOfFollowers);
            setReputation(profile.reputation);
        });
    }

    useEffect(() => retrieveUserData());

    return (
        <SafeAreaView style={style.sectionContainer}>
            <View>
                <Text>User: {name}</Text>
                <Text>Num Followers: {numberOfFollowers}</Text>
                <Text>Reputation: {reputation}</Text>
            </View>
            <View style={style.footer} />
        </SafeAreaView>
    );
}