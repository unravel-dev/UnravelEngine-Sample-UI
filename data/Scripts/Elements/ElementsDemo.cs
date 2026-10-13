using System.Collections.Generic;
using Unravel.Core;

namespace UISample
{
    /// <summary>
    /// Drives app:/data/UI/Elements/elements_demo.rhtml through the element API: no data model,
    /// just elements found by id and changed from code.
    ///   1. Q("#id"), .text and .clicked
    ///   2. CreateElement / AppendChild / Remove, reading a text field, keyboard events
    ///   3. classes (ToggleClass), inline style (style.width, style["prop"]) and Animate
    ///   4. event bubbling: listeners on the document root see events from every element
    /// </summary>
    [ScriptSourceFile]
    public class ElementsDemo : ScriptComponent
    {
        /// <summary>Lines kept in the event log.</summary>
        public int logLines = 7;

        UIDocument document;
        UIElement countLabel;
        UIElement taskList;
        UIElement taskEmpty;
        UIInput taskText;
        UIElement orb;
        UIElement log;
        int count;

        public override void OnCreate()
        {
            var ui = owner.GetComponent<UIDocumentComponent>();
            // documentLoaded runs for the document that is already loaded and again after every
            // reload (the .rhtml changed on disk, or ui.Reload()). A reload makes new elements, so
            // every lookup and subscription happens in Setup.
            ui.documentLoaded += Setup;
        }

        void Setup(UIDocument doc)
        {
            document = doc;
            SetupCounter();
            SetupTasks();
            SetupOrb();
            SetupLog();
        }

        // ---------- 1. query + click ----------

        void SetupCounter()
        {
            countLabel = document.Q("#count");
            // clicked fires for a pointer click or a Click() from code. Subscriptions end with the
            // element or with this script, whichever goes first: no cleanup is needed.
            document.Q("#add-one").clicked += () => SetCount(count + 1);
            document.Q("#reset").clicked += () => SetCount(0);
        }

        void SetCount(int value)
        {
            count = value;
            // .text is plain text: markup characters show as typed. Use InnerRml to parse markup.
            countLabel.text = count.ToString();
            // The "pop" keyframes live in the style sheet; RestartAnimation replays them.
            countLabel.SetClass("pop", true);
            countLabel.RestartAnimation();
            UISounds.Click();
        }

        // ---------- 2. create + remove ----------

        void SetupTasks()
        {
            taskList = document.Q("#task-list");
            taskEmpty = document.Q("#task-empty");
            // Q<T> returns the typed control, or null when the element is a different kind.
            taskText = document.Q<UIInput>("#task-text");
            document.Q("#task-add").clicked += AddTask;
            // Keys reach the document while the text field has focus.
            taskText.On<UIKeyEvent>("keydown", e =>
            {
                if (e.keyCode == KeyCode.Enter || e.keyCode == KeyCode.KpEnter)
                {
                    AddTask();
                }
            });
        }

        void AddTask()
        {
            var text = taskText.value.Trim();
            if (text.Length == 0)
            {
                UISounds.Error();
                taskText.Focus();
                return;
            }
            // Build <div class="task"><span>text</span><button class="ghost">Done</button></div>.
            var item = document.CreateElement("div");
            item.SetClass("task", true);
            var label = document.CreateElement("span");
            label.text = text;
            var done = document.CreateElement("button");
            done.SetClass("ghost", true);
            done.text = "Done";
            // The lambda captures "item": each Done button removes its own row.
            done.clicked += () =>
            {
                item.Remove();
                UISounds.Click();
                UpdateEmptyHint();
            };
            item.AppendChild(label);
            item.AppendChild(done);
            taskList.AppendChild(item);
            // Setting value from code also sends a "change" event (the log shows it);
            // SetValueWithoutNotify would not.
            taskText.value = "";
            UISounds.Confirm();
            UpdateEmptyHint();
        }

        void UpdateEmptyHint()
        {
            // visible = false hides an element; true restores the display its style sheet gives it.
            taskEmpty.visible = taskList.children.Count == 0;
        }

        // ---------- 3. classes, style, animation ----------

        void SetupOrb()
        {
            orb = document.Q("#orb");
            document.Q("#toggle-glow").clicked += () =>
            {
                // The .glow rule in the style sheet swaps the coin sprite for a crystal.
                bool on = orb.ToggleClass("glow");
                UISounds.Click();
                Write("class", on ? "glow on" : "glow off");
            };
            document.Q("#jump").clicked += Jump;
            var size = document.Q<UISlider>("#size");
            size.On<UIChangeEvent>(e => SetOrbSize(size.value));
        }

        void Jump()
        {
            UISounds.Click();
            // Animate tweens one property from its current value. completed fires at the end, so
            // two calls chain into an up-and-down jump.
            orb.Animate("transform", "translateY(-45dp)", 0.22f, UIEasing.QuadOut).completed += () =>
                orb.Animate("transform", "translateY(0dp)", 0.7f, UIEasing.BounceOut);
        }

        void SetOrbSize(float size)
        {
            // Numeric shortcuts take dp.
            orb.style.width = size;
            orb.style.height = size;
            // The indexer takes any RCSS property as a string: the smallest coin is tinted darker.
            orb.style["image-color"] = size <= 34.0f ? "#b0a090" : "#ffffff";
        }

        // ---------- 4. bubbling ----------

        void SetupLog()
        {
            log = document.Q("#log");
            // Clicks and changes bubble from the element they happen on up to the root, so one
            // listener here sees every button, slider and text field in the document.
            document.root.On<UIClickEvent>(e => Write("click", Describe(e.target)));
            document.root.On<UIChangeEvent>(e => Write("change", Describe(e.target) + " = " + Shorten(e.value)));
            // A named event on one element: mouseover fires when the pointer enters it.
            orb.On<UIPointerEvent>("mouseover", e => Write("mouseover", "#orb"));
            Write("ready", "listening on the document root");
        }

        void Write(string kind, string text)
        {
            var entry = document.CreateElement("div");
            entry.SetClass("entry", true);
            // InnerRml parses markup, so the kind can have its own style.
            entry.InnerRml = "<span class=\"kind\">" + kind + "</span>  " + Escape(text);
            log.AppendChild(entry);
            while (log.children.Count > logLines)
            {
                log.children[0].Remove();
            }
        }

        static string Describe(UIElement element)
        {
            if (element == null)
            {
                return "?";
            }
            var id = element.ElementId;
            return string.IsNullOrEmpty(id) ? element.tagName : "#" + id;
        }

        static string Shorten(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "\"\"";
            }
            return value.Length > 24 ? value.Substring(0, 24) + "..." : value;
        }

        /// <summary>Text going into InnerRml must not be parsed as markup.</summary>
        static string Escape(string text)
        {
            return text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }
    }
}
