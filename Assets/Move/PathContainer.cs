using UnityEngine;

public class PathContainer : MonoBehaviour
{
    [SerializeField] Path[] Paths;
    public int Length => Paths.Length;

    public Path this[int i] => this.Paths[i];
}