import * as React from 'react';
import { View, Text, TouchableOpacity } from 'react-native';

import SearchBar from '../components/molecules/SearchBar';
import { globalStyles } from '../styles/Global';

// export default function DiscoveryScreen({ navigation }) {
//     return (
//         <View>
//             <Text onPress={() => navigation.navigate('Activity')}>Discovery Screen</Text>
//             <SearchBar />
//             <TouchableOpacity style={[globalStyles.textButtonExtraSmall, globalStyles.textButtonPrimary]}>
//                 <Text style={globalStyles.textButtonExtraSmallText}>Create Event</Text>
//             </TouchableOpacity>
//         </View>
//     );
// };

const DiscoveryScreen = () => {
    return (
        <View>
            <Text>Discovery</Text>
            <SearchBar />
            <TouchableOpacity style={[globalStyles.textButtonExtraSmall, globalStyles.textButtonPrimary]}>
                <Text style={globalStyles.textButtonExtraSmallText}>Create Event</Text>
            </TouchableOpacity>
        </View>
    );
};

export default DiscoveryScreen