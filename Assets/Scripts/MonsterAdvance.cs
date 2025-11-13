using UnityEngine;

public class MonsterAdvance : MonoBehaviour
{
    public Transform target;   // assign Mom
    public float speed = 2f;
    private bool moving;

    void Update()
    {
        if (!moving || target == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );
    }

    // Called from Timeline via Signal
    public void StartAdvance()
    {
        moving = true;
    }
}
