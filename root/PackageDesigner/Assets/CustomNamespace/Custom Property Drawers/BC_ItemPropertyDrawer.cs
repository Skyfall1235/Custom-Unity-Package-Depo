using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using CustomNamespace;

//[CustomPropertyDrawer(typeof(BC_Item), true)]
public class BC_ItemPropertyDrawer : PropertyDrawer
{
    const string filePathForBC_ItemDrawerTree = "Assets/Scripts/Editor/UXML/ItemCustomInspector.uxml";
    const int ImageHeight = 128;
    bool init = true;
    static Quaternion m_previewCameraRotation = Quaternion.Euler(20, -120, 0);
    public PreviewRenderUtility m_previewRenderUtility;
    VisualTreeAsset m_itemCustomInspectorTree;
    public virtual void OnEnable(SerializedProperty property)
    {
        if (m_previewRenderUtility != null)
            m_previewRenderUtility.Cleanup();
        m_previewRenderUtility = new PreviewRenderUtility(true);
        System.GC.SuppressFinalize(m_previewRenderUtility);
        m_itemCustomInspectorTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(filePathForBC_ItemDrawerTree);
        var camera = m_previewRenderUtility.camera;
        camera.fieldOfView = 30f;
        camera.nearClipPlane = 0.01f;
        camera.farClipPlane = 1000;
        camera.transform.position = new Vector3(2.5f, 2.5f, -2.5f);
        camera.transform.LookAt(Vector3.zero);
    }


    public virtual void OnDisable()
    {
        if (m_previewRenderUtility != null)
        {
            m_previewRenderUtility.Cleanup();
            m_previewRenderUtility = null;
        }
    }
    public virtual void OnDestroy()
    {
        if (m_previewRenderUtility != null)
        {
            m_previewRenderUtility.Cleanup();
            m_previewRenderUtility = null;
        }
    }
    
