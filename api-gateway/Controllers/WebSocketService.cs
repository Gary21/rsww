using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace api_gateway.Controllers
{
    public class WebSocketService
    {
        public ConcurrentBag<WebSocket> ChangesSockets;
        public ConcurrentBag<WebSocket> PreferencesSockets;
        public ConcurrentDictionary<string, ConcurrentDictionary<Guid, WebSocket>> HotelsSockets;

        public WebSocketService() { 
            ChangesSockets = new();
            PreferencesSockets = new();
            HotelsSockets = new();
        }

        public void AddHotelSocket(string hotelId, WebSocket socket)
        {
            HotelsSockets.TryAdd(hotelId, new());
            HotelsSockets[hotelId][Guid.NewGuid()] = socket;
            
            //if (Sockets.TryGetValue(hotelId, out var sockets))
            //{
            //   sockets.TryAdd(Guid.NewGuid(), socket);
            //}
            //else
            //{
            //    sockets.
            //}
        }

        public void AddChangesSocket(WebSocket socket)
        {
            ChangesSockets.Add(socket);
        }
        public void AddPreferencesSocket(WebSocket socket)
        {
            PreferencesSockets.Add(socket);
        }

    }
}
