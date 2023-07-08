using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 染色器
/// </summary>
public class Stainer : MonoBehaviour {
    public List<Renderer> rendererList;
    
    public void SetMat(Material mat) {
        foreach (var tempRenderer in rendererList) {
            tempRenderer.material = mat;
        }
    }
}