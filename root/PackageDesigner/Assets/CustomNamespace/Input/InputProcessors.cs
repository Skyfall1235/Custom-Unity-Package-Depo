using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CustomNamespace
{
    /// <summary>
    /// An <see cref="InputProcessor{TValue}"/> that applies a constant offset to a <see cref="Vector2"/> input value.
    /// </summary>
    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    public class OffsetVector2 : InputProcessor<Vector2>
    {
    #if UNITY_EDITOR
        /// <summary>
        /// Static constructor that registers the processor in the Unity Editor.
        /// </summary>
        static OffsetVector2()
        {
            Initialize();
        }
    #endif
        /// <summary>
        /// The offset to apply to the X component of the input <see cref="Vector2"/>.
        /// </summary>
        [Tooltip("Offset in the X direction")]
        public float xOffset;
        /// <summary>
        /// The offset to apply to the Y component of the input <see cref="Vector2"/>.
        /// </summary>
        [Tooltip("Offset in the Y direction")]
        public float yOffset;

        /// <inheritdoc />
        public override Vector2 Process(Vector2 value, InputControl control)
        {
            return new Vector2(value.x + xOffset, value.y + yOffset);
        }

        /// <summary>
        /// Registers the <see cref="OffsetVector2"/> processor with the Unity Input System.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            InputSystem.RegisterProcessor<OffsetVector2>();
        }
    }

    /// <summary>
    /// An <see cref="InputProcessor{TValue}"/> that clamps the components of a <see cref="Vector2"/> input value within specified minimum and maximum ranges.
    /// </summary>
    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    public class ClampVector2 : InputProcessor<Vector2>
    {
    #if UNITY_EDITOR
        /// <summary>
        /// Static constructor that registers the processor in the Unity Editor.
        /// </summary>
        static ClampVector2()
        {
            Initialize();
        }
    #endif
        /// <summary>
        /// The minimum allowed value for the X component of the input <see cref="Vector2"/>.
        /// </summary>
        [Tooltip("The minimum of the X value")]
        public float xMin;
        /// <summary>
        /// The maximum allowed value for the X component of the input <see cref="Vector2"/>.
        /// </summary>
        [Tooltip("The maximum of the X value")]
        public float xMax;
        /// <summary>
        /// The minimum allowed value for the Y component of the input <see cref="Vector2"/>.
        /// </summary>
        [Tooltip("The minimum of the Y value")]
        public float yMin;
        /// <summary>
        /// The maximum allowed value for the Y component of the input <see cref="Vector2"/>.
        /// </summary>
        [Tooltip("The maximum of the Y value")]
        public float yMax;

        /// <inheritdoc />
        public override Vector2 Process(Vector2 value, InputControl control)
        {
            return new Vector2(Mathf.Clamp(value.x, xMin, xMax), Mathf.Clamp(value.y, yMin, yMax));
        }

        /// <summary>
        /// Registers the <see cref="ClampVector2"/> processor with the Unity Input System.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            InputSystem.RegisterProcessor<ClampVector2>();
        }
    }
}
