using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MapSO", menuName = "SO/MapSO")]
public class MapSO : ScriptableObject
{
    public int width;
    public int height;
    public List<Material> materials;
    public Texture2D texture;
}
