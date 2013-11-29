using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Do_min
{
    public class Board
    {
        public int nRow { get; set; }
        public int nCol { get; set; }
        public int nBomb { get; set; }

        public const int Opened = 0;
        public const int Close = 1;
        public const int Flag = 2;

        public const int Bomb = -1;
        public const int Empty = 0;

        public Board(int row, int col, int bomb)
        {
            this.nRow = row;
            this.nCol = col;
            this.nBomb = bomb;
        }

        public void Generate(string path)
        {
            XmlElement Board = XL_XML.Tao_goc("BOARD");
            Board.SetAttribute("nRow", nRow.ToString());
            Board.SetAttribute("nCol", nCol.ToString());
            Board.SetAttribute("nBomb", nBomb.ToString());
            CreateCells(Board);
            XL_XML.Ghi_nut(Board, path);
        }

        private void CreateCells(XmlElement Board)
        {
            for (int i = 0; i < nRow * nCol; i++)
            {
                XmlElement cell = XL_XML.Tao_nut("CELL", Board);
                cell.SetAttribute("row", (i / nRow).ToString());
                cell.SetAttribute("col", (i % nCol).ToString());
                cell.SetAttribute("state", Close.ToString());
                cell.SetAttribute("value", Empty.ToString());
            }
            CreateBombCells(Board);
        }

        private void CreateBombCells(XmlElement Board)
        {
            Random rdm = new Random();
            for (int i = 0; i < nBomb; i++)
            {
                int index = -1;
                XmlNode cell = null;
                do
                {
                    index = rdm.Next(0, Board.ChildNodes.Count - 1);
                    cell = Board.ChildNodes[index];
                }
                while(cell.Attributes["value"].Value == Bomb.ToString());
                cell.Attributes["value"].Value = Bomb.ToString();

                // danh so cho cac o xung quanh min
                int currentRow = index / nCol;
                int currentCol = index % nRow;
                int r = currentRow - 1;
                while (r <= currentRow + 1)
                {
                    int c = currentCol - 1;
                    while (c <= currentCol + 1)
                    {
                        if (!(r == currentRow && c == currentCol))
                        {                            
                            XmlNode nearCell = Board.SelectSingleNode(string.Format("CELL[@row='{0}' and @col='{1}']", r, c));
                            if (nearCell != null)
                            {
                                int value = int.Parse(nearCell.Attributes["value"].Value);
                                nearCell.Attributes["value"].Value = (value == Bomb) ? value.ToString() : (value + 1).ToString();
                            }
                        }
                        c++;
                    }
                    r++;
                }
            }
        }
    }
}
