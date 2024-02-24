import React, { Component } from "react";
import { StyleSheet, View } from "react-native";
import MapboxGL, { MapView, LocationPuck }  from "@rnmapbox/maps";

import Button, {
    ButtonType,
    ButtonSize,
    ButtonDisplay,
  } from '../../components/Button';

MapboxGL.setAccessToken("sk.eyJ1IjoiamFja2Fsb3BlNTIzIiwiYSI6ImNsb3o0Y3ZoYTA5aW4ya3Bwb3M5YjY0cXkifQ.g0fLvyL1wWfyotb8L5oigg");

const styles = StyleSheet.create({
  page: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "#F5FCFF"
  },
  container: {
    height: 300,
    width: 300,
    backgroundColor: "tomato"
  },
  map: {
    flex: 1
  }
});

const Map = () => {

    return (
      <View style={styles.page}>
        <View style={styles.container}>
          <MapView style={styles.map} scaleBarEnabled = {false}>
          <LocationPuck
          topImage="topImage"
          visible={true}
          scale={['interpolate', ['linear'], ['zoom'], 10, 1.0, 20, 4.0]}
          pulsing={{
            isEnabled: true,
            color: 'teal',
            radius: 50.0,
          }}
        />
          </MapView>

          <View style={{width:30, backgroundColor:"transparent",position:'absolute',top:"50%",left:"50%",zIndex:10}}>
            <Button text="Button"></Button>
          </View>
        </View>
      </View>
    );
};

export default Map;