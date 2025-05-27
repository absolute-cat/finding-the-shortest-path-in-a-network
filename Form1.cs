using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace Cource1
{
    // Основна форма програми
    public partial class Course : Form
    {
        private Graph graph;
        private Panel graphPanel;
        private Panel controlPanel;
        private ComboBox algorithmComboBox;
        private ComboBox fromVertexComboBox;
        private ComboBox toVertexComboBox;
        private Button addVertexButton;
        private Button addEdgeButton;
        private Button findPathButton;
        private Button clearButton;
        private Button saveButton;
        private Button loadButton;
        private TextBox edgeWeightTextBox;
        private Label statusLabel;
        private RichTextBox resultsTextBox;
        private List<Point> vertexPositions;
        private List<string> vertexNames;
        private bool isAddingEdge = false;
        private int selectedVertex1 = -1;
        private int selectedVertex2 = -1;

        public Course()
        {
            InitializeComponent();
            InitializeCustomComponents();
            graph = new Graph();
            vertexPositions = new List<Point>();
            vertexNames = new List<string>();
        }

        private void Course_Load(object sender, EventArgs e)
        {
            // Ініціалізація при завантаженні форми
            statusLabel.Text = "Готово до роботи";
            statusLabel.ForeColor = Color.Blue;
        }

        private void InitializeCustomComponents()
        {
            // Панель управління
            controlPanel = new Panel();
            controlPanel.Dock = DockStyle.Left;
            controlPanel.Width = 250;
            controlPanel.BackColor = Color.LightGray;
            controlPanel.Padding = new Padding(10);

            // Комбобокс для вибору алгоритму
            Label algorithmLabel = new Label();
            algorithmLabel.Text = "Алгоритм:";
            algorithmLabel.Location = new Point(10, 10);
            algorithmLabel.Size = new Size(100, 20);
            controlPanel.Controls.Add(algorithmLabel);

            algorithmComboBox = new ComboBox();
            algorithmComboBox.Location = new Point(10, 35);
            algorithmComboBox.Size = new Size(200, 25);
            algorithmComboBox.Items.AddRange(new[] { "Флойд-Воршелл", "Данціг" });
            algorithmComboBox.SelectedIndex = 0;
            controlPanel.Controls.Add(algorithmComboBox);

            // Кнопки для роботи з вершинами та ребрами
            addVertexButton = new Button();
            addVertexButton.Text = "Додати вершину";
            addVertexButton.Location = new Point(10, 70);
            addVertexButton.Size = new Size(200, 30);
            addVertexButton.Click += AddVertexButton_Click;
            controlPanel.Controls.Add(addVertexButton);

            addEdgeButton = new Button();
            addEdgeButton.Text = "Додати ребро";
            addEdgeButton.Location = new Point(10, 110);
            addEdgeButton.Size = new Size(200, 30);
            addEdgeButton.Click += AddEdgeButton_Click;
            controlPanel.Controls.Add(addEdgeButton);

            // Поле для ваги ребра
            Label weightLabel = new Label();
            weightLabel.Text = "Вага ребра:";
            weightLabel.Location = new Point(10, 150);
            weightLabel.Size = new Size(100, 20);
            controlPanel.Controls.Add(weightLabel);

            edgeWeightTextBox = new TextBox();
            edgeWeightTextBox.Location = new Point(10, 175);
            edgeWeightTextBox.Size = new Size(200, 25);
            edgeWeightTextBox.Text = "1";
            controlPanel.Controls.Add(edgeWeightTextBox);

            // Комбобокси для вибору вершин шляху
            Label fromLabel = new Label();
            fromLabel.Text = "Від вершини:";
            fromLabel.Location = new Point(10, 210);
            fromLabel.Size = new Size(100, 20);
            controlPanel.Controls.Add(fromLabel);

            fromVertexComboBox = new ComboBox();
            fromVertexComboBox.Location = new Point(10, 235);
            fromVertexComboBox.Size = new Size(200, 25);
            controlPanel.Controls.Add(fromVertexComboBox);

            Label toLabel = new Label();
            toLabel.Text = "До вершини:";
            toLabel.Location = new Point(10, 270);
            toLabel.Size = new Size(100, 20);
            controlPanel.Controls.Add(toLabel);

            toVertexComboBox = new ComboBox();
            toVertexComboBox.Location = new Point(10, 295);
            toVertexComboBox.Size = new Size(200, 25);
            controlPanel.Controls.Add(toVertexComboBox);

            // Кнопка пошуку шляху
            findPathButton = new Button();
            findPathButton.Text = "Знайти шлях";
            findPathButton.Location = new Point(10, 330);
            findPathButton.Size = new Size(200, 30);
            findPathButton.BackColor = Color.LightGreen;
            findPathButton.Click += FindPathButton_Click;
            controlPanel.Controls.Add(findPathButton);

            // Кнопки для очищення та збереження
            clearButton = new Button();
            clearButton.Text = "Очистити";
            clearButton.Location = new Point(10, 370);
            clearButton.Size = new Size(95, 30);
            clearButton.Click += ClearButton_Click;
            controlPanel.Controls.Add(clearButton);

            saveButton = new Button();
            saveButton.Text = "Зберегти";
            saveButton.Location = new Point(115, 370);
            saveButton.Size = new Size(95, 30);
            saveButton.Click += SaveButton_Click;
            controlPanel.Controls.Add(saveButton);

            loadButton = new Button();
            loadButton.Text = "Завантажити";
            loadButton.Location = new Point(10, 410);
            loadButton.Size = new Size(200, 30);
            loadButton.Click += LoadButton_Click;
            controlPanel.Controls.Add(loadButton);

            // Статус
            statusLabel = new Label();
            statusLabel.Text = "Готово до роботи";
            statusLabel.Location = new Point(10, 450);
            statusLabel.Size = new Size(200, 40);
            statusLabel.ForeColor = Color.Blue;
            controlPanel.Controls.Add(statusLabel);

            // Результати
            Label resultsLabel = new Label();
            resultsLabel.Text = "Результати:";
            resultsLabel.Location = new Point(10, 500);
            resultsLabel.Size = new Size(100, 20);
            controlPanel.Controls.Add(resultsLabel);

            resultsTextBox = new RichTextBox();
            resultsTextBox.Location = new Point(10, 525);
            resultsTextBox.Size = new Size(200, 200);
            resultsTextBox.ReadOnly = true;
            controlPanel.Controls.Add(resultsTextBox);

            // Панель для графіка
            graphPanel = new Panel();
            graphPanel.Dock = DockStyle.Fill;
            graphPanel.BackColor = Color.White;
            graphPanel.Paint += GraphPanel_Paint;
            graphPanel.MouseClick += GraphPanel_MouseClick;

            this.Controls.Add(graphPanel);
            this.Controls.Add(controlPanel);
        }

        private void AddVertexButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Клікніть на панелі для додавання вершини";
            statusLabel.ForeColor = Color.Orange;
        }

        private void AddEdgeButton_Click(object sender, EventArgs e)
        {
            if (graph.VertexCount < 2)
            {
                MessageBox.Show("Потрібно мінімум 2 вершини для створення ребра!");
                return;
            }
            isAddingEdge = true;
            selectedVertex1 = -1;
            selectedVertex2 = -1;
            statusLabel.Text = "Виберіть дві вершини для з'єднання";
            statusLabel.ForeColor = Color.Orange;
        }

        private void GraphPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (statusLabel.Text.Contains("додавання вершини"))
            {
                AddVertexAtPosition(e.Location);
            }
            else if (isAddingEdge)
            {
                SelectVertexForEdge(e.Location);
            }
        }

        private void AddVertexAtPosition(Point position)
        {
            try
            {
                string vertexName = $"V{graph.VertexCount}";
                graph.AddVertex();
                vertexPositions.Add(position);
                vertexNames.Add(vertexName);

                UpdateVertexComboBoxes();
                graphPanel.Invalidate();
                statusLabel.Text = $"Додано вершину {vertexName}";
                statusLabel.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка додавання вершини: {ex.Message}");
            }
        }

        private void SelectVertexForEdge(Point clickPosition)
        {
            int vertexIndex = FindVertexAtPosition(clickPosition);
            if (vertexIndex == -1) return;

            if (selectedVertex1 == -1)
            {
                selectedVertex1 = vertexIndex;
                statusLabel.Text = $"Вибрано вершину {vertexNames[vertexIndex]}. Виберіть другу вершину.";
            }
            else if (selectedVertex2 == -1 && vertexIndex != selectedVertex1)
            {
                selectedVertex2 = vertexIndex;
                AddEdgeBetweenVertices();
            }
        }

        private int FindVertexAtPosition(Point position)
        {
            for (int i = 0; i < vertexPositions.Count; i++)
            {
                Point vertexPos = vertexPositions[i];
                if (Math.Abs(position.X - vertexPos.X) <= 20 && Math.Abs(position.Y - vertexPos.Y) <= 20)
                {
                    return i;
                }
            }
            return -1;
        }

        private void AddEdgeBetweenVertices()
        {
            try
            {
                if (!double.TryParse(edgeWeightTextBox.Text, out double weight))
                {
                    MessageBox.Show("Невірний формат ваги ребра!");
                    return;
                }

                graph.AddEdge(selectedVertex1, selectedVertex2, weight);
                graphPanel.Invalidate();

                statusLabel.Text = $"Додано ребро між {vertexNames[selectedVertex1]} та {vertexNames[selectedVertex2]}";
                statusLabel.ForeColor = Color.Green;

                isAddingEdge = false;
                selectedVertex1 = -1;
                selectedVertex2 = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка додавання ребра: {ex.Message}");
            }
        }

        private void UpdateVertexComboBoxes()
        {
            fromVertexComboBox.Items.Clear();
            toVertexComboBox.Items.Clear();

            for (int i = 0; i < vertexNames.Count; i++)
            {
                fromVertexComboBox.Items.Add(vertexNames[i]);
                toVertexComboBox.Items.Add(vertexNames[i]);
            }
        }

        private void FindPathButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (fromVertexComboBox.SelectedIndex == -1 || toVertexComboBox.SelectedIndex == -1)
                {
                    MessageBox.Show("Виберіть початкову та кінцеву вершини!");
                    return;
                }

                int fromVertex = fromVertexComboBox.SelectedIndex;
                int toVertex = toVertexComboBox.SelectedIndex;
                string algorithm = algorithmComboBox.SelectedItem.ToString();

                Stopwatch stopwatch = Stopwatch.StartNew();
                PathResult result;

                if (algorithm == "Флойд-Воршелл")
                {
                    // Викликаємо з параметром printMatrix = true для виводу матриці
                    result = graph.FloydWarshall(fromVertex, toVertex, true);
                }
                else
                {
                    result = graph.Danzig(fromVertex, toVertex);
                }

                stopwatch.Stop();

                DisplayResults(result, algorithm, stopwatch.ElapsedMilliseconds, algorithm == "Флойд-Воршелл");
                graphPanel.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка обчислення: {ex.Message}");
            }
        }

        private void DisplayResults(PathResult result, string algorithm, long executionTime, bool showMatrix = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Алгоритм: {algorithm}");
            sb.AppendLine($"Час виконання: {executionTime} мс");
            sb.AppendLine($"Операцій: {result.Operations}");
            sb.AppendLine();

            // Додаємо матриці найкоротших відстаней для алгоритму Флойда
            if (showMatrix && algorithm == "Флойд-Воршелл")
            {
                sb.AppendLine("Матриця найкоротших відстаней:");
                sb.AppendLine(GetShortestDistanceMatrix());
                sb.AppendLine();
            }

            if (result.Distance == double.PositiveInfinity)
            {
                sb.AppendLine("Шлях не знайдено!");
            }
            else
            {
                sb.AppendLine($"Відстань: {result.Distance}");
                sb.AppendLine("Шлях:");
                for (int i = 0; i < result.Path.Count; i++)
                {
                    sb.Append(vertexNames[result.Path[i]]);
                    if (i < result.Path.Count - 1)
                        sb.Append(" → ");
                }
            }

            resultsTextBox.Text = sb.ToString();
        }
        private string GetShortestDistanceMatrix()
        {
            StringBuilder matrixSb = new StringBuilder();

            // Виконуємо алгоритм Флойда ще раз для отримання матриці
            var tempResult = graph.FloydWarshallMatrix();

            // Заголовок з іменами вершин
            matrixSb.Append("     ");
            for (int j = 0; j < graph.VertexCount; j++)
            {
                matrixSb.Append($"{vertexNames[j],-8}");
            }
            matrixSb.AppendLine();

            // Рядки матриці
            for (int i = 0; i < graph.VertexCount; i++)
            {
                matrixSb.Append($"{vertexNames[i],-4} ");
                for (int j = 0; j < graph.VertexCount; j++)
                {
                    if (tempResult[i, j] == double.PositiveInfinity)
                        matrixSb.Append("INF     ");
                    else
                        matrixSb.Append($"{tempResult[i, j],-8:0.##}");
                }
                matrixSb.AppendLine();
            }

            return matrixSb.ToString();
        }

        private void GraphPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Малювання ребер
            Pen edgePen = new Pen(Color.Black, 2);
            Pen pathPen = new Pen(Color.Red, 3);

            for (int i = 0; i < graph.VertexCount; i++)
            {
                for (int j = 0; j < graph.VertexCount; j++)
                {
                    if (graph.AdjacencyMatrix[i, j] != double.PositiveInfinity && i != j)
                    {
                        Point start = vertexPositions[i];
                        Point end = vertexPositions[j];

                        bool isInPath = false; // Тут можна додати логіку для підсвічування шляху

                        g.DrawLine(isInPath ? pathPen : edgePen, start, end);

                        // Малювання ваги ребра
                        Point midPoint = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);
                        g.DrawString(graph.AdjacencyMatrix[i, j].ToString(),
                                   new Font("Arial", 8), Brushes.Blue, midPoint);
                    }
                }
            }

            // Малювання вершин
            Brush vertexBrush = new SolidBrush(Color.LightBlue);
            Brush selectedBrush = new SolidBrush(Color.Yellow);
            Pen vertexPen = new Pen(Color.Black, 2);

            for (int i = 0; i < vertexPositions.Count; i++)
            {
                Point pos = vertexPositions[i];
                bool isSelected = (i == selectedVertex1 || i == selectedVertex2);

                g.FillEllipse(isSelected ? selectedBrush : vertexBrush,
                            pos.X - 20, pos.Y - 20, 40, 40);
                g.DrawEllipse(vertexPen, pos.X - 20, pos.Y - 20, 40, 40);

                g.DrawString(vertexNames[i], new Font("Arial", 10, FontStyle.Bold),
                           Brushes.Black, pos.X - 10, pos.Y - 8);
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            graph = new Graph();
            vertexPositions.Clear();
            vertexNames.Clear();
            UpdateVertexComboBoxes();
            resultsTextBox.Clear();
            graphPanel.Invalidate();
            statusLabel.Text = "Граф очищено";
            statusLabel.ForeColor = Color.Green;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Graph files (*.graph)|*.graph";
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    graph.SaveToFile(saveDialog.FileName, vertexPositions, vertexNames);
                    statusLabel.Text = "Граф збережено";
                    statusLabel.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}");
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Filter = "Graph files (*.graph)|*.graph";
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    var loadedData = Graph.LoadFromFile(openDialog.FileName);
                    graph = loadedData.Item1;
                    vertexPositions = loadedData.Item2;
                    vertexNames = loadedData.Item3;

                    UpdateVertexComboBoxes();
                    graphPanel.Invalidate();
                    statusLabel.Text = "Граф завантажено";
                    statusLabel.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження: {ex.Message}");
            }
        }
    }
}