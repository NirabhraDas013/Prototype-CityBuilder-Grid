using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public PlacementManager placementManager;
    public RoadFixer roadFixer;

    public List<Vector3Int> temporaryPlacementPositions = new List<Vector3Int>();
    public List<Vector3Int> roadPositionsToRecheck = new List<Vector3Int>();

    private Vector3Int startPosition;
    private bool placementMode = false;

    private void Start()
    {
        if (roadFixer == null)
        {
            roadFixer = GetComponent<RoadFixer>();
        }
    }

    public void PlaceRoad(Vector3Int position)
    {
        if (placementManager.CheckIfGridPositionIsInBounds(position) == false)
        {
            Debug.Log("Position Is Out Of Bounds");
            return;
        }

        if (placementManager.CheckIfGridPositionIsFree(position) == false)
        {
            Debug.Log("Clicked Position Already Has A Structure");
            return;
        }

        if (!placementMode)
        {
            temporaryPlacementPositions.Clear();
            roadPositionsToRecheck.Clear();

            placementMode = true;
            startPosition = position;

            temporaryPlacementPositions.Add(position);
            placementManager.PlaceTemporaryStructure(position, roadFixer.deadEndPrefab, CellType.Road);
        }
        else
        {
            placementManager.RemoveAllTemporaryStructures();
            temporaryPlacementPositions.Clear();

            foreach (var positionToFix in roadPositionsToRecheck)
            {
                roadFixer.FixRoadAtPosition(placementManager, positionToFix);
            }
            roadPositionsToRecheck.Clear();

            temporaryPlacementPositions = placementManager.GetPathBetween(startPosition, position);

            foreach (var temporaryPosition in temporaryPlacementPositions)
            {
                if (placementManager.CheckIfGridPositionIsFree(temporaryPosition) == false)
                {
                    Debug.Log("Clicked Position Already Has A Structure");
                    continue;
                }
                placementManager.PlaceTemporaryStructure(temporaryPosition, roadFixer.deadEndPrefab, CellType.Road);
            }
        }

        FixRoadPrefabs();
    }

    private void FixRoadPrefabs()
    {
        foreach (var tempRoadPosition in temporaryPlacementPositions)
        {
            roadFixer.FixRoadAtPosition(placementManager, tempRoadPosition);

            List<Vector3Int> neighbourPositions = placementManager.GetNeighboursOfTypeFor(tempRoadPosition, CellType.Road);

            foreach (var neighbourPosition in neighbourPositions)
            {
                if (!roadPositionsToRecheck.Contains(neighbourPosition))
                {
                    roadPositionsToRecheck.Add(neighbourPosition);
                }
            }
        }

        foreach (var positionToFix in roadPositionsToRecheck)
        {
            roadFixer.FixRoadAtPosition(placementManager, positionToFix);
        }
    }

    public void FinishPlacingRoad()
    {
        placementMode = false;
        placementManager.AddTemporaryStructurestoStructureDictionary();
        if (temporaryPlacementPositions.Count > 0)
        {
            AudioPlayer.instance.PlayPlacementSound();
        }
        temporaryPlacementPositions.Clear();
        startPosition = Vector3Int.zero;
    }
}
