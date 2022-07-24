using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadFixer : MonoBehaviour
{
    public GameObject deadEndPrefab;
    public GameObject roadStraightPrefab;
    public GameObject cornerPrefab;
    public GameObject threewayJunctionPrefab;
    public GameObject fourwayJunctionPrefab;

    public void FixRoadAtPosition(PlacementManager placementManager, Vector3Int temporaryPosition)
    {
        //This will return an array of CellTypes in the specific order of 
        //[Left, Top, Right, Down]
        CellType[] result = placementManager.GetNeighbourTypesFor(temporaryPosition);

        int roadCount = 0;
        roadCount = result.Where(x => x == CellType.Road).Count();

        if(roadCount == 0 || roadCount == 1)
        {
            CreateDeadEnd(placementManager, result, temporaryPosition);
        }
        else if (roadCount == 2)
        {
            if (CreateStraightRoad(placementManager, result, temporaryPosition))
            {
                return;
            }
            else
            {
                CreateCorner(placementManager, result, temporaryPosition);
            }
        }
        else if (roadCount == 3)
        {
            CreateThreeWayJunction(placementManager, result, temporaryPosition);
        }
        else
        {
            CreateFourWayJunction(placementManager, result, temporaryPosition);
        }
    }

    private void CreateDeadEnd(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        Quaternion replacementRoadRotation;

        if (result[1] == CellType.Road)
        {
            replacementRoadRotation = Quaternion.Euler(0, 270, 0);
        }
        else if (result[0] == CellType.Road)
        {
            replacementRoadRotation = Quaternion.Euler(0, 180, 0);
        }
        else if (result[3] == CellType.Road)
        {
            replacementRoadRotation = Quaternion.Euler(0, 90, 0);
        }
        else if(result[2] == CellType.Road)
        {
            replacementRoadRotation = Quaternion.identity;
        }
        else
        {
            return;
        }

        placementManager.ModifyStructureModel(temporaryPosition, deadEndPrefab, replacementRoadRotation);
    }

    private bool CreateStraightRoad(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        Quaternion replacementRoadRotation;

        if (result[0] == CellType.Road && result[2] == CellType.Road)
        {
            replacementRoadRotation = Quaternion.identity;
        }
        else if (result[1] == CellType.Road && result[3] == CellType.Road)
        {
            replacementRoadRotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            return false;
        }

        placementManager.ModifyStructureModel(temporaryPosition, roadStraightPrefab, replacementRoadRotation);
        return true;
    }

    private void CreateCorner(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        Quaternion replacementRoadRotation;

        if (result[0] == CellType.Road && result[3] == CellType.Road)
        {
            replacementRoadRotation = Quaternion.Euler(0, 270, 0);
        }
        else if (result[3] == CellType.Road && result[2] == CellType.Road)
        {
            replacementRoadRotation = Quaternion.Euler(0, 180, 0);
        }
        else if (result[2] == CellType.Road && result[1] == CellType.Road)
        {
            replacementRoadRotation = Quaternion.Euler(0, 90, 0);
        }
        else //if (result[1] == CellType.Road && result[0] == CellType.Road) //Keep the comment for completion's sake
        {
            replacementRoadRotation = Quaternion.identity;
        }

        placementManager.ModifyStructureModel(temporaryPosition, cornerPrefab, replacementRoadRotation);
    }

    //[Left, Top, Right, Down] -- The result array
    private void CreateThreeWayJunction(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        Quaternion replacementRoadRotation;

        if (result[3] != CellType.Road)
        {
            replacementRoadRotation = Quaternion.Euler(0, 270, 0);
        }
        else if (result[2] != CellType.Road)
        {
            replacementRoadRotation = Quaternion.Euler(0, 180, 0);
        }
        else if (result[1] != CellType.Road)
        {
            replacementRoadRotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            replacementRoadRotation = Quaternion.identity;
        }

        placementManager.ModifyStructureModel(temporaryPosition, threewayJunctionPrefab, replacementRoadRotation);
    }

    private void CreateFourWayJunction(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        placementManager.ModifyStructureModel(temporaryPosition, fourwayJunctionPrefab, Quaternion.identity);
    }
}
