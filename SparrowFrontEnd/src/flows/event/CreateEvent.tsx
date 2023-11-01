import * as React from 'react';
import { View, Text, TextInput } from 'react-native';
import { StackScreenProps } from '@react-navigation/stack';
import DatePicker from 'react-native-date-picker';

import { Colors } from '../../styles/Colors';
import { globalStyles } from '../../styles/Global';

import { EventStackParamList } from '../../components/atoms/types';
import Button from '../../components/atoms/Button';

import { eventShard, getEvent } from './eventPigeon';


type CreateEventProps = StackScreenProps<EventStackParamList, 'CreateEvent'>;

const CreateEventScreen = ({navigation}: CreateEventProps) => {
    const [errorText, setErrorText] = React.useState('');
    const [Title, setTitle] = React.useState('');
    const [Description, setDescription] = React.useState('');
    const [StartTime, setStartTime] = React.useState(new Date);

    let furthestDate = new Date;
    furthestDate.setDate(furthestDate.getDate() + 7);

    return(
        <View>
            <Text>Title</Text>
            <TextInput
                value={Title}
                onChangeText={setTitle}
                keyboardType='phone-pad' />
            <Text>Email</Text>
            <TextInput
                value={Description}
                onChangeText={setDescription} />
            <Text>Date and Time</Text>
            <DatePicker
                date={StartTime}
                onDateChange={setStartTime}
                mode='datetime'
                timeZoneOffsetInMinutes={0}
                maximumDate={furthestDate} />
            <Button
                btnText={'Create Event'}
                btnIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                btnStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonPrimary, globalStyles.buttonFull]}
                btnTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                btnActiveStyle={[globalStyles.textButtonExtraSmall, globalStyles.buttonFull, globalStyles.buttonPrimaryLight]}
                btnActiveTextStyle={[globalStyles.textButtonExtraSmall.text, globalStyles.textLight]}
                btnActiveIconStyle={[globalStyles.buttonIconSmall, globalStyles.buttonIconSmall.light]}
                />
        </View>
    );
};

export default CreateEventScreen