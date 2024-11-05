using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchAndStack : MonoBehaviour
{
    public Transform stackPoint; // Ponto onde os personagens serão empilhados nas costas
    public float stackOffsetY = 1.0f; // Altura entre personagens empilhados
    public Transform moneyZone; // Área onde personagens empilhados serão lançados
    public int moneyPerCharacter = 10; // Quantidade de dinheiro por personagem empilhado

    private List<GameObject> stackedCharacters = new List<GameObject>();
    private int money = 0;

    public void Punch(GameObject target)
    {
        // Código para soco...

        // Empilha o personagem atingido
        StackCharacter(target);
    }

    void StackCharacter(GameObject character)
    {
        // Desabilita componentes de movimento do personagem para fixá-lo
        character.GetComponent<Rigidbody>().isKinematic = true;
        character.GetComponent<Collider>().enabled = false;

        // Define a posição do personagem nas costas do jogador
        Vector3 stackPosition = stackPoint.position + Vector3.up * stackOffsetY * stackedCharacters.Count;
        character.transform.position = stackPosition;
        character.transform.parent = stackPoint;

        // Adiciona o personagem à lista de empilhados
        stackedCharacters.Add(character);
    }

    public void ThrowStackedCharacters()
    {
        foreach (GameObject character in stackedCharacters)
        {
            // Remove o personagem da hierarquia do jogador e lança em direção à moneyZone
            character.transform.parent = null;
            Vector3 direction = (moneyZone.position - character.transform.position).normalized;
            character.GetComponent<Rigidbody>().isKinematic = false;
            character.GetComponent<Rigidbody>().AddForce(direction * 5.0f, ForceMode.Impulse);

            // Adiciona dinheiro
            money += moneyPerCharacter;
        }

        // Limpa a lista
        stackedCharacters.Clear();

        Debug.Log("Dinheiro total: " + money);
    }

    void Update()
    {
        // Exemplo: Pressione "E" para jogar os personagens empilhados
        if (Input.GetKeyDown(KeyCode.E))
        {
            ThrowStackedCharacters();
        }
    }
}
