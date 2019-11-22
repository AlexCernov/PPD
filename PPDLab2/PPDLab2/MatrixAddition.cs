using PPDLab2.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace PPDLab2
{
    class MatrixAddition
    {
        public static Matrix PerformAddition(Matrix matrix1, Matrix matrix2, List<Thread> threads)
        {
            if (matrix1.Rows() != matrix2.Rows() || matrix1.Columns() != matrix2.Columns())
                throw new Exception("Number of rows/columns don't match!");

            Matrix resultMatrix = new Matrix(matrix1.Rows(), matrix1.Columns());
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            for(int tid = 0; tid < threads.Count; tid++)
            {
                    int temp = tid;
                    threads[tid] = new Thread(() => SubAddition(matrix1, matrix2, resultMatrix, threads.Count, temp));
                    threads[tid].Start();
            }

            foreach (Thread t in threads)
            {
                t.Join();
            }

            stopwatch.Stop();
            Console.WriteLine("TIME TAKEN: " + stopwatch.ElapsedMilliseconds + "ms");
            return resultMatrix;
        }

        private static void SubAddition(Matrix matrix1, Matrix matrix2, Matrix resultMatrix, int threadCount, int threadNumber)
        {
            int threadWork = threadNumber;
            for (int i = 0; i < matrix1.Rows(); ++i)
            {
                for (int j = 0; j < matrix1.Columns(); ++j)
                {
                    if (threadWork == 0)
                    {
                        resultMatrix.SetCell(i, j, matrix1.GetCell(i, j) + matrix2.GetCell(i, j));
                        threadWork = threadCount - 1;
                    }
                    else
                        threadWork--;
                }
            }
        }
    }
}
