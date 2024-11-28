using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Building : MonoBehaviour
{


    public BuildingData GetData()
    {
        BuildingData ans = new BuildingData();
        ans.position = this.transform.position;
        GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(this.gameObject);
        //Debug.Log(this.gameObject.name.Replace("(Clone)", ""));
        if (prefab != null)
        {
            ans.prefabName = prefab.name;
        }
        else
            ans.prefabName = this.gameObject.name.Replace("(Clone)", "");
        return ans;
    }

}
