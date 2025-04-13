using System.Collections;
using UnityEngine;

public class NPCWander : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private float waitDuration = 2f;
    [SerializeField] private float turnSpeed = 120f;
    [SerializeField] private Animator animator;

    private static readonly int IsWalking = Animator.StringToHash("IsWalking");

    //public bool isMoving = false;

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        StartCoroutine(WanderRoutine());
    }

    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            // Wait before moving
            yield return new WaitForSeconds(waitDuration);

            // Rotate to a random direction
            float randomAngle = Random.Range(0f, 360f);
            Quaternion targetRotation = Quaternion.Euler(0f, randomAngle, 0f);

            while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
                yield return null;
            }

            // Start walking forward
            animator.SetBool(IsWalking, true);
            //isMoving = true;

            float timer = 0f;
            while (timer < moveDuration)
            {
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }

            animator.SetBool(IsWalking, false);
            //isMoving = false;
        }
    }
}