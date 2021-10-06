using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{

    public enum requestState { Аuthorization, inGame,inMenu};
    abstract class Request
    {
        public string command;
        public string data;
        protected ClientInfo client;

        public abstract string callHandler();

    }
    public interface IProduct
    {
        void Operation();
    }


    class AutorizeRequest : Request
    {
        
        public AutorizeRequest(string command,string data,ClientInfo client)
        {
            
            this.client = client;
            this.command = command;
            this.data = data;
           

        }
        public override string callHandler()
        {
            Console.WriteLine("Handler");
           
            switch (command)
            { 
                case "authorization":
                    client.name = data;
                    Console.WriteLine("Подключен пользователь!");
                    return "OK";

            }
            return "";
        }
    }

    class GamePlayRequst : Request
    {
        public override string callHandler()
        {
            throw new NotImplementedException();
        }
    }

    class MenuRequest : Request
    {
        public override string callHandler()
        {
            throw new NotImplementedException();
        }
    }
}

  
