using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;

namespace TSP
{
    public class SimulatedAnnealing
    {
        private string filePath; // sciezka do pliku
        private List<int> currentOrder = new List<int>(); // aktualne rozwiazanie
        private List<int> nextOrder = new List<int>(); // nowe rozwiązanie
        private List<List<int>> Pw = new List<List<int>>(); // populacja wejściowa
        private List<List<int>> Pn = new List<List<int>>(); // nowa populacja
        private List<List<int>> P = new List<List<int>>(); // populacja wyjsciowa

        private double[,] distances; // tabela miast
        private Random random = new Random(); 
        private double shortestDistance = 0; // najlepsza ścieżka

        public double ShortestDistance
        {
            get
            {
                return shortestDistance;
            }
            set
            {
                shortestDistance = value;
            }
        }

        public bool CompareLists(List<List<int>> MainList, List<int> list)
        {
            bool flag = false;
            foreach (var k in MainList)
            {
                if (k.SequenceEqual(list))
                {
                    flag = true;
                    break;
                }
                else
                {
                    flag = false;
                }
            }
            return flag;
        }

        public string FilePath
        {
            get
            {
                return filePath;
            }
            set
            {
                filePath = value;
            }
        }

        public List<int> CitiesOrder
        {
            get
            {
                return currentOrder;
            }
            set
            {
                currentOrder = value;
            }
        }

