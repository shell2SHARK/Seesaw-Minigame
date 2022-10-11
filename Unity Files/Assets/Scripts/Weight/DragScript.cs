using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragScript : MonoBehaviour
{
    // Identifica os scripts da gangorra e dos pesos
    private Seesaw seesawScript;
    private Weight weightScript;
    // Variaveis de controle para coordenadas do mouse
    private Vector3 mOffset;
    private float mZCoord;
    // Variavel para identificar a posicao inicial do peso
    private Vector3 initialPos;
    // Controle de quando o peso esta dentro da gangorra ou nao
    private bool isInBucket;
    private Rigidbody rig;

    private void Start()
    {
        rig = GetComponent<Rigidbody>();
        weightScript = GetComponent<Weight>();
        seesawScript = GameObject.FindWithTag("Seesaw").GetComponent<Seesaw>();

        // Define a posicao inicial
        initialPos = transform.position;        
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 10f, Color.red);
    }

    private void OnMouseDown()
    {
        // Converte a posicao do objeto para posicao na tela 2D
        // Serve para manter a coordenada Z do objeto inalterada
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        // Define a diferenca de posicao entre o mouse e o objeto
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();       
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        // Coordenadas em pixel do moouse (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // Coordenada Z do objeto na tela
        mousePoint.z = mZCoord;

        // Converte a posicao do mouse para posicao global no mundo
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {
        // Define a posicao do objeto para a do mouse
        transform.position = GetMouseAsWorldPoint() + mOffset;
        rig.velocity = Vector3.zero;
        gameObject.layer = 6;
        Physics.IgnoreLayerCollision(0, 6, true);
    }

    private void RayWithMouse()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray,out hit))
        {
            print(hit.collider.name);
        }
    }

    private void RayWithObject()
    {
        // Cria o raycast a partir do objeto
        // Se caso o raycast tocar no objeto Releaser, o peso muda para a posiçao da gangorra
        RaycastHit hit;

        if(Physics.Raycast(transform.position,transform.forward * 10f,out hit))
        {
            if(hit.collider.tag == "Releaser" && !isInBucket)
            {
                rig.position = hit.collider.transform.position;
                rig.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                seesawScript.actualPlayerWeight += Mathf.FloorToInt(weightScript.weightValueKG * 100f);
                isInBucket = true;
                Destroy(this);
            }           
        }
        else
        {
            // Senao o peso retorna a posicao original que nasceu
            rig.position = initialPos;
            rig.freezeRotation = true;
            transform.rotation = Quaternion.identity;

            // Diminui o peso da gangorra quando o peso for solto
            if (isInBucket)
            {               
                seesawScript.actualPlayerWeight -= Mathf.FloorToInt(weightScript.weightValueKG * 100f);
                isInBucket = false;
            }                       
        }
    }

    private void OnMouseUp()
    {
        RayWithObject();
        gameObject.layer = 0;
        Physics.IgnoreLayerCollision(0, 6, false);
    }
}