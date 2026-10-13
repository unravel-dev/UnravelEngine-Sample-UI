using System.Collections.Generic;
using System.Globalization;
using Unravel.Core;

namespace UISample
{
    /// <summary>
    /// Drives app:/data/UI/DataModels/shop.rhtml through the data model "shop".
    ///
    /// The pattern: bind a plain C# object once (model.Bind), change its values, and tell the model
    /// which ones changed (model.Dirty). The markup decides how values look; this script never
    /// looks up an element.
    ///   - Methods the markup calls (Buy, Sell, Earn) refresh the whole model afterwards.
    ///   - A control bound with data-value / data-checked refreshes only the variable it wrote:
    ///     a setter that changes other values as well names them with Dirty (see difficulty).
    ///   - Changes made anywhere else (here: a timer) need Dirty(name) or DirtyAll().
    /// </summary>
    [ScriptSourceFile]
    public class ShopController : ScriptComponent
    {
        /// <summary>Gold the timer adds every incomeSeconds.</summary>
        public int income = 5;
        public float incomeSeconds = 3.0f;

        public enum Rarity
        {
            Common,
            Rare,
            Epic
        }

        /// <summary>One row of the shop. Nested objects read as {{ item.name }} in the markup.</summary>
        public class ShopItem
        {
            public string name;
            public string description;
            /// <summary>Atlas sprite of the item (data-attr-sprite="item.icon").</summary>
            public string icon;
            public int basePrice;
            /// <summary>basePrice after the difficulty; what the row shows and Buy charges.</summary>
            public int price;
            public int weight;
            public int owned;
            /// <summary>Enums reach the markup as their name: item.rarity == 'Rare'.</summary>
            public Rarity rarity;
        }

        /// <summary>The object behind the data model "shop".</summary>
        public class ShopModel
        {
            public const int MaxWeight = 100;

            public List<ShopItem> items = new List<ShopItem>
            {
                new ShopItem { name = "Health Potion", description = "Restores 50 health", icon = "item-potion", basePrice = 30, weight = 2, rarity = Rarity.Common },
                new ShopItem { name = "Iron Sword", description = "A dependable blade", icon = "item-sword", basePrice = 120, weight = 12, rarity = Rarity.Common },
                new ShopItem { name = "Frost Bow", description = "Arrows chill the target", icon = "item-bow", basePrice = 340, weight = 9, rarity = Rarity.Rare },
                new ShopItem { name = "Knight's Plate", description = "Heavy, and worth it", icon = "item-armor", basePrice = 650, weight = 35, rarity = Rarity.Epic },
                new ShopItem { name = "Traveler's Map", description = "Reveals the region", icon = "item-map", basePrice = 60, weight = 1, rarity = Rarity.Rare }
            };

            public int gold = 300;
            public int income;
            public float incomeSeconds;
            public string message = "Pick something.";
            public bool messageIsError;
            public bool showDetails = true;
            public string playerName = "";

            /// <summary>Computed values: read-only properties work like fields in the markup.</summary>
            public int itemCount
            {
                get
                {
                    int count = 0;
                    foreach (var item in items)
                    {
                        count += item.owned;
                    }
                    return count;
                }
            }

            public int weight
            {
                get
                {
                    int total = 0;
                    foreach (var item in items)
                    {
                        total += item.owned * item.weight;
                    }
                    return total;
                }
            }

            public string priceNote
            {
                get
                {
                    switch (difficultyLevel)
                    {
                        case "Easy": return "Prices are 25% lower.";
                        case "Hard": return "Prices are 50% higher.";
                        default: return "Fair prices today.";
                    }
                }
            }

            string difficultyLevel = "Normal";

            /// <summary>Set by the controller after Bind; not public, so the markup never sees it.</summary>
            internal UIDataModel model;

            /// <summary>
            /// The select writes this. The setter re-prices every item, which the model cannot see:
            /// a control write refreshes only the variable it wrote, so the setter names the rest.
            /// </summary>
            public string difficulty
            {
                get => difficultyLevel;
                set
                {
                    difficultyLevel = value;
                    UpdatePrices();
                    model?.Dirty(nameof(items));
                    model?.Dirty(nameof(priceNote));
                }
            }

            /// <summary>The slider writes this: the volume of every sound on the UI bus.</summary>
            public float uiVolume
            {
                get => Audio.GetBusVolume(AudioBus.Ui);
                set => Audio.SetBusVolume(AudioBus.Ui, value);
            }

            public ShopModel()
            {
                UpdatePrices();
            }

            /// <summary>data-event-click="Buy(i)".</summary>
            public void Buy(int index)
            {
                var item = items[index];
                if (gold < item.price)
                {
                    Say("Not enough gold for " + item.name + ".", true);
                    UISounds.Error();
                    return;
                }
                if (weight + item.weight > MaxWeight)
                {
                    Say(item.name + " will not fit in the bag.", true);
                    UISounds.Error();
                    return;
                }
                gold -= item.price;
                item.owned++;
                Say("Bought " + item.name + ".", false);
                UISounds.Confirm();
            }

            /// <summary>data-event-click="Sell(i)": sells for half the price.</summary>
            public void Sell(int index)
            {
                var item = items[index];
                if (item.owned == 0)
                {
                    return;
                }
                item.owned--;
                gold += item.price / 2;
                Say("Sold " + item.name + " for " + (item.price / 2) + " gold.", false);
                UISounds.Click();
            }

            /// <summary>data-event-click="Earn(25)": literal arguments work too.</summary>
            public void Earn(int amount)
            {
                gold += amount;
                Say("+" + amount + " gold.", false);
                UISounds.Click();
            }

            void Say(string text, bool isError)
            {
                message = text;
                messageIsError = isError;
            }

            void UpdatePrices()
            {
                float factor = difficultyLevel == "Easy" ? 0.75f : (difficultyLevel == "Hard" ? 1.5f : 1.0f);
                foreach (var item in items)
                {
                    item.price = Mathf.RoundToInt(item.basePrice * factor);
                }
            }
        }

        readonly ShopModel shop = new ShopModel();
        UIDataModel model;

        public override void OnCreate()
        {
            var ui = owner.GetComponent<UIDocumentComponent>();
            // The name matches <body data-model="shop">. Bind maps the object's public fields and
            // properties to variables and its public methods to events.
            model = ui.GetModel("shop");
            model.Bind(shop);
            shop.model = model;
            // {{ gold | gold }} and {{ item.price | gold }}: formatting lives in C#, the markup
            // only says where. The coin icon beside the number says what it counts.
            model.AddTransform<int>("gold", value => value.ToString("N0", CultureInfo.InvariantCulture));
        }

        public override void OnStart()
        {
            shop.income = income;
            shop.incomeSeconds = incomeSeconds;
            // A change the model cannot see by itself: name what changed.
            model.Dirty(nameof(ShopModel.income));
            model.Dirty(nameof(ShopModel.incomeSeconds));
            InvokeRepeating(incomeSeconds, incomeSeconds, AddIncome);
        }

        void AddIncome()
        {
            shop.gold += income;
            // Without Dirty the label would keep the old value until something else refreshed it.
            model.Dirty(nameof(ShopModel.gold));
        }
    }
}