        public bool Included(int[] tab, int k)
        {
            bool flag = false;
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i] == k)
                {
                    flag = false;
                    break;
                }
                else
                {
                    flag = true;
                }
            }
            return flag;
        }

        private List<List<int>> LoadCities(int NumberOfGenerations)
        {
            StreamReader reader = new StreamReader(filePath);

            string cities = reader.ReadToEnd();

            reader.Close();

            string[] rows = cities.Split('\n');

            distances = new double[rows.Length, rows.Length];
            int x = 0;
            int temp1 = 0;
            int temp2 = 0;

            for (int i = 0; i < rows.Length; i++)
            {
                string[] distance = rows[i].Split(' ');
                temp1 = 0;
                temp2 = 0;
                if (distance[x] == "")
                {
                    x++;
                    temp1++;
                    temp2++;
                    if (i != rows.Length - 1) distance[rows.Length] = distance[rows.Length].Substring(0, distance[rows.Length].Length-1);
                }
                else
                {
                    if (i!= rows.Length-1) distance[rows.Length - 1] = distance[rows.Length - 1].Substring(0, distance[rows.Length - 1].Length - 1);
                }
                // powyzsze sluzy do usuniecie ze stringa znaku nowej linii
                int j;
                for (j = 0; j < distance.Length-temp1; j++,x++)
                {
                    if (double.Parse(distance[x]) < 1)
                    {
                        distances[i, j] = 99999;
                    }
                    else
                    {
                        distances[i, j] = double.Parse(distance[x]);
                    }
                    if (j == distance.Length - 1 - temp2) x = -1;
                    //currentOrder.Add(i);
                }          
            }
            int k = 0;
            for (int i = 0; i < NumberOfGenerations; i++)
            {
                wroclista:
                List<int> currentOrder = new List<int>();
                int[] prohibitionTab = new int[rows.Length];
                int index = 0;
                int temp = 0;             
                currentOrder.Add(0);
                for (int j = 0; j < rows.Length; j++)
                {
                    wroc:
                    k = random.Next(0, rows.Length);
                    if (index < rows.Length - 1)
                    {
                        if (Included(prohibitionTab, k))
                        {
                            prohibitionTab[index] = k;
                            currentOrder.Add(k);

                            index++;
                            temp = k;
                        }
                        else
                        {
                            goto wroc;
                        }
                    }
                }
                if (CompareLists(Pw,currentOrder) == true)
                {
                    goto wroclista;
                }
                else
                {
                    Pw.Add(currentOrder);
                }
                
            }
            if (Pw.Count < 1)
                throw new Exception("No cities to order.");
            return Pw;
        }

        private bool CheckList(List<int> list, int number)
        {
            bool flag = false;
            foreach(int i in list)
            {
                if (number == i) flag = true;
            }
            return flag;
        }

        private List<int> Crossing(List<int> Parent1, List<int> Parent2)
        {
            List<int> child = new List<int>();

            for (int i = 0; i < Parent1.Count(); i++)
            {
                child.Add(0);
            }

            int length = Parent1.Count()/2;
            Random rand = new Random();
            int range = rand.Next(1, Parent1.Count()-1);
            int range2 = rand.Next(range+1, Parent1.Count());
            int length2 = range2 - range;
            int P1_position = range;
            int ile_razy = 0;
            for (int i = 0; i < length2; i++) // przypisanie od rodzica 1
            {
                child[P1_position] = Parent1[P1_position];
                P1_position++;
                ile_razy++;
            }
            int tempIndex = P1_position;

            int ilosc = P1_position;
            
            while(ile_razy < Parent1.Count-1)
            { 
                if (!CheckList(child, Parent2[tempIndex]))
                {
                    child[ilosc] = Parent2[tempIndex];
                    ilosc++;
                    ile_razy++;
                }
                tempIndex++;
                if (tempIndex == Parent2.Count())
                {                    
                    tempIndex = 0;
                }
                if (ilosc == Parent2.Count())
                {
                    ilosc = 1;
                }
            }

            return child;
        }

        private List<int> Mutate(List<int> Generation, int NumberOfMutation, int Propability)
        {
            int w = random.Next(1, Propability);
            if (w<=15)
            {
                for (int i = 0; i < NumberOfMutation; i++)
                {
                    int r1 = random.Next(1, Generation.Count());
                    int r2 = random.Next(1, Generation.Count());
                    while (r1 == r2)
                    {
                        r2 = random.Next(1, Generation.Count());
                    }
                    int temp = 0;
                    temp = Generation[r1];
                    Generation[r1] = Generation[r2];
                    Generation[r2] = temp;
                }
            }
            return Generation;
        }

        public double GetTotalDistance(List<int> order)
        {
            double distance = 0;

            for (int i = 0; i < order.Count - 1; i++)
            {
                distance += distances[order[i], order[i + 1]];
            }

            if (order.Count > 0)
            {
                distance += distances[order[order.Count - 1], 0];
            }

            return distance;
        }

        private List<int> GetNextArrangement(List<int> order)
        {
            List<int> newOrder = new List<int>();

            for (int i = 0; i < order.Count; i++)
                newOrder.Add(order[i]);

            int firstRandomCityIndex = random.Next(1, newOrder.Count);
            int secondRandomCityIndex = random.Next(1, newOrder.Count);

            int dummy = newOrder[firstRandomCityIndex];
            newOrder[firstRandomCityIndex] = newOrder[secondRandomCityIndex];
            newOrder[secondRandomCityIndex] = dummy;

            return newOrder;
        }

        public List<List<int>> Sort(List<List<int>> Lista)
        {
            Lista = Lista.Select(x => new
                {
                    InnerList = x,
                    InnerListProperty = GetTotalDistance(x)
                })
                .OrderBy(x => x.InnerListProperty)
                .Select(x => x.InnerList)
                .ToList();

            return Lista;
        }

        public List<List<int>> GenerateRandomPopulation(List<List<int>> Lista,int Size, int NumberOfCities)
        {
            Random random = new Random();
            distances = new double[NumberOfCities, NumberOfCities];
            for (int i = 0; i < NumberOfCities; i++)
            {
                for (int j = 0; j < NumberOfCities; j++)
                {
                    if (i == j)
                    {
                        distances[i, j] = int.MaxValue;
                    }
                    else
                    {
                        distances[i, j] = random.Next(1, 9);
                    }
                    
                }
            }
            int k = 0;
            for (int i = 0; i < Size; i++)
            {
                List<int> currentOrder = new List<int>();
                int[] prohibitionTab = new int[NumberOfCities];
                int index = 0;
                int temp = 0;
                currentOrder.Add(0);
                for (int j = 0; j < NumberOfCities; j++)
                {
                    wroc:
                    k = random.Next(0, NumberOfCities);
                    if (index < NumberOfCities - 1)
                    {
                        if (Included(prohibitionTab, k))
                        {
                            prohibitionTab[index] = k;
                            currentOrder.Add(k);
                            
                            index++;
                            temp = k;
                        }
                        else
                        {
                            goto wroc;
                        }
                    }
                }
                Lista.Add(currentOrder);
            }
            return Lista;
        }

        public double Run(int NumberOfGenerations, int k, int propability, int mutacje, int turniej)
        {
            Pw = LoadCities(NumberOfGenerations);
            int iterator = 0;
            Pw = Sort(Pw);
            while (iterator < k)
            {
                int ind = 0;     
                while (ind < NumberOfGenerations)
                {
                    // wybranie 2 osobnikow
                    
                    List<List<int>> Ptemp = new List<List<int>>();
                    List<int> Ptemp2= new List<int>();
                    List<int> Ptemp3 = new List<int>();
                    // losujemy pare rozwiazan
                    for (int i = 0; i < turniej; i++)
                    {
                        int nmb = random.Next(0, NumberOfGenerations);
                        Ptemp.Add(Pw[nmb]);
                    }
                    Ptemp = Sort(Ptemp); // i bierzemy lepsze z nich
                    Ptemp2 = Ptemp[0];
                    Ptemp.Clear();
                    for (int i = 0; i < 1; i++)
                    {
                        int nmb = random.Next(0, NumberOfGenerations);
                        Ptemp.Add(Pw[nmb]);
                    }
                    Ptemp = Sort(Ptemp);
                    Ptemp3 = Ptemp[0];
                    Ptemp.Clear();



                    // krzyzowanie
                    List<int> Ptemp4 = Crossing(Ptemp2, Ptemp3);
                    List<int> Ptemp5 = Crossing(Ptemp3, Ptemp2);
                    List<int> Ptemp6 = Mutate(Ptemp4,mutacje,propability);
                    List<int> Ptemp7 = Mutate(Ptemp5, mutacje, propability);

                    Pn.Add(Ptemp4);
                    Pn.Add(Ptemp5);
                    
                    ind += 2;
                }
                Pn = Sort(Pn);
                Pw = Sort(Pw);
                foreach (var pop in Pn)
                {
                    if (CompareLists(P, pop) == false)
                    {
                        P.Add(pop);
                    }
                }
                foreach (var pop in Pw)
                {
                    if (CompareLists(P, pop) == false)
                    {
                        P.Add(pop);
                    }
                }
                P = Sort(P);

                Pw.Clear();
                Pn.Clear();
                for (int z = 0; z < NumberOfGenerations; z++)
                {
                    Pw.Add(P[z]);
                }
                P.Clear();
                iterator++;
                
                //Console.WriteLine(iterator + " : " + GetTotalDistance(Pw[0]));
            }
            return GetTotalDistance(Pw[0]);
        }
    }

}