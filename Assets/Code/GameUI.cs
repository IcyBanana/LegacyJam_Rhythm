using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TotemEntities;
using UnityEngine;
using UnityEngine.Events;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject scoreEffectPrefab;
    [SerializeField] private Vector3 scoreEffectVelocity;
    [SerializeField] private TextMeshProUGUI scoreField;
    
    private List<GameObject> scoreEffects;

    private void Start()
    {
        scoreEffects = new List<GameObject>();
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
    }

    public void AddScoreEffect(int scoreToAdd, Vector3 position)
    {
        var scoreEffectGameObject = Instantiate(scoreEffectPrefab, position, Quaternion.identity, this.transform);
        var effectText = (scoreToAdd > 0 ? "+" : "") + scoreToAdd;
        scoreEffectGameObject.GetComponent<TextMeshProUGUI>().color = (scoreToAdd > 0 ? Color.green: Color.red);
        scoreEffectGameObject.GetComponent<TextMeshProUGUI>().text = effectText;
        scoreEffects.Add(scoreEffectGameObject);
    }
}
