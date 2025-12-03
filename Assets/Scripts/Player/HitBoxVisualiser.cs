using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxVisualiser : MonoBehaviour
{
    [SerializeField] private AttackCharacteristics stats;
    [SerializeField] private Transform cursor;
    private void Update()
    {
        transform.position = cursor.position;

        float r = stats.clickRange;
        transform.localScale = new Vector3(r * 2f, r * 2f, 1f);
    }
}
