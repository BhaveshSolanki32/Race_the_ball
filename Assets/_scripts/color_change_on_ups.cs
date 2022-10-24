using UnityEngine;
using System.Collections;

public class color_change_on_ups : MonoBehaviour
{

    Color color;
    Material material;
    float time;
    [SerializeField] float rapid_change, slow_change;
    [SerializeField] [Range(0.1f, 1)] float t;
    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    public void color_change_on(Color tar_color, float time)
    {
        this.color = tar_color;
        this.time = time;
        StartCoroutine("color_change");
        
    }
    //mathf.pingpong
    IEnumerator color_change()
    {
        while (time >= 1)
        {     
            //while(material.color!=color)
                 material.color = Color.Lerp(material.color, color, t*Time.deltaTime);

            time -= rapid_change;
            yield return new WaitForSeconds( rapid_change);

            //while (material.color != Color.white)
                material.color = Color.Lerp(material.color, Color.white, t * Time.deltaTime);

            time -= rapid_change;
            yield return new WaitForSeconds(rapid_change);
        }
        Debug.Log("done");
        while (time >= 0)
        {
           // while (material.color != color)
                material.color = Color.Lerp(material.color, color, t * Time.deltaTime);
            time -= slow_change;
            yield return new WaitForSeconds(slow_change);
          //  while (material.color != Color.white)
                material.color = Color.Lerp(material.color, Color.white, t * Time.deltaTime);
            time -= slow_change;
            yield return new WaitForSeconds(slow_change);
        }
        material.color = Color.white;
        StopCoroutine("color_change");
        yield return null;
    }

    /*
      color change
      void get color
      change rapidly and then slow down 
    */

}
