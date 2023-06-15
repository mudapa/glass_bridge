using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public bool canJump, isJumping, isDead, hasWon;

    private Rigidbody rb;

    private CapsuleCollider collider;

    public Transform jumpTarget;

    public Glass jumpTargetGlass;

    public int currentGlassIndex;

    public Animator animator;

    public Transform ragdollHips;
    public GameObject realPlayerbody, ragdollBody;
    public CameraFollow cameraFollow;

    public AudioClip jumpSfx, screamSfx, fallSfx, glassBreakSfx;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            PressJump(false);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            PressJump(true);
        }

        Jump();
    }

    void PressJump(bool jumpToRight)
    {
        if (!canJump || isJumping) return;
        canJump = false;

        StartCoroutine(PressJumpCoroutine(jumpToRight));
        IEnumerator PressJumpCoroutine(bool jumpToRight)
        {
            animator.SetTrigger("Jump");
            yield return new WaitForSeconds(0.5f);
            audioSource.PlayOneShot(jumpSfx);
            isJumping = true;
            rb.velocity = Vector3.up * 8;

            if(currentGlassIndex >= BridgeManager.instance.totalRow)
            {
                hasWon = true;
                jumpTarget = BridgeManager.instance.goalPivotTargetJump;
                yield break;
            }

            if (jumpToRight)
            {
                jumpTarget = BridgeManager.instance.glasses[currentGlassIndex, 1].transform;
                jumpTargetGlass = jumpTarget.GetComponent<Glass>();
            }
            else
            {
                jumpTarget = BridgeManager.instance.glasses[currentGlassIndex, 0].transform;
                jumpTargetGlass = jumpTarget.GetComponent<Glass>();
            }

            currentGlassIndex++;
        }
    }

    void Jump()
    {
        if(!isJumping) return;

        transform.position = Vector3.MoveTowards(transform.position, jumpTarget.position, 5 * Time.deltaTime);
        if (Vector3.Distance(transform.position, jumpTarget.position) < 0.5f)
        {
            animator.SetTrigger("Idle");
            CheckGlass();
        }
    }

    void CheckGlass()
    {
        if(jumpTargetGlass.isBroken)
        {
            audioSource.PlayOneShot(screamSfx);
            audioSource.PlayOneShot(glassBreakSfx);
            isJumping = false;
            collider.height = 1;
            animator.SetTrigger("Fall");
            print("kaca pecah");
            jumpTargetGlass.BreakGlass();
        }
        else
        {
            isJumping = false;
            canJump = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.GetComponent<Death>())
        {
            Death();
        }
    }

    void Death()
    {
        audioSource.PlayOneShot(fallSfx);
        realPlayerbody.SetActive(false);
        ragdollBody.SetActive(true);
        cameraFollow.playerTarget = ragdollHips;

        StartCoroutine(RestartGameCoroutine());

        IEnumerator RestartGameCoroutine()
        {
            yield return new WaitForSeconds(3);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
