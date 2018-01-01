using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSP
{
    class Program
    {
        static void func(int L, int k, int propability, int mutation_number, int repeatNumber, int tourney, string fileName)
        {
            double srednia = 0;
            double srednia2 = 9999999;

            // do liczenia mediany:
            ArrayList mediana = new ArrayList();
            ArrayList times = new ArrayList();

            TimeSpan diff = new TimeSpan();
            for (int i = 0; i < repeatNumber; i++)
            {
                SimulatedAnnealing problem = new SimulatedAnnealing();
                problem.FilePath = fileName;
                var startTime = DateTime.Now;
                var distance = problem.Run(L, k, propability, mutation_number, tourney);
                var stopTime = DateTime.Now;
                diff = stopTime - startTime;
                srednia += distance;
                mediana.Add(distance);
                times.Add(diff);
                if (srednia2 > distance) srednia2 = distance;
            }

            mediana.Sort();
            times.Sort();
            Console.WriteLine("Parametry algorytmu: " + "L: " + L + " k: " + k + " Prawdopodobienstwo: " + propability + " Ilosc mutacji: " + mutation_number + " Ilosc osobnikow w turnieju: " + tourney + " Plik: " + fileName + " Ilosc powtorzen: " + repeatNumber);
            TimeSpan czas = (TimeSpan)times[repeatNumber / 2];
            Console.WriteLine("Mediana: " + mediana[repeatNumber / 2]);
            Console.WriteLine("Średni czas pracy: " + (czas.TotalMilliseconds) / 1000 + "s");
            srednia /= repeatNumber;
            Console.WriteLine("Srednia: " + srednia);
            Console.WriteLine("Najlepsza sciezka koszt: " + srednia2);
            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");
        }

        static void Main(string[] args)
        {
            func(120, 6000, 100, 1, 5, 2, "Cities.txt");
            Console.ReadLine();

        }
    }
}
