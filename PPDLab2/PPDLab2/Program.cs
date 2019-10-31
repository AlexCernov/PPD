using System;
using System.Collections.Generic;
using System.Threading;
using PPDLab2.Model;

namespace PPDLab2
{
    class Program
    {
        static void Main(string[] args)
        {
            // should be divisible by total number of elements
            int numberOfThreads = 32;
            int n1 = 1000;
            int m1 = 1000;
            int n2 = 1000;
            int m2 = 1000;

            Console.WriteLine("ADDITION");
            DoMatrixAdditionMultiThreaded(numberOfThreads, n1, m1, n2, m2);
            Console.WriteLine("\n\n\nPRODUCT");
            DoMatrixProductMultiThreaded(numberOfThreads, n1, m1, n2, m2);
        }

        private static void DoMatrixAdditionMultiThreaded(int numberOfThreads, int n1, int m1, int n2, int m2)
        {
            Random r = new Random();
            Matrix a = new Matrix(n1, m1);
            Matrix b = new Matrix(n2, m2);

            for(int i = 0; i < a.Rows(); ++i)
            {
                for (int j = 0; j < a.Columns(); ++j)
                {
                    a.SetCell(i, j, r.Next(0, 11));
                    b.SetCell(i, j, r.Next(0, 11));
                }
            }

            List<Thread> threadList = new List<Thread>();

            for (int i = 0; i < numberOfThreads; ++i)
            {
                threadList.Add(null);
            }

            //Console.WriteLine("---FIRST MATRIX---\n" + a.ToString());
            //Console.WriteLine("---SECOND MATRIX---\n" + b.ToString());

            Matrix result = MatrixAddition.PerformAddition(a, b, threadList);

            //Console.WriteLine("---RESULT MATRIX---\n" + result.ToString());

            bool verdict = true;
            for(int i = 0; i < result.Rows(); ++i)
            {
                for(int j = 0; j < result.Columns(); ++j)
                {
                    if(a.GetCell(i, j) + b.GetCell(i, j) != result.GetCell(i, j))
                    {
                        verdict = false;
                        break;
                    }
                }
            }

            Console.WriteLine("CORRECT: " + verdict);
        }

        private static void DoMatrixProductMultiThreaded(int numberOfThreads, int n1, int m1, int n2, int m2)
        {
            Random r = new Random();
            Matrix a = new Matrix(n1, m1);
            Matrix b = new Matrix(n2, m2);

            for (int i = 0; i < a.Rows(); ++i)
            {
                for (int j = 0; j < a.Columns(); ++j)
                {
                    a.SetCell(i, j, r.Next(0, 11));
                    b.SetCell(i, j, r.Next(0, 11));
                }
            }

            List<Thread> threadList = new List<Thread>();

            for (int i = 0; i < numberOfThreads; ++i)
            {
                threadList.Add(null);
            }

            Matrix result = MatrixProduct.PerformProduct(a, b, threadList);

            //Console.WriteLine("---FIRST MATRIX---\n" + a.ToString());
            //Console.WriteLine("---SECOND MATRIX---\n" + b.ToString());
            //Console.WriteLine("---RESULT MATRIX---\n" + result.ToString());
        }
    }
}
