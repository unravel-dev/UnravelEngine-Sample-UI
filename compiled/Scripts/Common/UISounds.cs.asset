using Unravel.Core;

namespace UISample
{
    /// <summary>
    /// The sample's UI sound effects.
    ///
    /// Each sound is a one-shot: Audio.PlayOneShot plays a clip without an entity or an
    /// AudioSourceComponent and forgets it when it ends. The sounds play on the UI bus, so a
    /// settings screen can scale all of them at once with Audio.SetBusVolume(AudioBus.Ui, v).
    /// </summary>
    public static class UISounds
    {
        // Assets.GetAsset is cheap after the first call (the asset manager keeps loaded assets),
        // so the clips are looked up by key on every play instead of being cached in statics.
        const string ClickKey = "app:/data/Audio/ui_click.wav";
        const string HoverKey = "app:/data/Audio/ui_hover.wav";
        const string ConfirmKey = "app:/data/Audio/ui_confirm.wav";
        const string ErrorKey = "app:/data/Audio/ui_error.wav";
        const string CoinKey = "app:/data/Audio/coin.wav";

        /// <summary>
        /// Clip limits last for one play session, so every screen calls this from OnStart.
        /// Hover fires on every element the pointer crosses: at most one hover sound may play at a
        /// time, and never two within 80 ms. Coins stack up to four.
        /// </summary>
        public static void ApplyLimits()
        {
            Audio.SetClipLimit(Assets.GetAsset<AudioClip>(HoverKey), 1, 0.08f);
            Audio.SetClipLimit(Assets.GetAsset<AudioClip>(CoinKey), 4);
        }

        public static void Click() => Play(ClickKey, 0.8f);

        public static void Hover() => Play(HoverKey, 0.35f);

        public static void Confirm() => Play(ConfirmKey, 0.7f);

        public static void Error() => Play(ErrorKey, 0.6f);

        /// <summary>
        /// A coin at a world position: a spatial one-shot, quieter the farther it is from the
        /// listener (the camera).
        /// </summary>
        public static void Coin(Vector3 position)
        {
            Audio.PlayOneShot(Assets.GetAsset<AudioClip>(CoinKey), position, 0.9f, 1.0f, AudioBus.Sfx);
        }

        static void Play(string key, float volume)
        {
            // Priority 10 keeps UI feedback over ambient one-shots when the voice cap is reached.
            Audio.PlayOneShot(Assets.GetAsset<AudioClip>(key), volume, 1.0f, AudioBus.Ui, 10);
        }
    }
}
