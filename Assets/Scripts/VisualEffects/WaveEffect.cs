using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class WaveEffect : MonoBehaviour
{
    [Header("Text Wave")]
    [SerializeField] private TMP_Text textTarget;
    [SerializeField] private bool animateText = true;
    [SerializeField] private float textAmplitude = 10f;
    [SerializeField] private float textFrequency = 2f;
    [SerializeField] private float textPhaseOffset = 0.35f;

    [Header("Object Wave")]
    [SerializeField] private Transform[] objectTargets;
    [SerializeField] private bool animateObjects = true;
    [SerializeField] private bool useChildrenIfNoObjectsAssigned = true;
    [SerializeField] private float objectAmplitude = 0.25f;
    [SerializeField] private float objectFrequency = 2f;
    [SerializeField] private float objectPhaseOffset = 0.35f;
    [SerializeField] private Vector3 objectWaveDirection = Vector3.up;

    private readonly List<ObjectWaveState> _objectStates = new();
    
    // NEW: We use a Dictionary to remember the true starting position of each object
    private readonly Dictionary<Transform, Vector3> _initialPositions = new();

    private struct ObjectWaveState
    {
        public Transform Transform;
        public Vector3 InitialLocalPosition;
        public float Phase;
    }

    private void Awake()
    {
        if (textTarget == null) TryGetComponent(out textTarget);
        RefreshObjectCache();
    }

    private void OnEnable()
    {
        if (textTarget == null) TryGetComponent(out textTarget);
        RefreshObjectCache();
    }

    private void OnDisable()
    {
        // NEW: Restore all objects to their original base positions when disabled
        // so they don't get permanently stuck mid-wave.
        foreach (var kvp in _initialPositions)
        {
            if (kvp.Key != null)
            {
                kvp.Key.localPosition = kvp.Value;
            }
        }
    }

    private void OnValidate()
    {
        if (textTarget == null) TryGetComponent(out textTarget);
        RefreshObjectCache();
    }

    private void OnTransformChildrenChanged()
    {
        RefreshObjectCache();
    }

    private void LateUpdate()
    {
        if (animateText && textTarget != null)
        {
            AnimateTextWave();
        }

        if (animateObjects)
        {
            AnimateObjectWave();
        }
    }

    private void RefreshObjectCache()
    {
        _objectStates.Clear();

        // 1. Gather all targets we intend to animate
        List<Transform> currentTargets = new List<Transform>();

        if (objectTargets != null && objectTargets.Length > 0)
        {
            for (int i = 0; i < objectTargets.Length; i++)
            {
                Transform target = objectTargets[i];
                if (target != null && target != transform)
                {
                    currentTargets.Add(target);
                }
            }
        }
        else if (useChildrenIfNoObjectsAssigned)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child != null)
                {
                    currentTargets.Add(child);
                }
            }
        }

        // 2. Clean up our initial positions cache
        // If an object was removed from the list, restore its position and forget it.
        List<Transform> trackedTransforms = new List<Transform>(_initialPositions.Keys);
        foreach (Transform trackedTarget in trackedTransforms)
        {
            if (trackedTarget == null)
            {
                _initialPositions.Remove(trackedTarget);
            }
            else if (!currentTargets.Contains(trackedTarget))
            {
                // Restore its original position so it doesn't get left dangling mid-air
                trackedTarget.localPosition = _initialPositions[trackedTarget];
                _initialPositions.Remove(trackedTarget);
            }
        }

        // 3. Build our wave states safely
        for (int i = 0; i < currentTargets.Count; i++)
        {
            Transform target = currentTargets[i];

            // Only grab the localPosition if we haven't tracked it yet.
            // This prevents "position drifting" when tweaking variables in the inspector!
            if (!_initialPositions.ContainsKey(target))
            {
                _initialPositions[target] = target.localPosition;
            }

            _objectStates.Add(new ObjectWaveState
            {
                Transform = target,
                InitialLocalPosition = _initialPositions[target],
                Phase = i * objectPhaseOffset
            });
        }
    }

    private void AnimateTextWave()
    {
        textTarget.ForceMeshUpdate();

        TMP_TextInfo textInfo = textTarget.textInfo;
        if (textInfo.characterCount == 0) return;

        float time = Application.isPlaying ? Time.time : Time.unscaledTime;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo characterInfo = textInfo.characterInfo[i];
            if (!characterInfo.isVisible) continue;

            int materialIndex = characterInfo.materialReferenceIndex;
            int vertexIndex = characterInfo.vertexIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            float wave = Mathf.Sin((time * textFrequency) + (i * textPhaseOffset));
            Vector3 offset = Vector3.up * (wave * textAmplitude);

            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textTarget.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    private void AnimateObjectWave()
    {
        if (_objectStates.Count == 0) return;

        Vector3 waveDirection = objectWaveDirection.sqrMagnitude > 0.0001f
            ? objectWaveDirection.normalized
            : Vector3.up;

        float time = Application.isPlaying ? Time.time : Time.unscaledTime;

        for (int i = 0; i < _objectStates.Count; i++)
        {
            ObjectWaveState state = _objectStates[i];
            if (state.Transform == null) continue;

            float wave = Mathf.Sin((time * objectFrequency) + state.Phase);
            // Apply the wave offset on top of the securely cached initial position
            state.Transform.localPosition = state.InitialLocalPosition + (waveDirection * (wave * objectAmplitude));
        }
    }
}