using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowFight : MonoBehaviour
{
    public Transform FightTarget;
    public string FightTargetTag;
    public Transform HeadArmature;
    // Start is called before the first frame update
    void Start()
    {

        if (FightTarget == null)
        {
            FightTarget = GameObject.FindWithTag(FightTargetTag).transform;
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if (FightTarget != null)
        {
            HeadArmature.transform.LookAt(FightTarget);
        }
    }
}
