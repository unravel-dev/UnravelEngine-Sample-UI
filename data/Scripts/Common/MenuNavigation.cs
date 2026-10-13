using Unravel.Core;

namespace UISample
{
    /// <summary>
    /// Returns from a demo to the main menu.
    ///
    /// Put it on the same entity as a UI Document. The markup's
    ///     &lt;button onclick="BackToMenu"&gt;
    /// calls BackToMenu() below: an on&lt;event&gt; attribute calls a public method of a script on the
    /// document's entity (optionally taking the event, e.g. BackToMenu(UIClickEvent e)).
    /// </summary>
    [ScriptSourceFile]
    public class MenuNavigation : ScriptComponent
    {
        public const string MenuScene = "app:/data/Scenes/MainMenu.spfb";

        public override void OnStart()
        {
            UISounds.ApplyLimits();
        }

        public override void OnUpdate()
        {
            // "Cancel" is an input action (Escape in the default mapping), not a key code. Keys only
            // reach a UI document while one of its text fields has focus; otherwise the game gets them.
            if (Input.IsPressed("Cancel"))
            {
                BackToMenu();
            }
        }

        /// <summary>Called by onclick="BackToMenu" in the markup.</summary>
        public void BackToMenu()
        {
            UISounds.Click();
            Scene.LoadScene(MenuScene);
        }
    }
}
