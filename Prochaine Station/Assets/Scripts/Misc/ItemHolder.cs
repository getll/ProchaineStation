using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void ResizeToFitItem(Collider itemCollider)
    {
        // Resize the cube to fit the held item
        transform.localScale = itemCollider.bounds.size;
    }

    public void ResetScale()
    {
        // Reset the cube to its original scale
        transform.localScale = originalScale;
    }
}
