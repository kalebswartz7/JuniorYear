using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPDServer
{
    class Player
    {
        private int lifePower;
        private int fuelPods;
        private int phasors;
        private int torpedos;
        private bool shieldOn;
        private int startingLocation;
        private int shipAngle; 

        private int col;
        private int row;
        private Sector s;

        public Player()
        {
            s = new Sector('a', 5);
            lifePower = 100;
            shipAngle = 0;
            fuelPods = 50;
            phasors = 50;
            torpedos = 10;
            shieldOn = false;
            startingLocation = 0;
            shieldOn = false;

        }

        public int getPhasors()
        {
            return phasors;
        }
        public void setPhasors(int phasors)
        {
            this.phasors = phasors; 
        }
        public int getTorpedos()
        {
            return torpedos;
        }
        public void setTorpedos(int torpedos)
        {
            this.torpedos = torpedos;
        }

        public int getShipAngle()
        {
            return shipAngle;
        }
        public void setShipAngle(int shipAngle)
        {
            this.shipAngle = shipAngle;
        }

        public int getStartingLocation()
        {
            return startingLocation;
        }
        public void setStartingLocation(int startingLocation)
        {
            this.startingLocation = startingLocation;
        }

        public void setCol(int col)
        {
            this.col = col;
        }
        public int getCol()
        {
            return col;
        }

        public void setRow(int row)
        {
            this.row = row;
        }
        public int getRow()
        {
            return row;
        }
        public void setSector(char region, int value)
        {
            s.setRegion(region);
            s.setValue(value);
        }
        public void setSector(Sector s)
        {
            this.s = s;
        }
        public char getSectorRegion()
        {
            return s.getRegion();
        }
        public int getSectorValue()
        {
            return s.getValue();
        }
        public Sector getSector()
        {
            return s;
        }
        public void setFuel(int x)
        {
            fuelPods = x;
        }
        public int getFuel()
        {
            return fuelPods;
        }
        public int getHealth ()
        {
            return lifePower; 
        }
        public void setHealth(int x)
        {
            lifePower = x;
        }
        
        public bool getShield()
        {
            return shieldOn;
        }
        public void changeShield()
        {
            shieldOn = !shieldOn;
        }

    }
}
