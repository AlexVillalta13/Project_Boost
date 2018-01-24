
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rotationForce;
    [SerializeField] float thrustForce;
    [SerializeField] float levelLoadDealy = 2f;

    [SerializeField] AudioClip mainEngineSFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip levelClearSFX;

    [SerializeField] ParticleSystem mainEngineParticle;
    [SerializeField] ParticleSystem deathParticle;
    [SerializeField] ParticleSystem levelClearParticle;

    private Rigidbody rb;
    private AudioSource audioSource;

    private bool collisionOn = true;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update ()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }

        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticle.Stop();
        }
    }

    private void ApplyThrust()
    {
        float thrustThisFrame = thrustForce * Time.deltaTime;
        rb.AddRelativeForce(Vector3.up * thrustThisFrame);

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngineSFX);
        }
        mainEngineParticle.Play();
    }

    private void RespondToRotateInput()
    {
        rb.freezeRotation = true;

        float rotationThisFrame = rotationForce * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationThisFrame);
        }
        rb.freezeRotation = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // Do nothing
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence()
    {
            state = State.Transcending;
            audioSource.Stop();
            audioSource.PlayOneShot(levelClearSFX);
            levelClearParticle.Play();
            Invoke("LoadNextLevel", levelLoadDealy);
    }

    private void StartDeathSequence()
    {
        if (collisionOn == true)
        {
            state = State.Dying;
            audioSource.Stop();
            audioSource.PlayOneShot(deathSFX);
            deathParticle.Play();
            Invoke("RestartLevel", levelLoadDealy);
        }
    }

    private void LoadNextLevel()
    {
        int nextLevel;
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        if (currentLevel == SceneManager.sceneCountInBuildSettings)
        {
            nextLevel = 0;
        }
        else
        {
            nextLevel = currentLevel + 1;
        }

        SceneManager.LoadScene(nextLevel);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            collisionOn = !collisionOn;
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }
}
