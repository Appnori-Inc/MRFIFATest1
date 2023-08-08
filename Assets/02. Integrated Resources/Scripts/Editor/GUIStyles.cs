using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jisu.Utils
{
    public class GUIStyles
    {
        public static class LabelStyles
        {
            public static readonly GUIStyle headlineLabelStyle;
            public static readonly GUIStyle NormalLabelStyle;


            static LabelStyles()
            {
                headlineLabelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 9,
                    alignment = TextAnchor.MiddleCenter,
                    imagePosition = ImagePosition.ImageAbove,
                    fontStyle = FontStyle.Normal
                };

                NormalLabelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 16,
                    alignment = TextAnchor.MiddleLeft,
                    imagePosition = ImagePosition.ImageAbove,
                    fontStyle = FontStyle.Normal
                };
            }
        }

        public static class ToolbarStyles
        {
            public static readonly GUIStyle commandButtonStyle;

            static ToolbarStyles()
            {
                commandButtonStyle = new GUIStyle("Command")
                {
                    fontSize = 16,
                    alignment = TextAnchor.MiddleCenter,
                    imagePosition = ImagePosition.ImageAbove,
                    fontStyle = FontStyle.Normal
                };
            }
        }

        public static class ToggleStyles
        {
            public static readonly GUIStyle commandToggleStyle;

            static ToggleStyles()
            {
                commandToggleStyle = new GUIStyle(GUI.skin.toggle)
                {
                    fixedHeight = 18
                };
            }
        }

        static class TextFieldStyles
        {
            public static readonly GUIStyle EditTextFieldStyle;

            static TextFieldStyles()
            {
                EditTextFieldStyle = new GUIStyle("Edit")
                {
                    alignment = TextAnchor.MiddleCenter,
                };
            }
        }
    }
}
