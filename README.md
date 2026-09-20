# Unity Simple Mini Map
Mini map for uGUI without extra cameras or RenderTextures.

The map is a regular UI image and targets are projected onto it every frame. You do not need overhead cameras, RenderTexture assets, extra layers or layer masks.

| Demo |
| ---- |
| ![Demo](/SS~/DemoSS.gif "Demo Gif") |

## Features
- Follow any number of targets plus one centered target (usually the player)
- Rotation modes: `NorthUp` or `RotateWithTarget`
- Zoom with limits and steps
- 3D top-down worlds (`XZ` plane) and 2D worlds (`XY` plane)
- Icons can turn with their target or stay upright
- Icons of destroyed targets are removed automatically

## Installation
Add the package to `Packages/manifest.json`:
```json
"com.arikan.minimap": "https://github.com/bilal-arikan/UnitySimpleMiniMap.git",
```
Requires Unity 2021.3 or newer and uGUI.

## Setup
1. **Bounds:** add `MiniMapBounds` to an empty GameObject. It creates `TopRight` and `BottomLeft` corners. Move them to the corners of the area shown by your map image and pick the plane: `XZ` for 3D, `XY` for 2D.
2. **Map image:** use a top-down image of exactly that area.
3. **UI:** build this hierarchy under a Canvas (see `Demo/MiniMap.prefab`):
   ```
   MiniMap          MiniMapView
   └─ Mask          Mask or RectMask2D    -> centeredDotCanvas
      └─ Map        Image with the map    -> otherDotCanvas
   ```
   - `Map` must be a direct child of `Mask`.
   - Keep the aspect ratio of the `Map` rect equal to the aspect ratio of the bounds and turn off *Preserve Aspect* on its Image, so the picture covers exactly the bounds area. Otherwise icons drift away from the picture.
   - The map texture does not have to be square: any aspect ratio works as long as the rect matches it. Versions before 1.1.0 needed a 1:1 texture to stay aligned while the map rotated.
4. Assign an icon prefab (`Image`) to `uiDotPrefab` and optionally a `defaultSprite`.

## Usage
```C#
using Arikan;
using UnityEngine;

public class MiniMapExample : MonoBehaviour
{
    [SerializeField] private MiniMapView minimap;
    [SerializeField] private Transform player;
    [SerializeField] private Transform enemy;
    [SerializeField] private Transform shop;
    [SerializeField] private Sprite shopSprite;

    private void Start()
    {
        // Player stays at the center of the mini map
        minimap.FollowCentered(player).color = Color.red;

        // Icon turns with the enemy
        minimap.Follow(enemy).color = Color.green;

        // Custom sprite that stays upright while the map rotates
        minimap.Follow(shop, shopSprite, rotateWithTarget: false);

        minimap.RotationMode = MiniMapRotationMode.NorthUp;
        minimap.Zoom = 2f;
    }
}
```

## API
| Member | Description |
| ------ | ----------- |
| `FollowCentered(target, icon = null)` | Keeps the target at the center of the viewport and returns its `Image`. |
| `Follow(target, icon = null, rotateWithTarget = true)` | Shows the target on the map and returns its `Image`. |
| `UnfollowTarget(target)`, `ClearTargets()` | Removes icons. |
| `RotationMode` | `NorthUp` keeps world up at the top. `RotateWithTarget` turns the map with the centered target. |
| `Zoom`, `MinZoom`, `MaxZoom`, `SetZoomLimits(min, max)` | Zoom factor and its limits. |
| `ZoomIn()`, `ZoomOut()` | Multiplies or divides the zoom by the zoom step. |
| `Refresh()` | Updates the layout immediately. Called automatically in `LateUpdate`. |
| `CenteredTarget`, `FollowedTargetCount`, `IsFollowing(target)` | Current state. |
| `MiniMapBounds.plane` | `XZ` for 3D, `XY` for 2D. |
| `MiniMapMath.WorldToMap`, `MiniMapMath.GetMapAngle` | Conversion helpers. |

Icon rotation uses the target's forward vector on the `XZ` plane and its up vector on the `XY` plane, so icon sprites should point up.

## Tips
- Resize icons with the `sizeDelta` of the returned `Image`. Their `localScale` keeps the icon size constant while zooming.
- Put the mini map under its own Canvas so its per-frame updates do not rebuild the rest of your HUD.

## Demo
Open `Demo/MiniMapDemo.unity` and press Play. Use the buttons at the top right to switch the rotation mode and zoom.
The demo scene uses URP materials. In the Built-in or HDRP pipeline the meshes look pink, but the mini map works.
