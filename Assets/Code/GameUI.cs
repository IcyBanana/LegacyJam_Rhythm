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
    [SerializeField] private TextMeshProUGUI scoreField;
    
    private void Start()
    {
    }

    public void SetScore(int score)
    {
        scoreField.text = $"{score}";
    }
}
