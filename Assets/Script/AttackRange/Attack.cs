using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject starter;
    public float damage;
    public Hashtable hash = new Hashtable();
    public float endtime;
    public float dtime;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == starter) return;
        Stat stat = getStat(other);
        if(stat != null && hash.ContainsKey(stat) == false)
        {
            hash.Add(stat, stat);
            damageToStat(stat);
			action(other);

		}
    }

    public delegate Stat TARGETSTAT(Collider other);
    public TARGETSTAT getStat = delegate (Collider other)
    {
        return other.GetComponent<Stat>();
    };

    public virtual void damageToStat(Stat target)
    {
        target.Decrease(damage);

	}

    void Update()
    {
        dtime += Time.deltaTime;
        if (dtime >= endtime)
            Destroy(gameObject);
    }

    public void SetEndTime(float t)
    {
        endtime = t;
        dtime = 0f;
    }

	public delegate void ACTION(Collider col);
	public ACTION action;
}