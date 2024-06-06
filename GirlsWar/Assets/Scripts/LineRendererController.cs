using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // LineRenderer�R���|�[�l���g���Q�[���I�u�W�F�N�g�ɃA�^�b�`����
        var lineRenderer = gameObject.AddComponent<LineRenderer>();

        var material = Resources.Load("Materials/DotLine");

        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.numCornerVertices = 1;
        lineRenderer.material = (Material)material;

        var positions = new Vector3[]
        {
            new Vector3(0, 0, 0),               // �J�n�_
            //new Vector3(4, 0, 0),               // ���p�_
            new Vector3(4, 0, 0),              // �I���_
        };

        //lineRenderer.startWidth = 0.1f;                   // �J�n�_�̑�����0.1�ɂ���
        //lineRenderer.endWidth = 0.1f;                     // �I���_�̑�����0.1�ɂ���

        // �_�̐����w�肷��
        lineRenderer.positionCount = positions.Length;

        // ���������ꏊ���w�肷��
        lineRenderer.SetPositions(positions);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
