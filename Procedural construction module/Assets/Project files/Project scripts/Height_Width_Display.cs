using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Height_Width_Display : MonoBehaviour
{
    //[SerializeField] RectTransform canvas_to_display_measurements;
    [SerializeField] TMP_Text Height;
    [SerializeField] TMP_Text Width;
    void Start()
    {
        
    }

    
    void Update()
    {
        // canvas_to_display_measurements.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        Height.text = "<------" + transform.localScale.y.ToString("F2") + "m" + "------>";
        Width.text = "<------" + transform.localScale.x.ToString("F2") + "m" + "------>";
    }
}
