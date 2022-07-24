using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureModel : MonoBehaviour
{
    float yHeight = 0.0f;

    public void CreateModel(GameObject modelToBeCreated)
    {
        var instantiatedModel = Instantiate(modelToBeCreated, transform);
        yHeight = instantiatedModel.transform.position.y;
    }

    public void SwapModel(GameObject modelTobeSwappedIn, Quaternion rotation)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        var swappedModel = Instantiate(modelTobeSwappedIn, transform);
        swappedModel.transform.localPosition = new Vector3(0, yHeight, 0);
        swappedModel.transform.localRotation = rotation;
    }
}
