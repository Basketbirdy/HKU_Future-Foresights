using TMPro;
using UnityEngine;

public class TeleportButton : MonoBehaviour
{
    private Transform point;
    private Transform target;

    private TextMeshProUGUI textMeshPro;

    private void Awake()
    {
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Setup(Transform point, Transform target)
    {
        this.point = point;
        this.target = target;

        if(textMeshPro == null) { return; }
        textMeshPro.text = point.name;
    }

    public void Teleport()
    {
        target.position = point.position;
    }
}
