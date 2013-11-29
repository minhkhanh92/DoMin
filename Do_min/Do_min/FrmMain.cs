using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace Do_min
{
    public partial class FrmMain : Form
    {
        private const int SizeCell = 50;
        private Image flag;
        private Image bomb;
        private bool isPlaying = false;
        private bool isWin = false;
        private string path = "../../board.xml";

        public FrmMain()
        {
            InitializeComponent();

            flag = Image.FromFile("flag.png");
            bomb = Image.FromFile("bomb.png");
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            if (File.Exists(path))
            {
                LoadGame();
            }
            else
            {
                NewGame();
            }
        }

        private void NewGame()
        {
            Board board = new Board(9, 9, 10);
            board.Generate(path);
            LoadGame();
        }

        private void LoadGame()
        {
            XmlElement xmlBoard = XL_XML.Doc_goc(path);
            CreateBoard(xmlBoard);
            isPlaying = true;
            isWin = false;
        }

        private void CreateBoard(XmlElement Board)
        {
            int nRow = int.Parse(Board.GetAttribute("nRow"));
            int nCol = int.Parse(Board.GetAttribute("nCol"));
            this.pnBoard.Size = new Size(nCol * SizeCell, nRow * SizeCell);
            this.pnBoard.Controls.Clear();

            XmlNodeList cells = Board.ChildNodes;
            for(int i = 0; i < cells.Count; i++)
            {
                XmlNode cell = cells[i];
                Button bt = new Button();
                bt.Size = new System.Drawing.Size(SizeCell, SizeCell);
                bt.Location = new Point((i % nRow) * SizeCell, (i / nCol) * SizeCell);
                bt.Tag = cell;
                bt.MouseUp += bt_MouseUp;
                SetTypeCell(bt);
                bt.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                bt.ForeColor = System.Drawing.Color.Blue;
                this.pnBoard.Controls.Add(bt);
            }
        }

        void bt_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isPlaying) return;
            Button bt = sender as Button;
            XmlNode xmlCell = bt.Tag as XmlNode;
            int state = int.Parse(xmlCell.Attributes["state"].Value);
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (state == Board.Close)
                {
                    state = Board.Opened;
                    int value = int.Parse(xmlCell.Attributes["value"].Value);
                    if (value == Board.Empty)
                    {
                        OpenNearEmptyCells(xmlCell);
                        UpdateAllBoard();
                    }
                    else if (value == Board.Bomb)
                    {
                        OpenAllBombCells(xmlCell.ParentNode);
                        UpdateAllBoard();
                        isPlaying = false;
                        timer.Enabled = true;
                    }
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (state == Board.Flag)
                {
                    state = Board.Close;
                }
                else if (state == Board.Close)
                {
                    state = Board.Flag;
                }
            }
            xmlCell.Attributes["state"].Value = state.ToString();
            SetTypeCell(bt);
            if (CheckWin(xmlCell.ParentNode))
            {
                isWin = true;
                isPlaying = false;
                timer.Enabled = true;
            }
        }

        private bool CheckWin(XmlNode xmlBoard)
        {
            int nRow = int.Parse(xmlBoard.Attributes["nRow"].Value);
            int nCol = int.Parse(xmlBoard.Attributes["nCol"].Value);
            int nBomb = int.Parse(xmlBoard.Attributes["nBomb"].Value);

            return xmlBoard.SelectNodes(string.Format("CELL[@state='{0}']", Board.Opened)).Count == (nRow * nCol) - nBomb;
        }

        private void OpenAllBombCells(XmlNode xmlBoard)
        {
            foreach (XmlNode cell in xmlBoard.SelectNodes(string.Format("CELL[@value='{0}']", Board.Bomb)))
            {
                cell.Attributes["state"].Value = Board.Opened.ToString();
            }
        }

        private void UpdateAllBoard()
        {
            foreach (Button bt in pnBoard.Controls)
            {
                SetTypeCell(bt);
            }
        }

        private void OpenNearEmptyCells(XmlNode cell)
        {
            int currentRow = int.Parse(cell.Attributes["row"].Value);
            int currentCol = int.Parse(cell.Attributes["col"].Value);

            int r = currentRow - 1;
            while (r <= currentRow + 1)
            {
                int c = currentCol - 1;
                while (c <= currentCol + 1)
                {
                    if (!(r == currentRow && c == currentCol))
                    {
                        XmlNode nearCell = cell.ParentNode.SelectSingleNode(string.Format("CELL[@row='{0}' and @col='{1}']", r, c));
                        if (nearCell != null)
                        {
                            int state = int.Parse(nearCell.Attributes["state"].Value);
                            if (state != Board.Opened)
                            {
                                nearCell.Attributes["state"].Value = Board.Opened.ToString();
                                int value = int.Parse(nearCell.Attributes["value"].Value);
                                if (value != Board.Bomb)
                                {
                                    if (value == Board.Empty)
                                    {
                                        OpenNearEmptyCells(nearCell);
                                    }
                                }
                            }
                        }
                    }
                    c++;
                }
                r++;
            }
        }

        private void SetTypeCell(Button cell)
        {
            XmlNode xmlCell = cell.Tag as XmlNode;
            int state = int.Parse(xmlCell.Attributes["state"].Value);
            switch (state)
            {
                case Board.Flag:
                    cell.Image = flag;
                    break;
                case Board.Close:
                    cell.Image = null;
                    cell.BackColor = Color.Aqua;
                    break;
                case Board.Opened:
                    cell.BackColor = Color.White;
                    cell.Image = null;
                    int value = int.Parse(xmlCell.Attributes["value"].Value);
                    if (value == Board.Bomb)
                    {
                        cell.Image = bomb;
                        cell.BackColor = Color.Red;
                    }
                    else if (value != Board.Empty)
                    {
                        cell.Text = value.ToString();
                    }

                    break;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Enabled = false;
            string msg = isWin ? "Win" : "Game over";
            MessageBox.Show(this, msg, "Dò mìn", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SaveGame()
        {
            if (isPlaying)
            {
                XmlElement cell = (this.pnBoard.Controls[0] as Button).Tag as XmlElement;
                XL_XML.Ghi_nut((XmlElement)cell.ParentNode, path);
            }
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void saveGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveGame();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveGame();
        }
    }
}
