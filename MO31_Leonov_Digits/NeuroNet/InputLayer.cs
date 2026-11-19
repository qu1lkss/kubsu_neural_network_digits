using System;
using System.IO;

namespace MO31_Leonov_Digits.NeuroNet;

class InputLayer
{
    // Поля
    private double[,] trainset; // 100 изображений в обучающей выборке
    private double[,] testset;  // 10 изображений в тестовой выборке

    // Свойства
    public double[,] Trainset { get => trainset; }
    public double[,] Testset { get => testset; }

    // Конструктор
    public InputLayer(NetworkMode nm)
    {
        string path = AppDomain.CurrentDomain.BaseDirectory; // директорий, где находится exe-шник
        string[] tmpArrStr; // временный массив строк
        string[] tmpStr; // временный (вспомогательный) массив элементов в строке

        switch (nm)
        {
            case NetworkMode.Train:
                tmpArrStr = File.ReadAllLines(Path.Combine(path, "train.txt")); // считывание из файла обучающей выборки
                trainset = new double[tmpArrStr.Length, 16]; // определение массива обучающей выборки

                for (int i = 0; i < tmpArrStr.Length; i++) // цикл перебора строк обучающей выборки
                {
                    tmpStr = tmpArrStr[i].Split(' '); // разбиение i-ой строки на массив отдельных символов

                    for (int j = 0; j < 16; j++) // цикл заполнения i-ой строки обучающей выборки
                    {
                        trainset[i, j] = double.Parse(tmpStr[j]); // строковое значение числа преобразуется
                                                                 // в само число двойной точности
                    }
                }
                break;

            case NetworkMode.Test:
                tmpArrStr = File.ReadAllLines(Path.Combine(path, "test.txt")); // считывание из файла тестовой выборки
                testset = new double[tmpArrStr.Length, 16]; // определение массива тестовой выборки

                for (int i = 0; i < tmpArrStr.Length; i++) // цикл перебора строк тестовой выборки
                {
                    tmpStr = tmpArrStr[i].Split(' '); // разбиение i-ой строки на массив отдельных символов

                    for (int j = 0; j < 16; j++) // цикл заполнения i-ой строки тестовой выборки
                    {
                        testset[i, j] = double.Parse(tmpStr[j]); // строковое значение числа преобразуется
                                                                 // в само число двойной точности
                    }
                }
                break;
        }
    }

    // Перетасовка строк массива методом Фишера-Йетса. Fisher-Yates method.
    public void Shuffling_Array_Rows(double[,] arr)
    {
        int j; // номер случайно выбранной строки

        Random random = new Random(); // генератор псевдослучайных чисел

        int columns = arr.GetLength(1);
        for (int row = arr.GetLength(0) - 1; row > 0; row--)
        {
            j = random.Next(row + 1);
            for (int column = 0; column < columns; column++)
            {
                double temp = arr[row, column];
                arr[row, column] = arr[j, column];
                arr[j, column] = temp;
            }
        }
    }
}
