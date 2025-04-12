using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class AspectRatioElement : VisualElement
{
    // Padding elements to keep the aspect ratio.
    private int m_ratioWidth = 16;
    private int m_ratioHeight = 9;
    public int Width
    {
        get => m_ratioWidth;
        set
        {
            m_ratioWidth = value;
            UpdateAspect();
        }
    }
    public int Height
    {
        get => m_ratioHeight;
        set
        {
            m_ratioHeight = value;
            UpdateAspect();
        }
    }
    public AspectRatioElement()
    {
        // Update the padding elements when the geometry changes.
        RegisterCallback<GeometryChangedEvent>(UpdateAspectAfterEvent);
        // Update the padding elements when the element is attached to a panel.
        RegisterCallback<AttachToPanelEvent>(UpdateAspectAfterEvent);
    }
    static void UpdateAspectAfterEvent(EventBase evt)
    {
        var element = evt.target as AspectRatioElement;
        element?.UpdateAspect();
    }
    void UpdateAspect()
    {
        var designRatio = (float)Width / Height;
        var currRatio = resolvedStyle.width / resolvedStyle.height;
        var diff = currRatio - designRatio;

        if (Width <= 0.0f || Height <= 0.0f)
        {
            ClearPadding();
            Debug.LogError($"[AspectRatio] Invalid width:{Width} or height:{Height}");
            return;
        }

        if (float.IsNaN(resolvedStyle.width) || float.IsNaN(resolvedStyle.height))
        {
            return;
        }

        if (diff > 0.01f)
        {
            var w = (resolvedStyle.width - (resolvedStyle.height * designRatio)) * 0.5f;
            style.paddingLeft = w;
            style.paddingRight = w;
            style.paddingTop = 0;
            style.paddingBottom = 0;
        }
        else if (diff < -0.01f)
        {
            var h = (resolvedStyle.height - (resolvedStyle.width * (1 / designRatio))) * 0.5f;
            style.paddingLeft = 0;
            style.paddingRight = 0;
            style.paddingTop = h;
            style.paddingBottom = h;
        }
        else
        {
            ClearPadding();
        }
    }
    private void ClearPadding()
    {
        style.paddingLeft = 0;
        style.paddingRight = 0;
        style.paddingBottom = 0;
        style.paddingTop = 0;
    }
    #region UXML
    [Preserve]
    public new class UxmlFactory : UxmlFactory<AspectRatioElement, UxmlTraits> { }
    [Preserve]
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlIntAttributeDescription WidthAttr = new UxmlIntAttributeDescription {name = "Width", defaultValue = 16};
        UxmlIntAttributeDescription HeightAttr = new UxmlIntAttributeDescription {name = "Height", defaultValue = 9 };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            AspectRatioElement aspectRatioElement = ve as AspectRatioElement;
            aspectRatioElement.Width = WidthAttr.GetValueFromBag(bag, cc);
            aspectRatioElement.Height = HeightAttr.GetValueFromBag(bag, cc);
        }
    }
    #endregion
}
