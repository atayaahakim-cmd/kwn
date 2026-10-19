
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FinishGate : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (gameManager.AllMaterialsCollected())
        {
            gameManager.StartQuiz();
        }
        else
        {
            Debug.Log("Ambil semua bintang materi dahulu!");
        }
    }
}
