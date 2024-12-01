using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialRules
{
    public static System.Action ApplyLevelInit(int level)
    {
        switch (level)
        {
            case 4:
                return () =>
                {
                    GameController.instance.HasBoss = true;
                };
            case 6:
                return () =>
                {
                    var mobs = new List<NPCController>();
                    var cars = new List<Car>();
                    mobs.AddRange(GameObject.FindObjectsOfType<NPCController>());
                    cars.AddRange(GameObject.FindObjectsOfType<Car>());
                    var transfermob = mobs.Find((x) => x.waypoints.Count == 3);
                    var boss = mobs.Find((x) => x.transform.Find("sprite").GetComponent<SpriteRenderer>().sprite.name=="boss");

                    transfermob.gameObject.SetActive(false);
                    boss.gameObject.SetActive(false);

                    transfermob.Init();
                    boss.Init();


                    var truck = cars.Find((x) => x.GetComponent<SpriteRenderer>().sprite.name == "b64");
                    var limo = cars.Find(x => x.GetComponent<SpriteRenderer>().sprite.name == "b65");

                    truck.IsMoving = true;
                    limo.IsMoving = false;
                    truck.GetComponent<Car>().StopTime = 4f;
                    truck.GetComponent<Car>().StopPosition = new Vector2(1, -3);
                    truck.GetComponent<Car>().EndPosition = new Vector2(10, -3);

                    limo.GetComponent<Car>().StopTime = 5f;
                    limo.GetComponent<Car>().StopPosition = new Vector2(1, -0.5f);
                    limo.GetComponent<Car>().EndPosition = new Vector2(10, -0.5f);

                    GameController.instance.HasBoss = true;


                };
            case 7:
                return () =>
                {
                    var mobs = new List<NPCController>();
                    var cars = new List<Car>();
                    mobs.AddRange(GameObject.FindObjectsOfType<NPCController>());
                    cars.AddRange(GameObject.FindObjectsOfType<Car>());

                    var boss1 = mobs.Find(x => x.IsBoss && x.waypoints.Count == 2);
                    var boss2 = mobs.Find(x => x.IsBoss && x.waypoints.Count == 1);


                    cars[0].IsMoving = false;

                    cars[0].StopTime = 5f;
                    cars[0].StopPosition = new Vector2(2.5f, 10f);
                    cars[0].EndPosition = new Vector2(2.5f, 10f);

                    boss2.isMoving = false;
                    GameController.instance.HasBoss = true;

                };
            case 8:
                return () =>
                {
                    GameController.instance.DoNotWarn = true;
                };

            case 9:
                return () =>
                {
                    GameController.instance.DoNotWarn = true;
                    var mobs = new List<NPCController>();
                    var cars = new List<Car>();
                    mobs.AddRange(GameObject.FindObjectsOfType<NPCController>());

                    for (int i = GameController.instance.LastStageBoss; i < 4; i++)
                        GameObject.DestroyImmediate(mobs[i].gameObject);


                    cars.AddRange(GameObject.FindObjectsOfType<Car>());


                    cars[0].IsMoving = false;

                    cars[0].StopTime = 5f;
                    cars[0].StopPosition = new Vector2(-3f, 5f);
                    cars[0].EndPosition = new Vector2(-3f, 5f);
                };
            default:
                return () => { };
        }
    }
    public static System.Action ApplyLevelUpdate(int level)
    {
        switch (level)
        {
            case 2:
                return () =>
                {
                    var mobs = new List<NPCController>();
                    mobs.AddRange(GameObject.FindObjectsOfType<NPCController>());
                    if (mobs.Count == 1)
                    {
                        mobs[0].isWarned = true;
                        mobs[0].warnedPosition = new Vector2(10f, mobs[0].transform.position.x);
                        if (mobs[0].transform.position.x >= 5f)
                            GameController.instance.Warn();
                    }
                    //var mob1 = mobs.Find(x=> x.waypoints[0].)

                };
            case 4:
                return () => {
                    var mobs = new List<NPCController>();
                    mobs.AddRange(GameObject.FindObjectsOfType<NPCController>());
                    var reportMob = mobs.Find((x) => x.waypoints.Count == 4);
                    var boss = mobs.Find((x) => x.IsBoss);
                    var bossRight = new Vector2(0.5f, -1.5f);
                    var bossLeft = new Vector2(-1.5f, -1.5f);

                    if (boss == null)
                    {
                        GameController.instance.Win();
                        return;
                    }
                        

                    if (reportMob == null)
                    {
                        boss.isMoving = true;
                        boss.isWarned=true;
                        var warnpos = new Vector2(-2.5f, -1.5f);
                        boss.warnedPosition = warnpos;
                        if (Vector2.Distance(boss.transform.position, warnpos) <= 0.1f)
                            GameController.instance.Warn();
                        return;
                    }

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
                    mobs.AddRange(GameObject.FindObjectsOfType<NPCController>(true));
                    cars.AddRange(GameObject.FindObjectsOfType<Car>(true));
                    var transfermob = mobs.Find((x) => x.waypoints.Count == 3);
                    var boss = mobs.Find((x) => x.IsBoss);


                    var truck = cars.Find((x) => x.GetComponent<SpriteRenderer>().sprite.name == "b64");
                    var limo = cars.Find(x => x.GetComponent<SpriteRenderer>().sprite.name == "b65");

                    Vector2 mobEndPosition = new Vector2(-6f, -1f);

                    if (truck.isActiveAndEnabled == false)
                    {
                        limo.gameObject.SetActive(true);
                        limo.GetComponent<Car>().IsMoving = true;
                    }
                    if(Vector2.Distance(truck.transform.position, truck.GetComponent<Car>().StopPosition)<=0.1f)
                    {
                        transfermob.gameObject.SetActive(true);
                    }
                    if (Vector2.Distance(limo.transform.position, limo.GetComponent<Car>().StopPosition) <= 0.1f)
                    {
                        boss.gameObject.SetActive(true);
                    }

                    if (Vector2.Distance(transfermob.transform.position, mobEndPosition) <= 0.1f)
                        transfermob.gameObject.SetActive(false);

                    if (boss.transform.position.x <= -5.9f)
                    {
                        boss.gameObject.SetActive(false);
                        GameController.instance.Warn();
                    }
                        


                };

            case 7:
                return () =>
                {
                    var mobs = new List<NPCController>();
                    var cars = new List<Car>();
                    mobs.AddRange(GameObject.FindObjectsOfType<NPCController>());
                    cars.AddRange(GameObject.FindObjectsOfType<Car>());

                    var boss1 = mobs.Find(x => (x.IsBoss && x.waypoints.Count == 2));
                    var boss2 = mobs.Find(x => (x.IsBoss && x.waypoints.Count == 1));

                    var endpos = new Vector2(1.5f, -2.5f);


                    boss1.transform.position = new Vector2(boss1.transform.position.x, cars[0].transform.position.y - 1.5f);
                    boss2.transform.position = new Vector2(boss2.transform.position.x, cars[0].transform.position.y - 1.5f);


                    if (Vector2.Distance(boss1.transform.position, endpos) <= 0.1f)
                    {
                        boss1.isMoving = false;
                        if (Vector2.Distance(boss2.transform.position, endpos) <= 0.1f)
                        {
                            boss2.isMoving = false;
                            cars[0].IsMoving = true;
                        }
                            
                        else
                            boss2.isMoving = true;
                    }

                    if (Vector2.Distance(cars[0].transform.position, cars[0].StopPosition) <= 0.1f)
                        GameController.instance.Warn();

                };

            case 8:
                return () =>
                {
                    var mobs = new List<NPCController>();
                    mobs.AddRange(GameObject.FindObjectsOfType<NPCController>());
                    var endpos = new Vector2(-10f, -2.5f);
                    if (mobs.FindAll(x=>x.IsBoss).Count!=4)
                    {
                        if (mobs.FindAll(x => x.IsBoss).Count == 0)
                            GameController.instance.Win();

                        foreach (var i in mobs)
                            if (i.IsBoss)
                                i.isWarned = true;
                    }
                    foreach (var i in mobs)
                        if (i.IsBoss)
                        {
                            if (i.isWarned)
                            {
                                i.warnedPosition = endpos;
                                if (i.transform.position.x<=-8f)
                                {
                                    GameController.instance.LastStageBoss = mobs.FindAll(x => x.IsBoss).Count;
                                    GameController.instance.Win();
                                }
                            }
                        }
                    
                };
            case 9:
                return () =>
                {
                    var mobs = new List<NPCController>();
                    mobs.AddRange(GameObject.FindObjectsOfType<NPCController>());
                    var endpos = new Vector2(-2.5f, -1.5f);
                    var helipos = new Vector2(-3.5f, -0.5f);

                    var car = GameObject.FindObjectOfType<Car>();


                    if (mobs.Count == 0)
                        GameController.instance.Win();

                    if (mobs[0].gameObject == null)
                        mobs.RemoveAt(0);

                    foreach (var i in mobs)
                    {
                        i.isMoving = false;
                        i.warnedPosition = endpos;
                        
                    }
                        

                    mobs[0].isMoving = true;
                    mobs[0].isWarned = true;
                    //mobs[0].warnedPosition = endpos;
                    if(mobs[0].transform.position.x< endpos.x + 0.1f)
                    {
                        mobs[0].warnedPosition = helipos;

                    }
                    if(Vector2.Distance(mobs[0].transform.position, helipos) <= 0.1f)
                    {
                        car.IsMoving = true;
                        
                        
                        //GameController.instance.Lose();
                    }

                    if (car.IsMoving)
                    {
                        mobs[0].transform.position = new Vector2(car.transform.position.x + 0.5f, car.transform.position.y + 1.5f);
                        if (Vector2.Distance(car.transform.position, car.StopPosition) < 0.1f)
                            GameController.instance.Lose();
                    }

                };
            default:
                return () => { return; };
        }
    }
}
