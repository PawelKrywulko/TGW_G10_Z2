using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISticker : MonoBehaviour
{
    [SerializeField] private Text label;

    // Update is called once per frame
    void Update()
    {
        label.transform.position = transform.position;
    }

    public void AssignLabel(Text labelToAssign)
    {
        label = labelToAssign;
    }
}
