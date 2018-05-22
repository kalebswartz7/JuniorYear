using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPDServer
{
    class Universe
    {
        List<Sector> sectors;
        public Universe()
        {
            sectors = new List<Sector>();
            char region = 'A';
            int count = 0;
            for (int i = 0; i < 256; i++)
            {
                Sector s = new Sector(region, count);
                sectors.Add(s);
                if (count == 15)
                {
                    region = (char)((int)region + 1);
                    count = -1;
                }
                count++;

            }

        }

        public void printUniverse()
        {
            foreach (Sector b in sectors)
            {
                Console.WriteLine(System.Text.Encoding.UTF8.GetString(b.sendSectorData()));
            }
        }

        public Sector findSector(Sector s)
        {
            Sector dummy = new Sector();
            foreach (Sector s1 in sectors)
            {
                if (s.getValue() == s1.getValue() && s.getRegion().Equals(s1.getRegion()))
                {
                    return s1;
                }
            }
            return dummy;
        }

        public List <Sector> getSectors()
        {
            return sectors;
        }

    }
}
