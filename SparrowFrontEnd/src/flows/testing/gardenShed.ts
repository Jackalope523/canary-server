import { userShard, character, accountStatus } from "../auth/accountPigeon";
import { eventShard, eventState } from "../event/eventPigeon";

export const user1: userShard = {
    Id: 1,
    PhoneNumber: "",
    Email: "",
    Name: "",
    DateOfBirth: new Date(),
    IsPhoneConfirmed: false,
    IsEmailConfirmed: true,
    SecurityStamp: "",
    AccessTries: 10,
    AccountStatus: accountStatus.active,
    JoinDate: new Date(),
    Reputation: 100,
    NumberOfFollowers: 0,
    Character: {
        Extraversion: 50,
        Athleticism: 50,
        Chaoticness: 50,
        Competitiveness: 50,
        Industriousness: 50,
        NightOwl: 50,
        Openness: 50
    }
};

export const user2: userShard = {
    Id: 2,
    PhoneNumber: "",
    Email: "",
    Name: "",
    DateOfBirth: new Date(),
    IsPhoneConfirmed: false,
    IsEmailConfirmed: true,
    SecurityStamp: "",
    AccessTries: 10,
    AccountStatus: accountStatus.active,
    JoinDate: new Date(),
    Reputation: 100,
    NumberOfFollowers: 0,
    Character: {
        Extraversion: 50,
        Athleticism: 50,
        Chaoticness: 50,
        Competitiveness: 50,
        Industriousness: 50,
        NightOwl: 50,
        Openness: 50
    }
};

export const user3: userShard = {
    Id: 3,
    PhoneNumber: "",
    Email: "",
    Name: "",
    DateOfBirth: new Date(),
    IsPhoneConfirmed: false,
    IsEmailConfirmed: true,
    SecurityStamp: "",
    AccessTries: 10,
    AccountStatus: accountStatus.active,
    JoinDate: new Date(),
    Reputation: 100,
    NumberOfFollowers: 0,
    Character: {
        Extraversion: 50,
        Athleticism: 50,
        Chaoticness: 50,
        Competitiveness: 50,
        Industriousness: 50,
        NightOwl: 50,
        Openness: 50
    }
};

export const user4: userShard = {
    Id: 4,
    PhoneNumber: "",
    Email: "",
    Name: "",
    DateOfBirth: new Date(),
    IsPhoneConfirmed: false,
    IsEmailConfirmed: true,
    SecurityStamp: "",
    AccessTries: 10,
    AccountStatus: accountStatus.active,
    JoinDate: new Date(),
    Reputation: 100,
    NumberOfFollowers: 0,
    Character: {
        Extraversion: 50,
        Athleticism: 50,
        Chaoticness: 50,
        Competitiveness: 50,
        Industriousness: 50,
        NightOwl: 50,
        Openness: 50
    }
};

export const user5: userShard = {
    Id: 5,
    PhoneNumber: "",
    Email: "",
    Name: "",
    DateOfBirth: new Date(),
    IsPhoneConfirmed: false,
    IsEmailConfirmed: true,
    SecurityStamp: "",
    AccessTries: 10,
    AccountStatus: accountStatus.active,
    JoinDate: new Date(),
    Reputation: 100,
    NumberOfFollowers: 0,
    Character: {
        Extraversion: 50,
        Athleticism: 50,
        Chaoticness: 50,
        Competitiveness: 50,
        Industriousness: 50,
        NightOwl: 50,
        Openness: 50
    }
};

export const user6: userShard = {
    Id: 6,
    PhoneNumber: "",
    Email: "",
    Name: "",
    DateOfBirth: new Date(),
    IsPhoneConfirmed: false,
    IsEmailConfirmed: true,
    SecurityStamp: "",
    AccessTries: 10,
    AccountStatus: accountStatus.active,
    JoinDate: new Date(),
    Reputation: 100,
    NumberOfFollowers: 0,
    Character: {
        Extraversion: 50,
        Athleticism: 50,
        Chaoticness: 50,
        Competitiveness: 50,
        Industriousness: 50,
        NightOwl: 50,
        Openness: 50
    }
};

export const user7: userShard = {
    Id: 7,
    PhoneNumber: "",
    Email: "",
    Name: "",
    DateOfBirth: new Date(),
    IsPhoneConfirmed: false,
    IsEmailConfirmed: true,
    SecurityStamp: "",
    AccessTries: 10,
    AccountStatus: accountStatus.active,
    JoinDate: new Date(),
    Reputation: 100,
    NumberOfFollowers: 0,
    Character: {
        Extraversion: 50,
        Athleticism: 50,
        Chaoticness: 50,
        Competitiveness: 50,
        Industriousness: 50,
        NightOwl: 50,
        Openness: 50
    }
};

export const event1:eventShard = {
    Id: 1,
    Host: user1,
    Name: "",
    Description: "",
    StartTime: new Date(),
    Latitude: 40.723279,
    Longitude: -73.970895,
    State: eventState.upcoming,
    GroupMinimum: 0,
    GroupMaximum: 10,
    Radius: 100,
    IsDynamic: false,
    Character: {
        Extraversion: 50,
        Athleticism: 50,
        Chaoticness: 50,
        Competitiveness: 50,
        Industriousness: 50,
        NightOwl: 50,
        Openness: 50
    }
};

export const event2:eventShard = {
    Id: 2,
    Host: user2,
    Name: "",
    Description: "",
    StartTime: new Date(),
    Latitude: 40.723279,
    Longitude: -73.970895,
    State: eventState.upcoming,
    GroupMinimum: 0,
    GroupMaximum: 10,
    Radius: 100,
    IsDynamic: false,
    Character: {
        Extraversion: 50,
        Athleticism: 50,
        Chaoticness: 50,
        Competitiveness: 50,
        Industriousness: 50,
        NightOwl: 50,
        Openness: 50
    }
};

export const event3:eventShard = {
    Id: 3,
    Host: user3,
    Name: "",
    Description: "",
    StartTime: new Date(),
    Latitude: 40.723279,
    Longitude: -73.970895,
    State: eventState.upcoming,
    GroupMinimum: 0,
    GroupMaximum: 10,
    Radius: 100,
    IsDynamic: false,
    Character: {
        Extraversion: 50,
        Athleticism: 50,
        Chaoticness: 50,
        Competitiveness: 50,
        Industriousness: 50,
        NightOwl: 50,
        Openness: 50
    }
};

export const attendance:[number,number][] = [
    [1,2],
    [1,3],
    [2,1],
    [2,3]
];

export const follows:[number,number][] = [
    [1,2],
    [1,3],
    [1,4],
    [1,5],
    [1,6]
];

export const blocks:[number,number][] = [
    [1,7]
];