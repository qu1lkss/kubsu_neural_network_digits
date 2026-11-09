using System.Threading.Tasks;

namespace MO31_Leonov_Digits.NeuroNet;

class Network
{
    //все слои сети
    private InputLayer input_layer = null;
    private HiddenLayer hidden_layer1 = new HiddenLayer(70, 15, NeuronType.Hidden, "HiddenLayer1");
    private HiddenLayer hidden_layer2 = new HiddenLayer(30, 70, NeuronType.Hidden, "HiddenLayer2");
    private OutputLayer output_layer = new OutputLayer(10, 30, NeuronType.Output, "OutputLayer");

    private double[] fact = new double[10];//массив фактического выхода сети
    private double[] e_error_avr;// среднее значение энергии ошибки эпохи обучения

    // свойства
    public double[] Fact { get => fact; } //массив фактического выхода сети

    // среднее значение энергии ошибки эпохи обучения
    public double[] E_error_avr { get => e_error_avr; set => e_error_avr = value; }

    //конструктор
    public Network() { }

    // Загрузка или создание весов всех слоёв перед первым распознаванием.
    public async Task InitializeAsync()
    {
        await hidden_layer1.InitializeAsync();
        await hidden_layer2.InitializeAsync();
        await output_layer.InitializeAsync();
    }

    //прямой проход сети
    public void ForwardPass(double[] netInput)
    {
        hidden_layer1.Data = netInput;
        hidden_layer1.Recognize(null, hidden_layer2);
        hidden_layer2.Recognize(null, output_layer);
        output_layer.Recognize(this, null);
    }
}
