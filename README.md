# Mini Map Sprite Only
MiniMap feature without RenderTexure and Camera

| Video 1                             |
| ----------------------------------- |
| ![Alt text](/SS~/DemoSS.gif "Demo Gif") |

How to Import:
 - Add to manifest 
```
"com.arikan.minimap" : "https://github.com/bilal-arikan/MiniMapSpriteOnly.git",
```

API Example:

```C#
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
```

Import Demo Unity Package

![Alt text](/SS~/DemoUP.PNG "Demo Import")