using UnityEngine;
using System.Collections;

public class RandomColor : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        SetColor();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            SetColor();
        }
    }
#endif

    private void SetColor()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.materials[0].SetColor("_Color", new Color(Random.value, Random.value, Random.value));
    }
}
