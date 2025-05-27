using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cource1
{
    public class Graph
    {
        private double[,] adjacencyMatrix;
        private int vertexCount;

        public double[,] AdjacencyMatrix => adjacencyMatrix;
        public int VertexCount => vertexCount;

        public Graph()
        {
            vertexCount = 0;
            adjacencyMatrix = new double[0, 0];
        }

        public void AddVertex()
        {
            vertexCount++;
            double[,] newMatrix = new double[vertexCount, vertexCount];

            // Копіювання старої матриці
            for (int i = 0; i < vertexCount - 1; i++)
            {
                for (int j = 0; j < vertexCount - 1; j++)
                {
                    newMatrix[i, j] = adjacencyMatrix[i, j];
                }
            }

            // Ініціалізація нових елементів
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < vertexCount; j++)
                {
                    if (i == vertexCount - 1 || j == vertexCount - 1)
                    {
                        newMatrix[i, j] = (i == j) ? 0 : double.PositiveInfinity;
                    }
                }
            }

            adjacencyMatrix = newMatrix;
        }
        public void AddEdge(int from, int to, double weight)
        {
            if (from < 0 || from >= vertexCount || to < 0 || to >= vertexCount)
                throw new ArgumentException("Невірні індекси вершин");

            if (weight < 0)
                throw new ArgumentException("Вага ребра не може бути від'ємною");

            adjacencyMatrix[from, to] = weight;
            adjacencyMatrix[to, from] = weight; // Для неорієнтованого графу
        }
        public PathResult FloydWarshall(int start, int end, bool printMatrix = false)
        {
            int operations = 0;
            double[,] dist = new double[vertexCount, vertexCount];
            int[,] next = new int[vertexCount, vertexCount];

            // Ініціалізація
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < vertexCount; j++)
                {
                    dist[i, j] = adjacencyMatrix[i, j];
                    if (adjacencyMatrix[i, j] != double.PositiveInfinity && i != j)
                        next[i, j] = j;
                    else
                        next[i, j] = -1;
                    operations++;
                }
            }

            // Основний алгоритм Флойда-Воршелла
            for (int k = 0; k < vertexCount; k++)
            {
                for (int i = 0; i < vertexCount; i++)
                {
                    for (int j = 0; j < vertexCount; j++)
                    {
                        operations++;
                        if (dist[i, k] + dist[k, j] < dist[i, j])
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                            next[i, j] = next[i, k];
                        }
                    }
                }
            }

            // Друк матриці після завершення алгоритму, якщо потрібно
            if (printMatrix)
            {
                Console.WriteLine("Матриця найкоротших відстаней:");
                for (int i = 0; i < vertexCount; i++)
                {
                    for (int j = 0; j < vertexCount; j++)
                    {
                        if (dist[i, j] == double.PositiveInfinity)
                            Console.Write("INF\t");
                        else
                            Console.Write($"{dist[i, j]:0.##}\t");
                    }
                    Console.WriteLine();
                }
            }

            // Відновлення шляху
            List<int> path = new List<int>();
            if (next[start, end] != -1)
            {
                int current = start;
                path.Add(current);
                while (current != end)
                {
                    current = next[current, end];
                    path.Add(current);
                }
            }

            return new PathResult
            {
                Distance = dist[start, end],
                Path = path,
                Operations = operations
            };
        }
        public PathResult Danzig(int start, int end)
        {
            int operations = 0;
            double[,] dist = new double[vertexCount, vertexCount];
            int[,] next = new int[vertexCount, vertexCount];

            // Ініціалізація
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < vertexCount; j++)
                {
                    dist[i, j] = adjacencyMatrix[i, j];
                    if (adjacencyMatrix[i, j] != double.PositiveInfinity && i != j)
                        next[i, j] = j;
                    else
                        next[i, j] = -1;
                    operations++;
                }
            }

            // Алгоритм Данцига (послідовне додавання вершин)
            for (int k = 0; k < vertexCount; k++)
            {
                // Оновлення відстаней через нову вершину k
                for (int i = 0; i <= k; i++)
                {
                    for (int j = 0; j <= k; j++)
                    {
                        operations++;
                        if (dist[i, k] + dist[k, j] < dist[i, j])
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                            next[i, j] = next[i, k];
                        }
                    }
                }

                // Оновлення відстаней від та до нової вершини
                for (int i = 0; i <= k; i++)
                {
                    for (int j = k + 1; j < vertexCount; j++)
                    {
                        operations++;
                        if (dist[i, k] + dist[k, j] < dist[i, j])
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                            next[i, j] = next[i, k];
                        }
                    }
                }

                for (int i = k + 1; i < vertexCount; i++)
                {
                    for (int j = 0; j <= k; j++)
                    {
                        operations++;
                        if (dist[i, k] + dist[k, j] < dist[i, j])
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                            next[i, j] = next[i, k];
                        }
                    }
                }
            }

            // Відновлення шляху
            List<int> path = new List<int>();
            if (next[start, end] != -1)
            {
                int current = start;
                path.Add(current);
                while (current != end)
                {
                    current = next[current, end];
                    path.Add(current);
                }
            }

            return new PathResult
            {
                Distance = dist[start, end],
                Path = path,
                Operations = operations
            };
        }

        public void SaveToFile(string filename, List<Point> positions, List<string> names)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.WriteLine(vertexCount);

                // Збереження позицій та імен вершин
                for (int i = 0; i < vertexCount; i++)
                {
                    writer.WriteLine($"{names[i]},{positions[i].X},{positions[i].Y}");
                }

                // Збереження матриці суміжності
                for (int i = 0; i < vertexCount; i++)
                {
                    for (int j = 0; j < vertexCount; j++)
                    {
                        writer.Write(adjacencyMatrix[i, j]);
                        if (j < vertexCount - 1) writer.Write(",");
                    }
                    writer.WriteLine();
                }
            }
        }
        public double[,] FloydWarshallMatrix()
        {
            double[,] dist = new double[vertexCount, vertexCount];

            // Ініціалізація
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < vertexCount; j++)
                {
                    dist[i, j] = adjacencyMatrix[i, j];
                }
            }

            // Основний алгоритм Флойда-Воршелла
            for (int k = 0; k < vertexCount; k++)
            {
                for (int i = 0; i < vertexCount; i++)
                {
                    for (int j = 0; j < vertexCount; j++)
                    {
                        if (dist[i, k] + dist[k, j] < dist[i, j])
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                        }
                    }
                }
            }

            return dist;
        }
        public static Tuple<Graph, List<Point>, List<string>> LoadFromFile(string filename)
        {
            Graph graph = new Graph();
            List<Point> positions = new List<Point>();
            List<string> names = new List<string>();

            using (StreamReader reader = new StreamReader(filename))
            {
                int vertexCount = int.Parse(reader.ReadLine());

                // Завантаження вершин
                for (int i = 0; i < vertexCount; i++)
                {
                    graph.AddVertex();
                    string[] parts = reader.ReadLine().Split(',');
                    names.Add(parts[0]);
                    positions.Add(new Point(int.Parse(parts[1]), int.Parse(parts[2])));
                }

                // Завантаження матриці суміжності
                for (int i = 0; i < vertexCount; i++)
                {
                    string[] row = reader.ReadLine().Split(',');
                    for (int j = 0; j < vertexCount; j++)
                    {
                        double value = double.Parse(row[j]);
                        graph.adjacencyMatrix[i, j] = value;
                    }
                }
            }

            return new Tuple<Graph, List<Point>, List<string>>(graph, positions, names);
        }
    }

    // Клас для результату пошуку шляху
    public class PathResult
    {
        public double Distance { get; set; }
        public List<int> Path { get; set; }
        public int Operations { get; set; }
    }
}

