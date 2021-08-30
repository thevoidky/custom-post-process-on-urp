using CustomPostProcess.Runtime;
using UnityEngine;

public class CustomPostProcessViaScript : MonoBehaviour
{
    private CustomPostProcessAdapter _adapter = new CustomPostProcessAdapter();

    private bool _isOn;

    private void Awake()
    {
        // You can set target as CustomPostProcessSource
        _adapter.Target = Camera.main.GetComponent<CustomPostProcessSource>();

        // Set feature 0, if you want to set multiple feature 2 and 5, you can set as this; FeatureMask = 1 << 2 | 1 << 5;
        _adapter.FeatureMask = 1 << 0;

        _adapter.DurationToOn = 0.5f;
        _adapter.DurationToOff = 0.2f;

        _adapter.ValueAtOn = 1f;
        _adapter.ValueAtOff = 0f;
    }

    private void OnGUI()
    {
        using var area = new GUILayout.AreaScope(new Rect(50f, 50f, Screen.width - 100f, Screen.height - 100f));

        var boxStyle = new GUIStyle("Box") { alignment = TextAnchor.MiddleCenter, fixedHeight = 80f };
        GUILayout.Box($"{(_isOn ? "Active" : "Inactive")}", boxStyle);

        if (GUILayout.Button("Toggle", GUILayout.Height(80f)))
        {
            _adapter.Activate(_isOn = !_isOn);
        }
    }
}