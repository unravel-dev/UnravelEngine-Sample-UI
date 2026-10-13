using Unravel.Core;

namespace UISample
{
    /// <summary>
    /// Orbits the camera around a point: right mouse drag turns, the wheel zooms. The left button
    /// stays free for clicking world-space UI.
    /// </summary>
    [ScriptSourceFile]
    public class OrbitCamera : ScriptComponent
    {
        public Vector3 pivot = new Vector3(0.0f, 1.2f, 0.0f);
        public float distance = 8.0f;
        public float minDistance = 3.0f;
        public float maxDistance = 16.0f;
        /// <summary>Degrees per unit of mouse movement, scaled by the frame time like the samples.</summary>
        public float sensitivity = 10.0f;
        public float zoomSpeed = 1.0f;

        float yaw;
        float pitch;

        public override void OnStart()
        {
            // Start from where the camera was placed in the scene.
            var offset = transform.position - pivot;
            distance = Mathf.Clamp(offset.magnitude, minDistance, maxDistance);
            yaw = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
            pitch = Mathf.Asin(Mathf.Clamp(offset.y / Mathf.Max(offset.magnitude, 0.001f), -1.0f, 1.0f)) * Mathf.Rad2Deg;
            Apply();
        }

        public override void OnUpdate()
        {
            if (Input.IsDown("Mouse Right"))
            {
                yaw += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
                pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime, 5.0f, 75.0f);
            }
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, minDistance, maxDistance);
            Apply();
        }

        void Apply()
        {
            // Spherical coordinates, the inverse of what OnStart reads from the start position.
            float yawRadians = yaw * Mathf.Deg2Rad;
            float pitchRadians = pitch * Mathf.Deg2Rad;
            var direction = new Vector3(Mathf.Sin(yawRadians) * Mathf.Cos(pitchRadians),
                                        Mathf.Sin(pitchRadians),
                                        Mathf.Cos(yawRadians) * Mathf.Cos(pitchRadians));
            transform.position = pivot + direction * distance;
            transform.LookAt(pivot);
        }
    }
}
