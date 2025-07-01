using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactText;
    private MeshRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void Interact(Transform interactorTransform)
    {
        Material material = _renderer.material;

        material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
    }

    public string GetInteractText()
    {
        return interactText;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool IsInteractable()
    {
        return true;
    }
}

/* References:
 * 
 *  - https://www.youtube.com/watch?v=LdoImzaY6M4&t=811s&ab_channel=CodeMonkey
 */
