using UnityEngine;

public class Line : MonoBehaviour
{
    public Vector3 P0;
    public Vector3 P1;

	private void Reset()
    {
        P0 = new Vector3(0f, 0f, 0f);
        P1 = new Vector3(1f, 1f, 0f);
    }
}
