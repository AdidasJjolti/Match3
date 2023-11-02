using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] GameObject _explosionObj;

    public static PoolManager _instance = null;

    public static PoolManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new PoolManager();
            }

            return _instance;
        }
    }

    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public GameObject CreateExplosion()
    {
        GameObject explosion = Instantiate(_explosionObj,transform);
        explosion.SetActive(true);

        return explosion;
    }

    public GameObject PoolOut()
    {
        GameObject obj = null;
        if(transform.childCount <= 0)
        {
            obj = CreateExplosion();
        }
        else
        {
            obj = transform.GetChild(0).gameObject;
            obj.SetActive(true);
        }

        obj.transform.SetParent(null);

        return obj;
    }

    public void PoolIn(GameObject explosion)
    {
        explosion.transform.parent = transform;
        explosion.SetActive(false);
    }
}
