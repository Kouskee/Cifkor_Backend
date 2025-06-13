using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

public interface IBreedService
{
    UniTask<List<BreedModel>> GetBreedsAsync(CancellationToken cancellationToken = default);
    UniTask<BreedModel> GetBreedDetailsAsync(string breedId, CancellationToken cancellationToken = default);
}