using UnityEngine;

namespace CrowdRagdollSystem
{
    public class PerformanceHUD : MonoBehaviour
    {
        private float deltaTime = 0.0f;

        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        private void OnGUI()
        {
            int w = Screen.width, h = Screen.height;
            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(20, 20, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 3 / 100;
            style.normal.textColor = Color.green;

            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            
            string text = string.Format("{0:0.0} ms ({1:0.} FPS) | GC Alloc: {2} KB", 
                msec, fps, System.GC.GetTotalMemory(false) / 1024);
            
            GUI.Label(rect, text, style);
        }
    }
}
