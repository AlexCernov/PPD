#include <iostream>
#include <thread>
#include <cstdlib>
#include <chrono>
#include <vector>
#include <future>
#include <queue>
#include <tuple>
#include <chrono>
#include <mutex>
#include <math.h>
#include <condition_variable>

using namespace std;

int firstMatrix[1000][1000], secondMatrix[1000][1000], thirdMatrix[1000][1000], producerResult[1000][1000], consumerResult[1000][1000];
const int MAX_SIZE = 1000;
bool finished[1000];
condition_variable conditionalVariables[1000];
mutex mutx[32];

void initMatrices() {
	srand(time(NULL));

	for (int i = 0; i < MAX_SIZE; i++)
	{
		for (int j = 0; j < MAX_SIZE; j++)
		{
			firstMatrix[i][j] = rand() % 10;
			secondMatrix[i][j] = rand() % 10;
			thirdMatrix[i][j] = rand() % 10;
			producerResult[i][j] = 0;
			consumerResult[i][j] = 0;
		}
	}
}
void printFirstMatrix() {
	for (int i = 0; i < MAX_SIZE; i++)
	{
		for (int j = 0; j < MAX_SIZE; j++)
			cout << firstMatrix[i][j] << " ";
		cout << endl;
	}
}
void printSecondMatrix() {
	for (int i = 0; i < MAX_SIZE; i++)
	{
		for (int j = 0; j < MAX_SIZE; j++)
			cout << secondMatrix[i][j] << " ";
		cout << endl;
	}
}
void printThirdMatrix() {
	for (int i = 0; i < MAX_SIZE; i++)
	{
		for (int j = 0; j < MAX_SIZE; j++)
			cout << thirdMatrix[i][j] << " ";
		cout << endl;
	}
}
void printResultMatrices() {
	cout << "First Result" << endl;
	for (int i = 0; i < MAX_SIZE; i++)
	{
		for (int j = 0; j < MAX_SIZE; j++)
			cout << producerResult[i][j] << " ";
		cout << endl;
	}

	cout << "Second Result" << endl;
	for (int i = 0; i < MAX_SIZE; i++)
	{
		for (int j = 0; j < MAX_SIZE; j++)
			cout << consumerResult[i][j] << " ";
		cout << endl;
	}
}

void producer(int line, int numberOfThreads)
{
	lock_guard<mutex> lk(mutx[line]);

	for (int i = line; i < MAX_SIZE; i += numberOfThreads)
	{
		for (int j = 0; j < MAX_SIZE; j++)
		{
			for (int k = 0; k < MAX_SIZE; k++)
			{
				producerResult[i][j] += firstMatrix[i][k] * secondMatrix[k][j];
			}
			//cout << endl;
			//cout << "producer: "<<i<<" "<<j<< endl;
			//cout << "producer: " << i<<" "<< j << '\n';
		}

		finished[i] = true;
		if (numberOfThreads != 1)
			conditionalVariables[line].notify_all();
	}
}


void consumer(int line, int numberOfThreads)
{
	for (int i = line; i < MAX_SIZE; i += numberOfThreads)
	{
		for (int p = 0; p < MAX_SIZE; p++)
		{
			unique_lock<mutex> lk(mutx[line]);
			conditionalVariables[line].wait(lk, [line] { return finished[line] == true; });

			for (int k = 0; k < MAX_SIZE; k++)
			{
				consumerResult[p][i] += producerResult[i][k] * thirdMatrix[k][p];
			}

			//cout << "consumer: " << p << " " << i << '\n';
		}
	}
}

double startWork(int consumerThreads, int producerThreads)
{

	for (int i = 0; i < MAX_SIZE; i++) {
		finished[i] = false;
	}

	auto startTime = chrono::high_resolution_clock::now();
	vector<thread> producers, consumers;

	for (int i = 0; i < min(producerThreads, MAX_SIZE); i++)
	{
		producers.push_back(thread(producer, i, producerThreads));
	}

	for (int i = 0; i < min(consumerThreads, MAX_SIZE); i++)
	{
		consumers.push_back(thread(consumer, i, consumerThreads));
	}

	for (int i = 0; i < consumers.size(); i++)
	{
		consumers[i].join();
	}

	for (int i = 0; i < producers.size(); i++)
	{
		producers[i].join();
	}

	auto finishTime = chrono::high_resolution_clock::now();
	chrono::duration<double> elapsed = finishTime - startTime;

	return elapsed.count();
}

int main()
{

	initMatrices();
	/*printMatrixA();
	cout << endl;
	printMatrixB();
	cout << endl;
	printMatrixC();
	cout << endl;
	printResult();*/
	cout << "Time taken: " << startWork(8, 8) << "ms" << endl;
	return 0;
}