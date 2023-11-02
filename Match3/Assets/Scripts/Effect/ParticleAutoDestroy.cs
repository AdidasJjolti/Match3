using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effect
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleAutoDestroy : MonoBehaviour
    {
        void OnEnable()
        {
            StartCoroutine(CoCheckAlive());
        }

        IEnumerator CoCheckAlive()
        {
            while(true)
            {
                yield return new WaitForSeconds(0.5f);
                if(!GetComponent<ParticleSystem>().IsAlive(true))
                {
                    //Destroy(this.gameObject);
                    PoolManager.Instance.PoolIn(this.gameObject);
                    break;
                }
            }
        }
    }
}
