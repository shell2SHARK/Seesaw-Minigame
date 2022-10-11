using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weight : MonoBehaviour
{
    // Atributos necessarios para o peso funcionar
    [Header("Weight Attributes:")]
    [Range(0.1f, 1f)]
    public float weightValueKG = 0.1f;
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
        // Muda a massa do objeto de acordo com o que é recebido do Seesaw script
        rig.mass = weightValueKG;
        // Troca a cor do peso
        surfaceMaterial.GetComponent<Renderer>().material.SetColor("_Color", weightColor);
        // Muda o valor do texto para o valor inteiro em KG
        weightText.text = (weightValueKG * 100).ToString();
        // Muda o tamanho do objeto
        transform.localScale = new Vector3(ballSize, ballSize, ballSize);
    }
}
