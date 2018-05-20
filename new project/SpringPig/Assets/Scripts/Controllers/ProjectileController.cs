using System;
using System.Diagnostics;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] public float ProjectileSpeed = 10;

    private Stopwatch _timeToLive = new Stopwatch();

    void Start()
    {
        _timeToLive.Start();
    }

    private void Update()
    {
        if (_timeToLive.ElapsedMilliseconds >= 10000)
        {
            Destroy(this.gameObject);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (IsCollidingWithPlayer(collision))
        {
            UnityEngine.Debug.Log((float)(_timeToLive.ElapsedMilliseconds / 10) / 100f);
            Destroy(this.gameObject);
        }
    }

    private bool IsCollidingWithPlayer(Collision collision)
    {
        return TagList.ContainsTag(collision.gameObject, Constants.TAG_ENEMY);
    }
}
