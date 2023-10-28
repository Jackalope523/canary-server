import { useNavigation } from "@react-navigation/native";
import type { NativeStackScreenProps } from "@react-navigation/native-stack";

export type StackParamList = {
    DiscoverySearch: undefined;
}

export type BottomTabParamList = {
    Activity: undefined;
    Discovery: undefined;
    Feed: undefined;
    Account: undefined;
}

export type AuthStackParamList = {
    Landing: undefined;
    Login: undefined;
    Signup: undefined;
    Verify: { PhoneNumber: string, Forward: () => void, ContinueMessage?: string};
    Continue: { Message: string, Forward: () => void };
  };

export type AppStackParamList = {
    Auth: undefined;
    Regular: undefined;
  };

export type DiscoverySearchProp = NativeStackScreenProps<StackParamList>;