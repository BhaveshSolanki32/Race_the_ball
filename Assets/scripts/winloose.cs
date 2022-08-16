using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class winloose : MonoBehaviour
{
    public GameObject win_ui;
    public GameObject pausebtn;
    public GameObject coinbase;
    public GameObject looose_ui;

    [Space(10)]
    [Header("timer")]
    public Image timer_ring;
    public Text timer_txt;
    public bool lost = false;
    public float countdown;
    public Animator coinbase_animator;
    public Text scoretxt;
    public GameObject coin_particle;

    [Space(10)]
    [Header("speedup")]
    [SerializeField] float speedupforce;
    [SerializeField] ParticleSystem speed_particle;

    [Space(10)]
    [Header("canon")]
    [SerializeField] Animator cannon_anim;
    [SerializeField] List<BoxCollider> canon_lid;
    [SerializeField] List<int> canons;
    [SerializeField] int[] canons_value;
    [SerializeField] Text[] canon_txt;
    [SerializeField] GameObject multbar_txt;

    float timer;
    bool tap_bool = false;
    public GameObject tap_particle;
    public GameObject tap_bar;
    public GameObject tap_text;
    float tap_scale_lerp = 0;
    [SerializeField] float tap_scale_sped = 0.1f;
    float rate = 0;
    [SerializeField] GameObject X_disp;
    [SerializeField] Animator tap_can;

    public Slider tap_slid;
    int max_slidvalue;
    [SerializeField] List<Text> slid_values;
    [SerializeField] GameObject[] vict_glass;
    [SerializeField] CinemachineVirtualCamera cm_cam;
    public GameObject[] can_go;
    bool in_can_check=false;//check that ball dont trigger in_can event twice


    private void OnCollisionEnter(UnityEngine.Collision coll)
    {
        switch (coll.gameObject.tag)
        {
            case "winner":
                {
                    win_ui.SetActive(true);
                    gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    coinbase.SetActive(false);
                    pausebtn.SetActive(false);
                    FindObjectOfType<movement>().start = false;
                }
                break;

            case "bar":
                {
                    FindObjectOfType<ciencam_shake>().shakeon = true;
                    coinbase_animator.SetBool("revive", false);
                    Time.timeScale = 0.2f;
                    FindObjectOfType<movement>().start = false;
                    Invoke("loost", 0.5f);
                }
                break;
        }

    }

    void OnTriggerExit(Collider coll)
    {
        if(coll.gameObject.tag=="winner")
        Time.timeScale=1;
    }

    private void OnTriggerEnter(Collider coll)
    {
        switch (coll.gameObject.tag)
        {
            case "coined":
                Instantiate(coin_particle, coll.transform.position, transform.rotation);
                PlayerPrefs.SetInt("Totalcoins", PlayerPrefs.GetInt("Totalcoins") + 10);
                scoretxt.text = "" + PlayerPrefs.GetInt("Totalcoins");
                coinbase_animator.SetBool("coin_gain", true);
                Invoke("coingainoff", 0.34f);
                coll.gameObject.SetActive(false);
                break;

            case "speed trigger":
                GetComponent<Rigidbody>().AddForce(coll.transform.forward * speedupforce);
                if (speed_particle.isPlaying)
                {
                    speed_particle.Stop();
                }
                speed_particle.Play();

                break;
            case "LID_open":
                if (cannon_anim.GetBool("lid_open"))
                {
                    cannon_anim.SetBool("lid_close", true);
                    foreach (BoxCollider i in canon_lid)
                    {
                        i.enabled = true;
                    }

                }
                else
                {
                    cannon_anim.SetBool("lid_open", true);

                    while (canons.Count <= 2)
                    {
                        int J = Random.Range(0, canons_value.Length);
                        if (!canons.Contains(canons_value[J]))
                        { canons.Add(canons_value[J]); }
                    }
                    int i = 0;
                    foreach (int f in canons)
                    {
                        canon_txt[i].text = "X" + f;
                        i++;
                    }
                }
                break;

            case "in_canon":
                in_can_check=!in_can_check;
                if(in_can_check)
                {
                FindObjectOfType<movement>().start = false;
                FindObjectOfType<Rigidbody>().velocity = new Vector3(0, 0, 0);
                multbar_txt.SetActive(false);
                tap_bool = true;
                tap_particle.SetActive(true);
                tap_text.SetActive(true);
                tap_bar.SetActive(true);
                }
                break;
            case "final":
                FindObjectOfType<movement>().start = false;
                FindObjectOfType<Rigidbody>().velocity = new Vector3(0, 0, 0);
                break;
            case "winner":
                Time.timeScale = .5f;
                break;
        }
    }

    int k;
    void coingainoff() => FindObjectOfType<winloose>().coinbase_animator.SetBool("coin_gain", false);

    void FixedUpdate()
    {
        if (lost) lost_ui_init(); //timer>>restart

        if (tap_bool)  tap_tap_sys();
    }

    void loost()
    {
        Time.timeScale = 1f;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        coinbase_animator.SetBool("fail", true);
        looose_ui.SetActive(true);
        lost = true;
        FindObjectOfType<rewind>().loosecheck = true;
        pausebtn.SetActive(false);
    }

    void tap_timeup()
    {
        tap_bool = false;
        tap_particle.SetActive(false);
        tap_text.SetActive(false);
        tap_bar.SetActive(false);
        X_disp.SetActive(true);
        X_disp.GetComponent<Text>().text = "X" + max_end_val();
        tap_can.SetBool("tap_end", true);
        //cannon_anim.SetInteger("can_shoot_int",k);
        Invoke("shoot_can", 1.8f);
    }

    float maxendval = 0;
    float max_end_val() //max value after tap end
    {
        if (rate > 0.8 && rate < 1.7)
        {
            maxendval = Mathf.Round(0.5f + (max_slidvalue / 2));
            return Mathf.Round(0.5f + (max_slidvalue / 2));
        }
        if (rate > 1.7f)
        {
            maxendval = max_slidvalue;
            return max_slidvalue;
        }
        maxendval = 1;
        return 1;
    }


    void shoot_can()
    {

        float j;

        if (maxendval != 8) j = maxendval - 1;
        else  j = 4; 

        for (int i = 0; i < j; i++)
        {
            vict_glass[i].GetComponent<BoxCollider>().isTrigger = true;
        }

        cm_cam.m_Lens.FieldOfView = 139;        
        

      Vector3  throw_to = new Vector3(vict_glass[(int)j].transform.position.x, vict_glass[(int)j].transform.position.y, vict_glass[(int)j].transform.position.z - 0.1f);
        
        float velo = Vector3.Distance(transform.position,throw_to)/16.97f+42;
        GetComponent<Rigidbody>().velocity=new Vector3(0,velo/2,velo*0.866f);

    } 


    void tap_tap_sys()
    {
        timer += Time.deltaTime;
            if (timer > 10)
            {
                tap_bool=false;
                tap_timeup();
            }

            if (transform.position.x < 1 && transform.position.x > -1)
            {
                max_slidvalue = canons[0];k = 0;
            }
            if (transform.position.x > 1.2f)
            {
                max_slidvalue = canons[2];k = 1;
            }
            if (transform.position.x < -1.2f)
            {
                max_slidvalue = canons[1];k = 2;
            }

            slid_values[0].text = "X" + max_slidvalue + "-";
            slid_values[1].text = "X" + Mathf.Round(0.5f + (max_slidvalue / 2)) + "-";
            slid_values[2].text = "X" + 1 + "-";

            float scale_increase = Mathf.Lerp(0.2166198f, 0.3351325f, tap_scale_lerp);
            tap_text.transform.localScale = new Vector3(scale_increase, scale_increase + 0.05942f, tap_text.transform.localScale.z);

            tap_scale_lerp += tap_scale_sped;
            tap_scale_lerp = Mathf.Clamp01(tap_scale_lerp);
            if (tap_scale_lerp == 1)
            {
                tap_scale_lerp = 0;
            }

            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    rate += 0.09f;
                }
            }
            rate -= 0.01f;
            rate = Mathf.Clamp(rate, 0f, 2f);
            tap_slid.value = rate;
    }

    void lost_ui_init()
    {
        if (countdown >= 1)
            {
                if (timer_ring.fillAmount > 0)
                { timer_ring.fillAmount -= 0.015f; }
                else
                {
                    if (countdown != 1)
                    { timer_ring.fillAmount = 1; }
                    countdown -= 1;
                    timer_txt.text = "" + countdown;
                }
            }
            if (countdown == 0)
            {
                FindObjectOfType<pause_menu>().restart();
            }
    }


}
