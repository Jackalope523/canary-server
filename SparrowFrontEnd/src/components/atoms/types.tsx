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

export type DiscoverySearchProp = NativeStackScreenProps<StackParamList>;