using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class band_lancher : MonoBehaviour
{
    [SerializeField] LineRenderer band;
    [SerializeField] Transform handel1;
    [SerializeField] Transform handel2;
    float post_count = 12;
    [SerializeField] Vector3 anchorL;
    [SerializeField] Vector3 anchorR;

    [SerializeField] GameObject sphere;
    [SerializeField] Animator bandanim;
    public bool gamestart = false;
    public Animator cammove_anim;

    private void Update()
    {
        cammove_anim.SetBool("bandedd", false);
        List<Vector3> pointList = vertexpos(anchorL, handel1.position,handel2.position, anchorR,4 );
        band.positionCount = pointList.Count;
        band.SetPositions(pointList.ToArray());
        if (gamestart)
        {
            bandanim.enabled=true;
            cammove_anim.SetBool("bandedd", true);
            sphere.transform.position = new Vector3(sphere.transform.position.x, sphere.transform.position.y, pointList[6].z + 0.29f);

        }
        if (FindObjectOfType<movement>().launchstart >=1.6f)
        { gamestart = false; }
        if(FindObjectOfType<movement>().launchstart >= 3f) { Destroy(this.gameObject); }
    }

    Vector3 quadlerp(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);

        return Vector3.Lerp(ab, bc, t);
    }
    Vector3 cubiclerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 abc = quadlerp(a, b, c, t);
        Vector3 bcd = quadlerp(b, c, d, t);

        return Vector3.Lerp(abc, bcd, t);
    }
    List<Vector3> vertexpos(Vector3 a, Vector3 b, Vector3 c, Vector3 d, int t)
    {
        List<Vector3> pointList = new List<Vector3>();
        if (t == 4)
        {
            for (float ratio = 0; ratio <= 1; ratio += (1/post_count))
            {
                Vector3 curve = cubiclerp(a, b, c, d, ratio);
                pointList.Add(curve);
            }
            return pointList;
        }
        else if (t == 3)
        {
            for (float ratio = 0; ratio <= 1; ratio += (1 / post_count))
            {
                Vector3 curve = quadlerp(a, b, c, ratio);
                pointList.Add(curve);
            }
            return pointList;
        }
        else { return null; }
    }
}
