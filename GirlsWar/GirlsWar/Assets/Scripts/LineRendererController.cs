using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // LineRendererコンポーネントをゲームオブジェクトにアタッチする
        var lineRenderer = gameObject.AddComponent<LineRenderer>();

        var material = Resources.Load("Materials/DotLine");

        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.numCornerVertices = 1;
        lineRenderer.material = (Material)material;

        var positions = new Vector3[]
        {
            new Vector3(0, 0, 0),               // 開始点
            //new Vector3(4, 0, 0),               // 中継点
            new Vector3(4, 0, 0),              // 終了点
        };

        //lineRenderer.startWidth = 0.1f;                   // 開始点の太さを0.1にする
        //lineRenderer.endWidth = 0.1f;                     // 終了点の太さを0.1にする

        // 点の数を指定する
        lineRenderer.positionCount = positions.Length;

        // 線を引く場所を指定する
        lineRenderer.SetPositions(positions);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
