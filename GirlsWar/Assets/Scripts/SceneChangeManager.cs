using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeManager : MonoBehaviour
{
    public void ObjectDestroy()
    {
        var gManager = gameObject.GetComponent<GameManager>();
        Destroy(gameObject);
        //gManager.Invoke("Init", 1f);
        //gManager.Init();
    }
}
