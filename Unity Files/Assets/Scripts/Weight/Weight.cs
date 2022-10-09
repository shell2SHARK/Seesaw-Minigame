using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weight : MonoBehaviour
{
    [Header("Weight Attributes:")]
    [Range(1,99)]
    public int weightValueKG = 1;
    [Range(0.1f, 1f)]
    public float ballSize = 0.4f;
    public Color weightColor = Color.white;
    public GameObject surfaceMaterial;
    public TMPro.TextMeshPro weightText;
    
    private Rigidbody rig;

    private void Start()
    {
        rig = GetComponent<Rigidbody>();          
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        ChangeWeight();
    }

    private void ChangeWeight()
    {
        rig.mass = weightValueKG;
        surfaceMaterial.GetComponent<Renderer>().material.SetColor("_Color", weightColor);
        weightText.text = weightValueKG.ToString();
        transform.localScale = new Vector3(ballSize, ballSize, ballSize);
    }
}
