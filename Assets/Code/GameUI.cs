using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TotemEntities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject scoreEffectPrefab;
    [SerializeField] private Vector3 scoreEffectVelocity;
    [SerializeField] private TextMeshProUGUI scoreField;
    [SerializeField] private Image bossFaceImage;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject victoryScreen;

    private GameConfig config;

    private List<GameObject> scoreEffects;

    public void Init(GameConfig config)
    {
        this.config = config;
        scoreEffects = new List<GameObject>();
        gameOver.SetActive(false);
        victoryScreen.SetActive(false);
    }
    
    private void Update()
    {
        foreach (var effect in scoreEffects.ToArray()) {
            effect.transform.position += Time.deltaTime * scoreEffectVelocity;
            if (effect.transform.position.magnitude > 1000f) {
                scoreEffects.Remove(effect);
                Destroy(effect);
            }
        }
    }

    public void SetScoreDisplay(int score)
    {
        scoreField.text = $"{score}";

        var bossFace = config.bossFaceStates[0].bossFace;
        foreach (var faceStates in config.bossFaceStates) {
            if (score >= faceStates.minThreshold) {
                bossFace = faceStates.bossFace;
            }
        }
        bossFaceImage.sprite = bossFace;
    }

    public void AddScoreEffect(int scoreToAdd, Vector3 position)
    {
        var scoreEffectGameObject = Instantiate(scoreEffectPrefab, position, Quaternion.identity, this.transform);
        var effectText = (scoreToAdd > 0 ? "+" : "") + scoreToAdd;
        scoreEffectGameObject.GetComponent<TextMeshProUGUI>().color = (scoreToAdd > 0 ? Color.green: Color.red);
        scoreEffectGameObject.GetComponent<TextMeshProUGUI>().text = effectText;
        scoreEffects.Add(scoreEffectGameObject);
    }

    public void ShowGameOver()
    {
        gameOver.SetActive(true);
    }

    public void ShowVictory()
    {
        victoryScreen.SetActive(true);
    }
}
