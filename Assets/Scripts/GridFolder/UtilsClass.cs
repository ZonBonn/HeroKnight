using System;
using UnityEngine;

namespace ZonBon.Utils
{
    public class UtilsClass
    {
        public static Vector3 getMouseWorldPosition()
        {
            Vector3 mouseWorldPosition;
            mouseWorldPosition = getMouseWorldPositionWithoutZ(Input.mousePosition, Camera.main);
            return mouseWorldPosition;
        }

        public static Vector3 getMouseWorldPositionWithoutZ()
        {
            return getMouseWorldPositionWithoutZ(Input.mousePosition, Camera.main); // camera.main == OBJ có tag "Camera Main"
        }

        public static Vector3 getMouseWorldPositionWithoutZ(Camera worldCamera)
        {
            return getMouseWorldPositionWithoutZ(Input.mousePosition, Camera.main);
        }

        public static Vector3 getMouseWorldPositionWithoutZ(Vector3 mouseScreenPosition, Camera worldCamera)
        {
            Vector3 mousWorldPosition = worldCamera.ScreenToWorldPoint(mouseScreenPosition);
            return mousWorldPosition;
        }

        public static TextMesh CreateTextInWorld(string text, Vector3 location = default(Vector3), int fontSize = 40, float characterSize = 10, Transform parent = null)
        {
            return CreateTextInWorld(location, text, null, fontSize, characterSize, TextAnchor.MiddleCenter, parent);
        }

        public static TextMesh CreateTextInWorld(Vector3 location, string text, Color? color, int fontSize, float characterSize, TextAnchor textAnchor = TextAnchor.MiddleCenter, Transform parent = null)
        {
            GameObject textMeshOBJ = new GameObject("World_Text", typeof(TextMesh));
            textMeshOBJ.transform.position = location;
            TextMesh textMesh = textMeshOBJ.GetComponent<TextMesh>();
            textMesh.text = text;
            if (color == null) textMesh.color = Color.white;
            textMesh.fontSize = fontSize;
            textMesh.characterSize = characterSize;
            textMesh.anchor = textAnchor;
            textMeshOBJ.transform.SetParent(parent, false);
            return textMesh;
        }
    }
    public class FunctionPeriodic : MonoBehaviour
    {
        private float time;
        private float timer;
        
        private Action action;

        public static FunctionPeriodic Create(Action action, float timer)
        {
            GameObject gObj = new GameObject("FunctionPeriodic", typeof(FunctionPeriodic));
            FunctionPeriodic FP = gObj.GetComponent<FunctionPeriodic>();
            FP.time = timer; // biến ngoài muốn tham gia một hàm static thì phải 1. cũng là static, 2. Thuộc về class 
            FP.timer = timer;
            FP.action = action;
            return FP;
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                action.Invoke();
                timer = time;
            }
        }
    }
}
