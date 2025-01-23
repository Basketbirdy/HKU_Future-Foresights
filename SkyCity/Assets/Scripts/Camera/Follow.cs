using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private bool late;


    private void Update()
    {
        if (late) { return; }

        if(transform.position != target.position) { transform.position = target.position; }
        if(transform.rotation != target.rotation) { transform.rotation = target.rotation; }
    }

    private void LateUpdate()
    {
        if (!late) { return; }

        if (transform.position != target.position) { transform.position = target.position; }
        if (transform.rotation != target.rotation) { transform.rotation = target.rotation; }
    }
}
