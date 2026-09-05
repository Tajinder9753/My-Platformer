using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float value;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            Debug.Log("Adding coin to player");
            Destroy(gameObject);
        }
    }
}
