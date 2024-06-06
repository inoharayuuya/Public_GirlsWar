using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTest : MonoBehaviour
{
    [SerializeField] Text[] tmp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tmp[0].text = Screen.currentResolution.width.ToString();
        tmp[1].text = Screen.currentResolution.height.ToString();
    }
}
