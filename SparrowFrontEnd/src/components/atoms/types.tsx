import { useNavigation } from '@react-navigation/native';
import type { NativeStackScreenProps } from '@react-navigation/native-stack';

/*


TODO move types to the screens, components, files where they're used and DELETE this file after


*/

export type AppStackParamList = {
  Auth: undefined;
  Main: undefined;
  Account: undefined;
  Survey: undefined;
};

export type AuthStackParamList = {
  Landing: undefined;
  Login: undefined;
  Signup: undefined;
  Verify: { PhoneNumber: string; Forward: () => void };
  Continue: { Message: string; Forward: () => void };
  Main: undefined;
  Intro: undefined;
  RadioSurvey: undefined;
  Q1: undefined;
  Q2: undefined;
  Q3: undefined;
  Q4: undefined;
  Q5: undefined;
  Q6: undefined;
};

export type BottomTabParamList = {
  Activity: undefined;
  Discovery: undefined;
  Feed: undefined;
  Profile: undefined;
  Account: undefined;
};

export type StackParamList = {
  DiscoverySearch: undefined;
};

export type EventStackParamList = {
  Event: { EventID: string };
  CreateEvent: undefined;
};

export type AccountStackParamList = {
  Account: undefined;
};

export type DiscoverySearchProp = NativeStackScreenProps<StackParamList>;
