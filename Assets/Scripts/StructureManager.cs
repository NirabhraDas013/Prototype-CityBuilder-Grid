using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public PlacementManager placementManager;

    public WeightedStructurePrefab[] housePrefabs;
    public WeightedStructurePrefab[] specialPrefabs;

    private float[] houseWeights;
    private float[] specialWeights;

    private void Start()
    {
        houseWeights = housePrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        specialWeights = specialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
    }

    public void PlaceHouse(Vector3Int position)
    {
        if (CheckPositionBeforePlacement(position))
        {
            int randomIndex = GetRandomWeightedIndex(houseWeights);
            placementManager.PlaceObjectOnTheMap(position, housePrefabs[randomIndex].prefab, CellType.House);
            AudioPlayer.instance.PlayPlacementSound();
        }
    }

    public void PlaceSpecial(Vector3Int position)
    {
        if (CheckPositionBeforePlacement(position))
        {
            int randomIndex = GetRandomWeightedIndex(specialWeights);
            placementManager.PlaceObjectOnTheMap(position, specialPrefabs[randomIndex].prefab, CellType.House);
            AudioPlayer.instance.PlayPlacementSound();
        }
    }

    private int GetRandomWeightedIndex(float[] weights)
    {
        float weightSum = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            weightSum += weights[i];
        }

        float randomValue = UnityEngine.Random.Range(0, weightSum);
        float tempSum = 0.0f;

        for (int i = 0; i < weights.Length; i++)
        {
            if (randomValue >= tempSum && randomValue < tempSum + weights[i])
            {
                return i;
            }
            tempSum += weights[i];
        }
        return 0;
    }

    private bool CheckPositionBeforePlacement(Vector3Int position)
    {
        if (!placementManager.CheckIfGridPositionIsInBounds(position))
        {
            Debug.Log("Position Is OUT OF BOUNDS");
            return false;
        }

        if (!placementManager.CheckIfGridPositionIsFree(position))
        {
            Debug.Log("Position Is OCCUPIED");
            return false;
        }

        if (placementManager.GetNeighboursOfTypeFor(position, CellType.Road).Count <= 0)
        {
            Debug.Log("Structure MUST BE PLACED BESIDE A ROAD");
            return false;
        }

        return true;
    }
}

[Serializable]
public struct WeightedStructurePrefab
{
    public GameObject prefab;
    [Range(0, 1)]
    public float weight;
}
