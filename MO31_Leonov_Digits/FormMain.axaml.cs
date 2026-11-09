using System;
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

    // Конструктор
    public FormMain()
    {
        InitializeComponent();

        inputPixels = new double[15]; // инициализация массива
        network = new Network();
    }

    // Обработчик события клика кнопки "Распознать"
    private async void recognizebutton_Click(object? sender, RoutedEventArgs e)
    {
        button_Recognize.IsEnabled = false;
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
        }
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
