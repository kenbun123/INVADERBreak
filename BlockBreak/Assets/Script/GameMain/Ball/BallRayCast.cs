using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallRayCast : MonoBehaviour {

    LineRenderer m_linerender;

    public GameObject m_LineMnager;
    List<Vector3> m_lineposlist = new List<Vector3>();


    // Use this for initialization
    void Start () {
        m_linerender = transform.GetComponent<LineRenderer>();
        m_linerender.enabled = false;
        m_LineMnager = LineManager.Instance.gameObject;
	}

    // Update is called once per frame
    void Update() {

        if (!m_LineMnager.GetComponent<LineManager>().m_IsTouch)
        {
            CleanLineRendererPos();
            return;
        }
        if(transform.GetComponent<BallMain>().MainState.State == BallMain.BallStatus.Normal)
        DrawLineRenderer();


    }
    void DrawLineRenderer()
    {


        //ボールの向く方向
        Vector3 balldirection = transform.GetComponent<BallMain>().Direction;

        RaycastHit hitobj = new RaycastHit();
        //Lineタグに当っているかどうかを探す
        if (CheckHitObj("LineDummy", transform.position, balldirection, out hitobj))
        {
            if (Vector3.Distance(transform.position, hitobj.transform.position) <= 1.0f)
            {
                CleanLineRendererPos();
                return;
            }
            m_lineposlist.Add(transform.position);
            m_lineposlist.Add(hitobj.point);
        }
        else
        {
            CleanLineRendererPos();
            return;
        }

        Vector3 ballreflect = Vector3.Reflect(balldirection, hitobj.normal);

        Vector3 workpos = hitobj.point;
        if (CheckHitObj("ScreenFrame", hitobj.point, ballreflect, out hitobj))
        {
            //lineposlist.Add(workpos);
            m_lineposlist.Add(hitobj.point);
        }
        else
        {
            CleanLineRendererPos();
            return;
        }

        m_linerender.enabled = true;
        m_linerender.SetPositions(m_lineposlist.ToArray());
        m_lineposlist.Clear();
    }



    bool CheckHitObj(string tag, Vector3 origin,Vector3 direction,out RaycastHit hitobj)
    {
        Ray ray = new Ray(origin,direction);
        RaycastHit[] hits;
        hits = Physics.SphereCastAll(ray,transform.GetComponent<SphereCollider>().radius/2, 1000.0f);
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.transform.tag == tag)
            {
                hitobj = hit;
                return true;
            }
        }
        hitobj = new RaycastHit();
        return false;
    }


    void CleanLineRendererPos()
    {

        m_linerender.SetPosition(0,Vector3.zero);
        m_linerender.SetPosition(1, Vector3.zero);
        m_linerender.SetPosition(2, Vector3.zero);
    }
}
