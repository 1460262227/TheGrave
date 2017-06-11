using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodyNum : MonoBehaviour {

    public Ground Ground = null;
    public GameObject Num = null;

    public void PlayNumAt(int num, int bx, int by)
    {
        var pos = Ground.ToWorldPos(bx, by);
        var spos = Camera.main.WorldToScreenPoint(pos);

        var numObj = Instantiate(Num) as GameObject;
        numObj.transform.SetParent(Num.transform.parent, false);
        numObj.transform.position = spos;
        numObj.GetComponentInChildren<Text>().text = ((num < 0) ? "-" : "") + num.ToString();
        numObj.SetActive(true);
        StartCoroutine(DelayDestroyNumObj(numObj));
    }

    IEnumerator DelayDestroyNumObj(GameObject go)
    {
        yield return new WaitForSeconds(2);
        Destroy(go);
    }
}
