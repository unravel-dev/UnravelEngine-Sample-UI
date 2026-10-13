using System.Collections.Generic;
using Unravel.Core;

namespace UISample
{
    /// <summary>
    /// Screen-space markers that follow world positions (app:/data/UI/WorldSpace/world_hud.rhtml).
    ///
    /// The other way to put UI over the world: instead of a world-space document, a screen-space
    /// element is moved every frame to camera.WorldToScreenPoint(position). It keeps a constant
    /// size on screen and is never hidden behind geometry. One marker is made per Nameplate target.
    /// </summary>
    [ScriptSourceFile]
    public class WorldMarkers : ScriptComponent
    {
        public string cameraName = "Main Camera";

        /// <summary>Metres below the target's origin where the marker points (its feet).</summary>
        public float footOffset = 0.0f;

        class Marker
        {
            public Entity target;
            public string name;
            public UIElement element;
            public UIElement label;
        }

        readonly List<Marker> markers = new List<Marker>();
        UIDocument document;
        CameraComponent viewCamera;

        public override void OnCreate()
        {
            var ui = owner.GetComponent<UIDocumentComponent>();
            ui.documentLoaded += BuildMarkers;
        }

        public override void OnStart()
        {
            var cameraEntity = Scene.FindEntityByName(cameraName);
            viewCamera = cameraEntity ? cameraEntity.GetComponent<CameraComponent>() : null;
        }

        void BuildMarkers(UIDocument doc)
        {
            document = doc;
            markers.Clear();
            var layer = doc.Q("#markers");
            // Every entity with a Nameplate script floats over a target; mark those targets.
            foreach (var entity in Scene.FindEntitiesWithComponent<Nameplate>())
            {
                var plate = entity.GetComponent<Nameplate>();
                var marker = new Marker { target = plate.GetTarget(), name = plate.displayName };
                // <div class="marker"><div class="diamond"/><div class="label"/></div>
                marker.element = doc.CreateElement("div");
                marker.element.SetClass("marker", true);
                var diamond = doc.CreateElement("div");
                diamond.SetClass("diamond", true);
                marker.label = doc.CreateElement("div");
                marker.label.SetClass("label", true);
                marker.element.AppendChild(diamond);
                marker.element.AppendChild(marker.label);
                layer.AppendChild(marker.element);
                markers.Add(marker);
            }
        }

        public override void OnLateUpdate()
        {
            // LateUpdate: after the scripts that move things this frame.
            if (viewCamera == null || document == null)
            {
                return;
            }
            var cameraPosition = viewCamera.transform.position;
            foreach (var marker in markers)
            {
                if (!marker.target)
                {
                    marker.element.visible = false;
                    continue;
                }
                var world = marker.target.transform.position + Vector3.up * footOffset;
                // x, y in pixels from the top left (the space of Input.mousePosition); z is the
                // distance in front of the camera, 0 or less when the point is behind it.
                var screen = viewCamera.WorldToScreenPoint(world);
                bool onScreen = screen.z > 0.0f;
                marker.element.visible = onScreen;
                if (!onScreen)
                {
                    continue;
                }
                // RCSS lengths are dp and screen points are pixels: divide by pixelsPerDp.
                marker.element.style.left = screen.x / document.pixelsPerDp;
                marker.element.style.top = screen.y / document.pixelsPerDp;
                var distance = Vector3.Distance(cameraPosition, world);
                marker.label.text = marker.name + "  " + distance.ToString("0.0") + " m";
            }
        }
    }
}
