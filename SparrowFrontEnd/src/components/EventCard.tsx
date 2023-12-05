import { ImageBackground, Text, View, Pressable } from 'react-native';
import React, { useState } from 'react';
import { cardStyles } from '../styles/Cards';

// Icons font
import { createIconSetFromFontello } from 'react-native-vector-icons';
import fontelloConfig from '../config.json';
import { globalStyles } from '../styles/Global';
import { Colors } from '../styles/Colors';

const Icon = createIconSetFromFontello(fontelloConfig);

// TEMP. example imports
const bgImage = {
  uri: 'https://images.unsplash.com/photo-1562519819-016930ada31b?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=687&q=80',
};

interface EventCardProps {
  size?: EventCardSize;
  host?: EventCardHost;

  date?: string;
  time?: number;
  attendees?: number;
  title?: string;
  location?: string;

  eventHeroImage?: string;
  onPress?: any;
}

export const EventCard: React.FC<EventCardProps> = ({
  date = null,
  time = null,
  attendees = null,
  title = null,
  location = null,
}) => {
  // CODE HERE

  switch (size) {
    case EventCardSize.Medium:
      cardStyles.eventCardMedium;
      break;
  }
};
