using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    private Vector2 direction;
    private Rigidbody2D rb;
    private Quaternion _startRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _startRotation = transform.rotation;
    }
    void Update()
    {
        direction.x = Input.GetAxis("Horizontal");
        direction.y = Input.GetAxis("Vertical");
        transform.rotation = _startRotation;
    }
    private void FixedUpdate()//Движения героя
    {
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        transform.rotation = _startRotation;
    }
}
