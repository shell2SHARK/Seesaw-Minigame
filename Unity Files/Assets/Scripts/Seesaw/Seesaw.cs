using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Seesaw : MonoBehaviour
{
    // Peso atual do lado do player
    public int actualPlayerWeight = 0;
    // Peso total da fase
    [SerializeField]
    private int stageObjectWeight = 10;
    // Cor do peso da fase
    [SerializeField]
    private Color stageObjectColor = Color.white;
    // Tamanho do peso da fase
    [SerializeField]
    private float stageObjectSize = 10;
    // Estagio atual
    [SerializeField]
    private int actualStage = 1;

    // Atributos para o minigame 
    // O scriptable object e o responsavel por controlar os pesos de cada desafio
    // Para adicionar mais estagios basta ir ao scriptable object do minigame
    [Header("Scriptable Object:")]
    public LevelAttributes levelControlSO;
    [Header("Weight Prefab:")]
    public GameObject weightPrefab;
    [Header("Text Weights:")]
    public TMPro.TextMeshProUGUI stageWeightText;
    public TMPro.TextMeshProUGUI playerWeightText;
    [Header("Spawn Points:")]
    public GameObject spawnLeft;
    public GameObject spawnRigth;
    public GameObject floorSpawner;
    [Header("Position to Start in X Axis:")]
    public float XpositionToStart = -1f;
    [Header("HUD Slider:")]
    public Slider weightMetricSlider;

    private void Start()
    {
        // Coleta as informacoes de cor, tamanho e peso do scriptable object para iniciar o desafio
        stageObjectWeight = levelControlSO.levelValues[actualStage-1].stageWeightAttributes.weightValueToSolve;
        stageObjectColor = levelControlSO.levelValues[actualStage-1].stageWeightAttributes.weightColorToSolve;
        stageObjectSize = levelControlSO.levelValues[actualStage - 1].stageWeightAttributes.weightSizeToSolve;
        SpawnWeightToSolve();
        SpawnWeightToPlayer();
    }

    private void Update()
    {
        StageController();
        // Indica ao jogador o ponto de equilibrio da gangorra com o objeto Slider
        weightMetricSlider.maxValue = stageObjectWeight;
        weightMetricSlider.value = Mathf.Lerp(weightMetricSlider.value, actualPlayerWeight/2, 0.1f);
    }

    private void StageController()
    {
        // Mostra o peso para o jogador na tela
        stageWeightText.text = stageObjectWeight.ToString() + " KG";
        playerWeightText.text = actualPlayerWeight.ToString() + " KG";
    }

    private void SpawnWeightToSolve()
    {
        // Instancia o peso principal na cena
        GameObject SolveWeight = Instantiate(weightPrefab, spawnRigth.transform.position, Quaternion.identity);
        Weight weightScript = SolveWeight.GetComponent<Weight>();
        DragScript dragScript = SolveWeight.GetComponent<DragScript>();

        // Define os valores do desafio para o peso da cena
        // O valor em Kilos recebido do Scriptable é convertido para uma escala de 0.1 até 0.9
        // Cada decimal representa um valor inteiro do peso (0.1 = 10kg, 0.2 = 20kg, etc)
        // Isso se deve ao fato de valores de massa muito grandes pode ocorrer problemas de colisao
        weightScript.weightColor = stageObjectColor;
        weightScript.weightValueKG = (stageObjectWeight / 10f) / 10f;
        weightScript.ballSize = stageObjectSize;

        // Desativa o script de drag para o player nao conseguir arrastar o peso do desafio
        Destroy(dragScript);
    }

    private void SpawnWeightToPlayer()
    {
        // Identifica a classe responsavel pelos valores do Player
        LevelAttributes.PlayerWeightsValues[] allPlayerData = levelControlSO.levelValues[actualStage - 1].playerWeightsAttributes;
       
        // Para cada valor definido dentro da classe, um peso e criado
        for (int i = 0; i < allPlayerData.Length; i++)
        {
            // Usa o spawner como posicionador dos pesos para o player
            Vector3 randomPos = new Vector3(XpositionToStart, floorSpawner.transform.position.y, floorSpawner.transform.position.z);
            // Instancia e coleta o script do peso criado
            GameObject playerWeight = Instantiate(weightPrefab, randomPos, Quaternion.identity);
            Weight weightScript = playerWeight.GetComponent<Weight>();

            // Define os valores dos pessos do jogador
            // O valor em Kilos recebido do Scriptable é convertido para uma escala de 0.1 até 0.9
            // Cada decimal representa um valor inteiro do peso (0.1 = 10kg, 0.2 = 20kg, etc)
            // Isso se deve ao fato de valores de massa muito grandes pode ocorrer problemas de colisao
            weightScript.weightColor = allPlayerData[i].weightColorToPlayer;
            weightScript.weightValueKG = (allPlayerData[i].weightValueToPlayer / 10f) / 10f;
            weightScript.ballSize = allPlayerData[i].weightSizeToPlayer;

            // Aumenta o espacamento entre um peso e outro para que eles nao sejam criados na mesma posicao
            XpositionToStart += 1f;
        }        
    }
}
