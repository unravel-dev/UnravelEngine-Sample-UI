using Unravel.Core;

namespace UISample
{
    /// <summary>
    /// Drives the world-space kiosk (app:/data/UI/WorldSpace/kiosk.rhtml). A world-space document
    /// is scripted exactly like a screen-space one: the same data model calls, the same events.
    /// The only difference is where it is drawn.
    ///
    /// The panel controls the "Showpiece" entity: colour (a material instance per renderer),
    /// spin speed and bobbing (its Spinner script), and a pulse with a spatial one-shot sound.
    /// </summary>
    [ScriptSourceFile]
    public class KioskPanel : ScriptComponent
    {
        /// <summary>Name of the entity the panel controls.</summary>
        public string showpieceName = "Showpiece";

        /// <summary>The object behind the data model "kiosk".</summary>
        public class KioskModel
        {
            static readonly Color[] Colors =
            {
                new Color(0.91f, 0.92f, 0.96f, 1.0f),
                new Color(1.00f, 0.42f, 0.51f, 1.0f),
                new Color(1.00f, 0.71f, 0.28f, 1.0f),
                new Color(0.43f, 0.91f, 0.66f, 1.0f),
                new Color(0.35f, 0.78f, 1.00f, 1.0f),
                new Color(0.78f, 0.49f, 1.00f, 1.0f)
            };

            // Private: only public members are bound, so the markup never sees these.
            readonly Entity showpiece;
            readonly Spinner spinner;

            public int selected;
            /// <summary>Buttons pressed. Pick and Pulse are model events, so the label refreshes after each.</summary>
            public int clicks;

            public KioskModel(Entity showpiece)
            {
                this.showpiece = showpiece;
                spinner = showpiece ? showpiece.GetComponent<Spinner>() : null;
            }

            /// <summary>The slider writes this; the setter forwards it to the Spinner.</summary>
            public float spinSpeed
            {
                get => spinner != null ? spinner.speed : 0.0f;
                set
                {
                    if (spinner != null)
                    {
                        spinner.speed = value;
                    }
                }
            }

            public bool bob
            {
                get => spinner != null && spinner.bob;
                set
                {
                    if (spinner != null)
                    {
                        spinner.bob = value;
                    }
                }
            }

            /// <summary>data-event-click="Pick(n)" on each swatch.</summary>
            public void Pick(int index)
            {
                selected = index;
                clicks++;
                // SetColor on a model gives that renderer its own copy of the material the first
                // time, so other entities using the same material keep their colour.
                var model = showpiece ? showpiece.GetComponent<ModelComponent>() : null;
                model?.SetColor(MaterialProperty.BaseColor, Colors[index]);
                UISounds.Click();
            }

            /// <summary>data-event-click="Pulse()".</summary>
            public void Pulse()
            {
                clicks++;
                spinner?.Pulse();
                if (showpiece)
                {
                    // A spatial one-shot at the showpiece: it pans and fades with the camera.
                    UISounds.Coin(showpiece.transform.position);
                }
            }
        }

        KioskModel kiosk;

        public override void OnCreate()
        {
            var showpiece = Scene.FindEntityByName(showpieceName);
            if (!showpiece)
            {
                Log.Warning("KioskPanel: no entity named " + showpieceName);
            }
            kiosk = new KioskModel(showpiece);
            var ui = owner.GetComponent<UIDocumentComponent>();
            ui.GetModel("kiosk").Bind(kiosk);
        }

        public override void OnStart()
        {
            UISounds.ApplyLimits();
        }
    }
}
