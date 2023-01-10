using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapCharacters : MonoBehaviour
{
    [SerializeField] private MeshRenderer _mesh;
    [SerializeField] private float _speed = 5;
    public bool _done;
    public bool _swapped;
    public string _charName;

    private GameObject _char;

    private void Awake()
    {
        _mesh.material.color = Color.white;
    }

    public void StartSwap(string newChar="NaN", float speed=-3, bool fadeIn=true, bool skipFade=false)
    {
        if (speed < 0)
        {
            speed = _speed;
        }
        StartCoroutine(Swap(newChar, speed, fadeIn, skipFade));
    }

    public IEnumerator Swap(string newChar, float speed, bool fadeIn, bool skipFade)
    {
        _done = false;
        _swapped = false;
        
        if (_mesh.material.color.a != 0)
        {
            if (!skipFade)
            {
                while (_mesh.material.color.a >= 0)
                {
                    float a = _mesh.material.color.a;
                    _mesh.material.color = new Color(1f, 1f, 1f, a - (speed * Time.deltaTime));

                    yield return null;
                }
            }

            _mesh.material.color = new Color(1f, 1f, 1f, 0f);
        }

        if (newChar != "NaN")
        {
            if (_char != null)
            {
                Destroy(_char.gameObject);
            }
            _char = Instantiate(Resources.Load($"Prefabs/Characters/{newChar}", typeof(GameObject))) as GameObject;
            _mesh.material.color = new Color(1f, 1f, 1f, 0f);
            _charName = newChar;
            _swapped = true;
            if (!skipFade) yield return new WaitForSeconds(0.1f);
            
            if (Math.Abs(_mesh.material.color.a - 1) > 0.001f && fadeIn)
            {
                if (!skipFade)
                {
                    while (_mesh.material.color.a <= 1)
                    {
                        float a = _mesh.material.color.a;
                        _mesh.material.color = new Color(1f, 1f, 1f, a + (speed * Time.deltaTime));
                        yield return null;
                    }
                }

                _mesh.material.color = new Color(1f, 1f, 1f, 1f);
            }
        }

        _done = true;
    }
}
