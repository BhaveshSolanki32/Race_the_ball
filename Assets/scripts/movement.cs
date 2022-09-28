using UnityEngine;

public class movement : MonoBehaviour
{
    [Header("player")]
    Rigidbody rb;
    public float velocitylimit;
    public Vector3 movforce;
    public GameObject dummyplayer;

    [Space(10)]
    [Header("cam_move")]

    [Space(10)]
    [Header("start")]
    public bool start = false;
    public GameObject homeui;
    public float launchstart = 0;

    [Space(10)]
    [Header("touch")]
    Touch touch;
    public float sensititvty;
    Vector3 touchpos;

    [SerializeField] ParticleSystem speed_particle;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void begin()
    {
        Invoke("launched", 0.8f);
        Invoke("particleOnLaunch",2.1f);
        homeui.GetComponent<Animator>().SetBool("launch", true);
    }

    void FixedUpdate()
    {
        if (start)
        {
            launchstart += Time.deltaTime;
            if (launchstart >= 1.5f)
            {
                if (rb.velocity.z <= velocitylimit)
                { rb.AddForce(movforce); }


                if (Input.touchCount > 0)
                {
                    touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Ended)
                        st=0;

                    if(touch.phase==TouchPhase.Moved)
                        ballMove(new Vector3(touch.position.x,touch.position.y,10));
                        
                }
            }

        }
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -2.483f, 2.47f), transform.position.y, transform.position.z);
        dummyplayer.transform.position = new Vector3(transform.position.x*0.5f, transform.position.y, transform.position.z);
    }
    void particleOnLaunch()
    {  
        if (speed_particle.isPlaying)
        {
            speed_particle.Stop();
        }
        speed_particle.Play();
    }

    void launched()
    {       

        start = true;
        FindObjectOfType<winloose>().coinbase.SetActive(true);
        FindObjectOfType<winloose>().pausebtn.SetActive(true);
        FindObjectOfType<band_lancher>().gamestart = true;
    }

    float st=0,nd;//st previos recorded positon,, nd new recorded touch post

    void ballMove(Vector3 touch)
    {
        nd=Camera.main.ScreenToWorldPoint(touch).x;
        if(st==0)st=nd;
        transform.position=new Vector3(transform.position.x + (nd-st)*PlayerPrefs.GetFloat("sense") ,transform.position.y,transform.position.z);
        st=Camera.main.ScreenToWorldPoint(touch).x;
    }

}
