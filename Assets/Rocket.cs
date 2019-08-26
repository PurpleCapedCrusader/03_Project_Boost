using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    [SerializeField] float mainThrust = 100f;
    [SerializeField] float rotateThrust = 100f;
    [SerializeField] float levelLoadDelay = 3f;
    Rigidbody rigidBody;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip dead_voice;
    [SerializeField] AudioClip death;
    [SerializeField] ParticleSystem leftThrusterParticles;
    [SerializeField] ParticleSystem rightThrusterParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;
    // public GameObject Explosion;
    AudioSource audioSource;
    enum State { Alive, Dying, Trancending }
    State state = State.Alive;
    public GameObject RocketBody;
    public GameObject NoseCube1;
    public GameObject NoseCube2;
    public GameObject NoseCube3;
    public GameObject LeftThruster;
    public GameObject RightThruster;
    public GameObject LeftThrustFlame01;
    public GameObject LeftThrustFlame02;
    public GameObject LeftThrustFlame03;
    public GameObject LeftThrustFlame04;
    public GameObject RightThrustFlame01;
    public GameObject RightThrustFlame02;
    public GameObject RightThrustFlame03;
    public GameObject RightThrustFlame04;
    public GameObject ExplosionChildForDetachment;
    public GameObject CuteRocket;
    public GameObject Rock11;
    public Renderer rend;
    bool collisionDisabled = false;
    public float radius = 12.5F;
    public float power = 700.0F;
    public GameObject CollisionRock;
    public bool debugControls = false;


    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        NoseCube1 = GameObject.Find("/CuteRocket/RocketBody/Cube1");
        NoseCube2 = GameObject.Find("/CuteRocket/RocketBody/Cube2");
        NoseCube3 = GameObject.Find("/CuteRocket/RocketBody/Cube3");
        RocketBody = GameObject.Find("/CuteRocket/RocketBody");
        LeftThruster = GameObject.Find("/CuteRocket/LeftThruster");
        RightThruster = GameObject.Find("/CuteRocket/RightThruster");
        LeftThrustFlame01 = GameObject.Find("/CuteRocket/LeftThruster/LeftThrustFlame/Flame01");
        LeftThrustFlame02 = GameObject.Find("/CuteRocket/LeftThruster/LeftThrustFlame/Flame02");
        LeftThrustFlame03 = GameObject.Find("/CuteRocket/LeftThruster/LeftThrustFlame/Flame03");
        LeftThrustFlame04 = GameObject.Find("/CuteRocket/LeftThruster/LeftThrustFlame/Flame04");
        RightThrustFlame01 = GameObject.Find("/CuteRocket/RightThruster/RightThrustFlame/Flame01");
        RightThrustFlame02 = GameObject.Find("/CuteRocket/RightThruster/RightThrustFlame/Flame02");
        RightThrustFlame03 = GameObject.Find("/CuteRocket/RightThruster/RightThrustFlame/Flame03");
        RightThrustFlame04 = GameObject.Find("/CuteRocket/RightThruster/RightThrustFlame/Flame04");
        ExplosionChildForDetachment = GameObject.Find("/CuteRocket/ExplosionChildForDetachment");
        CuteRocket = GameObject.Find("/CuteRocket");
        CuteRocket.GetComponent<Renderer>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        //debug tools
        // print("debugControls = " + debugControls);
        if (debugControls)
        {
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            collisionDisabled = !collisionDisabled; // toggle
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);
        if (state != State.Alive)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                print("FRIENDLY");
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            case "MovingObstacle":
                CollisionRock = collision.gameObject; // gets name of the other collision object
                CollisionRock.GetComponent<Oscillator>().enabled = false;
                StartDeathSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence()
    {
        print("FINISH");
        state = State.Trancending;
        thrustFlamesOff();
        audioSource.Stop();
        audioSource.PlayOneShot(success, .3f);
        successParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void StartDeathSequence()
    {
        if (!collisionDisabled)
        {
            state = State.Dying;
            audioSource.Stop();
            audioSource.PlayOneShot(dead_voice);
            Invoke("expolde", 1f);
            thrustFlamesOff();
            Invoke("LoadFirstLevel", levelLoadDelay);
        }
        else
        {
            return;
        }
    }

    private void expolde()
    {
        audioSource.PlayOneShot(death, .3f);
        deathParticles.Play();
        ExplosionChildForDetachment.transform.parent = null;
        Invoke("explosionForce", .17f);
        Invoke("hideRocket", .17f);
    }
    private void explosionForce()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
        }
    }

    private void hideRocket()
    {
        NoseCube1.GetComponent<Renderer>().enabled = false;
        NoseCube2.GetComponent<Renderer>().enabled = false;
        NoseCube3.GetComponent<Renderer>().enabled = false;
        RocketBody.GetComponent<Renderer>().enabled = false;
        LeftThruster.GetComponent<Renderer>().enabled = false;
        RightThruster.GetComponent<Renderer>().enabled = false;
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if ((SceneManager.sceneCountInBuildSettings - 1) == currentSceneIndex)
        {
            LoadFirstLevel();
        }
        else
        {
            int nextSceneIndex = currentSceneIndex + 1;
            SceneManager.LoadScene(nextSceneIndex);
        }
        
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void RespondToThrustInput()
    {
        float thrustSpeed = mainThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            ApplyThrust();
        }
        else
        {
            thrustFlamesOff();
            audioSource.Stop();
            rightThrusterParticles.Stop();
            leftThrusterParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        flame();
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        rightThrusterParticles.Play();
        leftThrusterParticles.Play();
    }

    private void RespondToRotateInput()
    {
        float rotationSpeed = rotateThrust * Time.deltaTime;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            audioSource.Stop();
        }
        if (Input.GetKey(KeyCode.A))
        {
            rigidBody.freezeRotation = true; // take manual control of rotation
            transform.Rotate(Vector3.forward * rotationSpeed);
            GetComponent<Renderer>().enabled = false;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationSpeed);
            GetComponent<Renderer>().enabled = true;
        }

        rigidBody.freezeRotation = false;
    }

    private void flame()
    {
        if ((Input.GetKey(KeyCode.Space) && (state == State.Alive))) // can thrust while rotating
        {
            System.Random random = new System.Random();
            var leftThrustFlicker4 = random.Next(1, 4);
            var rightThrustFlicker4 = random.Next(1, 4);

            LeftThrustFlame01.GetComponent<Renderer>().enabled = true;
            LeftThrustFlame02.GetComponent<Renderer>().enabled = true;
            LeftThrustFlame03.GetComponent<Renderer>().enabled = true;
            LeftThrustFlame04.GetComponent<Renderer>().enabled = true;

            if (leftThrustFlicker4 == 1)
            {
                LeftThrustFlame04.GetComponent<Renderer>().enabled = false;
                var leftThrustFlicker3 = random.Next(1, 4);
                if (leftThrustFlicker3 == 1)
                {
                    LeftThrustFlame03.GetComponent<Renderer>().enabled = false;
                    var leftThrustFlicker2 = random.Next(1, 4);
                    if (leftThrustFlicker2 == 1)
                    {
                        LeftThrustFlame02.GetComponent<Renderer>().enabled = false;
                    }
                }
            }

            RightThrustFlame01.GetComponent<Renderer>().enabled = true;
            RightThrustFlame02.GetComponent<Renderer>().enabled = true;
            RightThrustFlame03.GetComponent<Renderer>().enabled = true;
            RightThrustFlame04.GetComponent<Renderer>().enabled = true;
            if (rightThrustFlicker4 == 1)
            {
                RightThrustFlame04.GetComponent<Renderer>().enabled = false;
                var rightThrustFlicker3 = random.Next(1, 4);
                if (rightThrustFlicker3 == 1)
                {
                    RightThrustFlame03.GetComponent<Renderer>().enabled = false;
                    var rightThrustFlicker2 = random.Next(1, 4);
                    if (rightThrustFlicker2 == 1)
                    {
                        RightThrustFlame02.GetComponent<Renderer>().enabled = false;
                    }
                }
            }
        }
        else
        {
            thrustFlamesOff();
        }
    }

    private void thrustFlamesOff()
    {
        LeftThrustFlame01.GetComponent<Renderer>().enabled = false;
        LeftThrustFlame02.GetComponent<Renderer>().enabled = false;
        LeftThrustFlame03.GetComponent<Renderer>().enabled = false;
        LeftThrustFlame04.GetComponent<Renderer>().enabled = false;
        RightThrustFlame01.GetComponent<Renderer>().enabled = false;
        RightThrustFlame02.GetComponent<Renderer>().enabled = false;
        RightThrustFlame03.GetComponent<Renderer>().enabled = false;
        RightThrustFlame04.GetComponent<Renderer>().enabled = false;
    }
}