using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPDServer
{
    class Star
    {
        private int row;
        private int col;
        private static Random r = new Random();

        public Star()
        {
            row = r.Next(0, 9);
            col = r.Next(0, 9);
        }
        public void setRow(int row)
        {
            this.row = row;
        }
        public int getRow()
        {
            return row;
        }
        public void setCol(int col)
        {
            this.col = col;
        }
        public int getCol()
        {
            return col;
        }
        public String toString()
        {
            return row.ToString() + col.ToString();
        }
    }
}
