using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Controller",menuName = "Educarte/Level Controller", order = 1)]
public class LevelAttributes: ScriptableObject
{
    // Controla o nivel atual da fase
    public int level = 1;
    // Controla as chances atuais do jogador na fase
    public int chances = 5;
    // Controla quando o game começou
    public bool gameStarted = false;

    [System.Serializable]
    public class PlayerWeightsValues
    {
        // Valores de peso que o player terá acesso
        [Range(10, 99)]
        public int weightValueToPlayer = 1;
        [Range(0.1f, 1f)]
        public float weightSizeToPlayer = 0.4f;        
        public Color weightColorToPlayer = Color.white;
    }

    [System.Serializable]
    public class StageWeightValues
    {
        // Valores do desafio que o player terá que resolver
        // A escala de grandeza em Kilos comeca de 10 até 99
        [Range(10, 99)]
        public int weightValueToSolve = 10;
        [Range(0.1f, 1f)]
        public float weightSizeToSolve = 0.4f;       
        public Color weightColorToSolve = Color.white;
    }

    [System.Serializable]
    public class LevelValues
    {
        // Classes designadas para setar os atributos do minigame
        [Header("Set the Player's Weights on Stage:")]
        public PlayerWeightsValues[] playerWeightsAttributes;
        [Header("Set the Stage's Weights on Stage:")]
        public StageWeightValues stageWeightAttributes;
    }

    public LevelValues[] levelValues;
}
