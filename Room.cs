using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class Room
    {
        public string name { get; set; }
        protected internal int seats { get; private set; }
        List<ClientObject> clientsInRoom = new List<ClientObject>();
        private bool IsStarted = false;

        public Room(string name)
        {
            this.name = name;
        }
        


    }
}
