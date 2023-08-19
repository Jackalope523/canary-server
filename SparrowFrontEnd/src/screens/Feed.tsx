import * as React from 'react';
import { View, Text } from 'react-native';

// Testing component styling
import TopNavbarDefaultTitled from '../components/organisms/TopNavbarDefaultTitled';
import TopNavbarEdit from '../components/organisms/TopNavbarEdit';
import TopNavbarEditSelected from '../components/organisms/TopNavbarEditSelected';
import TopNavbarFavorite from '../components/organisms/TopNavbarFavorite';
import TopNavbarOptions from '../components/organisms/TopNavbarOptions';

const FeedScreen = () => {
    return (
        <View>
            <Text>Feed</Text>
            <Text>Used as temporary styling testing screen</Text>
            <TopNavbarDefaultTitled />
            <TopNavbarFavorite />
            <TopNavbarOptions />
            <TopNavbarEdit />
            <TopNavbarEditSelected />
        </View>
    );
};

export default FeedScreen