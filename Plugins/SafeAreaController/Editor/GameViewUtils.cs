#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

using Object = UnityEngine.Object;

namespace UI.SafeArea
{

    public static class GameViewUtils
    {
        public enum GameViewSizeType
        {
            AspectRatio, FixedResolution
        }

        #region Private
        static Type GameViewWindowType => typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        static EditorWindow GameViewWindow => EditorWindow.GetWindow(GameViewWindowType);

        static object gameViewSizesInstance;
        static MethodInfo getGroup;

        static object GetGroup(GameViewSizeGroupType type)
            => getGroup.Invoke(gameViewSizesInstance, new object[] { (int)type });
        #endregion

        #region Properties
        public static int CurrentSelectedSizeIndex
        {
            get => (int)GameViewWindowType.GetProperty(
                    "selectedSizeIndex",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                ).GetValue(GameViewWindow, null);
            set => GameViewWindowType.GetProperty(
                    "selectedSizeIndex",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                ).SetValue(GameViewWindow, value, null);
        }

        public static float GameViewScale
        {
            get
            {
                var areaProp = GameViewWindowType.GetField(
                        "m_ZoomArea",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var scaleProp = areaProp.FieldType.GetProperty("scale");
                var areaValue = areaProp.GetValue(GameViewWindow);
                var scaleValue = (Vector2)scaleProp.GetValue(areaValue, null);
                return scaleValue.y;
            }

            set
            {
                var snapZoom = GameViewWindowType.GetMethod(
                    "SnapZoom", 
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                snapZoom.Invoke(GameViewWindow, new object[] { value });
            }
        }

        public static Vector2 Size => (Vector2)GameViewWindowType.GetMethod(
                "GetMainGameViewTargetSize",
                BindingFlags.NonPublic | BindingFlags.Static
            ).Invoke(null, null);

        public static float AspectRatio { get { Vector2 size = Size; return size.x / size.y; } }

        #endregion

        static GameViewUtils()
        {
            // gameViewSizesInstance  = ScriptableSingleton<GameViewSizes>.instance;
            var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
            var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
            var instanceProp = singleType.GetProperty("instance");
            getGroup = sizesType.GetMethod("GetGroup");
            gameViewSizesInstance = instanceProp.GetValue(null, null);
        }


        public static void AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width, int height, string text)
        {
            // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupTyge);
            // group.AddCustomSize(new GameViewSize(viewSizeType, width, height, text);

            var group = GetGroup(sizeGroupType);
            var addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize"); // or group.GetType().
            var gvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
            var ctor = gvsType.GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(string) });
            var newSize = ctor.Invoke(new object[] { (int)viewSizeType, width, height, text });
            addCustomSize.Invoke(group, new object[] { newSize });
        }

        public static int FindSize(GameViewSizeGroupType sizeGroupType, string text)
        {
            // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
            // string[] texts = group.GetDisplayTexts();
            // for loop...

            var group = GetGroup(sizeGroupType);
            var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
            var displayTexts = getDisplayTexts.Invoke(group, null) as string[];
            for (int i = 0; i < displayTexts.Length; i++)
            {
                string display = displayTexts[i];
                // the text we get is "Name (W:H)" if the size has a name, or just "W:H" e.g. 16:9
                // so if we're querying a custom size text we substring to only get the name
                // You could see the outputs by just logging
                // Debug.Log(display);
                int pren = display.IndexOf('(');
                if (pren != -1)
                    display = display.Substring(0, pren - 1); // -1 to remove the space that's before the prens. This is very implementation-depdenent
                if (display == text)
                    return i;
            }
            return -1;
        }

        public static int FindSize(GameViewSizeGroupType sizeGroupType, int width, int height)
        {
            // goal:
            // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
            // int sizesCount = group.GetBuiltinCount() + group.GetCustomCount();
            // iterate through the sizes via group.GetGameViewSize(int index)

            var group = GetGroup(sizeGroupType);
            var groupType = group.GetType();
            var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
            var getCustomCount = groupType.GetMethod("GetCustomCount");
            int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
            var getGameViewSize = groupType.GetMethod("GetGameViewSize");
            var gvsType = getGameViewSize.ReturnType;
            var widthProp = gvsType.GetProperty("width");
            var heightProp = gvsType.GetProperty("height");
            var indexValue = new object[1];
            for (int i = 0; i < sizesCount; i++)
            {
                indexValue[0] = i;
                var size = getGameViewSize.Invoke(group, indexValue);
                int sizeWidth = (int)widthProp.GetValue(size, null);
                int sizeHeight = (int)heightProp.GetValue(size, null);
                if (sizeWidth == width && sizeHeight == height)
                    return i;
            }
            return -1;
        }

        public static void SetMinGameViewScale()
        {
            float min = (float)GameViewWindowType.GetProperty(
                    "minScale",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                ).GetValue(GameViewWindow, null);
            GameViewScale = min;
        }

        public static GameViewSizeGroupType GetCurrentGroupType()
        {
            var getCurrentGroupTypeProp = gameViewSizesInstance.GetType().GetProperty("currentGroupType");
            return (GameViewSizeGroupType)(int)getCurrentGroupTypeProp.GetValue(gameViewSizesInstance, null);
        }

        public static bool SizeExists(GameViewSizeGroupType sizeGroupType, string text)
        {
            return FindSize(sizeGroupType, text) != -1;
        }

        public static bool SizeExists(GameViewSizeGroupType sizeGroupType, int width, int height)
        {
            return FindSize(sizeGroupType, width, height) != -1;
        }
    }
}
#endif