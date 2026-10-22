using static System.Math;

namespace MO31_Leonov_Digits.NeuroNet;

class Neuron
{
    // поля
    private NeuronType type; // тип нейрона
    private double[] weights; // его веса
    private double[] inputs; // его входы
    private double output; // его выход
    private double derivative; // производная

    // константы для функции активации
    private double a = 0.01d;

    // свойства
    public double[] Weights { get => weights; set => weights = value; }
    public double[] Inputs { get => inputs; set => inputs = value; }
    public double Output { get => output; }
    public double Derivative { get => derivative; }

    // конструктор
    public Neuron(double[] memoryWeights, NeuronType typeNeuron)
    {
        type = typeNeuron;
        weights = memoryWeights;
    }

    // вычисление выходного сигнала нейрона
    public void Activator(double[] i)
    {
        inputs = i; // передача вектора входного сигнала в массив входных данных нейрона

        double sum = weights[0]; // смещение нейрона
        for (int j = 0; j < inputs.Length; j++) // вычисление взвешенной суммы входов
        {
            sum += inputs[j] * weights[j + 1];
        }

        switch (type)
        {
            case NeuronType.Hidden: // для нейронов скрытого слоя
                output = LeakyReLU(sum);
                derivative = LeakyReLU_Derivator(sum);
                break;

            case NeuronType.Output: // для нейронов выходного слоя
                output = Exp(sum);
                break;
        }
    }

    // функция активации нейрона Leaky ReLU
    private double LeakyReLU(double sum)
    {
        return sum >= 0 ? sum : a * sum;
    }

    // производная Leaky ReLU; в точке 0 выбираем значение 1
    private double LeakyReLU_Derivator(double sum)
    {
        return sum >= 0 ? 1d : a;
    }
}
