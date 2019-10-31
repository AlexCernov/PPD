using PPDLab2.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace PPDLab2
{
    class MatrixProduct
    {
        public static Matrix PerformProduct(Matrix matrix1, Matrix matrix2, List<Thread> threads)
        {
            if (matrix1.Columns() != matrix2.Rows())
                throw new Exception("Matrix multiplication not possible.");

            Matrix resultMatrix = new Matrix(matrix1.Rows(), matrix2.Columns());
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int t = 0; t < threads.Count; t++)
            {
                int temp = t;
                threads[t] = new Thread(() => SubProduct(matrix1, matrix2, resultMatrix, threads.Count, temp));
                threads[t].Start();
            }

            foreach (Thread t in threads)
            {
                t.Join();
            }

            stopwatch.Stop();
            Console.WriteLine("TIME TAKEN: " + stopwatch.ElapsedMilliseconds + "ms");
            return resultMatrix;
        }

        public static void SubProduct(Matrix matrix1, Matrix matrix2, Matrix resultMatrix, int threadCount, int threadNumber)
        {
            int threadWork = threadNumber;

            for (int i = 0; i < matrix1.Rows(); ++i)
            {
                for (int j = 0; j < matrix2.Columns(); ++j)
                {
                    if (threadWork == 0)
                    {
                        resultMatrix.SetCell(i, j, 0);
                        for (int k = 0; k < matrix1.Columns(); ++k)
                        {
                            int oldValue = resultMatrix.GetCell(i, j);
                            resultMatrix.SetCell(i, j, oldValue + matrix1.GetCell(i, k) * matrix2.GetCell(k, j));
                        }
                        threadWork = threadCount - 1;
                    }
                    else
                        threadWork--;
                }
            }
        }
    }
}
