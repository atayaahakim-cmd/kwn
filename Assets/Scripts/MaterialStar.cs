
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MaterialStar : MonoBehaviour
{
    public MaterialData materialData;
    public GameManager gameManager;

    private bool collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag("Player"))
            return;

        collected = true;
        gameManager.CollectMaterial(materialData);
        gameObject.SetActive(false);
    }
}
