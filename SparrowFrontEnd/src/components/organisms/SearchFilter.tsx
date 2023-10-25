import { StyleSheet, Text, View, FlatList } from 'react-native'
import * as React from 'react'

import EventCardMedium from '../../components/organisms/EventCardMedium';
import { SAMPLEEVENTDATA } from '../../data/sampleEventData';

import { Spacing } from '../../styles/Spacing';

//  TODO make search filter, search for events based on TEXT input from Discovery -> searchBar -> TextInput component.
// TODO make FILTER and SORT buttons functional

const SearchFilter = () => {

  return (
    <View>
        <FlatList
            showsVerticalScrollIndicator={false}
            contentContainerStyle={{paddingVertical: Spacing.lg, paddingBottom: 800}}
            ItemSeparatorComponent={() => <View style={{height: Spacing.md}} />}
            overScrollMode='never'
            keyExtractor={(item) => item.id}
            data={SAMPLEEVENTDATA}
            renderItem={({ item }) => (
                <EventCardMedium
                    onPress={null}
                    eventHeroImage={item.uri}
                    eventDate={item.date}
                    eventTime={item.time}
                    eventAttendees={item.attendees}
                    eventLocation={item.location}
                    eventTitle={item.title}
                    />
            )}
            />
    </View>
  )
}

export default SearchFilter

const styles = StyleSheet.create({})