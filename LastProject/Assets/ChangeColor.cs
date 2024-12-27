using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    private bool iscontacted;
    Renderer pointRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        iscontacted = false;
        pointRenderer = gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!iscontacted)
        {
            pointRenderer.material.color = Color.green;
        }
        else
        {
            pointRenderer.material.color = Color.red;
        }
    }
    public void contacted()
    {
        iscontacted = true;
    }
    public void uncontacted()
    {
        iscontacted = false;
    }
}
