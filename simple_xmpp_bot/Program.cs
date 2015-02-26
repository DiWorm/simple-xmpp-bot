using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.x.muc;
using agsXMPP.protocol.x.muc.iq;
using agsXMPP.protocol.x.muc.iq.admin;
using agsXMPP.protocol.x.muc.iq.owner;
using agsXMPP.protocol.x.data;
using System.Net;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;

namespace simple_xmpp_bot
{
    class Program
    {

        static string nick = ""; //bot nickname
        static string jid = ""; //bot conference like a gamecoma@conference.jabber.ru
        static string username = ""; //connect username 
        static string password = ""; //connect password
        static string server = ""; //connect server (like a jabber.ru)

        static string phpbackend = ""; //URL to php page with plugins

        static ArrayList arrList = new ArrayList();
        static XmppClientConnection xmpp;
        static Jid Room;

        static void Main(string[] args)
        {
            xmpp = new XmppClientConnection();
            xmpp.Server = server;
            xmpp.ConnectServer = server;
            xmpp.Username = username;
            xmpp.Password = password;
            xmpp.Resource = "";
            xmpp.AutoAgents = false;
            xmpp.AutoPresence = true;
            xmpp.AutoRoster = true;
            xmpp.AutoResolveConnectServer = true;
            xmpp.Priority = 0;
            xmpp.Open();
            xmpp.OnLogin += new ObjectHandler(OnLoginEvent);
            xmpp.OnPresence += new PresenceHandler(xmpp_OnPresenceHandler);
            xmpp.OnMessage += new MessageHandler(xmpp_OnMessage);
            xmpp.OnError += new ErrorHandler(xmpp_OnError);



            string ConsoleLine = Console.ReadLine();
            while (true)
            {
                ConsoleLine = Console.ReadLine();
                int delay = 1000;
                switch (ConsoleLine)
                {
                    case "exit": return;
                    case "reconnect": xmpp.Close(); System.Threading.Thread.Sleep(delay); xmpp.Open(); continue;
                }
                xmpp.Send(new Message(Room, MessageType.groupchat, ConsoleLine));
            }


        }

        static void xmpp_OnError(object sender, Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        static void xmpp_OnMessage(object sender, Message msg)
        {
            string SenderNickname = msg.From.ToString().Replace(jid + "/", "");
            Console.WriteLine(SenderNickname + ": " + msg.Body);

            if (msg.XDelay != null) return;

            string pattern = "^![a-zA-Zа-яА-Я]+\\ ";
            string pattern2 = "^![a-zA-Zа-яА-Я]";
            string imgPattern = "\\.(jpeg|jpg|gif|png|bmp|ico|JPEG|JPG|GIF|PNG|BMP|ICO)$";
            string replacement = "";

            Regex r = new Regex(pattern);
            Regex r2 = new Regex(pattern2);
            Regex imgr = new Regex(imgPattern);

            if (r.IsMatch(msg.Body))
            {
                string[] result = Regex.Split(msg.Body, " ");
                string what = "";
                foreach (string newres in result)
                {
                    if (!newres.Contains("!"))
                        what += newres + " ";
                }
                xmpp.Send(new Message(Room, MessageType.groupchat, SenderNickname + ": " + GetFromSite(result[0], what, GerPerm(SenderNickname))));
            }
            else if (r2.IsMatch(msg.Body))
            { xmpp.Send(new Message(Room, MessageType.groupchat, SenderNickname + ": " + GetFromSite(msg.Body, null, GerPerm(SenderNickname)))); }

            else
            {
                if (msg.Body.Contains(nick + ": "))
                {
                    Regex rgx = new Regex("^" + nick + ": ");
                    string result = rgx.Replace(msg.Body, replacement);

                    if (imgr.IsMatch(SenderNickname + ": " + msg.Body))
                    {
                        xmpp.Send(new Message(Room, MessageType.groupchat, SenderNickname + ": " + GetFromSite("image", result, GerPerm(SenderNickname))));
                    }
                    else if (msg.Body.Contains(nick + ": "))
                    {
                        xmpp.Send(new Message(Room, MessageType.groupchat, SenderNickname + ": " + GetFromSite("talks", result, GerPerm(SenderNickname))));
                    }
                }
            }
        }


        public static string GetFromSite(string cmd, string what = "", string perm = "") //send data to php and recive info
        {
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;
            var values = new NameValueCollection();
            values["cmd"] = cmd;
            values["what"] = what;
            values["perm"] = perm;
            var response = client.UploadValues(phpbackend, values);
            var responseString = Encoding.UTF8.GetString(response);
            return responseString;
        }

        public static string GerPerm(string user) //cheking occupant permisions on channel
        {
            string perm = "0";
            for (int i = 0; i < arrList.Count; i++)
            {
                if (arrList[i].ToString().Contains(user))
                {
                    string[] splitedPerm = Regex.Split(arrList[i].ToString(), "/");
                    return splitedPerm[3];
                }
            }
            return perm;
        }

        static void OnLoginEvent(object sender)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Conected!");
            xmpp.SendMyPresence();
            MucManager mucManager = new MucManager(xmpp);
            Room = new Jid(jid);
            mucManager.AcceptDefaultConfiguration(Room);
            mucManager.JoinRoom(Room, nick);
            Presence p = new Presence(ShowType.chat, "Online");
            p.Type = PresenceType.available;
            xmpp.Send(p);
        }

        static void xmpp_OnPresenceHandler(object sender, Presence pres)
        {
            User u = pres.SelectSingleElement(typeof(User)) as User;
            if (u != null)
                arrList.Add(pres.From.Resource.ToString() + "/" + u.Item.Jid.ToString() + "/" + u.Item.Affiliation.ToString() + "/" + u.Item.Role.ToString());
        }
    }
}