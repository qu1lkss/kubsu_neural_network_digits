using System.Threading.Tasks;

namespace MO31_Leonov_Digits.NeuroNet;

class InputLayer : Layer
{
    private InputLayer(int non, int nopn, NeuronType nt, string nm_Layer)
        : base(non, nopn, nt, nm_Layer)
    {
    }

    // Создаёт полностью готовый слой после загрузки или инициализации его весов.
    public static async Task<InputLayer> CreateAsync(int non, int nopn, NeuronType nt, string nm_Layer)
    {
        var layer = new InputLayer(non, nopn, nt, nm_Layer);
        await layer.InitializeAsync();
        return layer;
    }

    // Реализацию прямого прохода добавим отдельно для входного слоя.
    public override void Recognize(Network net, Layer nextLayer)
    {
        throw new System.NotImplementedException();
    }

    // Реализацию обратного прохода добавим отдельно для входного слоя.
    public override double[] BackwardPass(double[] stuff)
    {
        throw new System.NotImplementedException();
    }
}
