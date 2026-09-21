# Changelog

## [1.1.0] - 2026-09-20
### Added
- `MiniMapView.RotationMode` with `NorthUp` and `RotateWithTarget` (default, previous behavior).
- Zoom: `Zoom`, `MinZoom`, `MaxZoom`, `SetZoomLimits`, `ZoomIn` and `ZoomOut`. Icons keep their size while zooming.
- 2D support: `MiniMapBounds.plane` selects the `XZ` (3D) or `XY` (2D) plane.
- `Follow(target, icon, rotateWithTarget)` keeps icons upright on a rotating map when `rotateWithTarget` is false.
- `Refresh()`, `CenteredTarget`, `FollowedTargetCount` and `IsFollowing()`.
- `MiniMapMath` conversion helpers.
- Demo: moving centered target and on-screen rotation and zoom controls.
- Offline PDF documentation with a setup guide and a script reference.

### Fixed
- Icons and the centered map were offset when the bounds center was not at the world origin.
- Adding `MiniMapBounds` to an object without corners moved the object and created mirrored corners.
- The centered target drifted away from the center when the map rect aspect differed from the bounds aspect and the map rotated.
- The map size was read from `sizeDelta`, which broke maps with stretched anchors. `rect.size` is used now.
- A scaled centered target moved the map by the wrong distance.
- Pitch and roll of the centered target changed the map rotation.
- `Follow(null)` left an orphan icon, and following the same target as normal and centered created two icons.
- Icons of destroyed targets stayed on the map, and the map pose was not restored after the centered target was removed.
- Missing bounds threw a `NullReferenceException` every frame. A single warning is logged now.
- Following without a sprite replaced the prefab sprite with `null`.
- The layout runs in `LateUpdate` to avoid a one-frame lag.
- Obsolete `Object.FindObjectOfType` warnings on Unity 2023.1 and newer.
- Demo: the map image matches the bounds, the unused EventSystem is removed, the demo assembly compiles for all platforms and the map image has an ASCII file name.
- `package.json`: minimum Unity version, repository URL and uGUI dependency.

### Changed
- Missing references throw `InvalidOperationException` and a null target throws `ArgumentNullException` instead of `NullReferenceException`.

### Removed
- Public `Arikan.Extensions` helpers (`XZ`, `GetPosition`, `GetRotation`). They conflicted with other Arikan packages.
- `Translate` and `TranslateReverse`. The layout is applied automatically.

## [1.0.0]
- Initial release.
