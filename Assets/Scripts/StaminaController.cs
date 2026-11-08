using UnityEngine;
using UnityEngine.UI;

public class GlideBar : MonoBehaviour
{
    public CharacterMovement player;
    public Image fillImage;
    public CanvasGroup group;
    public Color fullColor = Color.cyan;
    public Color emptyColor = Color.red;

    void Update()
    {
        if (player == null || fillImage == null) return;

        float f = player.GlideFraction;
        fillImage.fillAmount = f;
        fillImage.color = Color.Lerp(emptyColor, fullColor, f);

        if (group != null)
        {
            float targetAlpha = f >= 0.999f ? 0f : 1f;
            group.alpha = Mathf.MoveTowards(group.alpha, targetAlpha, 4f * Time.deltaTime);
        }
    }
}