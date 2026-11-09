using System;
using System.IO;
using System.Globalization;
using System.Threading.Tasks;

namespace MO31_Leonov_Digits.NeuroNet;

abstract class Layer
{
    // Поля
    // Модификаторы protected стоят для внутрииерархического использования членов класса
    protected string name_Layer; // наименование слоя, которое используется для
                                 // связи с одноимённым файлом памяти
    string pathDirWeights; // путь к каталогу, где находится файл
                           // синаптических весов для нейросети
    string pathFileWeights; // путь к файлу синаптических весов для нейросети
    protected int numofneurons; // число нейронов текущего слоя
    protected int numofprevneurons; // число нейронов предыдущего слоя
    protected const double learningrate = 0.05d; // скорость обучения
    protected const double momentum = 0.050d; // момент инерции
    protected double[,] lastdeltaweights; // веса предыдущей итерации обучения
    protected Neuron[] neurons; // массив нейронов текущего слоя
    private readonly NeuronType neuronType;

    // Свойства
    public double[] Data // передача входных данных на нейроны слоя и активация нейронов
    {
        set
        {
            for (int i = 0; i < numofneurons; i++)
            {
                neurons[i].Activator(value);
            }
        }
    }

    // Конструктор
    protected Layer(int non, int nopn, NeuronType nt, string nm_Layer)
    {
        if (non <= 0 || nopn <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(non), "Количество нейронов и входов должно быть положительным.");
        }
        neuronType = nt;
        // nm_Layer используется для
        // связи с одноимённым полю слоя файлом памяти
        numofneurons = non; // количество нейронов текущего слоя
        numofprevneurons = nopn; // количество нейронов предыдущего слоя
        neurons = new Neuron[non]; // определение массива нейронов
        name_Layer = nm_Layer; // наименование слоя, которое используется для связи
                              // с одноимённым файлом памяти

