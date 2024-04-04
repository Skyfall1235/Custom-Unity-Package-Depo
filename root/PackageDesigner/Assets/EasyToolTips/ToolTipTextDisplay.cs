using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class ToolTipTextDisplay : MonoBehaviour
{
    public TextMeshProUGUI HeaderField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public int characterWraplimit;

    void Update()
    {
        int headerLength = HeaderField.text.Length;
        int contentLength = contentField.text.Length;

        layoutElement.enabled = (headerLength > characterWraplimit || contentLength > characterWraplimit) ? true : false;
    }
}
