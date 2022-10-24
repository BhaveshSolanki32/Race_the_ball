using UnityEngine;

public class starsys : MonoBehaviour
{
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;
    public GameObject star11;
    public GameObject star21;
    public GameObject star31;
    [SerializeField] GameObject star_collect_particle;
    [SerializeField] GameObject player;
    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.name == "Sphere")
        {
            Instantiate(star_collect_particle, transform.position, transform.rotation);

            if (star1.activeSelf == true)
            {
                if (star2.activeSelf == true) { star3.SetActive(true);}
                else { star2.SetActive(true); }
            }
            else {star1.SetActive(true);}

            if (star11.activeSelf == true)
            {
                if (star21.activeSelf == true) { star31.SetActive(true); }
                else { star21.SetActive(true); }
            }
            else { star11.SetActive(true); }
            gameObject.SetActive(false);
        }
    }
}
