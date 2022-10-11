using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Seesaw : MonoBehaviour
{
    // Peso atual do lado do player
    public int actualPlayerWeight = 0;
    // Peso total da fase
    private int stageObjectWeight = 10;
    // Cor do peso da fase
    private Color stageObjectColor = Color.white;
    // Tamanho do peso da fase
    private float stageObjectSize = 10;
    // Controlador de quando o desafio for concluido
    private bool challengeDone;

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
    public TMPro.TextMeshProUGUI stageMark;
    public TMPro.TextMeshProUGUI chanceMark;

    [Header("Spawn Points:")]
    public GameObject spawnLeft;
    public GameObject spawnRigth;
    public GameObject floorSpawner;

    [Header("Position to Start in X Axis:")]
    public float XpositionToStart = -1f;

    [Header("HUD Slider:")]
    public Slider weightMetricSlider;

    [Header("Custom Events:")]
    public UnityEvent OnStart; // Quando finalizar o minigame
    public UnityEvent OnWin; // Quando finalizar o minigame
    public UnityEvent OnLose;// Quando perder o minigame
    public UnityEvent OnProceed; // Quando passar de fase no minigame

    private void Start()
    {
        // Coleta as informacoes de cor, tamanho e peso do scriptable object para iniciar o desafio
        stageObjectWeight = levelControlSO.levelValues[levelControlSO.level].stageWeightAttributes.weightValueToSolve;
        stageObjectColor = levelControlSO.levelValues[levelControlSO.level].stageWeightAttributes.weightColorToSolve;
        stageObjectSize = levelControlSO.levelValues[levelControlSO.level].stageWeightAttributes.weightSizeToSolve;

        // Instancia o peso do desafio
        SpawnWeightToSolve();

        // Instancia os pesos do jogador
        SpawnWeightToPlayer();

        // Inicia o metodo OnStart caso o jogo ja tenha sido executado
        if (levelControlSO.gameStarted)
        {
            OnStart.Invoke();
        }
        else
        {
            levelControlSO.gameStarted = true;
        }
    }

    private void Update()
    {
        // Metodo que controla as informacoes do jogo
        StageController();

        // Indica ao jogador o ponto de equilibrio da gangorra com o objeto Slider
        weightMetricSlider.maxValue = stageObjectWeight;
        weightMetricSlider.value = Mathf.Lerp(weightMetricSlider.value, actualPlayerWeight/2.0f, 0.1f);
    }

    private void StageController()
    {
        // Mostra informacoes para o jogador na tela
        stageWeightText.text = stageObjectWeight.ToString() + " KG";
        playerWeightText.text = actualPlayerWeight.ToString() + " KG";
        stageMark.text = "Fase " + (levelControlSO.level + 1).ToString();
        chanceMark.text = "Chances\n" + levelControlSO.chances;ToString();

        // Se o peso do player for o mesmo do desafio ele passa de nivel
        if(stageObjectWeight == actualPlayerWeight && !challengeDone)
        {
            OnProceed.Invoke();
            StartCoroutine(ChangeLevel());
            challengeDone = true;
        }
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
        LevelAttributes.PlayerWeightsValues[] allPlayerData = levelControlSO.levelValues[levelControlSO.level].playerWeightsAttributes;
       
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

    // Metodo para Event Buttons
    // Reseta o jogo quando pressionar o botao Reset
    public void ResetGame()
    {      
        if(levelControlSO.chances > 0)
        {
            // Diminui uma chance do jogador no Scriptable
            levelControlSO.chances -= 1;
            RestartLevel();
        }
        else
        {
            // Caso acabe as chances invoca os eventos em OnLose
            OnLose.Invoke();
        }
    }

    public void NextLevel()
    {
        // Se ainda existe niveis o jogador procede
        if(levelControlSO.level < levelControlSO.levelValues.Length - 1)
        {
            levelControlSO.level += 1;
            levelControlSO.chances = 5;
            RestartLevel();
        }
        else
        {
            // Senao existe mais niveis para jogar invoca os eventos em OnWin
            OnWin.Invoke();
        }
    }

    // Metodo para Event Buttons
    // Reinicia o game quando pressionar o botao de continuar
    public void RestartNewGame()
    {
        // Reseta os valores do jogo no Scriptable
        levelControlSO.level = 0;
        levelControlSO.chances = 5;
        RestartLevel();
    }

    IEnumerator ChangeLevel()
    {
        // Aguarda x segundos para avancar outro nivel
        yield return new WaitForSeconds(5f);
        NextLevel();
    }

    private void RestartLevel()
    {
        // Reinicia a cena atual
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}