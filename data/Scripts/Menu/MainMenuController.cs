using System.Collections.Generic;
using Unravel.Core;

namespace UISample
{
    /// <summary>
    /// The launcher (app:/data/UI/Menu/main_menu.rhtml). One data model drives the whole screen:
    ///   - a C# list rendered with data-for (one card per demo),
    ///   - a model method called from the markup with an argument: data-event-click="Open(i)",
    ///   - controls bound both ways (data-value / data-checked) to properties with side effects,
    ///   - a transform that formats a value for display: {{ masterVolume | percent }},
    ///   - onclick="Quit", which calls Quit() on this script.
    /// </summary>
    [ScriptSourceFile]
    public class MainMenuController : ScriptComponent
    {
        /// <summary>One card of the menu. Public fields are what the markup can read.</summary>
        public class Demo
        {
            public string title;
            public string description;
            /// <summary>Atlas sprite of the card (data-attr-sprite="demo.icon").</summary>
            public string icon;
            public string scene;
        }

        /// <summary>
        /// The object behind the data model "menu". Public fields and properties become variables
        /// the markup reads; public methods become events it can call (data-event-*).
        /// </summary>
        public class MenuModel
        {
            public List<Demo> demos = new List<Demo>
            {
                new Demo
                {
                    title = "Elements & Events",
                    description = "Query elements, change text and classes, build DOM, animate, listen to events.",
                    icon = "item-book",
                    scene = "app:/data/Scenes/01_Elements.spfb"
                },
                new Demo
                {
                    title = "Data Models",
                    description = "Bind a C# object: lists, conditions, two-way controls, events with arguments.",
                    icon = "item-chest",
                    scene = "app:/data/Scenes/02_DataModels.spfb"
                },
                new Demo
                {
                    title = "World-Space UI",
                    description = "Clickable panels and nameplates in the 3D scene, plus screen-space markers.",
                    icon = "item-map",
                    scene = "app:/data/Scenes/03_WorldSpace.spfb"
                }
            };

            /// <summary>
            /// A property, not a field: the slider writes it through the setter, which applies the
            /// value to the mixer. The model updates the label by itself after a control writes.
            /// </summary>
            public float masterVolume
            {
                get => Audio.masterVolume;
                set => Audio.masterVolume = value;
            }

            bool music = Audio.GetBusVolume(AudioBus.Music) > 0.0f;

            /// <summary>Fades the music bus in or out instead of cutting it.</summary>
            public bool musicOn
            {
                get => music;
                set
                {
                    music = value;
                    Audio.FadeBusVolume(AudioBus.Music, value ? 1.0f : 0.0f, 0.6f);
                }
            }

            /// <summary>data-event-click="Open(i)": the index arrives as an int.</summary>
            public void Open(int index)
            {
                UISounds.Confirm();
                Scene.LoadScene(demos[index].scene);
            }

            /// <summary>data-event-mouseover="Hover()". The clip limit keeps it from stacking.</summary>
            public void Hover()
            {
                UISounds.Hover();
            }
        }

        readonly MenuModel menu = new MenuModel();

        public override void OnCreate()
        {
            // GetModel works before the document has loaded, so the model is bound once, here.
            var ui = owner.GetComponent<UIDocumentComponent>();
            var model = ui.GetModel("menu");
            model.Bind(menu);
            model.AddTransform<float>("percent", value => Mathf.RoundToInt(value * 100.0f) + "%");
        }

        public override void OnStart()
        {
            UISounds.ApplyLimits();
        }

        /// <summary>onclick="Quit" in the markup. In the editor this ends play mode.</summary>
        public void Quit()
        {
            UISounds.Click();
            Application.Quit();
        }
    }
}
