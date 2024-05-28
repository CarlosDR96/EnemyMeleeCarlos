using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : EnemyDamage
{
    [SerializeField] private GameObject fireballPrefab; // Prefab de la bola de fuego
    [SerializeField] private Transform firePoint; // Punto desde donde se disparan las bolas de fuego
    [SerializeField] private float fireRate = 1f; // Velocidad de disparo (bolas por segundo)
    [SerializeField] private float fireballSpeed = 5f; // Velocidad de la bola de fuego
    [SerializeField] private float visionRange = 10f; // Rango de visión del enemigo
    [SerializeField] private float attackRange = 2f; // Rango de ataque del enemigo
    [SerializeField] private Animator animator; // Referencia al componente Animator
    [SerializeField] private Transform[] patrolPoints; // Puntos de patrullaje
    [SerializeField] private float patrolSpeed = 2f; // Velocidad de patrullaje

    private float nextFireTime = 0f;
    private bool isAttacking = false;
    private bool isPatrolling = true;
    private int currentPatrolIndex = 0;
    private Transform playerTransform;

    void Update()
    {
        if (isPatrolling)
        {
            Patrol();
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, visionRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                playerTransform = hit.transform;
                isPatrolling = false;
                animator.SetBool("isWalking", false); // Detener animación de caminar
                float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
                if (distanceToPlayer <= attackRange && !isAttacking)
                {
                    StartCoroutine(AttackPlayer(playerTransform));
                }
                else if (Time.time >= nextFireTime)
                {
                    ShootFireball(playerTransform);
                    nextFireTime = Time.time + 1f / fireRate;
                }
                FlipTowardsPlayer(playerTransform); // Voltear hacia el jugador
            }
        }
    }

    private void Patrol()
    {
        Transform targetPoint = patrolPoints[currentPatrolIndex];
        animator.SetBool("isWalking", true); // Activar animación de caminar

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

        Vector2 direction = (targetPoint.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);
        FlipTowardsPatrolPoint(targetPoint); // Voltear hacia el punto de patrullaje
    }

    private IEnumerator AttackPlayer(Transform player)
    {
        isAttacking = true;
        animator.SetTrigger("attack"); // Activar la animación de ataque

        // Esperar el tiempo que dura la animación de ataque
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Verificar si el jugador aún está en el rango y en contacto
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<Health>().TakeDamage(damage); // Hacer daño al jugador
            }
        }

        isAttacking = false;
    }

    private void ShootFireball(Transform target)
    {
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
        fireball.SetActive(true); // Asegurarse de que la bola de fuego esté activada
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        Vector2 direction = (target.position - firePoint.position).normalized;
        rb.velocity = direction * fireballSpeed;
    }

    private void FlipTowardsPlayer(Transform player)
    {
        Vector3 scale = transform.localScale;
        if ((player.position.x > transform.position.x && scale.x < 0) || (player.position.x < transform.position.x && scale.x > 0))
        {
            scale.x *= -1; // Invertir la escala en el eje X para voltear el sprite
        }
        transform.localScale = scale;
    }

    private void FlipTowardsPatrolPoint(Transform patrolPoint)
    {
        Vector3 scale = transform.localScale;
        if ((patrolPoint.position.x > transform.position.x && scale.x < 0) || (patrolPoint.position.x < transform.position.x && scale.x > 0))
        {
            scale.x *= -1; // Invertir la escala en el eje X para voltear el sprite
        }
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        // Dibujar el campo de visión en la escena
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // Dibujar el rango de ataque en la escena
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
