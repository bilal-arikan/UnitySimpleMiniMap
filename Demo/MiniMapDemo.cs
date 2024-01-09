using System.Collections;
using UnityEngine;

public class MiniMapDemo : MonoBehaviour
{
    public MeshRenderer obj1Centered;
    public MeshRenderer obj2;
    public MeshRenderer obj3;

    public Sprite obj3Sprite;


    // Start is called before the first frame update
    void Start()
    {
        var minimap = FindObjectOfType<Arikan.MiniMapView>();

        // Red object example
        var img = minimap.FollowCentered(obj1Centered.transform);
        img.color = obj1Centered.material.color;

        // Green object example
        var img2 = minimap.Follow(obj2.transform);
        img2.color = obj2.material.color;

        // Blue object example
        var img3 = minimap.Follow(obj3.transform, obj3Sprite);
        img3.color = obj3.material.color;
    }
}
