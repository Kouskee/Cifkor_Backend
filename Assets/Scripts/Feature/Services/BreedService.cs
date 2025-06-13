using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class BreedService : IBreedService
{
    private const string BASE_URL = "https://dogapi.dog/api/v2";
    private const int MAX_BREEDS_COUNT = 10;

    public async UniTask<List<BreedModel>> GetBreedsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            string url = $"{BASE_URL}/breeds?limit={MAX_BREEDS_COUNT}";

            using (var request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Accept", "application/json");

                await request.SendWebRequest().WithCancellation(cancellationToken);

                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception($"Request failed: {request.error}");
                }

                var responseText = request.downloadHandler.text;
                var response = JsonUtility.FromJson<BreedsListResponse>(responseText);

                return response.data
                    .Take(MAX_BREEDS_COUNT)
                    .Select(breed => new BreedModel(
                        breed.id,
                        breed.attributes.name,
                        breed.attributes.description ?? ""
                    ))
                    .ToList();
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error fetching breeds: {ex.Message}");
            throw;
        }
    }

    public async UniTask<BreedModel> GetBreedDetailsAsync(string breedId, CancellationToken cancellationToken = default)
    {
        try
        {
            string url = $"{BASE_URL}/breeds/{breedId}";

            using (var request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Accept", "application/json");

                await request.SendWebRequest().WithCancellation(cancellationToken);

                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception($"Request failed: {request.error}");
                }

                var responseText = request.downloadHandler.text;
                var response = JsonUtility.FromJson<BreedSingleResponse>(responseText);
                var breedData = response.data;

                return new BreedModel(
                    breedData.id,
                    breedData.attributes.name,
                    breedData.attributes.description ?? "No description available"
                );
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error fetching breed details for {breedId}: {ex.Message}");
            throw;
        }
    }
}