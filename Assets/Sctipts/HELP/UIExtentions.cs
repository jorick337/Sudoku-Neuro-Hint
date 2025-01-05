using UnityEngine;
using UnityEngine.UI;

namespace Help.UI
{
    public static class UIExtensions
    {
        #region SET

        public static void SetText(this Component component, string value)
        {
            if (component == null)
                return;

            if (component is Text text)
            {
                text.text = value;
                return;
            }

            if (component is InputField inputField)
            {
                inputField.text = value;
                return;
            }
        }

        public static void SetTransparency(this Component component, float value)
        {
            if (component == null)
                return;

            if (component is Button button)
            {
                ColorBlock colors = button.colors;
                colors.normalColor = new(colors.normalColor.r, colors.normalColor.g,
                    colors.normalColor.b, value);
                button.colors = colors;
                return;
            }

            if (component is Text text)
            {
                Color color = text.color;
                text.color = new(color.r, color.g, color.b, value);
                return;
            }

            if (component is Image image)
            {
                Color color = image.color;
                image.color = new(color.r, color.g, color.b, value);
            }
        }

        public static void SetColor(this Component component, Color color)
        {
            if (component == null)
                return;

            if (component is Image image)
            {
                image.color = color;
                return;
            }

            if (component is Text text)
            {
                text.color = color;
                return;
            }
        }

        public static void SetSortingOrder(this Canvas canvas, int value) =>
            canvas.sortingOrder = value;
        
        public static void SetRaycastTarget(this Image image, bool value) =>
            image.raycastTarget = value;
        public static void SetEnabled(this Image image, bool value) =>
            image.enabled = value;

        public static void SetInteractable(this Button button, bool value) =>
            button.interactable = value;

        public static void SetFontSize(this InputField inputField, int value) =>
            inputField.textComponent.fontSize = value;
        public static void SetReadOnly(this InputField inputField, bool value) =>
            inputField.readOnly = value;
        public static void SetCharacteLimit(this InputField inputField, int value) =>
            inputField.characterLimit = value;
        
        #endregion

        #region GET

        public static string GetText(this Component component)
        {
            if (component == null)
                return string.Empty;

            if (component is Text text)
                return text.text;

            if (component is InputField inputField)
                return inputField.text;

            return string.Empty;
        }

        #endregion

        #region BOOL

        public static bool IsActive(this GameObject gameObject) =>
            gameObject.activeSelf;

        #endregion
    }
}