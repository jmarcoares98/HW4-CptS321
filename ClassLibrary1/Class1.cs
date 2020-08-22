using System;
using System.ComponentModel;

namespace Cpts321
{
    public abstract class Cell : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected int mRowIndex;
        protected int mColIndex;
        protected string mText;
        protected string mValue;
   
        // Constructor
        public Cell(int row, int column)
        {
            mRowIndex = row;
            mColIndex = column;
            mText = "";
            mValue = "";
        }

        // notify if there is a property change on the cell
        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // getter and setter for row
        public int RowIndex
        {
            get { return mRowIndex; }
            protected set { this.mRowIndex = value; }
        }

        // getter and setter for column
        public int ColIndex
        {
            get { return mColIndex; }
            protected set { this.mColIndex = value; }
        }

        // getter and setter for text
        public string Text
        {
            get { return this.mText; }

            protected internal set
            {
                if (value != this.mText)
                {
                    this.mText = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Text"));
                }
                else
                {
                    return;
                }
            }
        }

        // getter and setter for value
        public string Value
        {
            get { return this.mValue; }

            protected internal set
            {
                if (value != this.mValue)
                {
                    this.mValue = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                }
                else
                {
                    return;
                }
            }
        }

    }

    /// <summary>
    /// initializes the row and column in its base
    /// </summary>
    public class CellInit : Cell
    {
        public CellInit(int row, int col) : base(row, col)
        {
            
        }
    }

    //class for the spreadsheet
    public class Spreadsheet
    {
        public event PropertyChangedEventHandler CellPropertyChanged; // to subscribe to a single event
        private int rowCount;
        private int colCount;
        private Cell[,] mCell;

        /// <summary>
        /// this is where we can put strings in the spreadsheet
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public Spreadsheet(int row, int col)
        {
            this.rowCount = row;
            this.colCount = col;
            mCell = new Cell[row, col];

            //2d array cell
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    mCell[i, j] = new CellInit(i, j);
                    mCell[i, j].PropertyChanged += OnPropChanged;
                }
            }
        }

        public int RowCount
        {
            get { return this.rowCount; }
        }

        public int ColCount
        {
            get { return this.colCount; }
        }

        /// <summary>
        /// returns the cell of a specific location
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public Cell GetCell(int row, int col)
        {
            if(row > rowCount || col > ColCount )
            {
                return null;
            }

            return mCell[row, col];
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPropChanged(object sender, PropertyChangedEventArgs e)
        {
            Cell c = sender as Cell; // makes the sender into a cell and setting a temp value for it
            if(e.PropertyName == "Text")
            {
                if(c.Text.StartsWith("=")) // checks if the text starts with an '='
                {
                    // formula for copying the cell
                    string incel = c.Text.Substring(1); 
                    int col = Convert.ToInt16(incel[0]) - 'A';
                    int row = Convert.ToInt16(incel.Substring(1)) - 1;
                    c.Value = (GetCell(row, col)).Value;
                }
                else
                {
                    c.Value = c.Text;
                }
            }
            CellPropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Value")); // this is to check if there is an inputted value and will check if there is a property changed
        }

        /// <summary>
        /// this is what the perform demo button does when pressed
        /// </summary>
        public void Demo()
        {
            // randomize which row and column it chooses to put the "I love C#" text 
            int randomRow = 0, randomCol = 0;
            Random random = new Random();

            for(int i = 0; i < 50; i++)
            {
                randomCol = random.Next(0, 25);
                randomRow = random.Next(0, 49);

                Cell fill = GetCell(randomRow, randomCol);
                fill.Text = "I love C#";
                mCell[randomRow, randomCol] = fill;
            }

            // goes to column B and places the string
            for(int i = 0; i < 50; i++)
            {
                this.mCell[i, 1].Text = "This is Cell B" + (i + 1).ToString();
            }

            // copies because the text starts with '='
            for(int i = 0; i < 50; i++)
            {
                this.mCell[i, 0].Text = "=B" + (i + 1).ToString();
            }
        }
    }
}
