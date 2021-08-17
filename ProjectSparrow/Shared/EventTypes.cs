using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    /*
     * Notes
     * 
     * Street Car Racing is illegal in most places but is a good example of an event that targets two interests.
     * (Though, someone interested in Sports and not Cars probably wouldn't be interested)
     * 
     * We could have a locale-lock inside EventType that would indicate where an event is allowed or not allowed (legality of certain events).
     * 
     * Though an excel sheet would be more readable, loading it would require some backwards flips through hoops. We can see.
     */
    
    public abstract class EventType
    {
        public abstract UserInterest TargetInterest { get; }
    }

    public class Car_Meet : EventType
    {
        public override UserInterest TargetInterest => UserInterest.Cars;
    }

    public class House_Party : EventType
    {
        public override UserInterest TargetInterest => UserInterest.Parties;
    }

    public class Piquenique : EventType
    {
        public override UserInterest TargetInterest => UserInterest.Socials;
    }

    public class Street_Car_Race : EventType
    {
        public override UserInterest TargetInterest => UserInterest.Sports | UserInterest.Cars;
    }

    public class Language_Exchange : EventType
    {
        public override UserInterest TargetInterest => UserInterest.Socials | UserInterest.Educational;
    }
}
