using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSP
{
    class Program
    {
        static void Main(string[] args)
        {
            TravellingSalesmanProblem problem = new TravellingSalesmanProblem();
            problem.FilePath = "Cities.txt";
            problem.Anneal();

            string path = "";
            for (int i = 0; i < problem.CitiesOrder.Count - 1; i++)
            {
                path += problem.CitiesOrder[i] + " -> ";
            }
            path += problem.CitiesOrder[problem.CitiesOrder.Count - 1];

            Console.WriteLine("Shortest Route: " + path);

            Console.WriteLine("The shortest distance is: " + problem.ShortestDistance.ToString());

            Console.ReadLine();
        }
    }
}
