using System;
using System.Collections.Generic;
using System.IO; // пространство имён для работы с файловой системой
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using MO31_Leonov_Digits.NeuroNet;

namespace MO31_Leonov_Digits;

public partial class FormMain : Window
{
    private double[] inputPixels; // массив входных данных
    private Network network; // объявление нейросети
    private bool networkInitialized;
    private readonly List<double> errorHistory = new();

    // Конструктор
    public FormMain()
    {
        InitializeComponent();

        inputPixels = new double[15]; // инициализация массива
        network = new Network();
        DrawErrorChart(Array.Empty<double>());
    }

    // Обработчик события клика кнопки "Распознать"
    private async void recognizebutton_Click(object? sender, RoutedEventArgs e)
    {
        button_Recognize.IsEnabled = false;
        button_Training.IsEnabled = false;
        try
        {
            if (!networkInitialized)
            {
                await network.InitializeAsync();
                networkInitialized = true;
            }

            network.ForwardPass(inputPixels);
            label_Output.Text = network.Fact.ToList().IndexOf(network.Fact.Max()).ToString();
            label_Probability.Text = (100 * network.Fact.Max()).ToString("0.00") + " %";
        }
        catch (OperationCanceledException)
        {
            // Закрытие предупреждения без ОК отменяет распознавание.
        }
        catch (Exception error)
        {
            // Ошибку чтения файла весов показываем в форме.
            label_Output.Text = "Ошибка";
            label_Probability.Text = error.Message;
        }
        finally
        {
            button_Recognize.IsEnabled = true;
            button_Training.IsEnabled = true;
        }
    }

    // Обработчик события клика кнопки "Обучить"
    private async void trainbutton_Click(object? sender, RoutedEventArgs e)
    {
        button_Training.IsEnabled = false;
        button_Recognize.IsEnabled = false;
        try
        {
            if (!networkInitialized)
            {
                await network.InitializeAsync();
                networkInitialized = true;
            }

            await network.Train(network);
            errorHistory.AddRange(network.E_error_avr);
            DrawErrorChart(errorHistory.ToArray());
            await WarningDialog.ShowInformationAsync("Обучение успешно завершено.");
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception error)
        {
            label_Output.Text = "Ошибка";
            label_Probability.Text = error.Message;
        }
        finally
        {
            button_Recognize.IsEnabled = true;
            button_Training.IsEnabled = true;
        }
    }

    private void DrawErrorChart(double[] errors)
    {
        ErrorChart.Children.Clear();
        const double left = 66, top = 12, width = 516, height = 274;
        int epochs = errors.Length > 1 ? errors.Length : 200;
        double maxError = errors.Where(double.IsFinite).DefaultIfEmpty(0).Max();
        const double yStep = 0.01;
        int yTicks = Math.Max(5, (int)Math.Ceiling(maxError / yStep));
        double scale = yTicks * yStep;

        void AddLine(double x1, double y1, double x2, double y2)
        {
            var line = new Border
            {
                Width = Math.Max(1, x2 - x1),
                Height = Math.Max(1, y2 - y1),
                Background = Brushes.Gray
            };
            Canvas.SetLeft(line, x1);
            Canvas.SetTop(line, y1);
            ErrorChart.Children.Add(line);
        }

        void AddLabel(string text, double x, double y)
        {
            var label = new TextBlock { Text = text, FontSize = 12 };
            Canvas.SetLeft(label, x);
            Canvas.SetTop(label, y);
            ErrorChart.Children.Add(label);
        }

        for (int tick = 0; tick <= yTicks; tick++)
        {
            double y = top + (1 - (double)tick / yTicks) * height;
            AddLine(left, y, left + width, y);
            AddLabel((tick * yStep).ToString("0.00"), 4, y - 8);
        }

        for (int tick = 0; tick <= 5; tick++)
        {
            double fraction = tick / 5.0;
            double x = left + fraction * width;
            AddLine(x, top, x, top + height);
            AddLabel((1 + (int)Math.Round(fraction * (epochs - 1))).ToString(), x - 8, top + height + 5);
        }

        AddLabel("Эпоха", left + width / 2 - 16, top + height + 28);
        var points = new Avalonia.Collections.AvaloniaList<Avalonia.Point>();
        for (int index = 0; index < errors.Length; index++)
        {
            if (double.IsFinite(errors[index]))
                points.Add(new Avalonia.Point(left + width * index / (epochs - 1),
                    top + height * (1 - errors[index] / scale)));
        }

        if (points.Count > 1)
            ErrorChart.Children.Add(new Avalonia.Controls.Shapes.Polyline
            {
                Name = "Series_Eavr",
                Width = ErrorChart.Width,
                Height = ErrorChart.Height,
                Stretch = Avalonia.Media.Stretch.None,
                Points = points,
                Stroke = Brushes.DodgerBlue,
                StrokeThickness = 3
            });
    }

    // Сохранить в файл обучающий пример
    private void ButtonSaveTrainSample_Click(object? sender, RoutedEventArgs e)
    {
        if (numericUpDown_NecessaryOutput.Value == null)
        {
            numericUpDown_NecessaryOutput.Focus();
            return;
        }

        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "train.txt");
        string tmpStr = numericUpDown_NecessaryOutput.Value.Value.ToString("0");

        for (int i = 0; i < inputPixels.Length; i++)
        {
            tmpStr += " " + inputPixels[i].ToString();
        }

        tmpStr += Environment.NewLine; // переход на новую строку текста

        File.AppendAllText(path, tmpStr); // добавление текста в файл без удаления предыдущих примеров
    }

    // Сохранить в файл тестовый пример
    private void ButtonSaveTestSample_Click(object? sender, RoutedEventArgs e)
    {
        if (numericUpDown_NecessaryOutput.Value == null)
        {
            numericUpDown_NecessaryOutput.Focus();
            return;
        }

        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.txt");
        string tmpStr = numericUpDown_NecessaryOutput.Value.Value.ToString("0");

        for (int i = 0; i < inputPixels.Length; i++)
        {
            tmpStr += " " + inputPixels[i].ToString();
        }

        tmpStr += Environment.NewLine; // переход на новую строку текста

        File.AppendAllText(path, tmpStr); // добавление текста в файл без удаления предыдущих примеров
    }

    // Обработчик события клика "пиксельной" кнопки
    private void Pixel_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            // если изначально кнопка белая
            if (button.Background is ISolidColorBrush brush && brush.Color == Colors.White)
            {
                button.Background = Brushes.DodgerBlue; // меняем цвет кнопки
                inputPixels[button.TabIndex] = 1d; // меняем значение входного пикселя
            }
            else // если изначально кнопка синяя
            {
                button.Background = Brushes.White; // меняем цвет кнопки
                inputPixels[button.TabIndex] = 0d; // меняем значение входного пикселя
            }
        }
    }
}
