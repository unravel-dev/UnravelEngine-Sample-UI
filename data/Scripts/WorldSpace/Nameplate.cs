using Unravel.Core;

namespace UISample
{
    /// <summary>
    /// A health nameplate floating above its parent entity (app:/data/UI/WorldSpace/nameplate.rhtml).
    /// Put it on a child entity with a world-space UI Document. It turns to face the camera every
    /// frame, so the document stays readable from any side.
    /// </summary>
    [ScriptSourceFile]
    public class Nameplate : ScriptComponent
    {
        public string displayName = "Target";
        public string cameraName = "Main Camera";

        /// <summary>The object behind this nameplate's data model "plate".</summary>
        public class PlateModel
        {
            public string name = "";
            public int health = 100;
            public int maxHealth = 100;

            public void Hit()
            {
                health = Mathf.Max(0, health - 15);
                UISounds.Click();
            }

            public void Heal()
            {
                health = maxHealth;
                UISounds.Confirm();
            }
        }

        readonly PlateModel plate = new PlateModel();
        UIDataModel model;
        Entity cameraEntity;

        /// <summary>
        /// The entity the nameplate floats over: its parent. A method, not a property: public
        /// properties of a script are saved with the scene, and this is computed.
        /// </summary>
        public Entity GetTarget() => transform.parent;

        public override void OnCreate()
        {
            model = owner.GetComponent<UIDocumentComponent>().GetModel("plate");
            model.Bind(plate);
        }

        public override void OnStart()
        {
            // Public fields are set by the time OnStart runs; push the name into the model.
            plate.name = displayName;
            model.Dirty(nameof(PlateModel.name));
            cameraEntity = Scene.FindEntityByName(cameraName);
        }

        public override void OnLateUpdate()
        {
            if (!cameraEntity)
            {
                return;
            }
            // Turn around the vertical axis only, so the plate stays upright. The document's
            // readable side faces away from its forward axis, so look away from the camera.
            var position = transform.position;
            var toCamera = cameraEntity.transform.position - position;
            toCamera.y = 0.0f;
            if (toCamera.sqrMagnitude > 1e-4f)
            {
                transform.LookAt(position - toCamera);
            }
        }
    }
}
