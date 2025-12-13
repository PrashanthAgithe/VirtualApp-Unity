using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PRPSARadioGroup : MonoBehaviour
{
    public ToggleGroup toggleGroup; // assign in inspector (PRPSA_Options ToggleGroup)
    public Toggle[] toggles;        // optional: assign toggles in order, otherwise will be discovered

    void Awake()
    {
        if (toggleGroup == null)
            toggleGroup = GetComponent<ToggleGroup>();

        if (toggles == null || toggles.Length == 0)
            toggles = GetComponentsInChildren<Toggle>();

        // Hook change events (optional)
        foreach (var t in toggles)
            t.onValueChanged.AddListener(_ => OnAnyToggleChanged());
    }

    void OnDestroy()
    {
        foreach (var t in toggles)
            t.onValueChanged.RemoveListener(_ => OnAnyToggleChanged());
    }

    public int GetSelectedIndex()
    {
        for (int i = 0; i < toggles.Length; i++)
            if (toggles[i].isOn) return i;
        return -1;
    }

    public string GetSelectedLabel()
    {
        var selected = toggles.FirstOrDefault(t => t.isOn);
        if (selected == null) return null;
        var textComp = selected.GetComponentInChildren<UnityEngine.UI.Text>(); // or TMP: TMPro.TextMeshProUGUI
        if (textComp != null) return textComp.text;
#if TMP_PRESENT
        var tmp = selected.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmp != null) return tmp.text;
#endif
        return selected.name;
    }

    public void SetSelectedIndex(int index)
    {
        if (index < 0 || index >= toggles.Length) return;
        toggles[index].isOn = true;
    }

    void OnAnyToggleChanged()
    {
        Debug.Log("Selected index: " + GetSelectedIndex() + " label: " + GetSelectedLabel());
    }
}
