using Cinemachine;
using System.Collections;
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
    bool in_can_check = true;//check that ball dont trigger in_can event twice
    [SerializeField] GameObject[] shattered_glass;
    float time_limit = 0; //time limit for magnetism
    [SerializeField] GameObject magnet_particle;
    [SerializeField] GameObject magnet_collect_particle;
    [SerializeField] GameObject tap_tap_particle;
    [SerializeField] GameObject vict_glasstext;
    [SerializeField] Camera ui_VFX_camera;
    [SerializeField] GameObject vfx_canonblast;

    [Space(10)]
    [SerializeField] GameObject[] obstacles;
    [SerializeField] ciencam_shake cam_shake;
    [SerializeField] GameObject debri_vfx;
    [SerializeField] GameObject sheild_vfx;
    [SerializeField] color_change_on_ups[] color_change_on_ups;

    private void OnCollisionEnter(UnityEngine.Collision coll)
    {
        switch (coll.gameObject.tag)
        {
            case "winner":
                {
                    glass_shatter(coll.gameObject, false);
                    win_ui.SetActive(true);
                    gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    coinbase.SetActive(false);
                    pausebtn.SetActive(false);
                    FindObjectOfType<movement>().start = false;
                }
                break;

            case "bar":
                {
                    cam_shake.shakeon = true;
                    coinbase_animator.SetBool("revive", false);
                    Time.timeScale = 0.2f;
                    FindObjectOfType<movement>().start = false;
                    Invoke("loost", 0.5f);

                }
                break;
        }

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

                if (speed_particle.isPlaying) speed_particle.Stop();

                if (coll.transform.forward.z == 1) speed_particle.Play();

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

                if (in_can_check)
                {
                    in_can_check = false;
                    FindObjectOfType<movement>().start = false;
                    FindObjectOfType<Rigidbody>().velocity = new Vector3(0, 0, 0);
                    multbar_txt.SetActive(false);
                    tap_bool = true;
                    tap_particle.SetActive(true);
                    tap_text.SetActive(true);
                    tap_bar.SetActive(true);
                    vict_glass[0].GetComponent<BoxCollider>().isTrigger = true;
                }
                break;
            case "final":
                FindObjectOfType<movement>().start = false;
                FindObjectOfType<Rigidbody>().velocity = new Vector3(0, 0, 0);
                break;
            case "winner":
                glass_shatter(coll.gameObject, true);
                break;
            case "magnet":
                GameObject magnet_vfx = coll.transform.GetChild(0).gameObject;
                magnet_vfx.transform.parent = null;
                Instantiate(magnet_collect_particle, transform.position, Quaternion.identity);
                StartCoroutine(magnet_collected(magnet_vfx));
                GameObject particle = Instantiate(magnet_particle, transform.position, Quaternion.identity);
                Destroy(coll.transform.gameObject);
                particle.transform.SetParent(transform);
                InvokeRepeating("magnetism", 0, 0.02f);
                Destroy(particle, 7);
                foreach(color_change_on_ups x in color_change_on_ups)
                x.color_change_on(Color.red, 70);
                break;
            case "jumper":
                GetComponent<Rigidbody>().AddForce(0, 375, 0);
                break;
            case "sheld":
                sheild_on(coll);
                break;
            case "bar":
                Destroy(coll.gameObject);
                Instantiate(debri_vfx, coll.transform.position, Quaternion.identity);
                cam_shake.shakeon = true;
                break;
        }
    }

    void sheild_on(Collider coll)
    {
        Transform coll_vfx = coll.transform.GetChild(0);
        coll_vfx.SetParent(null);
        coll_vfx.GetComponent<ParticleSystem>().Play();
        Destroy(coll_vfx.gameObject, 0.6f);
        Destroy(coll.gameObject);        
        GameObject sheild_vfx_temp = Instantiate(sheild_vfx, transform.position, Quaternion.identity);
        sheild_vfx_temp.transform.SetParent(transform);
        Destroy(sheild_vfx_temp, 7);
        foreach (GameObject x in obstacles)
        {
            x.GetComponent<Collider>().isTrigger = true;
        }
        Invoke("sheild_off", 7);
    }

    void sheild_off()
    {
        foreach (GameObject x in obstacles)
        {
            if(x!=null)
            x.GetComponent<Collider>().isTrigger = false;
        }
        //vfx off
    }


    IEnumerator magnet_collected(GameObject target)
    {
        while (target.transform.position == transform.position && target.transform.localScale.x < 0.1f)
        {
            target.transform.localScale /= 1.5f;
            target.transform.position = transform.position;
            yield return new WaitForSeconds(0.008f);
        }
        Destroy(target);
        StopCoroutine("magnet_collectd");
        yield return null;

    }

    int k; //which cannon the ball is
    void coingainoff() => coinbase_animator.SetBool("coin_gain", false);

    void timeToNormal() => Time.timeScale = 1f;

    void FixedUpdate()
    {
        if (lost) lost_ui_init(); //timer>>restart
        if (tap_bool) tap_tap_sys();
    }

    void Update()
    {
        raycaster();
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
        tap_particle.SetActive(false);
        X_disp.GetComponent<Text>().text = "X" + max_end_val();
        tap_can.SetBool("tap_end", true);
        cannon_anim.SetInteger("can_shoot_int", k);
        Invoke("cannon_blast_vfx", 1.655f);
        Invoke("shoot_can", 1.8f);
    }


    void cannon_blast_vfx()
    {
        Vector3 pos = new Vector3(-0.0399999991f, 1.74699998f, 207.449997f);
        if (k == 0) pos.x = -0.0399999991f;
        if (k == 2) pos.x = -1.65999997f;
        if (k == 1) pos.x = 1.42999995f;

        Instantiate(vfx_canonblast, pos, Quaternion.identity);
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
    GameObject previous_ray_hit;
    public void raycaster()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, FindObjectOfType<movement>().dummyplayer.transform.forward, out hit, 2.65f) && hit.transform.tag == "winner")
        {
            if (previous_ray_hit != hit.transform.gameObject)
            {
                Destroy(vict_glasstext.transform.GetChild(0).gameObject);
            }
            Time.timeScale = 0.3f;
            Invoke("timeToNormal", 0.25f);
            previous_ray_hit = hit.transform.gameObject;
        }
    }

    void shoot_can()
    {
        float j;

        if (maxendval != 8) j = maxendval - 1;
        else j = 4;

        vict_glass[(int)j].GetComponent<BoxCollider>().isTrigger = false;

        cm_cam.m_Lens.FieldOfView = 140; ;
        Vector3 throw_to = new Vector3(vict_glass[(int)j].transform.position.x, vict_glass[(int)j].transform.position.y, vict_glass[(int)j].transform.position.z - 0.1f);

        float velo = Vector3.Distance(transform.position, throw_to) / 16.97f + 42;
        can_go[k].GetComponentInChildren<MeshCollider>().enabled = false;
        GetComponent<Rigidbody>().velocity = new Vector3(0, velo / 2, velo * 0.866f);

    }

    void tap_tap_sys()
    {
        timer += Time.deltaTime;
        if (timer > 8)
        {
            tap_bool = false;
            tap_timeup();
        }

        if (transform.position.x < 1 && transform.position.x > -1)
        {
            max_slidvalue = canons[0]; k = 0; // center
        }
        if (transform.position.x > 1.2f)
        {
            max_slidvalue = canons[2]; k = 1; //right
        }
        if (transform.position.x < -1.2f)
        {
            max_slidvalue = canons[1]; k = 2; //left
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
            Touch touch = Input.GetTouch(0);
            Vector3 touch_post = touch.position;
            touch_post.z = 10;
            if (touch.phase == TouchPhase.Began)
            {
                Instantiate(tap_tap_particle, ui_VFX_camera.ScreenToWorldPoint(touch_post), Quaternion.identity);
                rate += 0.09f;
            }
        }
        rate -= 0.007f;
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

    void glass_shatter(GameObject coll, bool grav)
    {
        cam_shake.shakeon = true;
        int rand = Random.Range(0, 2);
        GameObject gs = Instantiate(shattered_glass[rand], coll.transform.position, shattered_glass[rand].transform.rotation);
        coll.SetActive(false);
        Rigidbody[] childRBgs = gs.GetComponentsInChildren<Rigidbody>();
        if (grav)
            Destroy(gs, 1);

        foreach (Rigidbody x in childRBgs)
        {
            if (grav) x.AddExplosionForce(750f, transform.position, 130f, 3f);
            else Destroy(x);
        }


    }


    void magnetism()
    {
        time_limit += 0.02f;

        if (time_limit >= 7f) { CancelInvoke("magnetism"); time_limit = 0; StopCoroutine("moveTowards"); }

        Collider[] coll = Physics.OverlapSphere(transform.position, 4);

        foreach (Collider x in coll)
            if (x.tag == "coined")
                StartCoroutine(moveTowards(x.gameObject, this.gameObject, 0.02f, 0.2f));

    }

    IEnumerator moveTowards(GameObject postGO, GameObject target, float duration, float dist_jump)
    {
        float numOfJumps = Vector3.Distance(postGO.transform.position, target.transform.position) / dist_jump;
        while (postGO.transform.position != target.transform.position)
        {
            postGO.transform.position = Vector3.MoveTowards(postGO.transform.position, target.transform.position, dist_jump);
            yield return new WaitForSeconds(duration / numOfJumps);
        }
        StopCoroutine("moveTowards");
        yield return null;
    }


}
