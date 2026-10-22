namespace MO31_Leonov_Digits.NeuroNet
{
    enum MemoryMode // режимы работы памяти
    {
        GET, // считывание памяти
        SET, // сохранение памяти
        INIT // инициализация памяти
    }

    enum NeuronType // тип нейрона
    {
        Hidden, // скрытый
        Output // выходной
    }

    enum NetworkMode // режимы работы сети
    {
        Train, // обучение
        Test, // проверка
        Demo // распознавание
    }
}
