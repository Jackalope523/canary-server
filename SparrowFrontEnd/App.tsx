import React from 'react';

import { NavigationContainer } from '@react-navigation/native';
import { createStackNavigator } from '@react-navigation/stack';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';

import PortalScreen from './src/features/portal/screens/PortalScreen';
import LoginScreen from './src/features/portal/screens/LoginScreen';
import VerifyScreen from './src/features/portal/screens/VerifyScreen';
import RegisterScreen from './src/features/portal/screens/RegisterScreen';

import FeedScreen from './src/features/events/screens/FeedScreen';
import DiscoverScreen from './src/features/events/screens/DiscoverScreen';
import ProfileScreen from './src/features/events/screens/ProfileScreen';
import { Provider } from 'react-redux';
import store from './src/lib/store';

/*
import type {PropsWithChildren} from 'react';
type SectionProps = PropsWithChildren<{
  title: string;
}>;

function Section({children, title}: SectionProps): JSX.Element
{
  const isDarkMode = useColorScheme() === 'dark';
  return (
    <View style={styles.sectionContainer}>
      <Text
        style={[
          styles.sectionTitle,
          {
            color: isDarkMode ? Colors.white : Colors.black,
          },
        ]}>
        {title}
      </Text>
      <Text
        style={[
          styles.sectionDescription,
          {
            color: isDarkMode ? Colors.light : Colors.dark,
          },
        ]}>
        {children}
      </Text>
    </View>
  );
}*/

export type RootStackParamList = {
  Portal: undefined;
  Authentication: undefined;
  Login: undefined;
  Verify: { phoneNumber: string};
  Register: { phoneNumber: string};
  Landing: undefined;
};

export type RootTabsParamList = {
  Feed: undefined;
  Discover: undefined;
  Profile: undefined;
};

const Stack = createStackNavigator<RootStackParamList>();
const Tabs  = createBottomTabNavigator<RootTabsParamList>();

export default function App(): JSX.Element {
  return (
    <NavigationContainer>
      <Stack.Navigator initialRouteName="Portal">
        <Stack.Screen name="Portal" component={PortalScreen}
          options={{headerShown: false}}
        />
        <Stack.Screen name="Authentication" component={Authentication}
          options={{headerShown: false}}
        />
        <Stack.Screen name="Landing" component={Landing}
          options={{headerShown: false}}
        />
      </Stack.Navigator>
    </NavigationContainer>
  );
}

const Authentication = () => {
  return (
    <Stack.Navigator initialRouteName="Login">
      <Stack.Screen name="Login" component={LoginScreen}
        options={{headerShown: false}}
      />
      <Stack.Screen name="Verify" component={VerifyScreen}
        options={{
          title: 'Verify',
          headerStyle: {
            backgroundColor: '#307ecc',
          },
          headerTintColor: '#fff',
          headerTitleStyle: {
            fontWeight: 'bold',
          },
        }}
      />
      <Stack.Screen name="Register" component={RegisterScreen}
        options={{
          title: 'Register',
          headerStyle: {
            backgroundColor: '#307ecc',
          },
          headerTintColor: '#fff',
          headerTitleStyle: {
            fontWeight: 'bold',
          },
        }}
      />
    </Stack.Navigator>
  );
};

const Landing = () => {
  return (
    <Tabs.Navigator initialRouteName='Discover'
      screenOptions={{ headerShown: false }}>      
        <Tabs.Screen name='Feed' component={FeedScreen} />
        <Tabs.Screen name='Discover' component={DiscoverScreen} />
        <Tabs.Screen name='Profile' component={ProfileScreen} />
    </Tabs.Navigator>
  );
};
