using System;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UPDServer
{

    public struct Received
    {
        public IPEndPoint Sender;
        public string Message;
    }

    abstract class UdpBase
    {
        protected UdpClient Client;

        protected UdpBase()
        {
            Client = new UdpClient();
        }

        public async Task<Received> Receive()
        {
            var result = await Client.ReceiveAsync();
            return new Received()
            {
                Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
                Sender = result.RemoteEndPoint
            };
        }
    }

    class UdpListener : UdpBase
    {
        private IPEndPoint _listenOn;

        public UdpListener() : this(new IPEndPoint(IPAddress.Any, 32123)) { }

        public UdpListener(IPEndPoint endpoint)
        {
            _listenOn = endpoint;
            Client = new UdpClient(_listenOn);
        }

        public void Reply(string message, IPEndPoint endpoint)
        {
            var datagram = Encoding.ASCII.GetBytes(message);
            Client.Send(datagram, datagram.Length, endpoint);
        }

    }


    class Program
    {
        static Player p1 = new Player();
        static char[] regionValues = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P' };
        static Random regionRandom = new Random();

        static void Main(string[] args)
        {
            //create a new server
            Universe u = new Universe();
            var server = new UdpListener();
            Random rnd = new Random();
            Dictionary<string, IPEndPoint> connections = new Dictionary<string, IPEndPoint>();
            Dictionary<string, Player> players = new Dictionary<string, Player>();
            List<string> ip = new List<string>();

            Console.WriteLine("============================================= Server");

            string[] parts;

            //start listening for messages and copy the messages back to the client
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var received = await server.Receive();
                    string msg = received.Message.ToString();
                    parts = msg.Split(':');
                    fixParts(parts);
                    Console.WriteLine(msg + " -- " + received.Sender.Address.MapToIPv4().ToString());

                    // Only add new connections to the list of clients
                    if (!connections.ContainsKey(received.Sender.Address.MapToIPv4().ToString()))
                    {
                        connections.Add(received.Sender.Address.MapToIPv4().ToString(), received.Sender);
                        p1 = new Player();
                        p1.setCol(rnd.Next(0, 9));
                        p1.setRow(rnd.Next(0, 9));
                        p1.setStartingLocation(rnd.Next(0, 255));
                        p1.setSector(u.getSectors()[p1.getStartingLocation()]);
                        u.getSectors()[p1.getStartingLocation()].addPlayer(p1);
                        players.Add(received.Sender.Address.MapToIPv4().ToString(), p1);
                        ip.Add(received.Sender.Address.MapToIPv4().ToString());
                        server.Reply(System.Text.Encoding.UTF8.GetString(p1.getSector().sendSectorData()), received.Sender);
                        server.Reply(String.Format("connected:true:{0}:{1}:{2}:{3}", p1.getSectorRegion(), p1.getSectorValue(), p1.getCol(), p1.getRow()), received.Sender);

                    }


                    string ret = "[Connected Users]";
                    if (received.Message.Equals("list"))
                    {
                        foreach (string s in connections.Keys)
                            ret += "\n>> " + s;
                        server.Reply(ret + "\n*****************", received.Sender);
                    }
                    else
                    {
                        // Okay, send message to everyone
                        //foreach (IPEndPoint ep in connections.Values)
                        //    server.Reply("[" + received.Sender.Address.ToString() + "] says: " + received.Message, ep);
                    }

                    if (received.Message == "quit")
                    {
                        connections.Remove(received.Sender.Address.ToString());     // Remove the IP Address from the list of connections
                    }
                    else if (received.Message == "shieldOn")
                    {
                        Player p = players[received.Sender.Address.MapToIPv4().ToString()];
                        p.changeShield();
                        Console.Write("reached");
                    }
                    else if (received.Message == "viewu")
                    {
                        string playerString = "";
                        foreach (Sector s in u.getSectors())
                        {
                            foreach (Player p in s.getPlayers())
                            {
                                playerString += "Player- ";
                                playerString += s.getRegion().ToString().ToUpper();
                                playerString += "-" + s.getValue().ToString();
                                playerString += System.Environment.NewLine;
                            }
                        }
                        server.Reply((String.Format("universe:{0}", playerString)), received.Sender);
                        Console.Write("reached");
      
                    }
                    else if (received.Message == "update0")
                    {
                        Player p = players[received.Sender.Address.MapToIPv4().ToString()];
                        p.setShipAngle(0);
                    }
                    else if (received.Message == "update90")
                    {
                        Player p = players[received.Sender.Address.MapToIPv4().ToString()];
                        p.setShipAngle(90);
                    }
                    else if (received.Message == "update180")
                    {
                        Player p = players[received.Sender.Address.MapToIPv4().ToString()];
                        p.setShipAngle(180);
                    }
                    else if (received.Message == "update270")
                    {
                        Player p = players[received.Sender.Address.MapToIPv4().ToString()];
                        p.setShipAngle(270);
                    }

                    else if (received.Message == ("fireP"))
                    {
                        Player p = players[received.Sender.Address.MapToIPv4().ToString()];
                        if (p.getPhasors() != 0)
                        {
                            if (p.getShield() == false)
                            {
                                p.setPhasors(p.getPhasors() - 1);
                                server.Reply((String.Format("phasors:{0}", p.getPhasors())), received.Sender);
                            }

                        }
                        else
                        {
                            server.Reply((String.Format("display:{0}", "Out of Ammo")), received.Sender);
                        }
                        if (p.getSector().getPlayers().Count > 1)
                        {
                            foreach (Player p1 in p.getSector().getPlayers())
                            {

                                if (p1.getRow() != p.getRow() || p1.getCol() != p.getCol())
                                {

                                    if (p.getShipAngle() == 0 && p1.getRow() < p.getRow() && p1.getCol() == p.getCol())
                                    {
                                        Console.Write("Hit");
                                        Console.Write(p.getShield());
                                        if (p1.getShield() == false && p.getShield() == false) p1.setHealth(p1.getHealth() - 5);

                                    }
                                    else if (p.getShipAngle() == 180 && p1.getRow() > p.getRow() && p1.getCol() == p.getCol())
                                    {
                                        Console.Write("Hit");
                                        if (p1.getShield() == false && p.getShield() == false) p1.setHealth(p1.getHealth() - 5);
                                    }
                                    else if (p.getShipAngle() == 90 && p1.getRow() == p.getRow() && p1.getCol() > p.getCol())
                                    {
                                        Console.Write("Hit");
                                        if (p1.getShield() == false && p.getShield() == false) p1.setHealth(p1.getHealth() - 5);
                                    }
                                    else if (p.getShipAngle() == 270 && p1.getRow() == p.getRow() && p1.getCol() < p.getCol())
                                    {
                                        Console.Write("Hit");
                                        if (p1.getShield() == false && p.getShield() == false) p1.setHealth(p1.getHealth() - 5);

                                    }
                                }
                            }
                        }
                       

                    }
                    else if (received.Message == ("fireT"))
                    {
                        Player p = players[received.Sender.Address.MapToIPv4().ToString()];
                        if (p.getTorpedos() != 0)
                        {
                            p.setTorpedos(p.getTorpedos() - 1);
                            server.Reply((String.Format("torpedos:{0}", p.getTorpedos())), received.Sender);

                        }
                        else
                        {
                            server.Reply((String.Format("display:{0}", "Out of Ammo")), received.Sender);
                        }
                        if (p.getSector().getPlayers().Count > 1)
                        {
                            foreach (Player p1 in p.getSector().getPlayers())
                            {

                                if (p1.getRow() != p.getRow() || p1.getCol() != p.getCol())
                                {

                                    if (p.getShipAngle() == 0 && p1.getRow() < p.getRow() && p1.getCol() == p.getCol())
                                    {
                                        Console.Write("Hit");
                                        if (p1.getShield() == false && p.getShield() == false) p1.setHealth(p1.getHealth() - 15);

                                    }
                                    else if (p.getShipAngle() == 180 && p1.getRow() > p.getRow() && p1.getCol() == p.getCol())
                                    {
                                        Console.Write("Hit");
                                        if (p1.getShield() == false && p.getShield() == false) p1.setHealth(p1.getHealth() - 15);
 
                                    }
                                    else if (p.getShipAngle() == 90 && p1.getRow() == p.getRow() && p1.getCol() > p.getCol())
                                    {
                                        Console.Write("Hit");
                                        if (p1.getShield() == false && p.getShield() == false) p1.setHealth(p1.getHealth() - 15);
 

                                    }
                                    else if (p.getShipAngle() == 270 && p1.getRow() == p.getRow() && p1.getCol() < p.getCol())
                                    {
                                        Console.Write("Hit");
                                        if (p1.getShield() == false && p.getShield() == false) p1.setHealth(p1.getHealth() - 15);

                                    }
                                }
                            }
                        }
                    }

                    else if (parts[0].Equals("mov"))
                    {
                        string message = "";

                        Player p = players[received.Sender.Address.MapToIPv4().ToString()];
                        p.setFuel(p.getFuel() - 1);
                        if (parts[1].Equals("n"))
                        {
                            p.setShipAngle(0);
                            p.setRow(p.getRow() - 1);
                            int x = isCollision(p.getSector(), p.getRow(), p.getCol());
                            if (x == 1)
                            {
                                p.setHealth(0);
                                if (p.getSector().getPlayers().Count > 1)
                                {
                                    p.getSector().getPlayers().Remove(p);

                                }
                            }
                            else if (x == 2)
                            {
                                p.setFuel(50);
                                p.setHealth(100);
                                p.setPhasors(50);
                                p.setTorpedos(10);
                            }
                            else if (x == 3)
                            {
                                p.setStartingLocation(rnd.Next(0, 255));
                                p.getSector().removePlayer(p);
                                p.setSector(u.getSectors()[p.getStartingLocation()]);
                                p.getSector().addPlayer(p);
                                var chars = p.getSector().getSafeLocation().ToCharArray();
                                string row = chars[0].ToString();
                                string col = chars[1].ToString();
                                p.setRow(Convert.ToInt32(row));
                                p.setCol(Convert.ToInt32(col));
                                server.Reply(System.Text.Encoding.UTF8.GetString(p.getSector().sendSectorData()), received.Sender);
                            }
                            else if (x == 4)
                            {
                                message = "You found treasure!";
                            }

                        }
                        else if (parts[1].Equals("s"))
                        {
                            p.setShipAngle(180);
                            p.setRow(p.getRow() + 1);
                            int x = isCollision(p.getSector(), p.getRow(), p.getCol());
                            if (x == 1)
                            {
                                p.setHealth(0);
                                if (p.getSector().getPlayers().Count > 1)
                                {
                                    p.getSector().getPlayers().Remove(p);

                                }
                            }
                            else if (x == 2)
                            {
                                p.setFuel(50);
                                p.setHealth(100);
                                p.setPhasors(50);
                                p.setTorpedos(10);
                            }
                            else if (x == 3)
                            {
                                p.setStartingLocation(rnd.Next(0, 255));
                                p.getSector().removePlayer(p);
                                p.setSector(u.getSectors()[p.getStartingLocation()]);
                                p.getSector().addPlayer(p);
                                var chars = p.getSector().getSafeLocation().ToCharArray();
                                string row = chars[0].ToString();
                                string col = chars[1].ToString();
                                p.setRow(Convert.ToInt32(row));
                                p.setCol(Convert.ToInt32(col));
                                p.setFuel(p.getFuel() - 5);
                                server.Reply(System.Text.Encoding.UTF8.GetString(p.getSector().sendSectorData()), received.Sender);
                            }
                            else if (x == 4)
                            {
                                message = "You found treasure!";
                            }
                        }
                        else if (parts[1].Equals("e"))
                        {
                            p.setShipAngle(90);
                            p.setCol(p.getCol() + 1);
                            int x = isCollision(p.getSector(), p.getRow(), p.getCol());
                            if (x == 1)
                            {
                                p.setHealth(0);
                                if (p.getSector().getPlayers().Count > 1)
                                {
                                    p.getSector().getPlayers().Remove(p);

                                }
                            }
                            else if (x == 2)
                            {
                                p.setFuel(50);
                                p.setHealth(100);
                                p.setPhasors(50);
                                p.setTorpedos(10);
                            }
                            else if (x == 3)
                            {
                                p.setStartingLocation(rnd.Next(0, 255));
                                p.getSector().removePlayer(p);
                                p.setSector(u.getSectors()[p.getStartingLocation()]);
                                p.getSector().addPlayer(p);
                                var chars = p.getSector().getSafeLocation().ToCharArray();
                                string row = chars[0].ToString();
                                string col = chars[1].ToString();
                                p.setRow(Convert.ToInt32(row));
                                p.setCol(Convert.ToInt32(col));
                                server.Reply(System.Text.Encoding.UTF8.GetString(p.getSector().sendSectorData()), received.Sender);
                            }
                            else if (x == 4)
                            {
                                message = "You found treasure!";
                            }
                        }
                        else if (parts[1].Equals("w"))
                        {
                            p.setShipAngle(270);
                            p.setCol(p.getCol() - 1);
                            int x = isCollision(p.getSector(), p.getRow(), p.getCol());
                            if (x == 1)
                            {
                                p.setHealth(0);
                                if (p.getSector().getPlayers().Count > 1)
                                {
                                    p.getSector().getPlayers().Remove(p);

                                }

                            }
                            else if (x == 2)
                            {
                                p.setFuel(50);
                                p.setHealth(100);
                                p.setTorpedos(10);
                                p.setPhasors(50);
                            }
                            else if (x == 3)
                            {
                                p.setStartingLocation(rnd.Next(0, 255));
                                p.getSector().removePlayer(p);
                                p.setSector(u.getSectors()[p.getStartingLocation()]);
                                p.getSector().addPlayer(p);
                                var chars = p.getSector().getSafeLocation().ToCharArray();
                                string row = chars[0].ToString();
                                string col = chars[1].ToString();
                                p.setRow(Convert.ToInt32(row));
                                p.setCol(Convert.ToInt32(col));
                                server.Reply(System.Text.Encoding.UTF8.GetString(p.getSector().sendSectorData()), received.Sender);
                            }
                            else if (x == 4)
                            {
                                message = "You found treasure!";
                            }
                        }
                        else if (parts[1].Equals("h"))
                        {
                            Console.Write("print");
                            p.setStartingLocation(rnd.Next(0, 255));
                            p.getSector().removePlayer(p);
                            p.setSector(u.getSectors()[p.getStartingLocation()]);
                            p.getSector().addPlayer(p);
                            var chars = p.getSector().getSafeLocation().ToCharArray();
                            string row = chars[0].ToString();
                            string col = chars[1].ToString();
                            p.setRow(Convert.ToInt32(row));
                            p.setCol(Convert.ToInt32(col));
                            p.setFuel(p.getFuel() - 5);
                            server.Reply(System.Text.Encoding.UTF8.GetString(p.getSector().sendSectorData()), received.Sender);

                        }
                        if (p.getCol() > 9)
                        {
                            p.setStartingLocation(p.getStartingLocation() + 16);
                            if (p.getSectorRegion().Equals('P'))
                            {
                                p.getSector().removePlayer(p);
                                p.setSector(u.getSectors()[p.getSectorValue()]);
                                p.getSector().addPlayer(p);
                                server.Reply(System.Text.Encoding.UTF8.GetString(p.getSector().sendSectorData()), received.Sender);
                                p.setStartingLocation(p.getSectorValue());

                            }


                            else
                            {
                                p.getSector().removePlayer(p);
                                p.setSector(u.getSectors()[p.getStartingLocation()]);
                                p.getSector().addPlayer(p);
                                server.Reply(System.Text.Encoding.UTF8.GetString(p.getSector().sendSectorData()), received.Sender);
                            }
                            p.setCol(0);
                        }
                        if (p.getCol() < 0)
                        {
                            p.setStartingLocation(p.getStartingLocation() - 16);
                            if ((p.getSectorRegion().Equals('A')))
                            {
                                p.getSector().removePlayer(p);
                                p.setSector(u.getSectors()[u.getSectors().Count - (16 - p.getSectorValue())]);
                                p.getSector().addPlayer(p);
                                server.Reply(System.Text.Encoding.UTF8.GetString(p.getSector().sendSectorData()), received.Sender);
                                p.setStartingLocation(u.getSectors().Count - (16 - p.getSectorValue()));
                            }
                            else
                            {
                                p.getSector().removePlayer(p);
                                p.setSector(u.getSectors()[p.getStartingLocation()]);
                                p.getSector().addPlayer(p);
                                server.Reply(System.Text.Encoding.UTF8.GetString(p.getSector().sendSectorData()), received.Sender);

                            }
                            p.setCol(9);
                        }

                        if (p.getRow() < 0)
                        {
                            p.setStartingLocation(p.getStartingLocation() - 1);
                            if (p.getSectorValue() == 0)
                            {
                                p.getSector().removePlayer(p);
                                p.setSector(u.getSectors()[p.getStartingLocation() + 16]);
                                p.getSector().addPlayer(p);
                                server.Reply(System.Text.Encoding.UTF8.GetString(p.getSector().sendSectorData()), received.Sender);
                                p.setStartingLocation(p.getStartingLocation() + 16);


                            }
                            else
                            {
                                p.getSector().removePlayer(p);
                                p.setSector(u.getSectors()[p.getStartingLocation()]);
                                p.getSector().addPlayer(p);
                                server.Reply(System.Text.Encoding.UTF8.GetString(p.getSector().sendSectorData()), received.Sender);
                            }
                            p.setRow(9);

                        }
                        if (p.getRow() > 9)
                        {
                            p.setStartingLocation(p.getStartingLocation() + 1);
                            if (p.getSectorValue() == 15)
                            {
                                p.getSector().removePlayer(p);
                                p.setSector(u.getSectors()[p.getStartingLocation() - 16]);
                                p.getSector().addPlayer(p);
                                server.Reply(System.Text.Encoding.UTF8.GetString(p.getSector().sendSectorData()), received.Sender);
                                p.setStartingLocation(p.getStartingLocation() - 16);
                            }
                            else
                            {
                                p.getSector().removePlayer(p);
                                p.setSector(u.getSectors()[p.getStartingLocation()]);
                                p.getSector().addPlayer(p);
                                server.Reply(System.Text.Encoding.UTF8.GetString(p.getSector().sendSectorData()), received.Sender);
                            }
                            p.setRow(0);
                        }

                        if (p.getSector().getPlayers().Count == 1)
                        {
                            string sendStream = "";
                            sendStream += p.getSectorRegion() + "-";
                            sendStream += p.getSectorValue() + "-";
                            sendStream += p.getRow() + "-";
                            sendStream += p.getCol() + "-";
                            sendStream += p.getHealth() + "-";
                            sendStream += p.getFuel() + "-";
                            sendStream += p.getPhasors() + "-";
                            sendStream += p.getTorpedos();
                            byte[] b = new byte[18];
                            b = Encoding.ASCII.GetBytes(sendStream);
                            server.Reply((String.Format("loc:{0}:{1}:{2}", System.Text.Encoding.UTF8.GetString(b), parts[1], message)), received.Sender);

                        }
                        else
                        {
                            Console.Write("Check 1");
                            string ourPlayer = "";
                            string otherPlayers = "";
                            byte[] b1 = new byte[18];
                            int x = p.getSector().getPlayers().Count;
                            byte[] b2 = new byte[x * 6];
                            foreach (Player p1 in p.getSector().getPlayers() )
                            {

                                if (p1.getRow() == p.getRow() && p1.getCol() == p.getCol())
                                {
                                    ourPlayer += p.getSectorRegion() + "-";
                                    ourPlayer += p.getSectorValue() + "-";
                                    ourPlayer += p.getRow() + "-";
                                    ourPlayer += p.getCol() + "-";
                                    ourPlayer += p.getHealth() + "-";
                                    ourPlayer += p.getFuel() + "-";
                                    ourPlayer += p.getPhasors() + "-";
                                    ourPlayer += p.getTorpedos() + "-";
                                }
                                else
                                {
                                    if (p1.getHealth() > 0)
                                    {
                                        otherPlayers += p1.getRow() + "-";
                                        otherPlayers += p1.getCol() + "-";
                                        otherPlayers += p1.getShipAngle() + "-";

                                    }
                                }
                            }
                            b1 = Encoding.ASCII.GetBytes(ourPlayer);
                            b2 = Encoding.ASCII.GetBytes(otherPlayers);

                            server.Reply((String.Format("loc2:{0}:{1}:{2}:{3}", System.Text.Encoding.UTF8.GetString(b1), System.Text.Encoding.UTF8.GetString(b2), parts[1], message)), received.Sender);

                        }


                    }
                }
            });


            // Endless loop for user's to send messages to Client
            string read;
            do
            {
                read = Console.ReadLine();
                foreach (IPEndPoint ep in connections.Values)
                    server.Reply(read, ep);
            } while (read != "quit");
        }

        private static void fixParts(string[] parts)
        {
            for (int i = 0; i < parts.Length; i++)
                parts[i] = parts[i].Trim().ToLower();
        }

        static private int isCollision(Sector s, int row, int col)
        {
            if (!s.checkSpaceUsed(row, col))
            {
                return 0;
            }
            else
            {
                foreach (Star st in s.getStars())
                {
                    if (st.getCol() == col && st.getRow() == row)
                    {
                        return 1;
                    }
                }
                foreach (Planet p in s.getPlanets())
                {
                    if (p.getCol() == col && p.getRow() == row)
                    {
                        return 2;
                    }
                }
                foreach (Blackhole b in s.getBlackhole())
                {
                    if (b.getCol() == col && b.getRow() == row)
                    {
                        return 3;
                    }
                }
                foreach (Treasure t in s.getTreasure())
                {
                    if (t.getCol() == col && t.getRow() == row)
                    {
                        return 4;
                    }
                }
            }
            return 0;
        }

        private static char getRandomRegion(char[] chars)
        {
            int val = regionRandom.Next(0, 15);
            return chars[val];
        }

        private Sector findSector(Sector s)
        {
            return s;
        }



    }
}
