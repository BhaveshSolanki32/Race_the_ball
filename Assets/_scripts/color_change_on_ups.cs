using UnityEngine;
using System.Collections;

public class color_change_on_ups : MonoBehaviour
{

    Material material;
    [SerializeField] float speed;
    float time;
    Color color;
    float temp_time1;
    private void Start() => material = GetComponent<MeshRenderer>().material;

    public void color_change_on(Color tar_color, float time)
    {
        StopAllCoroutines();
        this.color = tar_color;
        this.time = time;
        temp_time1 = Time.time;
        StartCoroutine("color_changer");
    }
    
    IEnumerator color_changer()
    {
        
        while (time >= 0 || material.color != Color.white)
        {
            float temp_time0 = Time.time;
            if (time >= 0)
            {
                
                material.color = Color.Lerp(Color.white, color, Mathf.PingPong(Time.time * speed, 1));
                time -= temp_time0 - temp_time1 ;
            }
            else
            {
                if (material.color != Color.white) material.color = Color.Lerp(material.color, Color.white, 1);

                else StopCoroutine("color_changer");

            }
            //  speed -= time * 0.02f;
             temp_time1 = Time.time;
            yield return new WaitForSeconds(0.02f);
        }
    }
    /*
      color change
      void get color
      change rapidly and then slow down 
    */

}
