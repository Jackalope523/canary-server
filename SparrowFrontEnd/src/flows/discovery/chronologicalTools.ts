// Function to check if a date is today
export const isToday = (date: Date): boolean => {
    const today = new Date();
    return date.getDate() === today.getDate() &&
        date.getMonth() === today.getMonth() &&
        date.getFullYear() === today.getFullYear();
};

// Function to check if a date is tomorrow
export const isTomorrow = (date: Date): boolean => {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    return date.getDate() === tomorrow.getDate() &&
        date.getMonth() === tomorrow.getMonth() &&
        date.getFullYear() === tomorrow.getFullYear();
};

// Function to check if a date is within the current week
export const isThisWeek = (date: Date): boolean => {
    const today = new Date();
    const lastDayOfWeek = new Date(today.getFullYear(), today.getMonth(), today.getDate() + (6 - today.getDay()));
    return date >= today && date <= lastDayOfWeek;
};

// Function to check if a date is during this weekend
export const isThisWeekend = (date: Date): boolean => {
    const today = new Date();
    const thisSaturday = new Date(today.getFullYear(), today.getMonth(), today.getDate() + (6 - today.getDay()));
    const thisSunday = new Date(today.getFullYear(), today.getMonth(), today.getDate() + (7 - today.getDay()));
    return date >= thisSaturday && date <= thisSunday;
};

// Function to check if a date is within next week
export const isNextWeek = (date: Date): boolean => {
    const today = new Date();
    const nextWeekStart = new Date(today.getFullYear(), today.getMonth(), today.getDate() + (8 - today.getDay()));
    const nextWeekEnd = new Date(today.getFullYear(), today.getMonth(), today.getDate() + (14 - today.getDay()));
    return date >= nextWeekStart && date <= nextWeekEnd;
};

// Function to check if a date is during next weekend
export const isNextWeekend = (date: Date): boolean => {
    const today = new Date();
    const nextSaturday = new Date(today.getFullYear(), today.getMonth(), today.getDate() + (6 - today.getDay()) + 7);
    const nextSunday = new Date(today.getFullYear(), today.getMonth(), today.getDate() + (7 - today.getDay()) + 7);
    return date >= nextSaturday && date <= nextSunday;
};

export const formatDate: (arg0: Date) => string = (date: Date) => {  
    if (isToday(date)) {
        return "Today";
    }
    else if (isTomorrow(date)) {
        return "Tomorrow";
    }
    else if (isThisWeek(date)) {
        let stringsThisWeek = ["This Monday", "This Tuesday", "This Wednesday", "This Thursday", "This Friday", "This Saturday", "This Sunday"];
        return stringsThisWeek[date.getDay()];
    }
    else if (isNextWeek(date)) {
        let stringsNextWeek = ["Next Monday", "Next Tuesday", "Next Wednesday", "Next Thursday", "Next Friday", "Next Saturday", "Next Sunday"];
        return  stringsNextWeek[date.getDay()]
    }
    else {
        return "Invalid"
    }
}

export const formatTime: (arg0: Date) => string = (time: Date) => {
    let hours = time.getHours();
    hours = hours % 12;
    hours = hours ? hours : 12;
    let formattedHours = hours.toString();

    let minutes = time.getMinutes();
    let formattedMinutes = minutes < 10 ? '0' + minutes.toString() : minutes.toString();

    let amOrPm = hours >= 12 ? 'pm' : 'am';

    return formattedHours + ':' + formattedMinutes + amOrPm;
}