        // путь к каталогу, где находятся файлы синаптических весов для нейросети
        pathDirWeights = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "memory");

        // путь к файлу синаптических весов для текущего слоя
        pathFileWeights = Path.Combine(pathDirWeights, name_Layer + "_memory.csv");

        // массив изменений весов: nopn входных весов и один вес смещения для каждого нейрона
        lastdeltaweights = new double[non, nopn + 1];
    }

    // Асинхронное создание слоя позволяет дождаться кнопки ОК без блокировки интерфейса.
    internal async Task InitializeAsync()
    {
        double[,] Weights; // временный массив синаптических весов текущего слоя

        // определяет, существует ли pathFileWeights — файл синаптических весов текущего слоя
        if (File.Exists(pathFileWeights))
        {
            // считываем файл весов в режиме GET
            Weights = await WeightInitialize(MemoryMode.GET, pathFileWeights);
        }
        else
        {
            Directory.CreateDirectory(pathDirWeights); // создание отсутствующего каталога
            // создание и немедленное сохранение начальных весов, если файла весов нет
            Weights = await WeightInitialize(MemoryMode.INIT, pathFileWeights);
        }

        ApplyWeights(Weights);
    }

    private void ApplyWeights(double[,] Weights)
    {
        for (int i = 0; i < numofneurons; i++) // цикл формирования нейронов слоя и заполнения ими массива
        {
            double[] tmp_weights = new double[numofprevneurons + 1]; // веса одного нейрона, включая смещение
            for (int j = 0; j < numofprevneurons + 1; j++)
            {
                tmp_weights[j] = Weights[i, j];
            }
            neurons[i] = new Neuron(tmp_weights, neuronType); // заполнение массива нейронами
        }
    }

    // Метод работы с массивом синаптических весов слоя
    // При сохранении вызываем через await, чтобы дождаться закрытия предупреждения.
    public async Task<double[,]> WeightInitialize(MemoryMode mm, string path)
    {
        char[] delim = new char[] { ';', ' ' }; // Разделитель слов — ; и "пробел"
        string tmpStr; // Временная строка для чтения
        string[] tmpStrWeights; // Временный массив строк
        double[,] weights = new double[numofneurons, numofprevneurons + 1]; // Массив синаптических весов текущего слоя

        switch (mm)
        {
            case MemoryMode.GET:
                tmpStrWeights = File.ReadAllLines(path); // считывание строк текстового массива весов
                string[] memory_elemnt;
                for (int i = 0; i < numofneurons; i++)
                {
                    // разбивает строку на элементы в зависимости от символов в массиве delim
                    memory_elemnt = tmpStrWeights[i].Split(delim);
                    for (int j = 0; j < numofprevneurons + 1; j++)
                    {
                        // преобразование строки в вещественное число и запись в массив синаптических весов
                        // замена десятичной запятой точкой для чтения числа с InvariantCulture
                        weights[i, j] = double.Parse(memory_elemnt[j].Replace(',', '.'),
                            System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                break;
            case MemoryMode.SET:
                tmpStrWeights = new string[numofneurons]; // построчный массив (строкового формата) синаптических весов

                if (!File.Exists(path)) // Если нет файла синаптических весов
                {
                    await WarningDialog.ShowAsync(
                        $"Файл {Path.GetFileName(path)} синаптических весов не найден!\n\n" +
                        "После нажатия кнопки ОК автоматически создастся отсутствующий файл, " +
                        "и все значения весов будут сохранены в нём.");
                }
                for (int i = 0; i < numofneurons; i++)
                {
                    weights[i, 0] = neurons[i].Weights[0];
                    tmpStr = weights[i, 0].ToString("R", CultureInfo.InvariantCulture);
                    for (int j = 1; j < numofprevneurons + 1; j++)
                    {
                        weights[i, j] = neurons[i].Weights[j];
                        tmpStr += delim[0] + weights[i, j].ToString("R", CultureInfo.InvariantCulture);
                    }
                    tmpStrWeights[i] = tmpStr;
                }

                // Создаёт новый файл, записывает в него указанный массив строк и затем закрывает файл.
                // Если целевой файл уже существует, он будет перезаписан.
                WriteWeightsFile(path, tmpStrWeights);
                break;
            case MemoryMode.INIT:
                /* В данном программном коде для каждого нейронного слоя имеется свой отдельный файл,
                 * в котором хранятся значения синаптических весов нейронов этого слоя.
                 * Файл для хранения весов является текстовым файлом с расширением .csv,
                 * т. е. может просматриваться как любым текстовым редактором, так и в Excel.
                 * Каждое значение веса в этом файле отделяется от соседнего значения знаком ';'.
                 *
                 * Каждая строка значений весов в файле относится к одному нейрону данного слоя,
                 * т. е. количество строк в файле соответствует количеству нейронов в слое.
                 * Каждая строка файла весов начинается со значения смещения («порога») нейрона,
                 * далее следуют значения весов синаптических связей этого нейрона
                 * с каждым по порядку нейроном предыдущего слоя.
                 * В одной строке содержится на одно значение больше, чем нейронов в предыдущем слое:
                 * вес смещения + количество нейронов предыдущего слоя.
                 *
                 * При начальной инициализации автоматически создаётся папка memory,
                 * где хранятся файлы весов всех слоёв нейронной сети.
                 * Эта папка располагается рядом с исполняемым файлом приложения.
                 * Если файла весов нет, программа предупреждает об этом и после нажатия ОК
                 * формирует начальные веса и сразу сохраняет недостающий файл.
                 *
                 * Синаптические веса генерируются из случайных значений равномерного распределения.
                 * NextDouble() даёт число от 0 включительно до 1 исключительно.
                 * Вычитание 0.5 центрирует распределение: его математическое ожидание равно нулю.
                 * Умножение на sqrt(12 / n) задаёт дисперсию 1 / n
                 * и стандартное отклонение 1 / sqrt(n), где n — количество входов нейрона.
                 * В исходном пояснении на фото дисперсия и стандартное отклонение перепутаны.
                 * Такой масштаб помогает контролировать величину взвешенной суммы входов.
                 * Это выбранный здесь вариант инициализации; оптимальный масштаб зависит
                 * от функции активации. У нас скрытые нейроны используют Leaky ReLU.
                 */
                await WarningDialog.ShowAsync(
                    (File.Exists(path)
                        ? $"Повторная инициализация весов слоя {name_Layer}.\nФайл {Path.GetFileName(path)} будет перезаписан.\n\n"
                        : $"Файл {Path.GetFileName(path)} синаптических весов НЕ НАЙДЕН!\n\n") +
                    "После нажатия кнопки ОК автоматически проинициализируются веса " +
                    "и создастся файл с начальными значениями.\n\n" +
                    "Нейросеть вернётся к исходному состоянию «новорождённости», " +
                    "и потребуется обучать её заново.");

                Random random = new Random(); // генератор случайных чисел

                // Инициализация исходных значений синаптических весов нейронного слоя
                double tmpRatio = Math.Sqrt(12d / numofprevneurons); // коэффициент масштаба
                double tmpShift = 0.5d; // смещение распределения к нулевому среднему
                double[] tmpArr = new double[numofprevneurons + 1]; // массив весов одного нейрона
                tmpStrWeights = new string[numofneurons]; // массив строк синаптических весов

                for (int i = 0; i < numofneurons; i++) // цикл перебора нейронов в слое
                {
                    for (int j = 0; j < numofprevneurons + 1; j++)
                    {
                        tmpArr[j] = random.NextDouble(); // случайная инициализация синаптического веса
                        tmpArr[j] = (tmpArr[j] - tmpShift) * tmpRatio; // центрирование и масштабирование
                        weights[i, j] = tmpArr[j];
                    }

                    tmpStr = tmpArr[0].ToString("R", CultureInfo.InvariantCulture);
                    for (int j = 1; j < numofprevneurons + 1; j++)
                    {
                        tmpStr += delim[0] + tmpArr[j].ToString("R", CultureInfo.InvariantCulture);
                    }
                    tmpStrWeights[i] = tmpStr; // одна строка — веса одного нейрона
                }

                WriteWeightsFile(path, tmpStrWeights); // немедленное сохранение начальных весов
                ApplyWeights(weights); // передача тех же весов нейронам текущего слоя
                Array.Clear(lastdeltaweights); // сброс изменений весов предыдущего обучения
                break;
            default:
                break;
        }
        return weights; // возвращаем массив весов текущего слоя
    }

    private static void WriteWeightsFile(string path, string[] lines)
    {
        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllLines(path, lines);
    }

    abstract public void Recognize(Network net, Layer nextLayer);//для прямых проходов
    abstract public double[] BackwardPass(double[] stuff);//и обратных
}
