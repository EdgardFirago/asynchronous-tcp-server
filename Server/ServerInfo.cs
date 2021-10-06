using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
namespace Server
{
    class ServerInfo
    {
        public List<Room> rooms = new List<Room>();//комнаты
        public List<ClientObject> clients = new List<ClientObject>();
        public string GetRoomInfo()
        {
            if (rooms.Count == 0)
                return "";
            List<string> name = new List<string>();
            foreach (Room room in rooms)
            {
                name.Add(room.name);
            }
            return JsonSerializer.Serialize(name);
        }

        public void createRoom(string _name)
        {
            rooms.Add(new Room(_name));
        }
    }
}
