using UnityEngine;
using UnityEngine.InputSystem;

public class player_movement : MonoBehaviour
{
    Animator anim ;
    float speed = 20f;
    Rigidbody2D rb ;
    Vector2 movement;
    bool facingRight = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>() ;
        movement = transform.position ;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 position = transform.position;
    
        //A->left
        //D->right
        //S->down
        //W->up

        if (Keyboard.current.aKey.isPressed)
        {
            if (facingRight)
{
    Flip();
}
            anim.SetBool("isWalking", true) ;
            position -= new Vector2(speed*Time.deltaTime, 0);
            movement = position;
            
        }

        else if (Keyboard.current.wKey.isPressed)
        {
            anim.SetBool("isWalking", true) ;
            position += new Vector2(0, speed*Time.deltaTime);
            movement = position ;
        }

        else if (Keyboard.current.dKey.isPressed)
        {
            if (!facingRight)
{
    Flip();
}
            anim.SetBool("isWalking", true) ;
            position += new Vector2(speed*Time.deltaTime, 0);
            movement = position ;
        }

        else if (Keyboard.current.sKey.isPressed)
        {
            anim.SetBool("isWalking", true) ;
            position -= new Vector2(0, speed*Time.deltaTime);
            movement = position ;
        }

        else
        {
            anim.SetBool("isWalking", false) ;
            movement = rb.position;
        }

    }

    void FixedUpdate()
    {
        rb.MovePosition(movement);
    }
    void Flip()
{
    facingRight = !facingRight;

    Vector3 localScale = transform.localScale;
    localScale.x *= -1;
    transform.localScale = localScale;
}
}