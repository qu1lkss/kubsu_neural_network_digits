using System;
using System.IO; // пространство имён для работы с файловой системой
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace MO31_Leonov_Digits;

public partial class FormMain : Window
{
    private double[] inputPixels; // массив входных данных

    // Конструктор
    public FormMain()
    {
        InitializeComponent();

        inputPixels = new double[15];
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
                button.Background = Brushes.Black; // меняем цвет кнопки
                inputPixels[button.TabIndex] = 1d; // меняем значение входного пикселя
            }
            else // если изначально кнопка чёрная
            {
                button.Background = Brushes.White; // меняем цвет кнопки
                inputPixels[button.TabIndex] = 0d; // меняем значение входного пикселя
            }
        }
    }
}
