using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class TriggerPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Certifique-se de que o player tem a tag "Player"
        {
            HudGames hudGames = FindObjectOfType<HudGames>(); // Encontra o HudGames na cena
            if (hudGames != null)
            {
                hudGames.TriggerActivated(); // Aciona a função para adicionar pontos no HudGames
            }
        }
    }
}
