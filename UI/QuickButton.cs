using UnityEngine;

namespace ClownMeister.UnityEssentials.UI
{
    public static class QuickButton
    {
        // Default initial resolution constant
        private const float DefaultInitialResolutionWidth = 1920f;
        private const float DefaultInitialResolutionHeight = 1080f;

        // Default button sizes
        private const float DefaultScreenWidthPercentage = 0.15f;
        private const float DefaultScreenHeightPercentage = 0.08f;
        private const float DefaultButtonWidthPx = 350f;
        private const float DefaultButtonHeightPx = 100f;

        // Default font size
        private const int DefaultFontSize = 24;

        // Create a button that sizes itself based on a percentage of the screen size
        public static bool ScreenSizeButton(string text, float widthPercentage = DefaultScreenWidthPercentage, float heightPercentage = DefaultScreenHeightPercentage)
        {
            float buttonWidth = Screen.width * widthPercentage;
            float buttonHeight = Screen.height * heightPercentage;

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.RoundToInt(DefaultFontSize * Mathf.Min(Screen.width / DefaultInitialResolutionWidth, Screen.height / DefaultInitialResolutionHeight))
            };

            // GUILayout.BeginArea(new Rect((Screen.width - buttonWidth) / 2, (Screen.height - buttonHeight) / 2, buttonWidth, buttonHeight));
            bool result = GUILayout.Button(text, buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight));
            // GUILayout.EndArea();

            return result;
        }

        // Create a button that scales based on an initial resolution, with optional custom initial resolution
        public static bool ScaledButton(string text, float targetWidth = DefaultButtonWidthPx, float targetHeight = DefaultButtonHeightPx,
            float? initialResolutionWidth = null, float? initialResolutionHeight = null)
        {
            float actualInitialWidth = initialResolutionWidth ?? DefaultInitialResolutionWidth;
            float actualInitialHeight = initialResolutionHeight ?? DefaultInitialResolutionHeight;

            float widthScale = Screen.width / actualInitialWidth;
            float heightScale = Screen.height / actualInitialHeight;

            float buttonWidth = targetWidth * widthScale;
            float buttonHeight = targetHeight * heightScale;

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = Mathf.RoundToInt(DefaultFontSize * Mathf.Min(widthScale, heightScale));

            // GUILayout.BeginArea(new Rect((Screen.width - buttonWidth) / 2, (Screen.height - buttonHeight) / 2, buttonWidth, buttonHeight));
            bool result = GUILayout.Button(text, buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight));
            // GUILayout.EndArea();

            return result;
        }

        // Create a button with an exact pixel size, no scaling
        public static bool UnscaledButton(string text, float width = DefaultButtonWidthPx, float height = DefaultButtonHeightPx)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = DefaultFontSize
            };

            // GUILayout.BeginArea(new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height));
            bool result = GUILayout.Button(text, buttonStyle, GUILayout.Width(width), GUILayout.Height(height));
            // GUILayout.EndArea();

            return result;
        }
    }
}