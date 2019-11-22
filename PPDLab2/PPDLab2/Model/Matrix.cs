using System.Text;

namespace PPDLab2.Model
{
    class Matrix
    {
        private int[,] matrix;
        private int rows;
        private int columns;

        public Matrix(int r, int c)
        {
            rows = r;
            columns = c;
            matrix = new int[rows, columns];
        }

        public void SetCell(int row, int column, int value)
        {
            matrix[row, column] = value;
        }

        public int GetCell(int row, int column)
        {
            return matrix[row, column];
        }

        public int Rows()
        {
            return rows;
        }

        public int Columns()
        {
            return columns;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder("");

            for(int i = 0; i < rows; ++i)
            {
                for(int j = 0; j < columns; ++j)
                {
                    s.Append(matrix[i, j].ToString() + " ");
                }
                s.Append("\n");
            }

            return s.ToString();
        }
    }
}
