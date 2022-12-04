using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    private Animator _anim;
    private float _timer;
    
    // Start is called before the first frame update
    void Start()
    {
        _timer = Random.Range(0f, 7f);
        _anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            _timer = Random.Range(1f, 7f);
            _anim.Play("Blink", 1, 0);
        }
    }
}
