using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PreferencesService.Entities;

namespace PreferencesService.DataStores
{
    public class DataStore
    {
        public ConcurrentQueue<Changes> LastChanges;

        public ConcurrentDictionary<string, ConcurrentDictionary<string, Preference>> Preferences;

        public void AddChange(Changes change)
        {
            if (LastChanges.Count >= 10)
            {
                LastChanges.TryDequeue(out _);
            }

            LastChanges.Enqueue(change);
        }
        

        public PreferenceUpdate AddPreference(string preferenceType, string preferenceName, Preference preference)
        {
            if (Preferences[preferenceType].TryGetValue(preferenceName, out var existingPreference))
            {
                existingPreference.Add(preference);
            }
            else
            {
                Preferences[preferenceType].TryAdd(preferenceName, 
                    new Preference() { 
                        PurchaseCount = preference.PurchaseCount, 
                        ReservationCount = preference.ReservationCount 
                    });
            }

            return new PreferenceUpdate() { 
                PreferenceName = preferenceName,
                PreferenceType = preferenceType,
                Preference = Preferences[preferenceType][preferenceName] };//return updated preference// for sending to front
        }

        public DataStore()
        {
            LastChanges = new();
            Preferences = new();

            Preferences.TryAdd(DestinationCity, new());
            Preferences.TryAdd(DestinationCountry, new());

            Preferences.TryAdd(HotelName, new());
            Preferences.TryAdd(RoomType, new());
            Preferences.TryAdd(TransportType, new());
        }
        public static string DestinationCity = "DestinationCity";
        public static string DestinationCountry = "DestinationCountry";
        public static string HotelName = "HotelName";
        public static string RoomType = "RoomType";
        public static string TransportType = "TransportType";
    }
}
