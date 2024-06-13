using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace api_gateway.Controllers
{
    public class WebSocketService
    {
        public ConcurrentDictionary<Guid,WebSocket> ChangesSockets;
        public ConcurrentDictionary<Guid,WebSocket> PreferencesSockets;
        public ConcurrentDictionary<string, ConcurrentDictionary<Guid, WebSocket>> HotelsSockets;

        public WebSocketService() { 
            ChangesSockets = new();
            PreferencesSockets = new();
            HotelsSockets = new();
        }

        public Guid AddHotelSocket(string hotelId, WebSocket socket)
        {
            var id = Guid.NewGuid();
            HotelsSockets.TryAdd(hotelId, new());
            HotelsSockets[hotelId][id] = socket;
            
            return id;
            //if (Sockets.TryGetValue(hotelId, out var sockets))
            //{
            //   sockets.TryAdd(Guid.NewGuid(), socket);
            //}
            //else
            //{
            //    sockets.
            //}
        }
        public void RemoveHotelSocket(string hotelId, Guid socket)
        {
            HotelsSockets[hotelId].TryRemove(socket, out _);
        }

        public Guid AddChangesSocket(WebSocket socket)
        {
            var id = Guid.NewGuid();
            ChangesSockets.TryAdd(id,socket);
            return id;
        }
        public void RemoveChangesSocket(Guid socket)
        {
            ChangesSockets.Remove(socket, out _);
        }

        public Guid AddPreferencesSocket(WebSocket socket)
        {
            var id = Guid.NewGuid();
            PreferencesSockets.TryAdd(id,socket);
            return id;
        }
        public void RemovePreferencesSocket(Guid socket)
        {
            PreferencesSockets.Remove(socket,out _);
        }

    }
}
