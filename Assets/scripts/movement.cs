using UnityEngine;
//test
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
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void begin()
    {
        Invoke("launched", 0.8f);
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
                    if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)
                    {
                        touchpos = touch.position;
                        touchpos.z = 10;
                    }
                    if (touch.phase == TouchPhase.Moved )
                    {
                        Vector3 touchendpos = touch.position;
                        touchendpos.z = 10;

                        float posdiff = (Camera.main.ScreenToWorldPoint(touchendpos).x - Camera.main.ScreenToWorldPoint(touchpos).x);
                        transform.position = new Vector3((posdiff * PlayerPrefs.GetFloat("sense")) + transform.position.x, transform.position.y, transform.position.z);
                    }

                }
            }

        }
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -2.483f, 2.47f), transform.position.y, transform.position.z);
        dummyplayer.transform.position = new Vector3(transform.position.x*0.5f, transform.position.y, transform.position.z);
    }
    void launched()
    {
        start = true;
        FindObjectOfType<winloose>().coinbase.SetActive(true);
        FindObjectOfType<winloose>().pausebtn.SetActive(true);
        FindObjectOfType<band_lancher>().gamestart = true;
    }

}
