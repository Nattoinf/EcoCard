using UnityEngine;

public class Billboard : MonoBehaviour
{
    Transform cam;

    void Start()
    {
        if (Camera.main != null)
            cam = Camera.main.transform;
        else
            cam = FindObjectOfType<Camera>()?.transform;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        transform.LookAt(cam);
        transform.Rotate(0, 180, 0);
    }
}
