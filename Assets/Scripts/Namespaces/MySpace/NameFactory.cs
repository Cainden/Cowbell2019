using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MySpace
{
    public static class NameFactory
    {
        static readonly string[] FirstNames =
        {
        "Alexander", "Michael", "Fred",   "James", "Benjamin",
        "Bob",       "Chris",   "Bert",   "Jack",  "William",
        "Jacob",     "Lucas",   "Thomas", "Joe",   "Hank"
    };

        static readonly string[] LastNames =
        {
        "Miller", "Cook", "Smith", "Jones",   "Rogers",
        "King",   "Bell", "Green", "Anderson","Baker",
        "Nelson", "Cox",  "Cooper", "White",  "Ross"
    };

        public static string GetNewFirstName()
        {
            return (FirstNames[Random.Range(0, FirstNames.Length)]);
        }

        public static string GetNewLastName()
        {
            return (LastNames[Random.Range(0, FirstNames.Length)]);
        }
    }
}
