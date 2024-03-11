import React, { useState, useEffect } from "react";

import { 
  StyleSheet, 
  View, 
  PermissionsAndroid,
  Platform
} from "react-native";

import MapboxGL, 
{ 
  MapView, 
  Camera, 
  LocationPuck, 
  UserTrackingMode,  
  Images, 
  Image, 
  UserLocation, 
  ShapeSource,
  SymbolLayer,
  Location
}  from "@rnmapbox/maps";

import { featureCollection, feature, point } from '@turf/helpers';

import exampleIcon from '../../assets/pins/Pin.png';

export interface MapProps {}

const Map : React.FC<MapProps> = () => {
  MapboxGL.setAccessToken("sk.eyJ1IjoiamFja2Fsb3BlNTIzIiwiYSI6ImNsb3o0Y3ZoYTA5aW4ya3Bwb3M5YjY0cXkifQ.g0fLvyL1wWfyotb8L5oigg");
  const [location, setLocation] = useState<Location>();

  const requestLocationPermission = async () => {
    try {
      const granted = await PermissionsAndroid.request(
        PermissionsAndroid.PERMISSIONS.ACCESS_FINE_LOCATION,
        {
          title: 'Precise Location Access',
          message:"Sparrow cordially invites you to share your location so that it may aid you with navigation.",
          buttonNeutral: 'Alright then.',
          buttonNegative: 'I\'m scared...',
          buttonPositive: 'Sounds grand!'
        },
      );
      if (granted === PermissionsAndroid.RESULTS.NEVER_ASK_AGAIN) {
        console.log('NEVER ASK AGAIN PROBLEM');
      } 
    } catch (err) {
      console.warn(err);
    }
  };

  if (Platform.OS == 'android') {
    useEffect(() => {
      requestLocationPermission();
      }, []);   
  }

  const [stateFeatureCollection, setStateFeatureCollection] =
  useState<GeoJSON.FeatureCollection>(
    featureCollection([point([-73.970895, 40.723279], {name: "A"}), point([-60.970895, 40.723279], {name: "B"}), point([-73.5674, 45.5019], {name: "Xavier's Birthday"}), point([-73.970895, 35.723279], {name: "D"})]),
  );
  
    return (
      <MapView 
      style={styles.map} 
      scaleBarEnabled = {false} 
      styleURL={MapboxGL.StyleURL.Dark}
      logoEnabled={false}
      attributionEnabled={false}
      projection='mercator'
      >
       <Images>
         <Image name="topImage">
           <View
             style={{
             borderColor: 'blue',
             borderWidth: 2,
             width: 16,
             height: 16,
             borderRadius: 8,
             backgroundColor: 'red'
             }}
           />
         </Image>
       </Images>
     <Camera
       defaultSettings={{
         centerCoordinate: [45.3013, 72.3150],
         zoomLevel: 14,
       }}
       followUserLocation={true}
       followUserMode={UserTrackingMode.Follow}
       followZoomLevel={14}
     />
     <UserLocation
       onUpdate={(newLocation) => setLocation(newLocation)}
     />
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
     <ShapeSource
       id="symbolLocationSource"
       hitbox={{ width: 20, height: 20 }}
       onPress={(e) => {console.log(e.features[0].properties?.name)}}
       shape={stateFeatureCollection}
     >
       <SymbolLayer
         id="symbolLocationSymbols"
         minZoomLevel={1}
         style={{ iconImage: 'pin', iconAllowOverlap: true }}
       />
       <Images images={{ pin: exampleIcon }} />
     </ShapeSource>
   </MapView>
    );
};

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

export default Map;