using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text;

namespace UPDServer
{
    class Sector
    {
        private char region;
        private int value;
        private List<Star> stars;
        private List<Planet> planets;
        private List<Player> players;
        private List<Blackhole> blackholes;
        private List<Treasure> treasures;
        public static int byteSize = 0;
        private byte[] b;

        public Sector()
        {      

        }

        public Sector(char region, int value)
        {
            this.region = region;
            this.value = value;
            b = new byte[byteSize];
            stars = new List<Star>();
            planets = new List<Planet>();
            players = new List<Player>();
            blackholes = new List<Blackhole>();
            treasures = new List<Treasure>();

            for (int i = 0; i < 5; i++)
            {
                Star s = new Star();
                if (!checkSpaceUsed(s.getRow(), s.getCol()))
                {
                    stars.Add(s);
                }
                else
                {
                    i = i - 1;
                }
                byteSize++;

            }
            for (int i = 0; i < 2; i++)
            {
                Planet p = new Planet();
                if (!checkSpaceUsed(p.getRow(), p.getCol()))
                {
                    planets.Add(p);
                }
                else
                {
                    i = i - 1;
                }
                byteSize++;

            }
            for (int i = 0; i < 1; i++)
            {
                Blackhole b = new Blackhole();
                if (!checkSpaceUsed(b.getRow(), b.getCol()))
                {
                    blackholes.Add(b);
                }
                else
                {
                    i = i - 1;
                }
                byteSize++;

            }
            for (int i = 0; i < 1; i++)
            {
                Treasure t = new Treasure();
                if (!checkSpaceUsed(t.getRow(), t.getCol()))
                { 
                    treasures.Add(t);
                }
                else
                {
                    i = i - 1;
                }
                byteSize++;

            }
        }
        public void setRegion(char region)
        {
            this.region = region;
        }
        public char getRegion()
        {
            return region;
        }
        public void setValue(int value)
        {
            this.value = value;
        }
        public int getValue()
        {
            return value;
        }
        public List<Star> getStars()
        {
            return stars;
        }
        public List<Planet> getPlanets()
        {
            return planets;
        }
        public List<Blackhole> getBlackhole()
        {
            return blackholes;
        }
        public List<Treasure> getTreasure()
        {
            return treasures;
        }

        public byte[] sendSectorData()
        {
            String byteStream = "";
            //Add stars 
            foreach (Star s in stars)
            {
                byteStream += s.getRow().ToString() + s.getCol().ToString() + "-" + "1";
            }
            foreach (Planet p in planets)
            {
                byteStream += p.getRow().ToString() + p.getCol().ToString() + "-" + "2";
            }
            foreach (Blackhole b in blackholes)
            {
                byteStream += b.getRow().ToString() + b.getCol().ToString() + "-" + "3";
            }
            foreach (Treasure t in treasures)
            {
                byteStream += t.getRow().ToString() + t.getCol().ToString() + "-" + "4";
            }

            b = Encoding.ASCII.GetBytes(byteStream);



            return b;
        }

        public void addPlayer(Player p)
        {
            players.Add(p);
        }

        public void removePlayer(Player p)
        {
            players.Remove(p);
        }

        public bool checkSpaceUsed(int row, int col)
        {
            bool returnValue = false;
            foreach (Star s in stars)
            {
                if (s.getRow() == row || s.getCol() == col)
                {
                    returnValue = true;
                }
            }
            foreach (Planet p in planets)
            {
                if (p.getRow() == row || p.getCol() == col)
                {
                    returnValue = true;
                }
            }
            foreach (Player p in players)
            {
                if (p.getRow() == row || p.getCol() == col)
                {
                    returnValue = true;
                }
            }
            foreach (Blackhole b in blackholes)
            {
                if (b.getRow() == row || b.getCol() == col)
                {
                    returnValue = true;
                }
            }
            return returnValue;
        }

        public string getSafeLocation()
        {
            Random r = new Random();
            int row = r.Next(0, 9);
            int col = r.Next(0, 9);
            while (checkSpaceUsed(row, col))
            {
                row = r.Next(0, 9);
                col = r.Next(0, 9);
            }
            return row.ToString() + col.ToString();
        }

        public List<Player> getPlayers()
        {
            return players;
        }

    }
}
