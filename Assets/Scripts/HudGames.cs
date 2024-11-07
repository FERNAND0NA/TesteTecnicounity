using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class HudGames : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] TMP_Text pointsText;  // Referência ao TextMeshPro para exibir os pontos
    [SerializeField] Slider progressSlider; // Referência à barra de progresso

    private int points = 0;
    private int maxPoints = 100;

    void Start()
    {
        UpdatePointsUI();
        UpdateProgressBar();
    }

    public void TriggerActivated() // Função chamada ao acionar o trigger
    {
        AddPoints(5);
    }

    private void AddPoints(int amount)
    {
        points += amount;
        if (points > maxPoints) points = maxPoints;
        UpdatePointsUI();
        UpdateProgressBar();
    }

    private void UpdatePointsUI()
    {
        pointsText.text = $"{points}/{maxPoints}"; // Exibe os pontos no formato "X/100"
    }

    private void UpdateProgressBar()
    {
        progressSlider.value = (float)points / maxPoints; // Atualiza a barra de progresso
    }
}