    void CreateModelDisplayForInspector(VisualElement root, SerializedProperty property)
    {
        VisualElement modelPreviewContainer = root.Query<VisualElement>("ModelPreviewContainer");
        Image image = root.Query<Image>("ModelPreviewImage");
        AspectRatioElement aspectRatioContainer = root.Query<AspectRatioElement>("ModelPreviewAspectController");
        if(aspectRatioContainer == null) 
        {
            aspectRatioContainer = new AspectRatioElement();
            aspectRatioContainer.name = "ModelPreviewAspectController";
            modelPreviewContainer.Add(aspectRatioContainer);
        }
        if (image == null)
        {
            image = new Image();
            image.name = "ModelPreviewImage";
            aspectRatioContainer.Add(image);
        }
        Mesh meshToDisplay = property.FindPropertyRelative("m_modelToDisplay").objectReferenceValue as Mesh;
        Material materialToDisplay = property.FindPropertyRelative("m_materialToDisplay").objectReferenceValue as Material;
        if (meshToDisplay == null)
        {
            image.image = null;
        }
        else
        {
            Quaternion rotation = Quaternion.identity;
            m_previewRenderUtility.camera.transform.LookAt(Vector3.zero);
            Matrix4x4 meshTransformations = Matrix4x4.TRS(-meshToDisplay.bounds.center, Quaternion.identity, Vector3.one);
            Vector2Int desiredImageDimensions = new Vector2Int(512, 512);
            m_previewRenderUtility.BeginPreview(new Rect(0, 0, desiredImageDimensions.x, desiredImageDimensions.y), GUIStyle.none);
            m_previewRenderUtility.lights[0].transform.localEulerAngles = new Vector3(30, -30, 0);
            m_previewRenderUtility.lights[0].intensity = 2;
            PositionCamera3D(m_previewRenderUtility.camera, meshToDisplay.bounds, Vector3.zero, 6);
            m_previewRenderUtility.DrawMesh(meshToDisplay, meshTransformations, materialToDisplay, 0);
            m_previewRenderUtility.camera.Render();
            image.image = m_previewRenderUtility.EndPreview();
            aspectRatioContainer.style.height = ImageHeight;

            aspectRatioContainer.Width = desiredImageDimensions.y;
            aspectRatioContainer.Height = desiredImageDimensions.x;
            aspectRatioContainer.RegisterCallback<GeometryChangedEvent>(changedEvent =>
            {
                image.style.width = aspectRatioContainer.contentRect.width;
                image.style.height = aspectRatioContainer.contentRect.width;
            });
        }
    }
    void CreateDisplaySprite(VisualElement root, SerializedProperty property)
    {
        VisualElement spriteContainer = root.Query<VisualElement>("SpritePreviewContainer");
        Image image = spriteContainer.Query<Image>("SpritePreview");
        AspectRatioElement aspectRatioContainer = root.Query<AspectRatioElement>("SpritePreviewAspectController");
        if (aspectRatioContainer == null)
        {
            aspectRatioContainer = new AspectRatioElement();
            aspectRatioContainer.name = "SpritePreviewAspectController";
            spriteContainer.Add(aspectRatioContainer);
            aspectRatioContainer.style.marginBottom = 10;
            aspectRatioContainer.style.marginTop = 10;
            aspectRatioContainer.style.marginLeft = 10;
            aspectRatioContainer.style.marginRight = 10;
        }
        if (image == null)
        {
            image = new Image();
            image.name = "SpritePreview";
            aspectRatioContainer.Add(image);
        }
        Sprite spriteToDisplay = property.FindPropertyRelative("m_spriteForInventory").objectReferenceValue as Sprite;
        if (spriteToDisplay != null)
        {
            image.sprite = spriteToDisplay;
            aspectRatioContainer.Width = Mathf.CeilToInt(spriteToDisplay.rect.width);
            aspectRatioContainer.Height = Mathf.CeilToInt(spriteToDisplay.rect.height);
        }
        else
        {
            image.sprite = null;
        }
        aspectRatioContainer.style.height = ImageHeight;
        aspectRatioContainer.RegisterCallback<GeometryChangedEvent>(changedEvent =>
        {
            image.style.width = aspectRatioContainer.contentRect.width;
            image.style.height = aspectRatioContainer.contentRect.width;
        });
    }
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        CheckInit(property);
        VisualElement root = new VisualElement();
        m_itemCustomInspectorTree.CloneTree(root);
        VisualElement propertyHolder = root.Query<VisualElement>("Properties");
        VisualElement modelPreviewContainer = root.Query<VisualElement>("ModelPreviewContainer");
        //REMOIVED .GETcHILDREN, NOT SURE IF IS NEEDED
        foreach (SerializedProperty childProperty in property)
        {
            PropertyField fieldToAdd = new PropertyField();
            fieldToAdd.BindProperty(childProperty);
            switch (childProperty.name)
            {
                case "m_name":
                    TextField titleField = root.Query<TextField>("TitleField");
                    titleField.BindProperty(childProperty);
                    break;
                case "m_description":
                    TextField descriptionField = root.Query<TextField>("DescriptionField");
                    descriptionField.BindProperty(childProperty);
                    break;
                case "m_modelToDisplay":
                    modelPreviewContainer.Add(fieldToAdd);

                    fieldToAdd.RegisterValueChangeCallback(value =>
                    {
                        CreateModelDisplayForInspector(root, property);
                    });
                    break;
                case "m_materialToDisplay":
                    modelPreviewContainer.Add(fieldToAdd);
                    fieldToAdd.RegisterValueChangeCallback(value =>
                    {
                        CreateModelDisplayForInspector(root, property);
                    });
                    break;
                case "m_spriteForInventory":
                    VisualElement spritePreviewContainer = root.Query<VisualElement>("SpritePreviewContainer");
                    spritePreviewContainer.Add(fieldToAdd);
                    fieldToAdd.RegisterValueChangeCallback(value =>
                    {
                        CreateDisplaySprite(root, property);
                    });
                    break;
                case "m_mass":
                    FloatField floatFieldToAdd = new FloatField();
                    floatFieldToAdd.label = ObjectNames.NicifyVariableName(childProperty.name);
                    floatFieldToAdd.BindProperty(childProperty);
                    floatFieldToAdd.AddToClassList("unity-base-field__aligned");
                    floatFieldToAdd.RegisterCallback<ChangeEvent<float>>(callbackEvent =>
                    {
                        if (callbackEvent.target == floatFieldToAdd)
                        {
                            float clampedvalue = Mathf.Clamp(callbackEvent.newValue, 0.0f, float.MaxValue);
                            if (!Mathf.Approximately(clampedvalue, callbackEvent.newValue))
                            {
                                callbackEvent.StopImmediatePropagation();
                                //callbackEvent.PreventDefault();
                                floatFieldToAdd.value = clampedvalue;
                            }
                        }
                    });
                    propertyHolder.Add(floatFieldToAdd);
                    break;
                default:
                    propertyHolder.Add(fieldToAdd);
                    break;
            };

        }
        CreateDisplaySprite(root, property);
        CreateModelDisplayForInspector(root, property);
        return root;

    }


    public static void PositionCamera3D(Camera camera, Bounds bounds, Vector3 center, float distMultiplier)
    {
        float halfSize = Mathf.Max(bounds.extents.magnitude, 0.0001f);
        float distance = halfSize * distMultiplier;

        Vector3 cameraPosition = center - m_previewCameraRotation * (Vector3.forward * distance);

        camera.transform.SetPositionAndRotation(cameraPosition, m_previewCameraRotation);
        camera.nearClipPlane = distance - halfSize * 1.1f;
        camera.farClipPlane = distance + halfSize * 1.1f;
    }

    #region OnEnable OnDisable and OnDestroy event handling
    void CheckInit(SerializedProperty property)
    {
        if (init)
        {
            Enable(property);
        }
    }
    ~BC_ItemPropertyDrawer()
    {
        Destroy();
    }

    private void PlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.ExitingEditMode:
            case PlayModeStateChange.ExitingPlayMode:
                Destroy();
                break;
        }
    }

    private void SelectionChanged()
    {
        Disable();
    }
    public void Enable(SerializedProperty property)
    {
        init = false;
        EditorApplication.playModeStateChanged += PlayModeStateChanged;
        Selection.selectionChanged += SelectionChanged;
        OnEnable(property);
    }

    public void Disable()
    {
        OnDisable();
        EditorApplication.playModeStateChanged -= PlayModeStateChanged;
        Selection.selectionChanged -= SelectionChanged;
        init = true;
    }

    public void Destroy()
    {
        OnDestroy();
        EditorApplication.playModeStateChanged -= PlayModeStateChanged;
        Selection.selectionChanged -= SelectionChanged;
        init = true;
    }
    #endregion
}