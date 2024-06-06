using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimSceneManager : MonoBehaviour
{
    [SerializeField] Text fpstext;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fpstext.text = (1 / Time.deltaTime).ToString();
    }
}
