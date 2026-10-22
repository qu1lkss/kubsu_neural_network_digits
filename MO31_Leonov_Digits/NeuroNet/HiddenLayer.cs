using System.Threading.Tasks;

namespace MO31_Leonov_Digits.NeuroNet;

class HiddenLayer : Layer
{
    private HiddenLayer(int non, int nopn, NeuronType nt, string nm_Layer)
        : base(non, nopn, nt, nm_Layer)
    {
    }

    // Создаёт полностью готовый слой после загрузки или инициализации его весов.
    public static async Task<HiddenLayer> CreateAsync(int non, int nopn, NeuronType nt, string nm_Layer)
    {
        var layer = new HiddenLayer(non, nopn, nt, nm_Layer);
        await layer.InitializeAsync();
        return layer;
    }
}
