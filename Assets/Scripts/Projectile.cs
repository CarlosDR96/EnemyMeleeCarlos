using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : EnemyDamage
{
    [SerializeField] private float speed;

    private bool hit;
    private Animator anim;
    private BoxCollider2D collider;
    private float direction;
    private float lifeTime;

    // Start is called before the first frame update
    private void Awake()
    {
        anim = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hit)
        {
            return;
        }

        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0 ,0);

        lifeTime += Time.deltaTime;
        if (lifeTime > 5)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Health>().TakeDamage(damage);
        }

        hit = true;
        collider.enabled = false;
        if (anim != null)
        {
            anim.SetTrigger("explode");
        }
        else
        {
            // Si no hay animador, desactivar el proyectil inmediatamente
            Deactivate();
        }
    }

    public void SetDirection(float direction)
    {
        lifeTime = 0.0f;
        this.direction = direction;
        gameObject.SetActive(true);
        collider.enabled = true;
        hit = false;

        float localScaleX = transform.localScale.x;
        // Flip del sprite según la dirección
        if (direction == -1 && localScaleX > 0 || direction == 1 && localScaleX < 0)
        {
            transform.localScale = new Vector3(-localScaleX, transform.localScale.y, transform.localScale.z);
        }
        else if (direction == 1 && localScaleX < 0)
        {
            transform.localScale = new Vector3(-localScaleX, transform.localScale.y, transform.localScale.z);
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
