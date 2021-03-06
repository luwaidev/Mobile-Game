using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableSpawn : MonoBehaviour
{

    public GameObject candy;
    public GameObject gem;
    public int candyRate;
    public int gemRate;
    // Start is called before the first frame update
    void Start()
    {
        if (Random.Range(0, candyRate + gemRate) > gemRate)
        {
            Instantiate(gem, transform);
        }
        else
        {
            Instantiate(candy, transform);
        }
    }

}
