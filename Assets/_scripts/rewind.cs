using System.Collections.Generic;
using UnityEngine;

public class rewind : MonoBehaviour
{
    List<Vector3> list = new List<Vector3>();
    public GameObject Sphere;
    bool rew_bool = false;
    public bool loosecheck = false;

    void FixedUpdate()
    {
        if (!rew_bool)
        {
            if (!loosecheck)
            {
                if (list.Count <= 32) {list.Insert(0, Sphere.transform.position);}
                else { list.RemoveAt(32); }
            }
        }
        else 
        {
            Sphere.transform.position = list[0];
            list.RemoveAt(0);
            if(list.Count == 0) 
            { 
                rew_bool = false;
                FindObjectOfType<movement>().start = true;
                Sphere.GetComponent<Rigidbody>().isKinematic = false;
                loosecheck = false;
            }
        }
    }
    public void rew_btn()
    {
        rew_bool = true;
        FindObjectOfType<winloose>().coinbase_animator.SetBool("revive", true);
        FindObjectOfType<winloose>().coinbase_animator.SetBool("fail", false);
        FindObjectOfType<winloose>().lost = false;
        FindObjectOfType<winloose>().countdown = 5;
        FindObjectOfType<winloose>().timer_ring.fillAmount = 1;
        FindObjectOfType<winloose>().timer_txt.text = "" + FindObjectOfType<winloose>().countdown;
        FindObjectOfType<winloose>().coinbase.SetActive(true);
        FindObjectOfType<winloose>().pausebtn.SetActive(true);

    }
}
