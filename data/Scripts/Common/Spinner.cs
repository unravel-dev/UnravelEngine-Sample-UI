using Unravel.Core;

namespace UISample
{
    /// <summary>
    /// Spins and optionally bobs its entity. The world-space kiosk drives it through its public
    /// members; the menu uses it for the background shapes.
    /// </summary>
    [ScriptSourceFile]
    public class Spinner : ScriptComponent
    {
        /// <summary>Degrees per second around the world up axis.</summary>
        public float speed = 30.0f;

        /// <summary>Moves the entity up and down around its start height.</summary>
        public bool bob = false;

        public float bobHeight = 0.15f;

        Vector3 home;
        Vector3 baseScale;
        float bobTime;
        float pulse;

        public override void OnStart()
        {
            home = transform.position;
            baseScale = transform.scale;
        }

        public override void OnUpdate()
        {
            transform.RotateAxis(speed * Time.deltaTime, Vector3.up);
            bobTime = bob ? bobTime + Time.deltaTime : 0.0f;
            var offset = Vector3.up * (Mathf.Sin(bobTime * 2.0f) * bobHeight);
            transform.position = home + offset;
            // A pulse scales the entity up and eases back over half a second.
            pulse = Mathf.MoveTowards(pulse, 0.0f, Time.deltaTime * 2.0f);
            transform.scale = baseScale * (1.0f + 0.25f * pulse * pulse);
        }

        /// <summary>Starts a short scale pulse.</summary>
        public void Pulse()
        {
            pulse = 1.0f;
        }
    }
}
