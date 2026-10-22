using System.Threading.Tasks;

namespace MO31_Leonov_Digits.NeuroNet;

class OutputLayer : Layer
{
    private OutputLayer(int non, int nopn, NeuronType nt, string nm_Layer)
        : base(non, nopn, nt, nm_Layer)
    {
    }

    // Создаёт полностью готовый слой после загрузки или инициализации его весов.
    public static async Task<OutputLayer> CreateAsync(int non, int nopn, NeuronType nt, string nm_Layer)
    {
        var layer = new OutputLayer(non, nopn, nt, nm_Layer);
        await layer.InitializeAsync();
        return layer;
    }
}
