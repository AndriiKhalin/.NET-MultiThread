
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

Console.WriteLine("Enter vector");
float[] myVector = Array.ConvertAll(Console.ReadLine().Split(' '), float.Parse);

Console.WriteLine("Enter number");
float myNumber = Single.Parse(Console.ReadLine());

Console.WriteLine("Enter thread");
int myThreadCount = Int32.Parse(Console.ReadLine());

MultiThreadedVectorMultiplier myAlgoritm = new MultiThreadedVectorMultiplier(myVector, myNumber, myThreadCount);

float[] myResult = myAlgoritm.SingleThreadedAlgorithm();


float[] myResult2 = myAlgoritm.MultiThreadedAlgorithm();




//Test the amount of acceleration
int[] vectorSizes = { 1000000, 2000000, 3000000 };

foreach (var size in vectorSizes)
{
    for (int count = 2; count <= Environment.ProcessorCount; count += 2)
    {
        Console.WriteLine($"Testing for vector size {size} and {count} threads:");



        float[] vector = MultiThreadedVectorMultiplier.GenerateRandomVector(size);

        MultiThreadedVectorMultiplier algoritm = new MultiThreadedVectorMultiplier(vector, 2, count);

        Stopwatch stopwatch = Stopwatch.StartNew();
        float[] result = algoritm.SingleThreadedAlgorithm();

        stopwatch.Stop();
        long singleThreadTime = stopwatch.ElapsedMilliseconds;

        stopwatch.Restart();
        float[] result2 = algoritm.MultiThreadedAlgorithm();

        stopwatch.Stop();
        long multiThreadTime = stopwatch.ElapsedMilliseconds;

        Console.WriteLine($"Single-threaded algorithm execution time: {singleThreadTime} ms");
        Console.WriteLine($"Multi-threaded algorithm execution time with {count} threads: {multiThreadTime} ms");

        double speedup = (double)singleThreadTime / multiThreadTime;
        Console.WriteLine($"Speedup: {speedup:F2}x\n");
    }
}




//class implementation of the algorithm
public class MultiThreadedVectorMultiplier
{
    private float[] vector;
    private float number;
    private int numThreads;

    public MultiThreadedVectorMultiplier(float[] vector, float number, int numThreads)
    {
        this.vector = vector;
        this.number = number;
        if (numThreads > Environment.ProcessorCount)
        {
            this.numThreads = Environment.ProcessorCount;
        }
        else
        {
            this.numThreads = numThreads;
        }
    }

    public static float[] GenerateRandomVector(int size)
    {

        Random rand = new Random();
        float[] vector = new float[size];
        for (int i = 0; i < size; i++)
        {
            vector[i] = rand.NextSingle();
        }

        return vector;
    }

    public float[] SingleThreadedAlgorithm()
    {
        float[] result = new float[this.vector.Length];

        for (int i = 0; i < this.vector.Length; i++)
        {
            result[i] = this.vector[i] * this.number;
        }

        return result;
    }


    public float[] MultiThreadedAlgorithm()
    {

        float[] result = new float[this.vector.Length];
        Thread[] threads = new Thread[this.numThreads];

        for (int i = 0; i < this.numThreads; i++)
        {
            int threadNumber = i;
            threads[i] = new Thread(() =>
            {
                for (int j = threadNumber; j < this.vector.Length; j += this.numThreads)
                {
                    result[j] = this.vector[j] * this.number;
                }
            });
            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return result;


    }
}
