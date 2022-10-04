using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public int width, height;
    Grid placementGrid;

    public Dictionary<Vector3Int, StructureModel> temporaryRoadObjects = new Dictionary<Vector3Int, StructureModel>();
    public Dictionary<Vector3Int, StructureModel> structureDictionary = new Dictionary<Vector3Int, StructureModel>();

    private void Start()
    {
        placementGrid = new Grid(width, height);
    }

    internal bool CheckIfGridPositionIsInBounds(Vector3Int position)
    {
        if (position.x >= 0 && position.x < width && position.z >= 0 && position.z < height)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    internal CellType[] GetNeighbourTypesFor(Vector3Int position)
    {
        return placementGrid.GetAllAdjacentCellTypes(position.x, position.z);
    }

    internal void PlaceObjectOnTheMap(Vector3Int position, GameObject structurePrefab, CellType cellType)
    {
        placementGrid[position.x, position.z] = cellType;
        StructureModel structure = CreateANewStructureModel(position, structurePrefab, cellType);
        temporaryRoadObjects.Add(position, structure);

        DestroyNatureAt(position);
    }

    private void DestroyNatureAt(Vector3Int position)
    {
        RaycastHit[] hits = Physics.BoxCastAll(position + new Vector3(0, 0, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), transform.up, Quaternion.identity, 1.0f, LayerMask.NameToLayer("Nature"));

        foreach (var item in hits)
        {
            Destroy(item.collider.gameObject);
        }
    }

    internal bool CheckIfGridPositionIsFree(Vector3Int position)
    {
        return CheckIfPositionIsOfType(position, CellType.Empty);
    }

    private bool CheckIfPositionIsOfType(Vector3Int position, CellType cellType)
    {
        return placementGrid[position.x, position.z] == cellType;
    }

    internal void PlaceTemporaryStructure(Vector3Int position, GameObject structurePrefab, CellType cellType)
    {
        placementGrid[position.x, position.z] = cellType;
        StructureModel structure = CreateANewStructureModel(position, structurePrefab, cellType);
        temporaryRoadObjects.Add(position, structure);
    }

    

    internal List<Vector3Int> GetNeighboursOfTypeFor(Vector3Int position, CellType type)
    {
        List<Point> neighbourPoints = placementGrid.GetAdjacentCellsOfType(position.x, position.z, type);
        List<Vector3Int> neighbourPositions = new List<Vector3Int>();
        foreach (var neighbourPoint in neighbourPoints)
        {
            neighbourPositions.Add(new Vector3Int(neighbourPoint.X, 0, neighbourPoint.Y));
        }

        return neighbourPositions;
    }

    private StructureModel CreateANewStructureModel(Vector3Int position, GameObject structurePrefab, CellType cellType)
    {
        GameObject structure = new GameObject(cellType.ToString());
        structure.transform.SetParent(transform);
        structure.transform.localPosition = position;
        var structureModel = structure.AddComponent<StructureModel>();
        structureModel.CreateModel(structurePrefab);
        return structureModel;
    }

    public void ModifyStructureModel(Vector3Int position, GameObject newModel, Quaternion rotation)
    {
        if (temporaryRoadObjects.ContainsKey(position))
        {
            temporaryRoadObjects[position].SwapModel(newModel, rotation);
        }
        else if (structureDictionary.ContainsKey(position))
        {
            structureDictionary[position].SwapModel(newModel, rotation);
        }
    }

    internal List<Vector3Int> GetPathBetween(Vector3Int startPosition, Vector3Int endPosition)
    {
        var resultPath = GridSearch.AStarSearch(placementGrid, new Point(startPosition.x, startPosition.z), new Point(endPosition.x, endPosition.z));

        List<Vector3Int> path = new List<Vector3Int>();
        foreach (var point in resultPath)
        {
            path.Add(new Vector3Int(point.X, 0, point.Y));
        }

        return path;
    }

    internal void RemoveAllTemporaryStructures()
    {
        foreach (StructureModel structure in temporaryRoadObjects.Values)
        {
            var position = Vector3Int.RoundToInt(structure.transform.position);
            placementGrid[position.x, position.z] = CellType.Empty;
            Destroy(structure.gameObject);
        }
        temporaryRoadObjects.Clear();
    }

    internal void AddTemporaryStructurestoStructureDictionary()
    {
        foreach (var structure in temporaryRoadObjects)
        {
            structureDictionary.Add(structure.Key, structure.Value);
            DestroyNatureAt(structure.Key);
        }

        temporaryRoadObjects.Clear();
    }
}
