using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialRules
{
    public static System.Action ApplyLevel(int level)
    {
        switch (level)
        {
            case 4:
                return () => {
                    var mobs = new List<NPCController>();
                    mobs.AddRange(GameObject.FindObjectsOfType<NPCController>());
                    var reportMob = mobs.Find((x) => x.waypoints.Count == 4);
                    var boss = mobs.Find((x) => x.IsBoss);
                    var bossRight = new Vector2(0.5f, -1.5f);
                    var bossLeft = new Vector2(-1.5f, -1.5f);

                    if (Vector2.Distance(reportMob.transform.position, new Vector2(-2f, -1.5f)) <= 0.1f)
                    {
                        if (Vector2.Distance(boss.transform.position, bossRight) <= 0.1f)
                        {
                            boss.isMoving = true;
                            boss.waitingTime = boss.maxWaitingTime;
                            reportMob.isMoving = false;

                        }
                        else if (Vector2.Distance(boss.transform.position, bossLeft) <= 0.1f)
                        {
                            boss.isMoving = false;
                            reportMob.isMoving = true;
                        }
                    }
                    else
                    {
                        if (Vector2.Distance(boss.transform.position, bossRight) <= 0.1f)
                            boss.isMoving = false;
                        else
                        {
                            boss.waitingTime = boss.maxWaitingTime;
                            boss.isMoving = true;
                        }

                    }


                };
            case 6:
                return () =>
                {
                    var mobs = new List<NPCController>();
                    var cars = new List<Car>();
                    mobs.AddRange(GameObject.FindObjectsOfType<NPCController>());
                    cars.AddRange(GameObject.FindObjectsOfType<Car>());
                    var tranfermob = mobs.Find((x) => x.waypoints.Count == 1);
                    var boss = mobs.Find((x) => x.IsBoss);
                    

                };
            default:
                return () => { return; };
        }
    }
}
