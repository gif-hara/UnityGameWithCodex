using UnityEngine;

namespace UnityGameWithCodex
{
    public class Crosshair : MonoBehaviour
    {
        [SerializeField] private Color color = Color.red;
        [SerializeField] private float lineLength = 12f;
        [SerializeField] private float lineThickness = 2f;

        private Texture2D texture;

        private void Awake()
        {
            texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
        }

        private void OnGUI()
        {
            if (texture == null)
            {
                return;
            }

            float centerX = Screen.width * 0.5f;
            float centerY = Screen.height * 0.5f;

            GUI.DrawTexture(
                new Rect(centerX - (lineThickness * 0.5f), centerY - (lineLength * 0.5f), lineThickness, lineLength),
                texture);

            GUI.DrawTexture(
                new Rect(centerX - (lineLength * 0.5f), centerY - (lineThickness * 0.5f), lineLength, lineThickness),
                texture);
        }

        private void OnDestroy()
        {
            if (texture != null)
            {
                Destroy(texture);
            }
        }
    }
}
