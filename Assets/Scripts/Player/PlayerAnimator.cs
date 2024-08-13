using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public AudioSource PaddleSource;
    [SerializeField] private AudioEvent PaddleEvent;

    public bool isTutorial;
    
    public Animator paddle1;
    public Animator paddle2;

    [SerializeField] private Animator characterL;
    [SerializeField] private Animator characterR;

    public bool paddle1playing = false;
    public bool paddle2playing = false;

    public PlayerController player;

    [SerializeField] private Animator boatFlip;
    
    private void Start()
    {
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (player.enabled)
        {
            paddle1.SetFloat("LeftKey", player.leftButton);
            paddle2.SetFloat("RightKey", player.rightButton);
            boatFlip.SetBool("Exhausted", false);
            characterL.SetFloat("LeftKey", player.leftButton);
            // Sergei: Parameter should be called right key or something generic instead but I was too lazy to change it.
            characterR.SetFloat("LeftKey", player.rightButton);
        }
        else
        {
            paddle1.SetFloat("LeftKey", player.leftButton - 1);
            paddle2.SetFloat("RightKey", player.rightButton - 1);
            boatFlip.SetBool("Exhausted", true);
            
            characterL.SetFloat("LeftKey", 0);
            characterR.SetFloat("LeftKey", 0);
        }

        paddle1playing = CheckForAnimation(paddle1, "paddleAnim");
        paddle2playing = CheckForAnimation(paddle2, "paddle2Anim");
    }

    // Sergei: They really fell off huh
    public void FallOff(bool value)
    {
        characterL.SetBool("FallOff", value);
        characterR.SetBool("FallOff", value);
    }

    public bool CheckForAnimation(Animator paddle, string animName)
    {
        
        if (isTutorial)
        {
            return false;
        }

        if (isTutorial == false)
        {
            bool paddlePlaying;
            if (paddle.GetCurrentAnimatorStateInfo(0).IsName(animName))
            {
                paddlePlaying = true;
                if (PaddleSource && !PaddleSource.isPlaying)
                {
                    PaddleEvent.Play(PaddleSource);
                }
            }
            else
            {
                paddlePlaying = false;
            }
            return paddlePlaying;
        }

        return false;
    }
}
